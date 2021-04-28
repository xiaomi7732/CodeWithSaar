using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JWT.Example.WithSQLDB
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        
        [JsonIgnore]
        public byte[] PasswordHash { get; set; }

        public bool IsActive { get; set; } = true;

        public List<Role> Roles { get; } = new List<Role>();
    }
}