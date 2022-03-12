using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
        public async Task<ActionResult<ICollection<GameLobby>>> GetLobbies()
        {
            var lobbies = await _unitOfWork.GameLobbyRepository.GetGameLobbiesAsync();
            return Ok(lobbies);
        }

        
        [HttpGet("{gameLobbyId}")]
        public async Task<ActionResult<GameLobby>> GetLobby(int gameLobbyId)
        {
            var lobby = await _unitOfWork.GameLobbyRepository.GetGameLobbyById(gameLobbyId);
            return Ok(lobby);
        }     


        // TESTED - Working
        [HttpPost("joinExistingLobby/{username}")]
        public async Task<ActionResult<GameLobbyDto>> JoinExisting(string username, [FromBody] JsonElement body)
        {
            bool SessionExists = await _unitOfWork.ConnectionRepository.SessionExists(username);
            if (SessionExists) return BadRequest("You are already in a game!");

            var game = JsonSerializer.Deserialize<ExistingGameDto>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var gameLobby = await _unitOfWork.GameLobbyRepository.JoinExistingLobby(game.gameLobbyId);
            if (gameLobby == null) return BadRequest("The lobby is full!");

            /*var connection = new Connection
            {
                Username = username,
                GameLobbyId = gameLobby.GameLobbyId,
                ConnectedGameLobby = gameLobby
            };

            await _unitOfWork.ConnectionRepository.CreateConnection(connection);*/

            // if (!created) return BadRequest("Could not create connection. Try again!");

            if (await _unitOfWork.Complete()) return Ok();

            return BadRequest("Failed to create a lobby!");
        }

        // TESTED - Working
        [HttpPost("joinNewLobby/{username}")]
        public async Task<ActionResult<int>> JoinNew(string username, [FromBody] JsonElement body)
        {
            //bool SessionExists = await _unitOfWork.ConnectionRepository.SessionExists(username); 
            //if (SessionExists) return BadRequest("You are already in a game!");

            var game = JsonSerializer.Deserialize<GameLobbyDto>(body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            var gameLobby = await _unitOfWork.GameLobbyRepository.JoinNewLobby(game.lobbyName);

            /*var connection = new Connection
            {
                Username = username,
                GameLobbyId = gameLobby.GameLobbyId,
                ConnectedGameLobby = gameLobby
            };

            await _unitOfWork.ConnectionRepository.CreateConnection(connection);*/

            // if (!created) return BadRequest("Could not create connection. Try again!");

            if (await _unitOfWork.Complete()) return Ok(gameLobby.GameLobbyId);

            return BadRequest("Failed to create a lobby!");
        }

        
        [HttpPost("pickColour")]
        public async Task<ActionResult<string>> PickColour(int gameLobbyId, string colour)
        {
            bool validate = await _unitOfWork.GameLobbyRepository.PickColour(colour);
            if (!validate) return BadRequest("Colour is not valid!");

            GameLobby gameLobby = await _unitOfWork.GameLobbyRepository.GetGameLobbyById(gameLobbyId);
            gameLobby.PickedColour = colour;

            var group = await _unitOfWork.GameLobbyRepository.GetGroup(gameLobby.GameLobbyName);            

            bool turn = await _unitOfWork.GameLobbyRepository.NextTurn(gameLobby, group);
            if (!turn) return BadRequest("I cannot get to the next turn!");

            if (await _unitOfWork.Complete()) return Ok("Next");
            return BadRequest("Couldnt save your play!");
        }        
      

        // empty deck
        [HttpGet("newDeck")]
        public async Task<ActionResult<string>> NewDeck(int gameLobbyId)
        {
            GameLobby gameLobby = await _unitOfWork.GameLobbyRepository.GetGameLobbyById(gameLobbyId);

            bool deckObtained = await _unitOfWork.GameLobbyRepository.GetNewDeck(gameLobby);
            if (!deckObtained) return BadRequest("You still have cards available to draw!");

            if (await _unitOfWork.Complete()) return Ok("New deck is available!");
            return BadRequest("Failed to initialize the game!");

        }
    

    }
}