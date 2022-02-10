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
        Task<GameLobby> GetGameLobbyAsync(int gameLobbyId);
        Task<GameLobby> CreateGame(GameLobby lobby);
        Task<GameLobby> AddGuestToLobby();
        Task<ICollection<Connection>> GetPlayersOfALobby(int gameLobbyId);
        bool VerifyCardsPlayed(List<Card> cards, Connection connection, GameLobby gameLobby);
        bool NextTurn(GameLobby gameLobby, ICollection<Connection> group, string username);
        Task StartGame(GameLobby lobby);

    }
}