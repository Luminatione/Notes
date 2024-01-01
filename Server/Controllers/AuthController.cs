using Common.DTO;
using Common.Model;
using Common.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Server.DTO;
using System.Security.Policy;

namespace Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
        {
            await Task.Delay(1000);
            if (ModelState.IsValid)
            {
                var result = await _authService.Login(loginDto.Username, loginDto.Password);
                if (result.IsSuccess)
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, result.Value);
                    return RedirectToAction("Index", "Note");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(loginDto);
        }

        [AllowAnonymous]
        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromForm] RegisterDto registerDto)
        {
            await Task.Delay(1000);
            if (ModelState.IsValid)
            {
                var result = await _authService.Register(registerDto);

                if (result.IsSuccess)
                {
                    var loginResult = await _authService.Login(registerDto.Username, registerDto.Password);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, loginResult.Value);
                    return RedirectToAction("Index", "Note");
                }

                ModelState.AddModelError(string.Empty, result.Message);
            }
            return View(registerDto);
        }
    }
}
