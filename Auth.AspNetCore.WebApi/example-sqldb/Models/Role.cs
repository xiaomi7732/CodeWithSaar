using System;
using System.Collections.Generic;

namespace JWT.Example.WithSQLDB
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public bool IsActive { get; set; } = true;

        public List<User> Users { get; } = new List<User>();
    }
}