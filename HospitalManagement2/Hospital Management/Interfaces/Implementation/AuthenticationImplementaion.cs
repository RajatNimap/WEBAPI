using Hospital_Management.Data;
using Hospital_Management.Interfaces.Services;
using Hospital_Management.Models.Entities;
using Hospital_Management.Models.EntitiesDto;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Management.Interfaces.Implementation
{
    public class AuthenticationImplementation : IAuthentication
    {
        private readonly DataContext Database;
        public AuthenticationImplementation(DataContext database)   
        {
            Database = database;
        }
        public async Task<RegisterDto> RegisterServices(RegisterDto Register)
        {
            string HashedPassword = BCrypt.Net.BCrypt.HashPassword(Register.Password);
            var data = new Register
            {
                Username = Register.Username,
                Email = Register.Email,
                Password = HashedPassword,
                Role = Register.Role,
            };
            await Database.registers.AddAsync(data);
            await Database.SaveChangesAsync();
           
                return new RegisterDto
                {
                    Username = Register.Username,
                    Email = Register.Email,
                    Password = HashedPassword,
                    Role = Register.Role,
                };
        }
        public async Task<string> Login(string email, string password)
        {
            var Data= await Database.registers.FirstOrDefaultAsync(x=>x.Email == email);
            

            if (Data == null) {
                return null;

            }
            var IsValid=BCrypt.Net.BCrypt.Verify(password,Data.Password);
            if (!IsValid) {
                return null;
            }
            return "Bearer";

        }

    }   



}
