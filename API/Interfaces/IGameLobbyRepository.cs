using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces
{
    public interface IGameLobbyRepository
    {
        Task<ICollection<GameLobby>> GetGameLobbiesAsync();
        Task<GameLobby> GetGameLobbyAsync(int gameLobbyId);
        Task<GameLobby> CreateGame(GameLobby lobby);
        Task<GameLobby> JoinExistingLobby(int gameLobbyId);
        GameLobby JoinNewLobby();
        Task<ICollection<Connection>> GetPlayersOfALobby(int gameLobbyId);
        //bool VerifyCardsPlayed(List<Card> cards, Connection connection, GameLobby gameLobby);
        Task<bool> NextTurn(GameLobby gameLobby, ICollection<Connection> group);
        Task<string> Play(Connection connection, GameLobby gameLobby, ICollection<Card> cards);
        Task<string> PlayWithChosenColour(Connection connection, GameLobby gameLobby, ICollection<Card> cards, string colour);
        Task<string> GetConsequence(GameLobby gameLobby, ICollection<Connection> group, Connection connection, ICollection<Card> cards);
        Task<bool> PickColour(string colour);
        Task<Card> Draw(int quantity, GameLobby gameLobby, Connection connection);
        Task<bool> GetNewDeck(GameLobby gameLobby);
        Task<bool> Playable(GameLobby gameLobby, Card pot, ICollection<Card> cards);
    }
}