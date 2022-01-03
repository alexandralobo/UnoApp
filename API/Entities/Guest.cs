using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class Guest //: IdentityUser<int>
    {
        // identityUser already has a property username so I do not have to implement it here
        public string Id { get; set; }
        public string UserName { get; set; }
        //public DateTime Created { get; set; } = DateTime.Now;
        // public ICollection<Message> MessagesSent { get; set; }
        // public ICollection<Message> MessagesReceived { get; set; }
        // public ICollection<AppUsserRole> UserRoless { get; set; }
    }
}