using EmployeeManagement.Models.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace EmployeeManagement.Models
{
    public class User : IdentityUser
    {
        public DateTime LasLoginDate { get; set; } = DateTime.Now;
        public DateTime RegisterDate { get; set; } = DateTime.Now;
        public Status Status { get; set; } = Status.UnBlocked;
    }
}
