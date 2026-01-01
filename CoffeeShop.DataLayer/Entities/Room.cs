using System;
using System.Collections.Generic;

namespace CoffeeShop.DataLayer.Entities;

public partial class Room
{
    public int RoomId { get; set; }

    public int ResidenceId { get; set; }

    public string? RoomDescription { get; set; }

    public int Capacity { get; set; }

    public decimal Price { get; set; }

    public bool RoomStatus { get; set; }

    public string RoomKind { get; set; } = null!;

    public virtual ICollection<DeactiveTime> DeactiveTimes { get; set; } = new List<DeactiveTime>();

    public virtual Residence Residence { get; set; } = null!;
}
