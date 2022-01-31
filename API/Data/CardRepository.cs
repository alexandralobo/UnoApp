using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using API.Interfaces;
using AutoMapper;

namespace API.Data
{
    public class CardRepository : ICardRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        public CardRepository(DataContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }

        public async Task<bool> AddCard(Card card)
        {
            _context.Cards.Add(card);
            return true;
        }
    }
}