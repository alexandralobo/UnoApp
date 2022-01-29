using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class GameLobbyController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GameLobbyController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<GameLobbyDto>>> GetLobbies()
        {
            var lobbies = await _unitOfWork.GameLobbyRepository.GetGameLobbiesAsync();
            return Ok(lobbies);
        }

        [HttpPost("addGuest")]
        public async Task<ActionResult<GameLobbyDto>> AddGuest(GuestDto guestDto)
        {
            bool SessionExists = await _unitOfWork.GameLobbyRepository.SessionExists(guestDto.Username);

            if (SessionExists) return BadRequest("You are already in a session!");

            var connection = new Connection
            {
                Username = guestDto.Username
            };

            var gameLobby = await _unitOfWork.GameLobbyRepository.AddGuestToLobby(connection);

            connection.GameLobbyId = gameLobby.GameLobbyId;
            connection.ConnectedGameLobby = gameLobby;

            await _unitOfWork.ConnectionRepository.CreateConnection(connection);

            // if (!created) return BadRequest("Could not create connection. Try again!");

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Failed to create a lobby!");
        }
    }
}