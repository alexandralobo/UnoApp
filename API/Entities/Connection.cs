using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Connection
    {
        public Connection() { }
        //public Connection(string connectionId, string username)
        //{
        //    ConnectionId = connectionId;
        //    Username = username;
        //    //GameLobbyId = connectedGameLobby.GameLobbyId;
        //    //ConnectedGameLobby = connectedGameLobby;
        //}

        public string ConnectionId { get; set; }
        public string Username { get; set; }
        public ICollection<Card> Cards { get; set; } = new List<Card>();
        
        public bool Uno { get; set; } = false;

        [JsonIgnore]
        public int GameLobbyId { get; set; }

        [JsonIgnore]
        public GameLobby ConnectedGameLobby { get; set; }
    }
}