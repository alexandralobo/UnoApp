using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class Guest
    {
        public int Id { get; set; }
        public string UserName { get; set; }
    }
}