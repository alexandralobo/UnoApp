using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class ExistingGameDto
    {
        [Required] public int gameLobbyId { get; set; }
        public string password { get; set; }

    }
}
