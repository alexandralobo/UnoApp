using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.DTOs
{
    public class GameLobbyDto
    {
        public int Id { get; set; }
        public ICollection<Connection> Connections { get; set; }
        public string CurrentPlayer { get; set; }
    }
}