using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces
{
    public interface IGameLobbyRepository
    {
        Task<IEnumerable<GameLobby>> GetGameLobbiesAsync();

        // Task<GameLobby> CreateGameLobby(GameLobby lobby);

        Task<GameLobby> AddGuestToLobby(Connection connection);

        Task<bool> SessionExists(string username);


    }
}