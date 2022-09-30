using AuthService.Models;

namespace AuthService.Services.IServices
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
