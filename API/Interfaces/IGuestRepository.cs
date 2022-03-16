using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces
{
    public interface IGuestRepository
    {
        Task<bool> CreateGuest(Guest guest);
        Task<bool> SignUp(LoginUser loginUser);
        Task<IEnumerable<Guest>> GetUsersAsync();
        Task<Guest> GetUserByIdAsync(int id);
        Task<Guest> GetGuestByUsernameAsync(string username);
        Task<LoginUser> GetLoginUserByUsernameAsync(string username);
        Task<bool> UserExists(string username);
    }
}