using System;
using System.Collections.Generic;

namespace CodeWithSaar.FishCard.Models;

public class User
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = default!;
    public string HashedPassword { get; set; } = default!;

    public List<Role> Roles { get; } = new List<Role>();
}
