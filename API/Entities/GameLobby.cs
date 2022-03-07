using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class GameLobby
    {
        public GameLobby()
        {
        }

        public int GameLobbyId { get; set; }
        public string GameLobbyName { get; set; }
        public ICollection<Card> DrawableCards { get; set; }
        public ICollection<Card> CardPot { get; set; } = new List<Card>();
        public int LastCard { get; set; }
        public string FileName { get; set; }
        public string PickedColour { get; set; } = "none";
        public string CurrentPlayer { get; set; }
        public string GameStatus { get; set; } = "waiting";
        public int NumberOfElements { get; set; } = 0;
        public string order { get; set; } = "normal";
    }
}