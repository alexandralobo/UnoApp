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
        public GameLobbyRepository(DataContext context)
        {
            _context = context;
        }
        public async Task<GameLobby> JoinExistingLobby(int gameLobbyId)
        {           
            var gameLobby = await _context.GameLobbies
                .Where(g => g.GameLobbyId == gameLobbyId)
                .FirstOrDefaultAsync();

            if (gameLobby.NumberOfElements == 4) return null;
            gameLobby.NumberOfElements += 1;
            
            return gameLobby;
        }
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
            return await _context.Guests.AnyAsync(user => user.UserName == username.ToLower()) || await _context.Users.AnyAsync(user => user.UserName == username.ToLower());
        }

        public async Task<ICollection<GameLobby>> GetGameLobbiesAsync()
        {
            return await _context.GameLobbies
                .Where(g => g.Password == "" && g.GameStatus == "waiting" && g.NumberOfElements > 0)
                .ToListAsync();
        }

        public GameLobby GetGameLobbyByName(string groupName)
        {
            GameLobby lobby = _context.GameLobbies
                .Where(g => g.GameLobbyName == groupName)
                .Include(g => g.CardPot)
                .Include(g => g.DrawableCards)
                .FirstOrDefault();

            return lobby;
        }

        public GameLobby GetGameLobbyById(int id)
        {
            GameLobby lobby = _context.GameLobbies
                .Where(g => g.GameLobbyId == id)
                .Include(g => g.CardPot)
                .Include(g => g.DrawableCards)
                .FirstOrDefault();

            return lobby;
        }

        public GameLobby GetGameLobbyWithPassword(string password)
        {
            GameLobby lobby = _context.GameLobbies
                .Where(g => g.Password == password)
                .Include(g => g.CardPot)
                .Include(g => g.DrawableCards)
                .FirstOrDefault();

            return lobby;
        }
        
        public async Task<GameLobby> StartGame(GameLobby lobby)
        {
            
            var group = await GetGroup(lobby.GameLobbyName);

            // initial data of a game lobby         
            lobby.DrawableCards = await _context.Cards.ToListAsync();
            lobby.CurrentPlayer = group.Connections.First().Username;
            lobby.GameStatus = "ongoing";

            Random r = new();
            int cardIndex;
            Card card;

            foreach (var connection in group.Connections)
            {
                for (int i = 1; i <= 7; i++)
                {
                    // randomly choose a card from the desk
                    cardIndex = r.Next(lobby.DrawableCards.Count);
                    card = lobby.DrawableCards.ElementAt(cardIndex);
              
                    // remove card from the desk
                    lobby.DrawableCards.Remove(card);

                    // add card to the player's card
                    connection.Cards.Add(card);
                }
            }

            // initialise game by putting a card in the pot
            lobby = GetFirstCard(lobby);

            // get consequence for the first player
            var firstPlayer = _context.Connections
                .Where(player => player.Username == lobby.CurrentPlayer)
                .FirstOrDefault();

            await GetConsequence(lobby, group, firstPlayer, lobby.CardPot);

            return lobby;
        }
        
        private GameLobby GetFirstCard(GameLobby lobby)
        {
            Random r = new();
            int cardIndex = r.Next(lobby.DrawableCards.Count);
       
            Card card = lobby.DrawableCards.ElementAt(cardIndex);
            while (card.Type != "Wild" && card.Type != "WildDraw4")
            {
               card = lobby.DrawableCards.ElementAt(cardIndex);
            }

            lobby.DrawableCards.Remove(card);
            lobby.CardPot.Add(card);
            lobby.LastCard = card.CardId;
            lobby.FileName = card.FileName;            

            return lobby;
        }
        // Working
        public async Task<Group> GetGroup(string groupName)
        {
            Group group = await _context.Groups
                .Include(x => x.Connections)
                .ThenInclude(x => x.Cards)
                .FirstOrDefaultAsync(x => x.Name == groupName);

            return group;  
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _context.Groups
                .Include(c => c.Connections)
                .Where(c => c.Connections.Any(x => x.ConnectionId == connectionId))
                .FirstOrDefaultAsync();
        }
        public void RemoveConnection(GameLobby gameLobby, Group group, Connection connection)
        {
            if (group.Connections.Count == 0 || gameLobby.GameStatus == "finished")
            {
                _context.Connections.Remove(connection);
                _context.Groups.Remove(group);
                _context.GameLobbies.Remove(gameLobby);
            } else if (gameLobby.GameStatus == "waiting")
            {
                _context.Connections.Remove(connection);
                group.Connections.Remove(connection);
                gameLobby.NumberOfElements -= 1;
            } else
            {
                _context.Connections.Remove(connection);
                group.Connections.Remove(connection);
                gameLobby.NumberOfElements -= 1;
            }
        }

        public async Task<string> Play(Connection connection, GameLobby gameLobby, ICollection<Card> cards)
        {
            var pot = await _context.Cards.FindAsync(gameLobby.LastCard);
            var firstCard = cards.First();

            // same colour, same value, same type - allowed
            if (pot.Value == firstCard.Value && pot.Value != -1 || pot.Type == firstCard.Type && firstCard.Type != "Number" || pot.Colour == firstCard.Colour
                || firstCard.Type == "Wild" || firstCard.Type == "WildDraw4")
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

        public string PlayWithChosenColour(Connection connection, GameLobby gameLobby, ICollection<Card> cards, string colour)
        {
            var firstCard = cards.First();

            if (firstCard.Colour == colour || firstCard.Type == "Wild" || firstCard.Type == "WildDraw4")
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
            // check if all cards have the same value/type
            foreach (Card card in cards)
            {
                foreach (Card cardNext in cards)
                {
                    if (card.Value != cardNext.Value || card.Type != cardNext.Type) 
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
                gameLobby.FileName = cardInConnection.FileName;
            }
            return true;
        }

        public async Task<string> GetConsequence(GameLobby gameLobby, Group group, Connection connection, ICollection<Card> cards)
        {
            var firstCard = cards.First();
            switch (firstCard.Type)
            {                
                case "Skip":
                    foreach (Card card in cards)
                    {
                        NextTurn(gameLobby, group);
                    }

                    if (cards.Count == 1) return ("A player was skipped!");
                    if (cards.Count == 4) return ("You skipped all players!");
                    return ("A few players were skipped!");

                case "Reverse":
                    gameLobby.Order = gameLobby.Order == "normal" ? "reverse" : "normal";
                    return "Next";
                
                case "Draw2":
                    await NextPlayerDraw(gameLobby, 2);
                    return "Next";
                
                case "WildDraw4":
                    await NextPlayerDraw(gameLobby, 4);
                    return "Pick a colour";

                case "Wild":
                    return "Pick a colour";

                default:
                    return "Card type is incorrect!";
            }
        }

        public bool PickColour(string colour)
        {
            if (colour == "red" || colour == "blue" || colour == "yellow" || colour == "green") return true;
            return false;
        }
   
        public bool NextTurn(GameLobby gameLobby, Group group)
        {
            Connection nextPlaying = GetPlayer(gameLobby, group);
            if (nextPlaying.Username != gameLobby.CurrentPlayer)
            {
                gameLobby.CurrentPlayer = nextPlaying.Username;
            }
            return true;
        }
        
        public async Task<Card> Draw(int quantity, GameLobby lobby, Connection connection)
        {           

            Card card = new();
            for (int i = 0; i < quantity; i++)
            {
                if (lobby.DrawableCards.Count == 0)
                {
                    await GetNewDeck(lobby);
                }

                Random r = new();
                int cardIndex = r.Next(lobby.DrawableCards.Count);
                card = lobby.DrawableCards.ElementAt(cardIndex);

                connection.Cards.Add(card);
                lobby.DrawableCards.Remove(card);

            }
            connection.Uno = false;
            return card;
        }     

        private async Task NextPlayerDraw(GameLobby lobby, int quantity)
        {
            Group group = await GetGroup(lobby.GameLobbyName);
            Connection playerToDraw = GetPlayer(lobby, group);

            Card card;
            for (int i = 0; i < quantity; i++)
            {
                if (lobby.DrawableCards.Count == 0)
                {
                    await GetNewDeck(lobby);
                }

                Random r = new();
                int cardIndex = r.Next(lobby.DrawableCards.Count);
                card = lobby.DrawableCards.ElementAt(cardIndex);             

                playerToDraw.Cards.Add(card);
                lobby.DrawableCards.Remove(card);
            }

            playerToDraw.Uno = false;
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
            int nextIndex;
            if (lobby.Order == "normal")
            {
                if (currIndex == group.Connections.Count - 1)
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
                    nextIndex = group.Connections.Count - 1;
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
            if (gameLobby.DrawableCards.Count > 0) return false;

            // not sure if it works

            Card pot = await _context.Cards.FindAsync(gameLobby.LastCard); 
            
            gameLobby.DrawableCards = gameLobby.CardPot
                .Where(c => c.CardId != gameLobby.LastCard)
                .ToList();

            gameLobby.CardPot.Add(pot);

            return true;
        }

        public bool Playable(Card pot, ICollection<Card> cards)
        {
            // verify if the current player have a valid card to play
            foreach (Card card in cards)
            {
                if (pot.Value == card.Value && pot.Value != -1 || pot.Type == card.Type && card.Type != "Number" || pot.Colour == card.Colour
                || card.Type == "Wild" || card.Type == "WildDraw4")
                {
                    return true;
                }
            }
            return false;
        }
        public bool PlayableWithColour(ICollection<Card> cards, string pickedColour)
        {
            // verify if the current player have a valid card to play
            foreach (Card card in cards)
            {
                if (pickedColour == card.Colour || card.Type == "Wild" || card.Type == "WildDraw4")
                {
                    return true;
                }
            }
            return false;
        }

        public string UnoStatus(Connection connection)
        {
            if (connection.Uno == false) // when a player can say uno
            {         
                if (connection.Cards.Count == 1)
                {
                    connection.Uno = true; 
                    return "Uno!";

                } else
                {
                    return "You cannot say uno!";
                }
                
                
            } else // after saying uno, some player requests that player to get more cards
            {
                connection.Uno = false;
                return "Counter Uno!";
            }            
        }

        public string DeleteGame(int gameLobbyId)
        {
            var gameLobby = _context.GameLobbies.Find(gameLobbyId);
            var removed = _context.GameLobbies.Remove(gameLobby);

            if (removed != null) return "Game Lobby w/ id " + gameLobbyId + " deleted!";
            return "Game lobbyy does not exist";
        }

    }
}