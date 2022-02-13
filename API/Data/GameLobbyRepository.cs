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

        public async Task<GameLobby> AddGuestToLobby()
        {
            // check if there is any lobby with less than 4 people
            var gameLobby = await _context.GameLobbies
                .Where(g => g.NumberOfElements < 4)
                .FirstOrDefaultAsync();

            if (gameLobby == null)
            {
                gameLobby = new GameLobby
                {
                    NumberOfElements = 1
                };

                _context.GameLobbies.Add(gameLobby);
            }
            else
            {
                gameLobby.NumberOfElements += 1;
            }
            return gameLobby;
        }

        public async Task<bool> UserExists(string username)
        {
            return await _context.Guests.AnyAsync(user => user.UserName == username.ToLower());
        }

        public async Task<IEnumerable<GameLobby>> GetGameLobbiesAsync()
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
        public async Task<GameLobby> CreateGame(GameLobby lobby)
        {
            // not null
            var group = GetPlayersOfALobby(lobby.GameLobbyId);

            // initial data of a game lobby         
            lobby.DrawableCards = await _context.Cards.ToListAsync();
            lobby.CurrentPlayer = group.Result.First().Username;
            lobby.GameStatus = "ongoing";

            Random r = new Random();
            int cardIndex = 0;
            Card card = new Card();

            foreach (var connection in group.Result)
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
            lobby = StartGame(lobby);

            return lobby;
        }

        // Working
        private GameLobby StartGame(GameLobby lobby)
        {
            Random r = new Random();
            int cardIndex = r.Next(lobby.DrawableCards.Count());
            Card card = lobby.DrawableCards.ElementAt(cardIndex);                        
                        
            lobby.DrawableCards.Remove(card);
            lobby.CardPot.Add(card);
            lobby.LastCard = card.CardId;

            return lobby;
        }
        public async Task<ICollection<Connection>> GetPlayersOfALobby(int gameLobbyId)
        {
            var lobbyMembers = await _context.Connections
                .Where(connection => connection.GameLobbyId == gameLobbyId)
                .Include(connection => connection.Cards)
                .OrderBy(c => c.ConnectionId)
                .ToListAsync();

            return lobbyMembers;
        }

        public async Task<string> Play(string username, GameLobby gameLobby, List<Card> cards)
        {          
            if (gameLobby.CurrentPlayer != username) return ("It is " + gameLobby.CurrentPlayer + "'s turn, it is not your turn!");

            Connection connection = _context.Connections
                .Where(c => c.Username == username)
                .Include(c => c.Cards)
                .FirstOrDefault();

            var pot = gameLobby.CardPot;

            // same colour, same value, same type - allowed
            if (pot.Last().Value == cards.First().Value && pot.Last().Value != -1 || pot.Last().Type == cards.First().Type || pot.Last().Colour == cards.First().Colour)
            {                
                // verify if the cards played are allowed
                var verification = VerifyCardsPlayed(cards, connection, gameLobby);
                if (!verification) return "This play is not allowed";                
            }
            else
            {
                return "This play is not allowed!";
            }

            var group = await GetPlayersOfALobby(gameLobby.GameLobbyId);
            // get the next turn
            var turn = NextTurn(gameLobby, group, username);
            if (!turn) return "I cannot get to the next turn!";

            return "Next";

        }

        private bool VerifyCardsPlayed(List<Card> cards, Connection connection, GameLobby gameLobby)
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

        // TESTED - Working
        private bool NextTurn(GameLobby gameLobby, ICollection<Connection> group, string username)
        {
            int currIndex = -1;
            int i = 0;
            foreach (var member in group)
            {                
                if (member.Username == username)
                {
                    currIndex = i;
                    break;
                }
                i++;
            }

            if (currIndex == group.Count() - 1)
            {
                gameLobby.CurrentPlayer = group.ElementAt(0).Username;
            }
            else
            {
                gameLobby.CurrentPlayer = group.ElementAt(currIndex+1).Username;
            }
            return true;
        }
    }
}