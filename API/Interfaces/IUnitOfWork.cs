using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Interfaces
{
    public interface IUnitOfWork
    {
        IMemberRepository MemberRepository { get; }
        IGameLobbyRepository GameLobbyRepository { get; }
        IConnectionRepository ConnectionRepository { get; }
        ICardRepository CardRepository { get; }
        Task<bool> Complete();
        bool HasChanges();
    }
}