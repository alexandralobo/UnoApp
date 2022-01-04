using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.EphemeralData;

namespace API.Entities
{
    public class GameLobby
    {
        public GameLobby()
        {
        }

        public int GameLobbyId { get; set; }
        public ICollection<Connection> Connections { get; set; }
        public ICollection<Card> DrawableCards { get; set; }
        public ICollection<Card> CardPot { get; set; }
        public int CurrentPlayerId { get; set; }
        public string GameStatus { get; set; } = "ongoing";
    }
}