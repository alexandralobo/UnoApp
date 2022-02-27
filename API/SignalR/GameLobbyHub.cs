using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using API.SignalIR;
using AutoMapper;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{
    // incomplete
    public class GameLobbyHub : Hub
    {
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly PresenceTracker _tracker;
        private readonly IUnitOfWork _unitOfWork;

        public GameLobbyHub(IUnitOfWork unitOfWork, IMapper mapper,
            IHubContext<PresenceHub> presenceHub, PresenceTracker tracker)
        {
            _unitOfWork = unitOfWork;
            _tracker = tracker;
            _presenceHub = presenceHub;
            _mapper = mapper;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();

            var strgameLobbyId = httpContext.Request.Query["lobbyId"].ToString();
            var gameLobbyId = Int32.Parse(strgameLobbyId);
            var guest = Context.User;

            // later check if it works 
            //var gameLobbies = GetLobbies();
            //await Clients.Caller.SendAsync("GetGameLobbies", gameLobbies);

            var gameLobby = await _unitOfWork.GameLobbyRepository.GetGameLobbyAsync(gameLobbyId);
            var groupName = gameLobby.GameLobbyName;                 
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var group = await AddToLobby(groupName, gameLobby);
            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);
            

            if (_unitOfWork.HasChanges()) await _unitOfWork.Complete();

            // Clients of the lobby get game lobby
            await Clients.Caller.SendAsync("GetGameLobby", gameLobby);

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveFromLobby();
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
            await base.OnDisconnectedAsync(exception);
        }        

        private async Task<ICollection<GameLobby>> GetLobbies()
        {
            return await _unitOfWork.GameLobbyRepository.GetGameLobbiesAsync();          
        }
        //private async Task<GameLobby> GetLobby(int gameLobbyId)
        //{

        //    return await _unitOfWork.GameLobbyRepository.GetGameLobbyAsync(gameLobbyId);
        //}

        private async Task<Group> AddToLobby(string groupName, GameLobby gameLobby)
        {
            //var connection = new Connection(Context.ConnectionId, Context.User.GetUsername()/*, game*/);

            var connection = new Connection
            {
                ConnectionId = Context.ConnectionId,
                Username = Context.User.GetUsername(),
                GameLobbyId = gameLobby.GameLobbyId,
                ConnectedGameLobby = gameLobby
            };

            await _unitOfWork.ConnectionRepository.CreateConnection(connection);            

            var group = await _unitOfWork.GameLobbyRepository.GetGroup(groupName);            
            group.Connections.Add(connection);
            

            if (await _unitOfWork.Complete()) return group;
            throw new HubException("Failed to join group");

        }

        private async Task<Group> RemoveFromLobby()
        {
            var group = await _unitOfWork.GameLobbyRepository.GetLobbyForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);

            await _unitOfWork.GameLobbyRepository.RemoveConnection(connection);
            if (await _unitOfWork.Complete()) return group;
            throw new HubException("Failed to remove from group");

        }

        public async Task StartGame(int id)
        {
            var gameLobby = await _unitOfWork.GameLobbyRepository.GetGameLobbyAsync(id);
            if (gameLobby == null) throw new HubException("That game lobby does not exist!");
            
            if (gameLobby.NumberOfElements < 2) throw new HubException("Waiting for more players");

            if (gameLobby.GameStatus == "ongoing") throw new HubException("The game has started.");

            gameLobby = await _unitOfWork.GameLobbyRepository.StartGame(gameLobby);

            if (await _unitOfWork.Complete()) return;
            throw new HubException("Failed to initialize the game!");
        }

        public async Task<string> Play(string username, int gameLobbyId, List<Card> cards)
        {
            GameLobby gameLobby = await _unitOfWork.GameLobbyRepository.GetGameLobbyAsync(gameLobbyId);
            if (gameLobby.CurrentPlayer != username) throw new HubException("It is " + gameLobby.CurrentPlayer + "'s turn, it is not your turn!");
           
            var group = await _unitOfWork.GameLobbyRepository.GetGroup(gameLobby.GameLobbyName);
            Connection connection = await _unitOfWork.ConnectionRepository.GetConnection(username);

            string message = await _unitOfWork.GameLobbyRepository.Play(connection, gameLobby, cards);
            if (message != "Next") throw new HubException(message);

            if (cards.First().Value == -1)
            {
                int i = 0;
                while (i < cards.Count())
                {
                    message = await _unitOfWork.GameLobbyRepository.GetConsequence(gameLobby, group, connection, cards);
                    if (message == "Card type is incorrect!") throw new HubException(message);
                    i++;
                }
            }

            if (message != "Pick a colour")
            {
                // get the next turn
                var turn = await _unitOfWork.GameLobbyRepository.NextTurn(gameLobby, group);
                if (!turn) throw new HubException("I cannot get to the next turn!");
            }

            if (await _unitOfWork.Complete()) return message;
            throw new HubException("Couldnt save your play!");
        }

    }
}