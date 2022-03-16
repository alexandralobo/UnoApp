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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{    
    public class MemberController : BaseApiController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public readonly ITokenService _tokenService;
        private readonly UserManager<LoginUser> _userManager;
        private readonly SignInManager<LoginUser> _signInManager;
        public MemberController(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            UserManager<LoginUser> userManager,
            SignInManager<LoginUser> signInManager,
            ITokenService tokenService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        //[Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserWithTokenDto>>> GetGuests()
        {
            var guests = await _unitOfWork.GuestRepository.GetUsersAsync();

            var guestsDto = new List<UserWithTokenDto>();
            foreach (var user in guests)
            {
                guestsDto.Add(new UserWithTokenDto
                {
                    Username = user.UserName
                });
            }

            return Ok(guestsDto);
        }

        // TESTED - Working
        [HttpPost("join")]
        public async Task<ActionResult<UserWithTokenDto>> GuestCreation(GuestDto guestDto)
        {
            bool UsersExists = await _unitOfWork.GuestRepository.UserExists(guestDto.Username);

            if (UsersExists) return BadRequest("Username is taken!");

            var guest = _mapper.Map<Guest>(guestDto);
            guest.UserName = guestDto.Username.ToLower();

            var saved = await _unitOfWork.GuestRepository.CreateGuest(guest);
            if (!saved) return BadRequest("A problem occurred");

            guest = await _unitOfWork.GuestRepository.GetGuestByUsernameAsync(guest.UserName);
            var details = new DetailsToTokenDto
            {
                Id = guest.Id,
                UserName = guest.UserName
            };

            return new UserWithTokenDto
            {
                Username = guest.UserName,
                Token = _tokenService.CreateToken(details)

            };
        }
        
        // Working
        [HttpPost("signUp")]
        public async Task<ActionResult<UserWithTokenDto>> SignUp(LoginUserDto loginUserDto)
        {
            bool UsersExists = await _unitOfWork.GuestRepository.UserExists(loginUserDto.Username);
            if (UsersExists) return BadRequest("Username is taken!");

            var loginUser = _mapper.Map<LoginUser>(loginUserDto);
            loginUser.UserName = loginUserDto.Username.ToLower();

            var result = await _userManager.CreateAsync(loginUser, loginUserDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            //var saved = await _unitOfWork.GuestRepository.SignUp(loginUser);
            //if (!saved) return BadRequest("A problem occurred");

            loginUser = await _unitOfWork.GuestRepository.GetLoginUserByUsernameAsync(loginUser.UserName);
            var details = new DetailsToTokenDto
            {
                Id = loginUser.Id,
                UserName = loginUser.UserName
            };

            return new UserWithTokenDto
            {
                Username = loginUser.UserName,
                Token = _tokenService.CreateToken(details)

            };
        }
    }
}