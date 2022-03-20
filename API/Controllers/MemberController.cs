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

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserWithTokenDto>>> GetGuests()
        {
            var guests = await _unitOfWork.MemberRepository.GetUsersAsync();

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
                
        [HttpPost("join")]
        public async Task<ActionResult<UserWithTokenDto>> GuestCreation(GuestDto guestDto)
        {
            bool UsersExists = await _unitOfWork.MemberRepository.UserExists(guestDto.Username);

            if (UsersExists) return BadRequest("Username is taken!");

            var guest = _mapper.Map<Guest>(guestDto);
            guest.UserName = guestDto.Username.ToLower();

            var saved = await _unitOfWork.MemberRepository.CreateGuest(guest);
            if (!saved) return BadRequest("A problem occurred");

            guest = await _unitOfWork.MemberRepository.GetGuestByUsernameAsync(guest.UserName);
            var details = new DetailsToTokenDto
            {
                UserName = guest.UserName,
                Type = "Guest"
            };

            return new UserWithTokenDto
            {
                Username = guest.UserName,
                Token = _tokenService.CreateToken(details)

            };
        }
        

        [HttpPost("signUp")]
        public async Task<ActionResult<UserWithTokenDto>> SignUp(SignUpDto signUpDto)
        {
            bool UsersExists = await _unitOfWork.MemberRepository.UserExists(signUpDto.Username);
            if (UsersExists) return BadRequest("Username is taken!");

            var loginUser = _mapper.Map<LoginUser>(signUpDto);
            loginUser.UserName = signUpDto.Username.ToLower();

            var result = await _userManager.CreateAsync(loginUser, signUpDto.Password);
            if (!result.Succeeded) return BadRequest(result.Errors);

            //var saved = await _unitOfWork.GuestRepository.SignUp(loginUser);
            //if (!saved) return BadRequest("A problem occurred");

            loginUser = await _unitOfWork.MemberRepository.GetLoginUserByUsernameAsync(loginUser.UserName.ToLower());
            var details = new DetailsToTokenDto
            {
                UserName = loginUser.UserName,
                Type = "LoginUser"
            };

            return new UserWithTokenDto
            {
                Username = loginUser.UserName,
                Token = _tokenService.CreateToken(details)

            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserWithTokenDto>> Login(LoginDto loginDto)
        {

            var user = await _unitOfWork.MemberRepository.GetLoginUserByUsernameAsync(loginDto.Username.ToLower());

            if (user == null) return Unauthorized("Invalid username");

            var result = await _signInManager
                .CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded) return Unauthorized();

            var details = new DetailsToTokenDto
            {
                UserName = user.UserName,
                Type = "LoginUser"
            };

            return new UserWithTokenDto
            {
                Username = loginDto.Username,
                Token = _tokenService.CreateToken(details)
            };
        }
    }
}