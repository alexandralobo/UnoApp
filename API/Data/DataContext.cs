using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : IdentityDbContext<Guest, AppRole, int, IdentityUserClaim<int>, GuestRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            builder.EnableDetailedErrors()
                   .EnableSensitiveDataLogging();
        }

        public DbSet<Guest> Guests { get; set; }
        public DbSet<GameLobby> GameLobbies { get; set; }
        public DbSet<Connection> Connections { get; set; }
        public DbSet<Card> Cards { get; set; }
        //private DbSet<CardGameLobbyInPot> CardsInPot { get; set; }
        //private DbSet<CardGameLobbyDrawable> CardsDrawable { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<GameLobby>()
                .HasMany<Card>(t => t.CardPot)
                .WithMany(t => t.GameLobbyPots);
            builder.Entity<GameLobby>()
                .HasMany<Card>(t => t.DrawableCards)
                .WithMany(t => t.GameLobbyDrawables);   
            

            builder.Entity<Connection>().HasKey(k => k.ConnectionId);
            builder.Entity<Connection>()
                .HasMany<Card>(t => t.Cards)
                .WithMany(t => t.Connections);            

            /*builder.Entity<Connection>()
                .HasOne<GameLobby>(c => c.ConnectedGameLobby)
                .WithMany(g => g.Connections)
                .HasForeignKey(c => c.GameLobbyId);*/

            builder.Entity<Card>().HasKey(k => k.CardId);

            base.OnModelCreating(builder);
        }
    }
}