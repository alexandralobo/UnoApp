using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs
{
    public class UserWithTokenDto
    {
        public string Username { get; set; }
        public string Token { get; set; }   
    }
}