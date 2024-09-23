using EmployeeManagement.Models;

namespace EmployeeManagement.Interfaces;

public interface IAuthService
{
    Task<string> RegisterAsync(RegisterModel register);
    Task<string> LoginAsync(LoginModel login);
    string GenerateBlockedToken(User user);
}
