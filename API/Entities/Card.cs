using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace API.Entities
{
    public class Card
    {
        public Card()
        {
        }

        public Card(string colour, int value, string type)
        {
            Colour = colour;
            Value = value;
            Type = type;
        }

        // Colour and Value can be null in some cases
        public int CardId { get; set; }
        public string Colour { get; set; }
        public int Value { get; set; }
        public string Type { get; set; }
        public string FileName { get; set; }

        [JsonIgnore]
        public ICollection<Connection> Connections { get; set; } = new List<Connection>();
        [JsonIgnore]
        public ICollection<GameLobby> GameLobbyPots { get; set; } = new List<GameLobby>();
        [JsonIgnore]
        public ICollection<GameLobby> GameLobbyDrawables { get; set; } = new List<GameLobby>();

    }
}