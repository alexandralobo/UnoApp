using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Connection
    {
        public Connection()
        {
        }

        public int ConnectionId { get; set; }
        public string Username { get; set; }
        public ICollection<Card> Cards { get; set; } = new List<Card>();
        public int GameLobbyId { get; set; }
        public GameLobby ConnectedGameLobby { get; set; }
    }
}