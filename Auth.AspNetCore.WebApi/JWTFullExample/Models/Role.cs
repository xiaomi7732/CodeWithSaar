using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace JWT.Example.WithSQLDB
{
    public class Role
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public bool IsActive { get; set; } = true;

        [JsonIgnore]
        public List<User> Users { get; } = new List<User>();
    }
}