using System;
using System.Collections.Generic;

namespace CoffeeShop.DataLayer.Entities;

public partial class Clientlist
{
    public string UserId { get; set; } = null!;

    public string Pin { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public bool? Sex { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
