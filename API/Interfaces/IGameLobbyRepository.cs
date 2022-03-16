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
        Task<GameLobby> GetGameLobbyByName(string name);
        Task<GameLobby> GetGameLobbyById(int id);
        Task<GameLobby> GetGameLobbyWithPassword(string pw);
        Task<GameLobby> StartGame(GameLobby lobby);
        Task<GameLobby> JoinExistingLobby(int gameLobbyId);
        Task<GameLobby> JoinNewLobby(string name);
        Task<Group> GetGroup(string groupName);
        Task<Group> GetGroupForConnection(string connectionId);
        Task RemoveConnection(GameLobby gameLobby, Group group, Connection connection);
        Task<bool> NextTurn(GameLobby gameLobby, Group group);
        Task<string> Play(Connection connection, GameLobby gameLobby, ICollection<Card> cards);
        Task<string> PlayWithChosenColour(Connection connection, GameLobby gameLobby, ICollection<Card> cards, string colour);
        Task<string> GetConsequence(GameLobby gameLobby, Group group, Connection connection, ICollection<Card> cards);
        Task<bool> PickColour(string colour);
        Task<Card> Draw(int quantity, GameLobby gameLobby, Connection connection);
        Task<bool> GetNewDeck(GameLobby gameLobby);
        Task<bool> Playable(Card pot, ICollection<Card> cards);
        Task<bool> PlayableWithColour(ICollection<Card> cards, string pickedColour);
        Task<string> UnoStatus(Connection connection);
    }
}