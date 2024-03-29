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
    public class ConnectionRepository : IConnectionRepository
    {
        private readonly DataContext _context;
        public ConnectionRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> CreateConnection(Connection connection)
        {
            return await _context.Connections.AddAsync(connection) != null;
        }

        public async Task<bool> SessionExists(string username)
        {
            return await _context.Connections.AnyAsync(c => c.Username == username);
        }

        public Connection GetConnection(string username)
        {
            return _context.Connections
                .Where(c => c.Username == username)
                .Include(c => c.Cards)
                .FirstOrDefault();
        }

        public string DeleteConnection(string username)
        {
            var connection = _context.Connections
                .Where(c => c.Username == username)
                .FirstOrDefault();

            var removed =_context.Connections.Remove(connection);

            if (removed != null) return "Connection w/ username " + username + " deleted!";
            return "Connection does not exist";
        }
    }
}