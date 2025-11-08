//using Hospital_Management.Interfaces.Implementation;
//using Hospital_Management.Interfaces.Services;
//using Hospital_Management.Models.Entities;
//using Hospital_Management.Models.EntitiesDto;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Mvc.Filters;
//using Newtonsoft.Json.Linq;
//using Org.BouncyCastle.Ocsp;

//namespace Hospital_Management.Controllers
//{
//    public class HomeController : Controller
//    {
//        public override void OnActionExecuting(ActionExecutingContext context)
//        {
//            Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, max-age=0";
//            Response.Headers["Pragma"] = "no-cache";
//            Response.Headers["Expires"] = "-1";
//            base.OnActionExecuting(context);
//        }
//        private readonly IAuthentication Reg;
//        private readonly TokenImplementation Token;
//        private readonly RefreshTokenImplementation RefreshTokenIm;
//        public HomeController(IAuthentication _reg,TokenImplementation token, RefreshTokenImplementation refreshTokenImplementation) {

//            Reg = _reg;
//            Token = token;
//            RefreshTokenIm = refreshTokenImplementation;    
//        }
    
//        [HttpGet("~/")]
//        public IActionResult Index()
//        {
//            return View();
//        }
//        [HttpGet("register")]
//        public IActionResult Register()
//        {
//            return View();
//        }
//        [HttpGet("login")]
//        public IActionResult Login()
//        {
//            return View();
//        }

//        [HttpPost("login")]
//        public async Task<IActionResult> Login(LoginDto loginDto )
//        {
//            // Here you would typically validate the user credentials
//            // For now, we will just return a view with the username

//            if (!ModelState.IsValid)
//            {
//                return View(loginDto);
//            }

//            var data =  await Reg.Login(loginDto.Email, loginDto.Password);
//            if (data == false)
//            {
//                ModelState.AddModelError("", "Invalid login attempt.");
//                return View(loginDto);
//            }
            
//            var AccessToken =   Token.GeneratejwtToken(loginDto.Email);
//            var GenRefreshToken = Token.GeneratejwtRefreshToken();

//            var StoreToken = RefreshTokenIm.StoreToken(loginDto.Email, GenRefreshToken);
//            Token.SetAuthCooked(AccessToken, GenRefreshToken, HttpContext);

//            TempData["Message"] = "Login successful!";  

//            return RedirectToAction("RecepLogin");   
//        }
//        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
//        [Authorize(Roles = "admin")]
//        public IActionResult RecepLogin()
//        {
//            return View();
//        }
//        public async Task<IActionResult> Logout()
//        {
//            var refreshToken = Request.Cookies["refreshToken"];
//            if (refreshToken != null)
//            {
//                await RefreshTokenIm.InvalidateRefreshToken(refreshToken);
//            }
//            // Clear the authentication cookies
//            Response.Cookies.Delete("jwt", new CookieOptions { Path = "/" });
//            Response.Cookies.Delete("refreshToken", new CookieOptions { Path = "/" });
//            HttpContext.Session.Clear();
//            return RedirectToAction("Index");
//        }

//    }
//}
