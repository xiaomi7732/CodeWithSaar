using System;
using System.Collections.Generic;

namespace JWT.Example.WithSQLDB
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public byte[] PasswordHash { get; set; }
        public bool IsActive { get; set; }

        // Reference to Roles
        public List<Role> Roles { get; } = new List<Role>();
    }
}