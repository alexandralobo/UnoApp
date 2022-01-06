using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class GuestRepository : IGuestRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mappers;
        public GuestRepository(DataContext context, IMapper mappers)
        {
            _mappers = mappers;
            _context = context;
        }

        public async Task<bool> CreateGuest(Guest guest)
        {
            _context.Guests.Add(guest);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Guest> GetUserByIdAsync(int id)
        {
            return await _context.Guests.FindAsync(id);
        }

        public async Task<Guest> GetUserByUsernameAsync(string username)
        {
            return await _context.Guests
                .SingleOrDefaultAsync(x => x.UserName == username);
        }

        public async Task<IEnumerable<Guest>> GetUsersAsync()
        {
            return await _context.Guests.ToListAsync();
        }

        public async Task<bool> UserExists(string username)
        {
            return await _context.Guests.AnyAsync(user => user.UserName == username.ToLower());
        }
    }
}