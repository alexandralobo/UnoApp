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
        Task<IEnumerable<Guest>> GetUsersAsync();
        Task<Guest> GetUserByIdAsync(int id);
        Task<Guest> GetUserByUsernameAsync(string username);
        Task<bool> UserExists(string username);
    }
}