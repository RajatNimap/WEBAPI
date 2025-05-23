using System.ComponentModel.DataAnnotations;
using Hospital_Management.Models.EntitiesDto;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_Management.Interfaces.Services
{
   public interface IAuthentication
    {
        Task<RegisterDto> RegisterServices(RegisterDto Register);
        Task<string> Login(string email, string password);

    }
    public interface IGenerateToken
    {
        public string GeneratejwtToken(string email);
        public string GeneratejwtRefreshToken();
    }

    public interface IRefreshTokenServices
    {
       Task StoreToken(string email, string token);
       Task<bool> ValidateRefreshToken(string token);
       Task InvalidateRefreshToken(string token);
       Task<string> GetEmailfromToken(string token);
    }
}
