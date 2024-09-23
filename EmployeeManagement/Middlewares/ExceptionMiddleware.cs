using EmployeeManagement.Interfaces;
using EmployeeManagement.Models.Authorization;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Security.Authentication;

namespace EmployeeManagement.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context,
            IAuthService authService,
            UserManager<User> userManager)
        {
            try
            {
                await _next(context);
            }
            catch (AuthenticationException)
                {
                context.Response.Redirect("/Auth/Login?error=AuthenticationFailed");
            }
            catch (Exception ex)
            {
                context.Response.Redirect("/Auth/Login?error=UnexpectedError");
            }
        }
    }
}
