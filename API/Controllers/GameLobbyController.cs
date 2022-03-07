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

        //// TESTED - working
        //[HttpGet("members/{gameLobbyId}")]
        //public async Task<ActionResult<ICollection<Connection>>> GetPlayers(int gameLobbyId)
        //{
        //    var id = gameLobbyId.ToString();
        //    var players = await _unitOfWork.GameLobbyRepository.GetGroup(id);

        //    if (players == null) return BadRequest("No players for that lobby!");
        //    return Ok(players);
        //}


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


        //// TESTED - Working
        //[HttpPost("createGame/{gameLobbyId}")]
        //public async Task<ActionResult<GameLobbyDto>> StartGame(int gameLobbyId)
        //{
        //    var gameLobby = await _unitOfWork.GameLobbyRepository.GetGameLobbyAsync(gameLobbyId);

        //    if (gameLobby == null) return BadRequest("That game lobby does not exist!");

        //    if (gameLobby.NumberOfElements < 4) return BadRequest("Waiting for more players");

        //    if (gameLobby.GameStatus == "ongoing") return BadRequest("The game has started.");

        //    gameLobby = await _unitOfWork.GameLobbyRepository.StartGame(gameLobby);

        //    if (await _unitOfWork.Complete()) return Ok(gameLobby);
        //    return BadRequest("Failed to initialize the game!");
        //}

        //// Everything working without the consequence
        //[HttpPost("play/{gameLobbyId}")]
        //public async Task<ActionResult<string>> Play(string username, int gameLobbyId, List<Card> cards)
        //{
        //    GameLobby gameLobby = await _unitOfWork.GameLobbyRepository.GetGameLobbyAsync(gameLobbyId);
        //    if (gameLobby.CurrentPlayer != username) return "It is " + gameLobby.CurrentPlayer + "'s turn, it is not your turn!";
        //    // working until here

        //    var group = await _unitOfWork.GameLobbyRepository.GetPlayersOfALobby(gameLobby.GameLobbyId.ToString());
        //    Connection connection = await _unitOfWork.ConnectionRepository.GetConnection(username);

        //    string message = await _unitOfWork.GameLobbyRepository.Play(connection, gameLobby, cards);
        //    if (message != "Next") return BadRequest(message);

        //    if (cards.First().Value == -1)
        //    {
        //        int i = 0;
        //        while(i < cards.Count()) {
        //            message = await _unitOfWork.GameLobbyRepository.GetConsequence(gameLobby, group, connection, cards);
        //            if (message == "Card type is incorrect!") return BadRequest(message);
        //            i++;
        //        }     
        //    }

        //    if (message != "Pick a colour")
        //    {
        //        // get the next turn
        //        var turn = await _unitOfWork.GameLobbyRepository.NextTurn(gameLobby, group);
        //        if (!turn) return BadRequest("I cannot get to the next turn!");
        //    }

        //    if (await _unitOfWork.Complete()) return Ok(message);
        //    return BadRequest("Couldnt save your play!");
        //}

        
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
        
        //[HttpPost("playWithChosenColour")]
        //public async Task<ActionResult<string>> PlayWithChosenColour(int gameLobbyId, string username, ICollection<Card> cards, string colour)
        //{
        //    GameLobby gameLobby = await _unitOfWork.GameLobbyRepository.GetGameLobbyById(gameLobbyId);
        //    if (gameLobby.CurrentPlayer != username) return "It is " + gameLobby.CurrentPlayer + "'s turn, it is not your turn!";
            
        //    var group = await _unitOfWork.GameLobbyRepository.GetGroup(gameLobby.GameLobbyName);
        //    Connection connection = await _unitOfWork.ConnectionRepository.GetConnection(username);

        //    var message = await _unitOfWork.GameLobbyRepository.PlayWithChosenColour(connection, gameLobby, cards, colour);
        //    if (message != "Next") return BadRequest(message);

        //    if (cards.First().Value == -1)
        //    {
        //        int i = 0;
        //        while (i < cards.Count())
        //        {
        //            message = await _unitOfWork.GameLobbyRepository.GetConsequence(gameLobby, group, connection, cards);
        //            if (message == "Card type is incorrect!") return BadRequest(message);
        //            i++;
        //        }
        //    }

        //    if (message != "Pick a colour")
        //    {
        //        // get the next turn
        //        var turn = await _unitOfWork.GameLobbyRepository.NextTurn(gameLobby, group);
        //        if (!turn) return BadRequest("I cannot get to the next turn!");
        //    }

        //    if (await _unitOfWork.Complete()) return Ok(message);
        //    return BadRequest("Couldnt save your play!");

        //}

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

        //// get card from deck
        //[HttpGet("getCard")]
        //public async Task<ActionResult<string>> GetCard(int gameLobbyId)
        //{
        //    GameLobby gameLobby = await _unitOfWork.GameLobbyRepository.GetGameLobbyById(gameLobbyId);
        //    Connection connection = await _unitOfWork.ConnectionRepository.GetConnection(gameLobby.CurrentPlayer);


        //    //if (connection.Cards.Count() == 0 /*&& gameLobby.GameStatus != "finished"*/)
        //    //{
        //    //    await _unitOfWork.GameLobbyRepository.Draw(4, gameLobby, connection);
        //    //    return "Next";
        //    //}

        //    Card pot = await _unitOfWork.CardRepository.GetCard(gameLobby.LastCard);
        //    ICollection<Card> cards = connection.Cards;

        //    if (connection.Cards.Count == 0)
        //    {
        //        cards = new List<Card>();
        //    }

        //    // verify if the current player have a valid card to play
        //    bool playable = await _unitOfWork.GameLobbyRepository.Playable(gameLobby, pot, cards);
        //    if (playable) return "You have cards that you can play!";
            
        //    // get a card from deck until we can play
        //    Card cardFromDeck = new Card();  

        //    // working - test with more cases
        //    do
        //    {
        //        cardFromDeck = await _unitOfWork.GameLobbyRepository.Draw(1, gameLobby, connection);
        //    } while (!(pot.Value == cardFromDeck.Value && pot.Value != -1
        //        || pot.Type == cardFromDeck.Type && cardFromDeck.Type != "Number" 
        //        || pot.Colour == cardFromDeck.Colour
        //        || cardFromDeck.Type == "Wild" 
        //        || cardFromDeck.Type == "Wild Draw 4"));

        //    if (await _unitOfWork.Complete()) return Ok("You obtained the cards required!");
        //    return BadRequest("Could not get the cards!");
        //}


    }
}