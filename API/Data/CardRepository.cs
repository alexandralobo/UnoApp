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
        public CardRepository(DataContext context)
        {
            _context = context;
        }

        public ICollection<Card> GetDeck()
        {
            return _context.Cards.ToList();
        }

        public bool AddCard(Card card)
        {
            _context.Cards.Add(card);
            return true;
        }

        public async Task<Card> GetCard(int cardId)
        {
            Card card = await _context.Cards.FindAsync(cardId);
            return card;
        }
    }
}