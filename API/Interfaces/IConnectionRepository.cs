using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces
{
    public interface IConnectionRepository
    {
        Task<bool> CreateConnection(Connection connection);

        Task<bool> SessionExists(string username);
        Connection GetConnection(string username);
        string DeleteConnection(string username);
    }
}