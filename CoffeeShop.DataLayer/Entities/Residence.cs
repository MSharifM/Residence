using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

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

    [MaxLength(70)]
    public string MainImage { get; set; }

    public decimal Price { get; set; }

    public int RemainingCapacity { get; set; }

    [MaxLength(200)]
    public string? Description { get; set; }

    public DateTime CreateDate { get; set; } = DateTime.Now;

    #region Relations

    public virtual City City { get; set; } = null!;

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual HHost User { get; set; } = null!;

    public virtual ICollection<HOption> OptionNames { get; set; } = new List<HOption>();

    public virtual ICollection<DeactiveTime> DeactiveTimes { get; set; } = new List<DeactiveTime>();

    #endregion Relations
}