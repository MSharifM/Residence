using System;
using System.Collections.Generic;

namespace CoffeeShop.DataLayer.Entities;

public partial class Payment
{
    public int PayId { get; set; }

    public DateOnly CreatePay { get; set; }

    public decimal? Price { get; set; }

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
