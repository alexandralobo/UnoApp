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
        private readonly IMapper _mapper;
        public ConnectionRepository(DataContext context, IMapper mapper)
        {

            _mapper = mapper;
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
    }
}