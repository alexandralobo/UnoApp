using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.DTOs
{
    public class GameLobbyDto
    {
        // public ICollection<Connection> Connections { get; set; }
        public string lobbyName { get; set; }
        public int NumberOfElements { get; set; }
    }
}