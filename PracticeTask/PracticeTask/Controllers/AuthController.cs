using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PracticeTask.Data;
using PracticeTask.Interfaces;
using PracticeTask.Model;


namespace PracticeTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly DataContext Database;
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;
        private readonly IRefeshTokenService _refeshTokenService;
        public AuthController(DataContext database, IConfiguration configuration,IAuthService authService, ITokenService _tokenService, IRefeshTokenService refeshTokenService)
        {
            Database = database;
            _configuration = configuration;
            _authService = authService;
            this._tokenService = _tokenService;
            _refeshTokenService = refeshTokenService;
        }

        [HttpPost("Login")]
        
        public async Task <IActionResult> LoginAuthentication([FromBody]LoginReqDto loginReq)
        {
               var IsValid= await _authService.ValidateUser(loginReq.Email, loginReq.Password);

            if (!IsValid) { 
            
                    return Unauthorized("Invalid credential");
            }

            var token=_tokenService.GenerateAccessToken(loginReq.Email);
            var RefreshToken=_tokenService.GenerateRefreshToken(); 

            var existingdata=Database.refreshTokens.Where(x=>x.Email==loginReq.Email && ! x.IsExpired ).ToList();
            foreach (var tk in existingdata) {

               tk.IsExpired=true;

            }

            await _refeshTokenService.StoreToken(loginReq.Email, RefreshToken);    

            return Ok("User Login Successfully " +new { token, RefreshToken } ); 
      
        }

        [HttpPost("RefreshToken")]
        public async Task<IActionResult> LoginRefreshToken([FromBody] RefreshTokenDto refreshdto)
        {
            var IsValid = await _refeshTokenService.ValidateRefreshToken(refreshdto.RefreshToken);
            if (!IsValid) { return Unauthorized("Invalid or token may be expired"); }

            var Emaildata = await _refeshTokenService.GetEmailToken(refreshdto.RefreshToken);
            await _refeshTokenService.InvalidateRefreshToken(refreshdto.RefreshToken);
            var newtoken = _tokenService.GenerateAccessToken(Emaildata);
            var refreshtoken = _tokenService.GenerateRefreshToken();
            
            await _refeshTokenService.StoreToken(Emaildata, refreshtoken);

            return Ok("Refresh Token Login successfully  " + new {newtoken ,refreshtoken});
        }

        //[HttpPost("LoginAuth")]
        //public async Task<IActionResult> LoginAuth([FromBody]LoginReqDto logindto)
        //{
        //    var data=await Database.userDetails.FirstOrDefaultAsync(x=>x.Email == logindto.Email);
        //    if (data == null) {
        //        return NotFound("user Not found");
        //    }
        //    bool isValid=BCrypt.Net.BCrypt.Verify( logindto.Password, data.Password);
            
        //    if (isValid)
        //    {
        //        var token = GenerateJwtToken(logindto);
        //        return Ok(token+" User Login Successfully");
        //    }
        //    else
        //    {
        //        return BadRequest("Invalid crenditials");
        //    }

        //}


        //private string GenerateJwtToken([FromBody]LoginReqDto logindto)
        //{
        //    var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

        //    var SigningKey= new SigningCredentials(securitykey,SecurityAlgorithms.HmacSha256);

        //    var claims = new[]
        //    {
        //        new Claim(JwtRegisteredClaimNames.Email, logindto.Email),
        //    };

        //    var token = new JwtSecurityToken(
        //        _configuration["Jwt:Issuer"],
        //        _configuration["Jwt:Audience"],
        //        claims,
        //        expires: DateTime.UtcNow.AddMinutes(15),
        //        signingCredentials:SigningKey
        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}


        //private string GenerateToken([FromBody] LoginReqDto Login)
        //{
        //    var securitykey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Issuer"]));
        //    var credential = new SigningCredentials(securitykey, SecurityAlgorithms.HmacSha256);

        //    var claims = new[]
        //    {
        //        new Claim(JwtRegisteredClaimNames.Email,Login.Email),
        //    };

        //    var token = new JwtSecurityToken(
        //     _configuration["Jwt:Issuer"],
        //     _configuration["Jwt:Audience"],
        //     claims,
        //     expires: DateTime.UtcNow.AddMinutes(15),
        //     signingCredentials:credential

        //     );

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

    }
}
