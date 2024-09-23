using EmployeeManagement.Interfaces;
using EmployeeManagement.Models;
using EmployeeManagement.Models.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace EmployeeManagement.Middlewares;

public class CheckActiveUsersMiddleware
{
    private readonly RequestDelegate _next;

    public CheckActiveUsersMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context,
        IAuthService authService,
        UserManager<User> userManager)
    {
        if (context.User.Identity.IsAuthenticated)
        {
            var userId = context.User.FindFirstValue(AppClaimTypes.UserId);
            if (userId != null)
            {
                var user = await userManager.FindByIdAsync(userId);

                if (user?.Status == Status.Blocked && !IsAlreadyRedirected(context.User))
                {
                    SetBlockedUserToken(context, authService, user);
                    context.Response.Redirect("/Auth/Login?error=You're");
                    return;
                }
            }
        }

        await _next(context);
    }

    private bool IsAlreadyRedirected(ClaimsPrincipal user)
    {
        return bool.TryParse(user.FindFirstValue(AppClaimTypes.IsRedirected), out var isRedirected) && isRedirected;
    }

    private void SetBlockedUserToken(HttpContext context, IAuthService authService, User user)
    {
        var token = authService.GenerateBlockedToken(user);
        context.Response.Cookies.Append("AuthToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            Expires = DateTimeOffset.UtcNow.AddDays(1)
        });
    }
}
