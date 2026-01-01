using System;
using System.Collections.Generic;

namespace CoffeeShop.DataLayer.Entities;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public int ResidenceId { get; set; }

    public int? PayId { get; set; }

    public DateOnly? DateOfStart { get; set; }

    public DateOnly DateOfEnd { get; set; }

    public int? NumberOfGuests { get; set; }

    public decimal? AmountPaid { get; set; }

    public string Situation { get; set; } = null!;

    public virtual ICollection<ClientReserveComment> ClientReserveComments { get; set; } = new List<ClientReserveComment>();

    public virtual Payment? Pay { get; set; }

    public virtual Residence Residence { get; set; } = null!;

    public virtual ICollection<Clientlist> Clientlists { get; set; } = new List<Clientlist>();
}
