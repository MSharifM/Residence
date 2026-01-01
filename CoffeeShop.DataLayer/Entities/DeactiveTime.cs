using System;
using System.Collections.Generic;

namespace CoffeeShop.DataLayer.Entities;

public partial class DeactiveTime
{
    public int RoomId { get; set; }

    public int ResidenceId { get; set; }

    public DateOnly StartTime { get; set; }

    public DateOnly EndTime { get; set; }

    public string? DisableDescription { get; set; }

    public virtual Room Room { get; set; } = null!;
}
