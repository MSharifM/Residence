using System;
using System.Collections.Generic;

namespace CoffeeShop.DataLayer.Entities;

public partial class Comment
{
    public int CommentId { get; set; }

    public string? CommentDescription { get; set; }

    public int ResidenceId { get; set; }

    public int Rate { get; set; }

    public DateOnly? CreateDate { get; set; }

    public string CommentStatus { get; set; } = null!;

    public virtual ICollection<ClientReserveComment> ClientReserveComments { get; set; } = new List<ClientReserveComment>();

    public virtual Residence Residence { get; set; } = null!;

    public virtual ICollection<Admin> Users { get; set; } = new List<Admin>();
}
