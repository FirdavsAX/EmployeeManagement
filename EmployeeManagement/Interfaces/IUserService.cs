using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;

namespace EmployeeManagement.Interfaces;

public interface IUserService
{
    Task<List<UserViewModel>> GetAll();
    Task<UserViewModel> GetById(string id);
    Task Delete(List<string> ids);
    Task BlockUser(List<string> id);
    Task UnBlockUser(List<string> ids);
}
