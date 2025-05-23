using Hospital_Management.Data;
using Hospital_Management.Interfaces.Services;
using Hospital_Management.Models.Entities;
using Hospital_Management.Models.EntitiesDto;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Management.Interfaces.Implementation
{
    public class RefreshTokenImplementation : IRefreshTokenServices
    {
        private readonly DataContext Database;
        public RefreshTokenImplementation(DataContext database)
        {
            Database = database;    
        }
   
        public async Task<string> GetEmailfromToken(string token)
        {

            var data = await Database.refreshTokens.FirstOrDefaultAsync(x => x.Token == token);  
            if (data == null)
            {
                return null;
            }
            return data.Email;  
            
        }

        public async Task InvalidateRefreshToken(string token)
        {
            var data=await Database.refreshTokens.FirstOrDefaultAsync(x=>x.Token == token);

            
            if ( data != null)  { 
                        
                data.IsRevoked = true;
               await Database.SaveChangesAsync();
           
            }
           
        }

        public async Task StoreToken(string email, string token)
        {
                var data=new RefreshToken {
                    Email = email, 
                    Token = token ,
                    ExpiryDate =DateTime.UtcNow.AddMinutes(10),
                    IsRevoked=false 
                };

               await Database.refreshTokens.AddAsync(data);
               await Database.SaveChangesAsync();
        }

        public async Task<bool> ValidateRefreshToken(string token)
        {
                var data=await Database.refreshTokens.FirstOrDefaultAsync(x=>x.Token ==token);
                 if (data == null)
                 {
                        return false;
                 }

                if(data !=null && !data.IsRevoked && data.ExpiryDate > DateTime.UtcNow)
                {
                     return true;
                }
                return false;   

        }
    }
}
