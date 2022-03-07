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

            var gameLobby = await _unitOfWork.GameLobbyRepository.GetGameLobbyById(gameLobbyId);
            var groupName = gameLobby.GameLobbyName;                 
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var group = await AddToLobby(groupName, gameLobby);           
            

            if (_unitOfWork.HasChanges()) await _unitOfWork.Complete();

            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);
            await Clients.Group(groupName).SendAsync("GetGameLobby", gameLobby);

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            //var group = await RemoveFromLobby();
            //await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
            //await base.OnDisconnectedAsync(exception);
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
            var connection = new Connection();

            // case where the user disconnects
            if (gameLobby.GameStatus == "ongoing")
            {
                connection = await _unitOfWork.ConnectionRepository.GetConnection(Context.User.GetUsername());
                if (connection == null) throw new HubException("This game already started!");
                connection.ConnectionId = Context.ConnectionId;

            } 
            else if (gameLobby.GameStatus == "waiting")
            {
                connection = new Connection
                {
                    ConnectionId = Context.ConnectionId,
                    Username = Context.User.GetUsername(),
                    GameLobbyId = gameLobby.GameLobbyId,
                    ConnectedGameLobby = gameLobby
                };
                await _unitOfWork.ConnectionRepository.CreateConnection(connection);

            } else
            {
                throw new HubException("Game finished");
            }  
            var group = await _unitOfWork.GameLobbyRepository.GetGroup(groupName);            
            group.Connections.Add(connection);
            

            if (await _unitOfWork.Complete()) return group;
            throw new HubException("Failed to join group");

        }

        private async Task<Group> RemoveFromLobby()
        {
            var group = await _unitOfWork.GameLobbyRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            var gameLobby = await _unitOfWork.GameLobbyRepository.GetGameLobbyByName(group.Name);

            await _unitOfWork.GameLobbyRepository.RemoveConnection(gameLobby, group, connection);
            if (await _unitOfWork.Complete()) return group;
            throw new HubException("Failed to remove from group");

        }

        // Currently working on this
        public async Task<GameLobby> StartGame(int id)
        {
            var gameLobby = await _unitOfWork.GameLobbyRepository.GetGameLobbyById(id);
            if (gameLobby == null) throw new HubException("That game lobby does not exist!");
            
            if (gameLobby.NumberOfElements < 2) throw new HubException("Waiting for more players");

            if (gameLobby.GameStatus == "ongoing") throw new HubException("The game has started.");

            gameLobby = await _unitOfWork.GameLobbyRepository.StartGame(gameLobby);

            var group = await _unitOfWork.GameLobbyRepository.GetGroup(gameLobby.GameLobbyName);

            if (await _unitOfWork.Complete())
            {
                await Clients.Group(group.Name).SendAsync("GetGameLobby", gameLobby);
                await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
                return gameLobby;
            } else
            {
                throw new HubException("Failed to initialize the game!");
            }           
        }

        public async Task<string> Play(/*string username, int gameLobbyId,*/ List<Card> cards)
        {
            var httpContext = Context.GetHttpContext();
            var gameLobbyId = Int32.Parse(httpContext.Request.Query["lobbyId"].ToString());
            //var gameLobbyId = Int32.Parse(strgameLobbyId);
            var username = Context.User.GetUsername();

            GameLobby gameLobby = await _unitOfWork.GameLobbyRepository.GetGameLobbyById(gameLobbyId);
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

            if (await _unitOfWork.Complete())
            {
                await Clients.Group(group.Name).SendAsync("GetGameLobby", gameLobby);
                await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
                return message;
            } else
            {
                throw new HubException("Couldnt save your play!");
            }
        }

        public async Task<string> PlayWithChosenColour(ICollection<Card> cards, string colour)
        {
            var httpContext = Context.GetHttpContext();
            var gameLobbyId = Int32.Parse(httpContext.Request.Query["lobbyId"].ToString());
            var username = Context.User.GetUsername();

            GameLobby gameLobby = await _unitOfWork.GameLobbyRepository.GetGameLobbyById(gameLobbyId);
            if (gameLobby.CurrentPlayer != username) return "It is " + gameLobby.CurrentPlayer + "'s turn, it is not your turn!";

            var group = await _unitOfWork.GameLobbyRepository.GetGroup(gameLobby.GameLobbyName);
            Connection connection = await _unitOfWork.ConnectionRepository.GetConnection(username);

            var message = await _unitOfWork.GameLobbyRepository.PlayWithChosenColour(connection, gameLobby, cards, colour);
            if (message != "Next") throw new HubException(message);

           
            gameLobby.PickedColour = "none";

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

            if (await _unitOfWork.Complete())
            {
                await Clients.Group(group.Name).SendAsync("GetGameLobby", gameLobby);
                await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
                return message;
            }
            else
            {
                throw new HubException("Couldnt save your play!");
            }

        }
        public async Task<string> GetCard()
        {
            var httpContext = Context.GetHttpContext();
            var gameLobbyId = Int32.Parse(httpContext.Request.Query["lobbyId"].ToString());

            GameLobby gameLobby = await _unitOfWork.GameLobbyRepository.GetGameLobbyById(gameLobbyId);
            var group = await _unitOfWork.GameLobbyRepository.GetGroup(gameLobby.GameLobbyName);
            Connection connection = await _unitOfWork.ConnectionRepository.GetConnection(gameLobby.CurrentPlayer);

            //if (connection.Cards.Count() == 0 /*&& gameLobby.GameStatus != "finished"*/)
            //{
            //    await _unitOfWork.GameLobbyRepository.Draw(4, gameLobby, connection);
            //    return "Next";
            //}

            Card pot = await _unitOfWork.CardRepository.GetCard(gameLobby.LastCard);
            ICollection<Card> cards = connection.Cards;

            if (connection.Cards.Count == 0)
            {
                cards = new List<Card>();
            }

            // verify if the current player have a valid card to play
            bool playable = await _unitOfWork.GameLobbyRepository.Playable(gameLobby, pot, cards);
            if (playable) return "You have cards that you can play!";

            // get a card from deck until we can play
            Card cardFromDeck = new Card();
            // working - test with more cases
            do
            {
                cardFromDeck = await _unitOfWork.GameLobbyRepository.Draw(1, gameLobby, connection);
            } while (!(pot.Value == cardFromDeck.Value && pot.Value != -1
                || pot.Type == cardFromDeck.Type && cardFromDeck.Type != "Number"
                || pot.Colour == cardFromDeck.Colour
                || cardFromDeck.Type == "Wild"
                || cardFromDeck.Type == "Wild Draw 4"));

            if (await _unitOfWork.Complete()) {

                await Clients.Group(group.Name).SendAsync("GetGameLobby", gameLobby);
                await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
                return "You obtained the cards required!";
            } else
            {
                throw new HubException("Could not get the cards!");
            }
        }

        public async Task<string> PickColour(string colour)
        {
            var httpContext = Context.GetHttpContext();
            var gameLobbyId = Int32.Parse(httpContext.Request.Query["lobbyId"].ToString());

            bool validate = await _unitOfWork.GameLobbyRepository.PickColour(colour);
            if (!validate) throw new HubException("Colour is not valid!");

            GameLobby gameLobby = await _unitOfWork.GameLobbyRepository.GetGameLobbyById(gameLobbyId);
            gameLobby.PickedColour = colour;

            var group = await _unitOfWork.GameLobbyRepository.GetGroup(gameLobby.GameLobbyName);

            bool turn = await _unitOfWork.GameLobbyRepository.NextTurn(gameLobby, group);
            if (!turn) throw new HubException("It is not possible get to the next turn!");

            if (await _unitOfWork.Complete())
            {
                await Clients.Group(group.Name).SendAsync("GetGameLobby", gameLobby);
                await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
                return "Next";
            }
            throw new HubException("Couldn't save your play!");
        }

    }
}