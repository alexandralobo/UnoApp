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
        GameLobby GetGameLobbyByName(string name);
        GameLobby GetGameLobbyById(int id);
        GameLobby GetGameLobbyWithPassword(string pw);
        Task<GameLobby> StartGame(GameLobby lobby);
        Task<GameLobby> JoinExistingLobby(int gameLobbyId);
        Task<GameLobby> JoinNewLobby(string name);
        Task<Group> GetGroup(string groupName);
        Task<Group> GetGroupForConnection(string connectionId);
        void RemoveConnection(GameLobby gameLobby, Group group, Connection connection);
        bool NextTurn(GameLobby gameLobby, Group group);
        Task<string> Play(Connection connection, GameLobby gameLobby, ICollection<Card> cards);
        string PlayWithChosenColour(Connection connection, GameLobby gameLobby, ICollection<Card> cards, string colour);
        Task<string> GetConsequence(GameLobby gameLobby, Group group, Connection connection, ICollection<Card> cards);
        bool PickColour(string colour);
        Task<Card> Draw(int quantity, GameLobby gameLobby, Connection connection);
        Task<bool> GetNewDeck(GameLobby gameLobby);
        bool Playable(Card pot, ICollection<Card> cards);
        bool PlayableWithColour(ICollection<Card> cards, string pickedColour);
        string UnoStatus(Connection connection);
        string DeleteGame(int gameLobbyId);
    }
}