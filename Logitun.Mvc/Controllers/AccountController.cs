using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Logitun.Core.Interfaces;
using Logitun.Shared.DTOs;
using Microsoft.Extensions.Logging;
using Logitun.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Logitun.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AccountController> _logger;
        private readonly ApplicationDbContext _context;

        public AccountController(IAuthService authService, ILogger<AccountController> logger, ApplicationDbContext context)
        {
            _authService = authService;
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            try
            {
                var loginDto = new LoginDto { Username = username, Password = password };
                var authResponse = await _authService.AuthenticateAsync(loginDto);
                if (authResponse != null)
                {
                    // Get user information
                    var user = await _context.AuthUsers
                        .Include(u => u.Information)
                        .Include(u => u.Credentials)
                        .FirstOrDefaultAsync(u => u.Credentials.Login == username);

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, username),
                        new Claim("JWT", authResponse.Token)
                    };
                    
                    if (user?.Information != null)
                    {
                        claims.Add(new Claim("FirstName", user.Information.FirstName));
                        claims.Add(new Claim("LastName", user.Information.LastName));
                    }

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login attempt for user {Username}", username);
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            return View();
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Response.Cookies.Delete(".AspNetCore.Cookies");
            }
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}