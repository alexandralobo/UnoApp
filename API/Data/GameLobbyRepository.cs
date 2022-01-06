using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public Task<IEnumerable<GameLobby>> GetGameLobbiesAsync()
        {
            throw new NotImplementedException();
        }

        /* async Task<IEnumerable<GameLobby>> GetGameLobbiesAsync()
        {
            return  await _context.GameLobbies.ToListAsync();
        }*/

        // public void AddGameLobby()
        // public void UpdateGameLobby()


    }
}