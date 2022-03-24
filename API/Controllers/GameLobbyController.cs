using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers
{
    [Authorize]
    public class GameLobbyController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        public GameLobbyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        
        [HttpGet]
        public async Task<ActionResult<ICollection<GameLobby>>> GetLobbies()
        {
            var lobbies = await _unitOfWork.GameLobbyRepository.GetGameLobbiesAsync();
            return Ok(lobbies);
        }

        
        [HttpGet("{gameLobbyId}")]
        public ActionResult<GameLobby> GetLobby(int gameLobbyId)
        {
            var lobby = _unitOfWork.GameLobbyRepository.GetGameLobbyById(gameLobbyId);
            return Ok(lobby);
        }

        [HttpPost("joinExistingLobby/{username}")]
        public async Task<ActionResult<GameLobbyDto>> JoinExisting(string username, [FromBody] JsonElement body)
        {
            bool SessionExists = await _unitOfWork.ConnectionRepository.SessionExists(username);
            if (SessionExists) return BadRequest("You are already in a game!");

            var game = JsonSerializer.Deserialize<ExistingGameDto>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var gameLobby = await _unitOfWork.GameLobbyRepository.JoinExistingLobby(game.gameLobbyId);
            if (gameLobby == null) return BadRequest("The lobby is full!");

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Failed to join a lobby!");
        }

        
        [HttpPost("joinNewLobby/{username}")]
        public async Task<ActionResult<GameLobbyDto>> JoinNewLobby(string username, [FromBody] JsonElement body)
        {
            bool SessionExists = await _unitOfWork.ConnectionRepository.SessionExists(username); 
            if (SessionExists) return BadRequest("You are already in a game!");

            var game = JsonSerializer.Deserialize<GameLobbyDto>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var gameLobby = await _unitOfWork.GameLobbyRepository.JoinNewLobby(game.lobbyName);

            if (await _unitOfWork.Complete()) return Ok(gameLobby.GameLobbyId);

            return BadRequest("Failed to create a lobby!");
        }

       
        [HttpPost("joinPrivateRoom/{username}")]
        public async Task<ActionResult<int>> JoinPrivateRoom(string username, [FromBody] JsonElement body)
        {
            bool SessionExists = await _unitOfWork.ConnectionRepository.SessionExists(username); 
            if (SessionExists) return BadRequest("You are already in a game!");

            var game = JsonSerializer.Deserialize<ExistingGameDto>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var gameLobby = _unitOfWork.GameLobbyRepository.GetGameLobbyWithPassword(game.password);
            if (gameLobby == null) return BadRequest("Password is incorrect!");

            if (gameLobby.NumberOfElements == 4) return BadRequest("The lobby is full!");            
            await _unitOfWork.GameLobbyRepository.JoinExistingLobby(gameLobby.GameLobbyId);

            if (await _unitOfWork.Complete()) return Ok(gameLobby.GameLobbyId);
            return BadRequest("Failed to join a lobby!");

        }

        
        [HttpPost("pickColour")]
        public async Task<ActionResult<string>> PickColour(int gameLobbyId, string colour)
        {
            bool validate =  _unitOfWork.GameLobbyRepository.PickColour(colour);
            if (!validate) return BadRequest("Colour is not valid!");

            GameLobby gameLobby = _unitOfWork.GameLobbyRepository.GetGameLobbyById(gameLobbyId);
            gameLobby.PickedColour = colour;

            var group = await _unitOfWork.GameLobbyRepository.GetGroup(gameLobby.GameLobbyName);            

            bool turn = _unitOfWork.GameLobbyRepository.NextTurn(gameLobby, group);
            if (!turn) return BadRequest("I cannot get to the next turn!");

            if (await _unitOfWork.Complete()) return Ok("Next");
            return BadRequest("Couldnt save your play!");
        }

        [HttpGet("newDeck")]
        public async Task<ActionResult<string>> NewDeck(int gameLobbyId)
        {
            GameLobby gameLobby = _unitOfWork.GameLobbyRepository.GetGameLobbyById(gameLobbyId);

            bool deckObtained = await _unitOfWork.GameLobbyRepository.GetNewDeck(gameLobby);
            if (!deckObtained) return BadRequest("You still have cards available to draw!");

            if (await _unitOfWork.Complete()) return Ok("New deck is available!");
            return BadRequest("Failed to initialize the game!");

        }
    

    }
}