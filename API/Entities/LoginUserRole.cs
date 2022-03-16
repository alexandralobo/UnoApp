using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class LoginUserRole : IdentityUserRole<int>
    {
        public LoginUser LoginUser { get; set; }

        public AppRole Role { get; set; }
    }
}