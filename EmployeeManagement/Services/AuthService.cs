using EmployeeManagement.Interfaces;
using EmployeeManagement.Models;
using EmployeeManagement.Models.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Authentication;
using System.Security.Claims;
using System.Text;

namespace EmployeeManagement.Services;

public class AuthService(IConfiguration configuration,
    UserManager<User> userManager
    ) : IAuthService
{
    private readonly IConfiguration _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    private readonly UserManager<User> _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));

    public async Task<string> RegisterAsync(RegisterModel model)
    {
        var name = await _userManager.FindByNameAsync(model.UserName);

        if (name is not null)
        {
            throw new AuthenticationException($"Already exist user name");
        }

        var user = new User()
        {
            UserName = model.UserName,
            Email = model.Email,
            RegisterDate = DateTime.Now,
            LasLoginDate = DateTime.Now,
            Status = Status.UnBlocked,
            SecurityStamp = Guid.NewGuid().ToString(),
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            throw new AuthenticationException($"Failed to register {model.UserName}");
        }

        var token = GenerateToken(user);

        return token ?? "";
    }
    public async Task<string> LoginAsync(LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName);
        var result = await _userManager.CheckPasswordAsync(user, model.Password);

        if (!result)
        {
            return null;
        }

        user.LasLoginDate = DateTime.Now;
        await _userManager.UpdateAsync(user);

        var token = GenerateToken(user);

        return token ?? "";

    }
    private string? GenerateToken(User user, bool? isRedirected = false)
    {
        //Get secret key, audince , issuer in configuration
        var secret = configuration["JwtConfig:Secret"];
        var audience = configuration["JwtConfig:ValidAudiences"];
        var issuer = configuration["JwtConfig:ValidIssuer"];

        if (secret is null ||  audience is null || issuer is null)
        {
            throw new ApplicationException("Jwt is not set in the configuration");
        }

        //Encoding Secret Key
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var tokenHandler = new JwtSecurityTokenHandler();
        List<Claim> claims = new List<Claim>
        {
            new(ClaimTypes.Name, user.UserName),
            new(AppClaimTypes.UserId, user.Id),
            new Claim(AppClaimTypes.IsRedirected,isRedirected.ToString() ?? Boolean.FalseString),
            new(AppClaimTypes.Status,user.Status.ToString())
        };

        var tokenDescriptor = new SecurityTokenDescriptor()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddDays(1),
            Issuer = issuer,
            Audience = audience,
            SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        };

        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
        var token = tokenHandler.WriteToken(securityToken);
        return token;
    }

    public string GenerateBlockedToken(User user)
    {
        var token = GenerateToken(user,true);
        return token ?? "";
    }
}
