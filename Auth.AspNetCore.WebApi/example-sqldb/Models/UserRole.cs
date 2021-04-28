using System;

namespace JWT.Example.WithSQLDB
{
    public class UserRole
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }
    }
}