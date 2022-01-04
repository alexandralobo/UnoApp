using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Entities
{
    public class Card
    {
        public Card()
        {
        }

        public Card(int cardId, string colour, int value, int type)
        {
            CardId = cardId;
            Colour = colour;
            Value = value;
            Type = type;
        }

        // Colour and Value can be null in some cases
        public int CardId { get; set; }
        public string Colour { get; set; }
        public int Value { get; set; }
        public int Type { get; set; }
        public ICollection<Connection> Connections { get; set; }
        public ICollection<GameLobby> GameLobbies { get; set; }
    }
}