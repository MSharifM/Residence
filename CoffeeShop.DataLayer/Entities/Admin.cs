using System;
using System.Collections.Generic;

namespace CoffeeShop.DataLayer.Entities;

public partial class Admin
{
    public string UserId { get; set; } = null!;

    public string Access { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}
