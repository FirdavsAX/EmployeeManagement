using AutoMapper;
using EmployeeManagement.Models;
using EmployeeManagement.ViewModels;

namespace EmployeeManagement.Mappings
{
    public class UserMappings : Profile
    {
        public UserMappings()
        {
            CreateMap<User,UserViewModel>().ReverseMap();
        }
    }
}
