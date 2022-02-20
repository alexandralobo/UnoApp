using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{    
    public class GuestsController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public readonly ITokenService _tokenService;
        public GuestsController(IUnitOfWork unitOfWork, IMapper mapper, ITokenService tokenService)
        {
            _tokenService = tokenService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        // TESTED - working
        //a[Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GuestDto>>> GetGuests()
        {
            var guests = await _unitOfWork.GuestRepository.GetUsersAsync();

            var guestsDto = new List<GuestDto>();
            foreach (var user in guests)
            {
                guestsDto.Add(new GuestDto
                {
                    Username = user.UserName
                });
            }

            return Ok(guestsDto);
        }

        // TESTED - Working
        [HttpPost("creation")]
        public async Task<ActionResult<GuestDto>> UserCreation(GuestDto guestDto)
        {
            bool UsersExists = await _unitOfWork.GuestRepository.UserExists(guestDto.Username);

            if (UsersExists) return BadRequest("Username is taken!");

            var guest = _mapper.Map<Guest>(guestDto);
            guest.UserName = guestDto.Username.ToLower();

            var saved = await _unitOfWork.GuestRepository.CreateGuest(guest);
            if (!saved) return BadRequest("A problem occurred");

            return new GuestDto
            {
                Username = guest.UserName,
                Token = await _tokenService.CreateToken(guest)
            };
        }
    }
}