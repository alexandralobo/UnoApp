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

        public async Task<GameLobby> GetGameLobbyAsync(string gameLobbyId)
        {
            return await _context.GameLobbies.FindAsync(Int32.Parse(gameLobbyId));
        }

        public async Task<GameLobby> CreateGame(GameLobby lobby)
        {
            // not null
            var group = GetPlayersOfALobby(lobby.GameLobbyId);

            // initial data of a game lobby         
            lobby.DrawableCards = await _context.Cards.ToListAsync();
            lobby.CurrentPlayer = group.Result.First().Username;
            lobby.GameStatus = "ongoing";

            Random r = new Random();
            foreach (var connection in group.Result)
            {
                for (int i = 1; i <= 7; i++)
                {
                    // randomly choose a card from the desk
                    int cardIndex = r.Next(lobby.DrawableCards.Count());
                    Card card = lobby.DrawableCards.ElementAt(cardIndex);

                    // remove card from the desk
                    lobby.DrawableCards.Remove(card);

                    // add card to the player's card
                    connection.Cards.Add(card);
                }
            }
            return lobby;
        }

        public async Task<ICollection<Connection>> GetPlayersOfALobby(int gameLobbyId)
        {
            var lobbyMembers = await _context.Connections
                .Where(connection => connection.GameLobbyId == gameLobbyId)
                .OrderBy(c => c.ConnectionId)
                .ToListAsync();
                    
            return lobbyMembers;
        }
    }
}