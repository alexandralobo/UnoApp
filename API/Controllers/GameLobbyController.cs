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

        // not found
        [HttpGet("{gameLobbyId}")]
        public async Task<ActionResult<GameLobby>> GetLobby(int gameLobbyId)
        {
            var lobby = await _unitOfWork.GameLobbyRepository.GetGameLobbyAsync(gameLobbyId);
            return Ok(lobby);
        }

       // working
        [HttpGet("members/{gameLobbyId}")]
        public async Task<ActionResult<ICollection<Connection>>> GetPlayers(int gameLobbyId)
        {
            var players = await _unitOfWork.GameLobbyRepository.GetPlayersOfALobby(gameLobbyId);

            if (players == null) return BadRequest("No players for that lobby!");
            return Ok(players);
        }


        // TESTED - Working
        [HttpPost("addGuest")]
        public async Task<ActionResult<GameLobbyDto>> AddGuest(GuestDto guestDto)
        {
            bool SessionExists = await _unitOfWork.ConnectionRepository.SessionExists(guestDto.Username);

            if (SessionExists) return BadRequest("You are already in a session!");

            var gameLobby = await _unitOfWork.GameLobbyRepository.AddGuestToLobby();

            var connection = new Connection
            {
                Username = guestDto.Username,
                GameLobbyId = gameLobby.GameLobbyId,
                ConnectedGameLobby = gameLobby
            };

            await _unitOfWork.ConnectionRepository.CreateConnection(connection);

            // if (!created) return BadRequest("Could not create connection. Try again!");

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Failed to create a lobby!");
        }

        // TESTED - Working
        [HttpPost("createGame/{gameLobbyId}")]
        public async Task<ActionResult<GameLobbyDto>> CreateGame(int gameLobbyId)
        {
            var gameLobby = await _unitOfWork.GameLobbyRepository.GetGameLobbyAsync(gameLobbyId);

            if (gameLobby == null) return BadRequest("That game lobby does not exist!");

            if (gameLobby.NumberOfElements < 4) return BadRequest("Waiting for more players");

            if (gameLobby.GameStatus == "ongoing") return BadRequest("The game has started.");

            gameLobby = await _unitOfWork.GameLobbyRepository.CreateGame(gameLobby);

            if (await _unitOfWork.Complete()) return Ok(gameLobby);
            return BadRequest("Failed to initialize the game!");
        }

        /*[HttpPost("start/{gameLobbyId}")]
        public async Task<ActionResult<GameLobby>> StartGame(int gameLobbyId)
        {
            GameLobby gameLobby = await _unitOfWork.GameLobbyRepository.GetGameLobbyAsync(gameLobbyId);

            await _unitOfWork.GameLobbyRepository.StartGame(gameLobby);

            if (await _unitOfWork.Complete()) return Ok(gameLobby);
            return BadRequest("Could not start the game!");
        }*/

    }
}