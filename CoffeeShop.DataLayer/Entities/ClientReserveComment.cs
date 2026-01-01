using System;
using System.Collections.Generic;

namespace CoffeeShop.DataLayer.Entities;

public partial class ClientReserveComment
{
    public string UserId { get; set; } = null!;

    public int CommentId { get; set; }

    public int ReservationId { get; set; }

    public virtual Comment Comment { get; set; } = null!;

    public virtual Reservation Reservation { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
