using System.Collections.Generic;

namespace CodeWithSaar.FishCard.Models;

public class Role
{
    public string RoleId { get; set; } = default!;
    public string Description { get; set; } = default!;
    public List<User> Users { get; } = new List<User>();
}