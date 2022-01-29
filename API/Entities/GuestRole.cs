using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class GuestRole : IdentityUserRole<int>
    {
        public Guest Guest { get; set; }

        public AppRole Role { get; set; }
    }
}