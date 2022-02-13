using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.Interfaces
{
    public interface ICardRepository
    {
        Task<bool> AddCard(Card card);
        Task<Card> GetCard(int id);
    }
}