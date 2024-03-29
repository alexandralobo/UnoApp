using System.Text;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR
{    
    public class GameLobbyHub : Hub
    {
        private readonly IUnitOfWork _unitOfWork;

        public GameLobbyHub(
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var strgameLobbyId = httpContext.Request.Query["lobbyId"].ToString();
            var gameLobbyId = Int32.Parse(strgameLobbyId);           

            // later check if it works 
            //var gameLobbies = GetLobbies();
            //await Clients.Caller.SendAsync("GetGameLobbies", gameLobbies);

            var gameLobby = _unitOfWork.GameLobbyRepository.GetGameLobbyById(gameLobbyId);
            //if (gameLobby != null) throw new HubException("No Game Available");

            var groupName = gameLobby.GameLobbyName;                 
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

            var group = await AddToLobby(groupName, gameLobby);           
            

            if (_unitOfWork.HasChanges()) await _unitOfWork.Complete();

            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);
            await Clients.Group(groupName).SendAsync("GetGameLobby", gameLobby);

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveFromLobby();
            if (group != null)
            {
                await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
            }            
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
            Connection connection;

            // case where the user disconnects
            if (gameLobby.GameStatus == "ongoing")
            {
                connection = _unitOfWork.ConnectionRepository.GetConnection(Context.User.GetUsername());
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
            if (group == null) return group;

            if (group.Connections.Count > 0)
            {
                var gameLobby = _unitOfWork.GameLobbyRepository.GetGameLobbyByName(group.Name);
                var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
                _unitOfWork.GameLobbyRepository.RemoveConnection(gameLobby, group, connection);
            }          
            
            if (await _unitOfWork.Complete()) return group;
            throw new HubException("Failed to remove from group");

        }

        public async Task<string> IsPrivate(bool privateRoom)
        {
            var httpContext = Context.GetHttpContext();
            var gameLobbyId = Int32.Parse(httpContext.Request.Query["lobbyId"].ToString());
            var gameLobby = _unitOfWork.GameLobbyRepository.GetGameLobbyById(gameLobbyId);
            var group = await _unitOfWork.GameLobbyRepository.GetGroup(gameLobby.GameLobbyName);

            if (privateRoom) 
            {
                StringBuilder builder = new StringBuilder();
                Random random = new();
                char ch;
                for (int i = 0; i < 10; i++)
                {
                    ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                    builder.Append(ch);
                }

                gameLobby.Password = builder.ToString();
            } else
            {
                gameLobby.Password = "";
            }

            if (await _unitOfWork.Complete())
            {
                await Clients.Group(group.Name).SendAsync("GetGameLobby", gameLobby);
                return gameLobby.Password;
            }
            else
            {
                throw new HubException("Failed to make it private/public the game!");
            }
        }        
        
        public async Task<GameLobby> StartGame()
        {
            var httpContext = Context.GetHttpContext();
            var gameLobbyId = Int32.Parse(httpContext.Request.Query["lobbyId"].ToString());

            var gameLobby = _unitOfWork.GameLobbyRepository.GetGameLobbyById(gameLobbyId);
            if (gameLobby == null) throw new HubException("That game lobby does not exist!");

            if (gameLobby.NumberOfElements < 2) throw new HubException("Waiting for more players");

            if (gameLobby.GameStatus == "ongoing") throw new HubException("The game has already started.");

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

        public async Task<string> Play(List<Card> cards)
        {
            var httpContext = Context.GetHttpContext();
            var gameLobbyId = Int32.Parse(httpContext.Request.Query["lobbyId"].ToString());
            var username = Context.User.GetUsername();

            GameLobby gameLobby = _unitOfWork.GameLobbyRepository.GetGameLobbyById(gameLobbyId);
            if (gameLobby.CurrentPlayer != username) throw new HubException("It is " + gameLobby.CurrentPlayer + "'s turn, it is not your turn!");
           
            var group = await _unitOfWork.GameLobbyRepository.GetGroup(gameLobby.GameLobbyName);
            Connection connection = _unitOfWork.ConnectionRepository.GetConnection(username);

            string message = await _unitOfWork.GameLobbyRepository.Play(connection, gameLobby, cards);
            if (message != "Next") throw new HubException(message);

            if (cards.First().Value == -1)
            {
                int i = 0;
                while (i < cards.Count)
                {
                    message = await _unitOfWork.GameLobbyRepository.GetConsequence(gameLobby, group, connection, cards);
                    if (message == "Card type is incorrect!") throw new HubException(message);
                    i++;
                }
            }   

            var numberOfCards = connection.Cards.Count;
            if  (numberOfCards == 0)
            {
                gameLobby.Winner = gameLobby.CurrentPlayer;
                gameLobby.GameStatus = "finished";
            }

            if (message != "Pick a colour" && gameLobby.GameStatus != "finished")
            {
                var turn = _unitOfWork.GameLobbyRepository.NextTurn(gameLobby, group);
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

            GameLobby gameLobby = _unitOfWork.GameLobbyRepository.GetGameLobbyById(gameLobbyId);
            if (gameLobby.CurrentPlayer != username) return "It is " + gameLobby.CurrentPlayer + "'s turn, it is not your turn!";

            var group = await _unitOfWork.GameLobbyRepository.GetGroup(gameLobby.GameLobbyName);
            Connection connection = _unitOfWork.ConnectionRepository.GetConnection(username);

            var message = _unitOfWork.GameLobbyRepository.PlayWithChosenColour(connection, gameLobby, cards, colour);
            if (message != "Next") throw new HubException(message);
           
            gameLobby.PickedColour = "none";

            if (cards.First().Value == -1)
            {
                int i = 0;
                while (i < cards.Count)
                {
                    message = await _unitOfWork.GameLobbyRepository.GetConsequence(gameLobby, group, connection, cards);
                    if (message == "Card type is incorrect!") throw new HubException(message);
                    i++;
                }
            }

            var numberOfCards = connection.Cards.Count;
            if (numberOfCards == 0)
            {
                gameLobby.Winner = gameLobby.CurrentPlayer;
                gameLobby.GameStatus = "finished";
            }

            if (message != "Pick a colour" && gameLobby.GameStatus != "finished")
            {
                var turn = _unitOfWork.GameLobbyRepository.NextTurn(gameLobby, group);
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

        public async Task<string> PickColour(string colour)
        {
            var httpContext = Context.GetHttpContext();
            var gameLobbyId = Int32.Parse(httpContext.Request.Query["lobbyId"].ToString());

            bool validate = _unitOfWork.GameLobbyRepository.PickColour(colour);
            if (!validate) throw new HubException("Colour is not valid!");

            GameLobby gameLobby = _unitOfWork.GameLobbyRepository.GetGameLobbyById(gameLobbyId);
            gameLobby.PickedColour = colour;

            var group = await _unitOfWork.GameLobbyRepository.GetGroup(gameLobby.GameLobbyName);
            
            bool turn = _unitOfWork.GameLobbyRepository.NextTurn(gameLobby, group);
            if (!turn) throw new HubException("It is not possible get to the next turn!");

            if (gameLobby.FileName == "bWildDraw4")
            {
                turn = _unitOfWork.GameLobbyRepository.NextTurn(gameLobby, group);
            }

            if (await _unitOfWork.Complete())
            {
                await Clients.Group(group.Name).SendAsync("GetGameLobby", gameLobby);
                await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
                return "Next";
            }
            throw new HubException("Couldn't save your play!");
        }

        public async Task<string> GetCard()
        {
            var httpContext = Context.GetHttpContext();
            var gameLobbyId = Int32.Parse(httpContext.Request.Query["lobbyId"].ToString());

            GameLobby gameLobby = _unitOfWork.GameLobbyRepository.GetGameLobbyById(gameLobbyId);
            Group group = await _unitOfWork.GameLobbyRepository.GetGroup(gameLobby.GameLobbyName);
            Connection connection = _unitOfWork.ConnectionRepository.GetConnection(gameLobby.CurrentPlayer);                      

            Card pot = await _unitOfWork.CardRepository.GetCard(gameLobby.LastCard);
            ICollection<Card> cards = connection.Cards;

            // verify if the current player has a valid card to play
            bool playable = _unitOfWork.GameLobbyRepository.Playable(pot, cards);
            if (playable) return "You have cards that you can play!";

            if (connection.Uno == true) connection.Uno = false;

            // get a card from deck until the player has 'playable' card
            Card cardFromDeck;
            do
            {
                cardFromDeck = await _unitOfWork.GameLobbyRepository.Draw(1, gameLobby, connection);
            } while (!(pot.Value == cardFromDeck.Value && pot.Value != -1
                || pot.Type == cardFromDeck.Type && cardFromDeck.Type != "Number"
                || pot.Colour == cardFromDeck.Colour
                || cardFromDeck.Type == "Wild"
                || cardFromDeck.Type == "WildDraw4"));

            if (await _unitOfWork.Complete())
            {
                await Clients.Group(group.Name).SendAsync("GetGameLobby", gameLobby);
                await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
                return "You obtained the cards required!";
            }
            else
            {
                throw new HubException("Could not get the cards!");
            }
        }

        public async Task<string> GetCardWithChosenColour(string colour)
        {
            var httpContext = Context.GetHttpContext();
            var gameLobbyId = Int32.Parse(httpContext.Request.Query["lobbyId"].ToString());

            GameLobby gameLobby = _unitOfWork.GameLobbyRepository.GetGameLobbyById(gameLobbyId);
            var group = await _unitOfWork.GameLobbyRepository.GetGroup(gameLobby.GameLobbyName);
            Connection connection = _unitOfWork.ConnectionRepository.GetConnection(gameLobby.CurrentPlayer);      

            ICollection<Card> cards = connection.Cards;

            // verify if the current player has a valid card to play
            bool playable = _unitOfWork.GameLobbyRepository.PlayableWithColour(cards, colour);
            if (playable) return "You have cards that you can play!";

            if (connection.Uno == true) connection.Uno = false;

            // get a card from deck until the player has 'playable' card
            Card cardFromDeck;
            do
            {
                cardFromDeck = await _unitOfWork.GameLobbyRepository.Draw(1, gameLobby, connection);
            } while (!(colour == cardFromDeck.Colour
                || cardFromDeck.Type == "Wild"
                || cardFromDeck.Type == "WildDraw4"));

            if (await _unitOfWork.Complete())
            {

                await Clients.Group(group.Name).SendAsync("GetGameLobby", gameLobby);
                await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
                return "You obtained the cards required!";
            }
            else
            {
                throw new HubException("Could not get the cards!");
            }
        }
    
        public async Task<string> ChangeUnoStatus()
        {
            var httpContext = Context.GetHttpContext();
            var gameLobbyId = Int32.Parse(httpContext.Request.Query["lobbyId"].ToString());

            GameLobby gameLobby = _unitOfWork.GameLobbyRepository.GetGameLobbyById(gameLobbyId);
            var group = await _unitOfWork.GameLobbyRepository.GetGroup(gameLobby.GameLobbyName);               
            
            Connection connection = _unitOfWork.ConnectionRepository.GetConnection(Context.User.GetUsername());

            var message = _unitOfWork.GameLobbyRepository.UnoStatus(connection);
            if (message == "You cannot say uno!") throw new HubException(message);
                       
            if (await _unitOfWork.Complete())
            {
                await Clients.Group(group.Name).SendAsync("GetGameLobby", gameLobby);
                await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
                return message;
            }
            else
            {
                throw new HubException("Could not change the uno status!");
            }
        }

        public async Task<string> CatchUno(string username)
        {
            Connection connection = _unitOfWork.ConnectionRepository.GetConnection(username);
            if (connection.Uno == true) return "Too late! " + connection.Username + " has already said UNO!";

            var httpContext = Context.GetHttpContext();
            var gameLobbyId = Int32.Parse(httpContext.Request.Query["lobbyId"].ToString());
            GameLobby gameLobby = _unitOfWork.GameLobbyRepository.GetGameLobbyById(gameLobbyId);
            var group = await _unitOfWork.GameLobbyRepository.GetGroup(gameLobby.GameLobbyName);

            await _unitOfWork.GameLobbyRepository.Draw(4, gameLobby, connection);

            if (await _unitOfWork.Complete())
            {
                await Clients.Group(group.Name).SendAsync("GetGameLobby", gameLobby);
                await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);
                return ("Good catch! " + connection.Username + " got 4 cards!");
            }
            else
            {
                throw new HubException("Could not change the uno status!");
            }
        }

        public async Task<string> FinishedGame()
        {
            var httpContext = Context.GetHttpContext();
            var gameLobbyId = Int32.Parse(httpContext.Request.Query["lobbyId"].ToString());

            var msg = _unitOfWork.GameLobbyRepository.DeleteGame(gameLobbyId);
            if (msg == "Connection does not exist") return msg;

            GameLobby gameLobby = _unitOfWork.GameLobbyRepository.GetGameLobbyById(gameLobbyId);
            Group group = await _unitOfWork.GameLobbyRepository.GetGroup(gameLobby.GameLobbyName);

            foreach (Connection connection in group.Connections)
            {
                _unitOfWork.ConnectionRepository.DeleteConnection(connection.Username);
            }

            if (await _unitOfWork.Complete()) return "Game deleted!";
            throw new HubException("Could not delete the game!");
        }
    }
}