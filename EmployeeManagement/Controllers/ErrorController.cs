using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers;

public class ErrorController : Controller
{
    public IActionResult UnAuthorized()
    {
        return View();
    }
}
