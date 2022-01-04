using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Guest> Guests { get; set; }
        public DbSet<GameLobby> GameLobbies { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<Card> Cards { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<Connection>().HasKey(k => k.ConnectionId);

            //builder.Entity<Connection>().HasOne(u => u.Username).WithMany(c => );

            builder.Entity<GameLobby>().HasKey(k => k.GameLobbyId);

            builder.Entity<GameLobby>().HasMany<Card>(t => t.CardPot).WithMany(t => t.GameLobbies).UsingEntity<CardGameLobbyInPot>();
            builder.Entity<GameLobby>().HasMany<Card>(t => t.DrawableCards).WithMany(t => t.GameLobbies).UsingEntity<CardGameLobbyDrawable>();

            builder.Entity<Connection>().HasMany<Card>(t => t.Cards).WithMany(t => t.Connections);

            base.OnModelCreating(builder);
        }
    }
}