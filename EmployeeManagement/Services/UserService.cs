using AutoMapper;
using AutoMapper.QueryableExtensions;
using EmployeeManagement.Data;
using EmployeeManagement.Interfaces;
using EmployeeManagement.Models;
using EmployeeManagement.Models.Authorization;
using EmployeeManagement.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Services;

public class UserService(EmployeeDbContext employeeDbContext, IMapper mapper) : IUserService
{
    private readonly EmployeeDbContext _employeeDbContext = employeeDbContext 
        ?? throw new ArgumentNullException(nameof(employeeDbContext));
    private readonly IMapper _mapper = mapper;

    public async Task BlockUser(List<string> ids)
    {
        var users = _employeeDbContext.Users
            .Where(x => ids.Contains(x.Id)) 
            ?? throw new ArgumentNullException($"User with ids is not found!");
        
        foreach(var user in users)
            { user.Status = Status.Blocked; }

        _employeeDbContext.Users.UpdateRange(users);

        await _employeeDbContext.SaveChangesAsync();
    }

    public async Task Delete(List<string> ids)
    {
        var users = _employeeDbContext.Users
          .Where(x => ids.Contains(x.Id))
          ?? throw new ArgumentNullException($"User with ids is not found!");

        _employeeDbContext.Users.RemoveRange(users);

        await _employeeDbContext.SaveChangesAsync();
    }

    public async Task<List<UserViewModel>> GetAll()
    {
        var users = await _employeeDbContext.Users.ProjectTo<UserViewModel>(_mapper.ConfigurationProvider).ToListAsync();
       
        return users;
    }

    public async Task<UserViewModel> GetById(string id)
    {
        var user =  await _employeeDbContext.Users.FirstOrDefaultAsync(x => x.Id == id);
        return _mapper.Map<UserViewModel>(user);
    }

    public async Task UnBlockUser(List<string> ids)
    {
        var users = _employeeDbContext.Users
          .Where(x => ids.Contains(x.Id))
          ?? throw new ArgumentNullException($"User with ids is not found!");

        foreach (var user in users)
        { user.Status = Status.UnBlocked; }

        _employeeDbContext.Users.UpdateRange(users);

        await _employeeDbContext.SaveChangesAsync();
    }
}
