﻿using EmployeeManagement.Models.Authorization;

namespace EmployeeManagement.ViewModels;

public class UserViewModel
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public DateTime LastLoginDate { get; set; } = DateTime.Now;
    public DateTime RegisteredDate { get; set; } = DateTime.Now;
    public Status Status { get; set; }

}
