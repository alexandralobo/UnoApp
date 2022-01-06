using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
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

        [HttpPost]
        public Task<ActionResult<GameLobbyDto>> CreateLobby()
        {
            throw new NotImplementedException();
        }
    }
}