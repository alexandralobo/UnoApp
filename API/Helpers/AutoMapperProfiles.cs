using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using AutoMapper;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<Guest, UserWithTokenDto>();
            CreateMap<UserWithTokenDto, Guest>();
            CreateMap<GuestDto, Guest>();
            CreateMap<Guest, GuestDto>();
            CreateMap<LoginUser, SignUpDto>();
            CreateMap<SignUpDto, LoginUser>();
            CreateMap<GameLobby, GameLobbyDto>();
            CreateMap<GameLobbyDto, GameLobby>();
        }
    }
}