using EmployeeManagement.Interfaces;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel login)
        {
            if (!ModelState.IsValid)
                return View(login);

            var token = await _authService.LoginAsync(login);

            if (token == null)
            {
                ModelState.AddModelError("", "Invalid login credentials.");
                return View(login);
            }

            Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            return RedirectToAction(nameof(Index), "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel addOrUpdateUser)
        {
            if (!ModelState.IsValid)
                return View(addOrUpdateUser);

            var token = await _authService.RegisterAsync(addOrUpdateUser);

            if (token == null)
            {
                ModelState.AddModelError("", "Registration failed.");
                return View(addOrUpdateUser);
            }

            Response.Cookies.Append("AuthToken", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.UtcNow.AddHours(1)
            });

            return RedirectToAction(nameof(Login));
        }
    }
}
