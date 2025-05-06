namespace PracticeTask.Interfaces
{
    public interface IAuthService
    {
        Task<bool> ValidateUser(string username, string password);    
    }

    public interface ITokenService
    {
        string GenerateAccessToken(string email);
        string GenerateRefreshToken();
    }
    public interface IRefeshTokenService
    {
        Task StoreToken(string email,string token);
        Task<bool> ValidateRefreshToken(string token);
        Task InvalidateRefreshToken(string token);
        Task <string> GetEmailToken(string token);
    }
    
}
