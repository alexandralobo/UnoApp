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
            // Verify if guest is already in a lobby
            /*var gameLobby = await _context.GameLobbies
                .Where(g => g.Connections.All(c => c.ConnectionId == connection.ConnectionId))
                .FirstOrDefaultAsync();*/


            // check if there is any lobby with less than 4 people
            var gameLobby = await _context.GameLobbies
                .Where(g => g.NumberOfElements < 4)
                .FirstOrDefaultAsync();

            if (gameLobby == null)
            {
                // ICollection<Connection> connections = new List<Connection>();

                // connections.Add(connection);

                gameLobby = new GameLobby
                {
                    // Connections = connections,
                    NumberOfElements = 1
                };

                _context.GameLobbies.Add(gameLobby);
            }
            else
            {
                // gameLobby.Connections.Add(connection);
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

        public async Task<bool> CreateGame(GameLobby lobby)
        {
            var group = await _context.Connections
                .Where(connection => connection.GameLobbyId == lobby.GameLobbyId)
                .OrderBy(c => c.ConnectionId)
                .ToListAsync();




            foreach (var player in group)
            {

            }

            throw new NotImplementedException();
        }

    }
}