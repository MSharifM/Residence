using System;
using System.Collections.Generic;

namespace CoffeeShop.DataLayer.Entities;

public partial class Residence
{
    public int ResidenceId { get; set; }

    public string UserId { get; set; } = null!;

    public int CityId { get; set; }

    public int? Capacity { get; set; }

    public string Street { get; set; } = null!;

    public int PostalCode { get; set; }

    public string ResidenceName { get; set; } = null!;

    public string ResidenceType { get; set; } = null!;

    public string Star { get; set; } = null!;

    public string Situation { get; set; } = null!;

    public DateOnly? ResidenceDate { get; set; }

    public virtual City City { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual ICollection<Room> Rooms { get; set; } = new List<Room>();

    public virtual HHost User { get; set; } = null!;

    public virtual ICollection<HOption> OptionNames { get; set; } = new List<HOption>();
}
