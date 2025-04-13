﻿namespace invmanager.Models
{
    public class UserRolesViewModel
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public List<string> Roles { get; set; }
        public List<string> AvailableRoles { get; set; } // List of available roles
        public List<string> SelectedRoles { get; set; } // List of roles to be assigned
    }
}