using System.ComponentModel.DataAnnotations;
using AuthService.Data;
using AuthService.Dtos;
using AuthService.Filters;
using AuthService.Models;
using AuthService.Services.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Controllers
{
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
        public async Task<ActionResult<object>> Register([FromBody]RegisterDto userDto)
        {
            try
            {
                if (await UserExistsAsync(userDto.Email))
                {
                    return new ErrorModel()
                    {
                        error = $"User with email: {userDto.Email} already exists!",
                        success = false
                    };
                }

                using var hmac = new HMACSHA512();

                var user = new User()
                {
                    Email = userDto.Email,
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(userDto.Password)),
                    PasswordSalt = hmac.Key
                };

                _context.ApplicationUsers.Add(user);
                await _context.SaveChangesAsync();
                var userToReturn = _mapper.Map<UserDto>(user);
                userToReturn.Token = _tokenService.CreateToken(user);
                var result = new SuccessModel
                {
                    data = userToReturn,
                    message = "User Created",
                    success = true
                };
                return result;
            }
            catch (Exception e)
            {
                var result = new ErrorModel
                {
                    error = e.Message,
                    success = false
                };
                return result;
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<object>> Login([FromBody] LoginDto userDto)
        {
            try
            {
                var user = await _context.ApplicationUsers.SingleOrDefaultAsync(u => u.Email == userDto.Email);
                if (user == null)
                {
                    return new ErrorModel
                    {
                        error = "Invalid credentials",
                        success = false
                    };
                }

                using var htmac = new HMACSHA512(user.PasswordSalt);
                var computedHash = htmac.ComputeHash(Encoding.UTF8.GetBytes(userDto.Password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != user.PasswordHash[i]) return new ErrorModel
                    {
                        error = "Invalid credentials",
                        success = false
                    };
                }

                var userToReturn = _mapper.Map<UserDto>(user);
                userToReturn.Token = _tokenService.CreateToken(user);
                return new SuccessModel
                {
                    data = userToReturn,
                    message = "User registered",
                    success = true
                };
            }
            catch (Exception e)
            {
                return new ErrorModel
                {
                    error = e.Message,
                    success = false
                };
            }
        }

        [HttpGet("GetSelf")]
        [ServiceFilter(typeof(AuthorizationAttribute))]
        public ActionResult<object> GetSelf()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                IEnumerable<Claim> claims = identity.Claims;
                var userEmail = identity.FindFirst("userEmail").Value;
                var userId = identity.FindFirst("userId").Value;
                return new SuccessModel
                {
                    data = new
                    {
                        userEmail, userId
                    },
                    message = "User is authorized",
                    success = true
                };
            }
            return new ErrorModel()
            {
                error = "Could not get user claims",
                success = false
            };
        }

        [HttpGet("Users/{email}")]
        public async Task<ActionResult<object>> GetUserByEmail(string email)
        {
            try
            {
                var user = await _context.ApplicationUsers.FirstOrDefaultAsync(u => u.Email == email);
                if (user == null)
                {
                    return new ErrorModel
                    {
                        error = "UserNotFound",
                        success = false
                    };
                }

                var userToReturn = _mapper.Map<UserDto>(user);
                userToReturn.Token = _tokenService.CreateToken(user);
                return new SuccessModel
                {
                    data = userToReturn,
                    message = "User returned",
                    success = true
                };
            }
            catch (Exception e)
            {
                return new ErrorModel
                {
                    error = e.Message,
                    success = false
                };
            }
        }


        private async Task<bool> UserExistsAsync(string userName)
        {
            return await _context.ApplicationUsers.AnyAsync(u => u.Email.ToLower() == userName.ToLower());
        }
    }
}