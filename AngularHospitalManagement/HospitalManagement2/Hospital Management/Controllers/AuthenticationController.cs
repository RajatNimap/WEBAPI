using Hospital_Management.Data;
using Hospital_Management.Interfaces.Implementation;
using Hospital_Management.Interfaces.Services;
using Hospital_Management.Models.EntitiesDto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Hospital_Management.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly DataContext Database;
        private readonly IAuthentication Reg;
        private readonly TokenImplementation Token;
        private readonly RefreshTokenImplementation RefreshTokenIm;
        public AuthenticationController(DataContext database,IAuthentication reg,TokenImplementation token,RefreshTokenImplementation refreshtoken) 
        {
            Database = database;
            Reg = reg;
            Token = token;  
            RefreshTokenIm = refreshtoken;  
        }
        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task <IActionResult> RegisterData(RegisterDto register)
        {
            if(register == null)
            {
                return BadRequest("please enter the valid detail");
            }
           var list=new List<string>() { "admin", "doctor", "receptionist" };
            if (!list.Contains(register.Role))
            {
                return BadRequest("Enter correct Details");
                    
            }
            var data = await Reg.RegisterServices(register);
            
            return Ok("Sign up Successs");
        }

        
        [HttpPost("Login")]
        public async  Task <IActionResult> LoginUser(LoginDto loginDto)
        {
            var data=await Reg.Login(loginDto.Email,loginDto.Password);
            if (data == null)
            {
                return BadRequest("Data Not Found");
            };

            var AccessToken = Token.GeneratejwtToken(loginDto.Email);

            var GenRefreshToken = Token.GeneratejwtRefreshToken();

            var StoreToken = RefreshTokenIm.StoreToken(loginDto.Email,GenRefreshToken);

            if (data == false) {
                return NotFound("Data Not found");
            }
           
            return Ok(data+"  "+AccessToken+"         "+GenRefreshToken);     
        }
        [HttpPost("JWtRefresh")]
        public async Task<IActionResult> JWTRefresh([FromBody] RefreshTokenDto refresh)
        {

            var ValidateToken = await RefreshTokenIm.ValidateRefreshToken(refresh.Token);

            if (ValidateToken == false)
            {
                return NotFound("Invalid Token");
            }
            var EmailByToken = await RefreshTokenIm.GetEmailfromToken(refresh.Token);

                      await RefreshTokenIm.InvalidateRefreshToken(refresh.Token);
                   

                    var AccessTokenInRefresh=Token.GeneratejwtToken(EmailByToken);
                    var RefreshTokenInRefresh = Token.GeneratejwtRefreshToken();

                    await RefreshTokenIm.StoreToken(EmailByToken,RefreshTokenInRefresh);   

                    return Ok(AccessTokenInRefresh+" "+RefreshTokenInRefresh);
        }


        
    }
}
