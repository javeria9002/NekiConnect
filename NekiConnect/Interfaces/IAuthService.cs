namespace NekiConnect.Interfaces
{
    public interface IAuthService
    {
        Task<(bool ok, string? error, string? role, string? token)> LoginAsync(
            string email, string password);
    }
}