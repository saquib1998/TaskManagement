using TaskManagement.API.Data;

namespace TaskManagement.API.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(AppUser user);
    }
}
