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
        Task<GameLobby> GetGameLobbyAsync(string gameLobbyId);
        Task<GameLobby> CreateGame(GameLobby lobby);
        Task<GameLobby> AddGuestToLobby();

    }
}