using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace API.Data
{
    public class GameLobbyRepository : IGameLobbyRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public GameLobbyRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        // working
        public async Task<GameLobby> JoinExistingLobby(int gameLobbyId)
        {           
            var gameLobby = await _context.GameLobbies
                .Where(g => g.GameLobbyId == gameLobbyId)
                .FirstOrDefaultAsync();

            if (gameLobby.NumberOfElements == 4) return null;
            gameLobby.NumberOfElements += 1;
            
            return gameLobby;
        }

        // working
        public async Task<GameLobby> JoinNewLobby(string name)
        {
            var gameLobby = new GameLobby
            {
                GameLobbyName = name,
                NumberOfElements = 1
            };
            await _context.GameLobbies.AddAsync(gameLobby);

            var group = new Group
            {
                Name = name,                
            };
            await _context.Groups.AddAsync(group);

            return gameLobby;
        }

        public async Task<bool> UserExists(string username)
        {
            return await _context.Guests.AnyAsync(user => user.UserName == username.ToLower());
        }

        public async Task<ICollection<GameLobby>> GetGameLobbiesAsync()
        {
            return await _context.GameLobbies.ToListAsync();
        }

        public async Task<GameLobby> GetGameLobbyAsync(int gameLobbyId)
        {
            GameLobby lobby = _context.GameLobbies
                .Where(g => g.GameLobbyId == gameLobbyId)
                .Include(g => g.CardPot)
                .Include(g => g.DrawableCards)
                .FirstOrDefault();

            return lobby;
        }

        // working
        public async Task<GameLobby> StartGame(GameLobby lobby)
        {
            
            var group = await GetGroup(lobby.GameLobbyId.ToString());

            // initial data of a game lobby         
            lobby.DrawableCards = await _context.Cards.ToListAsync();
            lobby.CurrentPlayer = group.Connections.First().Username;
            lobby.GameStatus = "ongoing";

            Random r = new Random();
            int cardIndex = 0;
            Card card = new Card();

            foreach (var connection in group.Connections)
            {
                for (int i = 1; i <= 7; i++)
                {
                    // randomly choose a card from the desk
                    cardIndex = r.Next(lobby.DrawableCards.Count());
                    card = lobby.DrawableCards.ElementAt(cardIndex);

                    // remove card from the desk
                    lobby.DrawableCards.Remove(card);

                    // add card to the player's card
                    connection.Cards.Add(card);
                }
            }

            // initialise game by putting a card in the pot
            lobby = GetFirstCard(lobby);
            return lobby;
        }

        // Working
        private GameLobby GetFirstCard(GameLobby lobby)
        {
            Random r = new Random();
            int cardIndex = r.Next(lobby.DrawableCards.Count());
            Card card = lobby.DrawableCards.ElementAt(cardIndex);

            lobby.DrawableCards.Remove(card);
            lobby.CardPot.Add(card);
            lobby.LastCard = card.CardId;

            return lobby;
        }
        // Working
        public async Task<Group> GetGroup(string groupName)
        {
            return await _context.Groups
                .Include(x => x.Connections)
                .FirstOrDefaultAsync(x => x.Name == groupName);           
     

            /*var lobbyMembers = await _context.Connections
                .Where(connection => connection.GameLobbyId == gameLobbyId)
                .Include(connection => connection.Cards)
                .OrderBy(c => c.ConnectionId)
                .ToListAsync();

            return lobbyMembers;*/
        }

        public async Task<Group> GetLobbyForConnection(string connectionId)
        {
            return await _context.Groups
                .Include(c => c.Connections)
                .Where(c => c.Connections.Any(x => x.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }
        public async Task RemoveConnection(Connection connection)
        {
            _context.Connections.Remove(connection);
        }

        public async Task<string> Play(Connection connection, GameLobby gameLobby, ICollection<Card> cards)
        {
            var pot = await _context.Cards.FindAsync(gameLobby.LastCard);
            var firstCard = cards.First();

            // same colour, same value, same type - allowed
            if (pot.Value == firstCard.Value && pot.Value != -1 || pot.Type == firstCard.Type && firstCard.Type != "Number" || pot.Colour == firstCard.Colour
                || firstCard.Type == "Wild" || firstCard.Type == "Wild Draw 4")
            {
                // verify if the cards played together are allowed
                var validation = ValidateCardsPlayed(cards, connection, gameLobby);
                if (!validation) return "This play is not allowed!";
            }
            else
            {
                return "This play is not allowed!";
            }
            return "Next";
        }

        public async Task<string> PlayWithChosenColour(Connection connection, GameLobby gameLobby, ICollection<Card> cards, string colour)
        {
            var firstCard = cards.First();

            if (firstCard.Colour == colour || firstCard.Type == "Wild" || firstCard.Type == "Wild Draw 4")
            {
                var validation = ValidateCardsPlayed(cards, connection, gameLobby);
                if (!validation) return "This play is not allowed!";
            }
            else
            {
                return "This play is not allowed!";
            }
            return "Next";
        }
        // Working
        private bool ValidateCardsPlayed(ICollection<Card> cards, Connection connection, GameLobby gameLobby)
        {
            // check if all cards players have the same value/type
            foreach (Card card in cards)
            {
                foreach (Card cardNext in cards)
                {
                    if (card.Value != cardNext.Value && card.Type != cardNext.Type) // && or ||
                    {
                        return false;
                    }
                }
            }

            foreach (Card card in cards)
            {
                Card cardInConnection = connection.Cards
                        .Where(c => c.CardId == card.CardId)
                        .FirstOrDefault();

                if (cardInConnection == null) return false;

                connection.Cards.Remove(cardInConnection);
                gameLobby.CardPot.Add(cardInConnection);
                gameLobby.LastCard = cardInConnection.CardId;
            }
            return true;
        }

        public async Task<string> GetConsequence(GameLobby gameLobby, Group group, Connection connection, ICollection<Card> cards)
        {
            var firstCard = cards.First();
            switch (firstCard.Type)
            {
                // Skipped working
                // Several skips together do not work
                case "Skip":
                    foreach (var card in cards) await NextTurn(gameLobby, group);

                    // Message to be shown in the frontend according to the nr of cards
                    if (cards.Count() == 1) return ("A player was skipped!");
                    if (cards.Count() == 4) return ("You skipped all players!");
                    return ("A few players were skipped!");

                // Reverse Working
                case "Reverse":
                    gameLobby.order = gameLobby.order == "normal" ? "reverse" : "normal";
                    return "Next";
                
                 // Draw 2 working
                 // Several Draws Working
                case "Draw 2":
                    await NextPlayerDraw(gameLobby, 2);
                    return "Next";
                // Wild Draw 4 working
                // Several Wild Draw 4 Working
                case "Wild Draw 4":
                    await NextPlayerDraw(gameLobby, 4);
                    return "Pick a colour";

                case "Wild":
                    return "Pick a colour";

                default:
                    return "Card type is incorrect!";
            }
        }

        public async Task<bool> PickColour(string colour)
        {
            if (colour == "red" || colour == "blue" || colour == "yellow" || colour == "green") return true;
            return false;
        }

        // TESTED - Working
        public async Task<bool> NextTurn(GameLobby gameLobby, Group group)
        {
            Connection nextPlaying = GetPlayer(gameLobby, group);
            gameLobby.CurrentPlayer = nextPlaying.Username;
            return true;
        }

        // Working
        public async Task<Card> Draw(int quantity, GameLobby lobby, Connection connection)
        {           

            Card card = new Card();
            for (int i = 0; i < quantity; i++)
            {
                // No cards available to draw
                // Not tested
                if (lobby.DrawableCards.Count() == 0)
                {
                    await GetNewDeck(lobby);
                }

                Random r = new Random();
                int cardIndex = r.Next(lobby.DrawableCards.Count());
                card = lobby.DrawableCards.ElementAt(cardIndex);

                connection.Cards.Add(card);
                lobby.DrawableCards.Remove(card);

            }
            return card;
        }

        private async Task NextPlayerDraw(GameLobby lobby, int quantity)
        {

            Card card = new Card();
            for (int i = 0; i < quantity; i++)
            {
                if (lobby.DrawableCards.Count() == 0)
                {
                    await GetNewDeck(lobby);
                }

                Random r = new Random();
                int cardIndex = r.Next(lobby.DrawableCards.Count());
                card = lobby.DrawableCards.ElementAt(cardIndex);

                Group group = await GetGroup(lobby.GameLobbyId.ToString());
                Connection playerToDraw = GetPlayer(lobby, group);

                playerToDraw.Cards.Add(card);
                lobby.DrawableCards.Remove(card);
            }
        }
        private Connection GetPlayer(GameLobby lobby, Group group)
        {
            // similar to next turn, create a new method for this
            int currIndex = -1;
            int i = 0;
            foreach (var member in group.Connections)
            {
                if (member.Username == lobby.CurrentPlayer)
                {
                    currIndex = i;
                    break;
                }
                i++;
            }

            // verify the order
            int nextIndex = 0;
            if (lobby.order == "normal")
            {
                if (currIndex == group.Connections.Count() - 1)
                {
                    nextIndex = 0;
                }
                else
                {
                    nextIndex = currIndex + 1;
                }

            }
            else // reversed order
            {
                if (currIndex == 0)
                {
                    nextIndex = group.Connections.Count() - 1;
                }
                else
                {
                    nextIndex = currIndex - 1;
                }

            }

            return group.Connections.ElementAt(nextIndex);
        }
        public async Task<bool> GetNewDeck(GameLobby gameLobby)
        {
            // there is card in the deck
            if (gameLobby.DrawableCards.Count() > 0) return false;

            // not sure if it works

            Card pot = await _context.Cards.FindAsync(gameLobby.LastCard); 
            
            gameLobby.DrawableCards = gameLobby.CardPot
                .Where(c => c.CardId != gameLobby.LastCard)
                .ToList();

            gameLobby.CardPot.Add(pot);

            return true;
        }

        // Tested
        public async Task<bool> Playable(GameLobby gameLobby, Card pot, ICollection<Card> cards)
        {
            // verify if the current player have a valid card to play
            foreach (Card card in cards)
            {
                if (pot.Value == card.Value && pot.Value != -1 || pot.Type == card.Type && card.Type != "Number" || pot.Colour == card.Colour
                || card.Type == "Wild" || card.Type == "Wild Draw 4")
                {
                    return true;
                }
            }
            return false;
        }

    }
}