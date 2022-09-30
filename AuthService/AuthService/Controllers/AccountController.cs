using System.Security.Cryptography;
using System.Text;
using AuthService.Dtos;
using AuthService.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public AccountController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("register")]
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
            return _mapper.Map<UserDto>(user);
        }

        [HttpPost("Login")]
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

            return Ok(user);
        }

        private async Task<bool> UserExistsAsync(string userName)
        {
            return await _context.ApplicationUsers.AnyAsync(u => u.UserName.ToLower() == userName.ToLower());
        }
    }
}