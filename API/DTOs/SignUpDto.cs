using System.ComponentModel.DataAnnotations;

namespace API.DTOs
{
    public class SignUpDto
    {
        [Required] public string Username { get; set; }
        [Required] public string Password { get; set; }
        [Required] public DateTime DateOfBirth { get; set; }
    }
}
