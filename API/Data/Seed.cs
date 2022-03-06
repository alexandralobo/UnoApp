using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.Entities;

namespace API.Data
{
    public class Seed
    {
        public static async Task SeedCards(DataContext context)
        {
            if (context.Cards.Any()) return;

            var cardData = await System.IO.File.ReadAllTextAsync("Data/CardsData.json");
            var cards = JsonSerializer.Deserialize<List<Card>>(cardData, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            if (cards == null) return;

            foreach (var card in cards)
            {
                if (card.Value == -1)
                {
                    card.FileName = card.Colour.ElementAt(0) + "" + card.Type + ".jpg";
                } else
                {
                    card.FileName = card.Colour.ElementAt(0) + "" + card.Value + ".jpg";
                }
                await context.Cards.AddAsync(card);
            }

            await context.SaveChangesAsync();
        }
    }
}