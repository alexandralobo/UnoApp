﻿// <auto-generated />
using System;
using API.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace API.Data.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20220106104544_RemovingTokenFromGuestEntity")]
    partial class RemovingTokenFromGuestEntity
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.1");

            modelBuilder.Entity("API.Entities.Card", b =>
                {
                    b.Property<int>("CardId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Colour")
                        .HasColumnType("TEXT");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Value")
                        .HasColumnType("INTEGER");

                    b.HasKey("CardId");

                    b.ToTable("Cards");
                });

            modelBuilder.Entity("API.Entities.CardGameLobbyDrawable", b =>
                {
                    b.Property<int>("CardId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameLobbyId")
                        .HasColumnType("INTEGER");

                    b.HasKey("CardId", "GameLobbyId");

                    b.HasIndex("GameLobbyId");

                    b.ToTable("CardGameLobbyDrawable");
                });

            modelBuilder.Entity("API.Entities.CardGameLobbyInPot", b =>
                {
                    b.Property<int>("CardId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("GameLobbyId")
                        .HasColumnType("INTEGER");

                    b.HasKey("CardId", "GameLobbyId");

                    b.HasIndex("GameLobbyId");

                    b.ToTable("CardGameLobbyInPot");
                });

            modelBuilder.Entity("API.Entities.Connection", b =>
                {
                    b.Property<string>("ConnectionId")
                        .HasColumnType("TEXT");

                    b.Property<int?>("GameLobbyId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Username")
                        .HasColumnType("TEXT");

                    b.HasKey("ConnectionId");

                    b.HasIndex("GameLobbyId");

                    b.ToTable("Connections");
                });

            modelBuilder.Entity("API.Entities.GameLobby", b =>
                {
                    b.Property<int>("GameLobbyId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("CurrentPlayerId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("GameStatus")
                        .HasColumnType("TEXT");

                    b.HasKey("GameLobbyId");

                    b.ToTable("GameLobbies");
                });

            modelBuilder.Entity("API.Entities.Guest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConcurrencyStamp")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedEmail")
                        .HasColumnType("TEXT");

                    b.Property<string>("NormalizedUserName")
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("TEXT");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("TEXT");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("UserName")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Guests");
                });

            modelBuilder.Entity("CardConnection", b =>
                {
                    b.Property<int>("CardsCardId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("ConnectionsConnectionId")
                        .HasColumnType("TEXT");

                    b.HasKey("CardsCardId", "ConnectionsConnectionId");

                    b.HasIndex("ConnectionsConnectionId");

                    b.ToTable("CardConnection");
                });

            modelBuilder.Entity("API.Entities.CardGameLobbyDrawable", b =>
                {
                    b.HasOne("API.Entities.Card", null)
                        .WithMany()
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API.Entities.GameLobby", null)
                        .WithMany()
                        .HasForeignKey("GameLobbyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("API.Entities.CardGameLobbyInPot", b =>
                {
                    b.HasOne("API.Entities.Card", null)
                        .WithMany()
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API.Entities.GameLobby", null)
                        .WithMany()
                        .HasForeignKey("GameLobbyId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("API.Entities.Connection", b =>
                {
                    b.HasOne("API.Entities.GameLobby", null)
                        .WithMany("Connections")
                        .HasForeignKey("GameLobbyId");
                });

            modelBuilder.Entity("CardConnection", b =>
                {
                    b.HasOne("API.Entities.Card", null)
                        .WithMany()
                        .HasForeignKey("CardsCardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API.Entities.Connection", null)
                        .WithMany()
                        .HasForeignKey("ConnectionsConnectionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("API.Entities.GameLobby", b =>
                {
                    b.Navigation("Connections");
                });
#pragma warning restore 612, 618
        }
    }
}
