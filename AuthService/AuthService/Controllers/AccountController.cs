using System.Security.Cryptography;
using System.Text;
using AuthService.Dtos;
using AuthService.Models;
using AuthService.Services.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;

        public AccountController(ApplicationDbContext context, IMapper mapper, ITokenService tokenService)
        {
            _context = context;
            _mapper = mapper;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Register(RegisterDto userDto)
        {
            if (await UserExistsAsync(userDto.Username))
            {
                return BadRequest($"User with name: {userDto.Username} already exists!");
            }

            using var hmac = new HMACSHA512();

            var user = new User()
            {
                UserName = userDto.Username,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userDto.Password)),
                PasswordSalt = hmac.Key
            };

            _context.ApplicationUsers.Add(user);
            await _context.SaveChangesAsync();
            var userToReturn = _mapper.Map<UserDto>(user);
            userToReturn.Token = _tokenService.CreateToken(user);
            return userToReturn;
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<ActionResult<UserDto>> Login(LoginDto userDto)
        {
            var user = await _context.ApplicationUsers.SingleOrDefaultAsync(u => u.UserName == userDto.Username);
            if (user == null)
            {
                return BadRequest("Invalid credentials");
            }

            using var htmac = new HMACSHA512(user.PasswordSalt);
            var computedHash = htmac.ComputeHash(Encoding.UTF8.GetBytes(userDto.Password));

            for(int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid credentials");
            }

            var userToReturn = _mapper.Map<UserDto>(user);
            userToReturn.Token = _tokenService.CreateToken(user);

            return userToReturn;
        }

        [HttpGet("IsAuthorized")]
        public ActionResult IsAuthorized()
        {
            return Ok();
        }

        private async Task<bool> UserExistsAsync(string userName)
        {
            return await _context.ApplicationUsers.AnyAsync(u => u.UserName.ToLower() == userName.ToLower());
        }
    }
}