using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using AutoMapper;

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

        public async Task CreateConnection(Connection connection)
        {
            _context.Connections.Add(connection);
        }
    }
}