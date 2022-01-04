using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IGuestRepository GuestRepository { get; }
        IGameLobbyRepository GameLobbyRepository { get; }
        Task<bool> Complete();
    }
}