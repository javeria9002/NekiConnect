using NekiConnect.Models;

namespace NekiConnect.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(ApplicationUser user);
    }
}