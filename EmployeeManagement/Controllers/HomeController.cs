using EmployeeManagement.Interfaces;
using EmployeeManagement.Models;
using EmployeeManagement.Models.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace EmployeeManagement.Controllers
{
    [Authorize(Policy =AppAuthorizationPolicies.RequireAccountActive)] // Ensures the user must be authenticated to access this controller
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserService _userService;

        public HomeController(ILogger<HomeController> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAll();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BlockUser([FromBody] List<string> ids)
        {
            await _userService.BlockUser(ids); // Implement logic in UserService to block users
            return Json(new { success = true });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UnblockUser([FromBody] List<string> ids)
        {
            await _userService.UnBlockUser(ids); // Implement logic in UserService to unblock users
            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser([FromBody] List<string> ids)
        {
            await _userService.Delete(ids); // Implement logic in UserService to delete users
            return Json(new { success = true });
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
