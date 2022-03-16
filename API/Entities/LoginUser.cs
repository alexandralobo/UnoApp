using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace API.Entities
{
    public class LoginUser : IdentityUser<int>
    {
        [Required] public DateTime DateOfBirth { get; set; }     
    }
}
