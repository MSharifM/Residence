using Microsoft.AspNetCore.Identity;

namespace CoffeeShop.DataLayer.Entities;

public partial class User : IdentityUser
{
    public DateTime CreateDate { get; set; } = DateTime.Now;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? PhoneNumber2 { get; set; }

    public bool? Sex { get; set; }

    public virtual Admin? Admin { get; set; }

    public virtual ICollection<ClientReserveComment> ClientReserveComments { get; set; } = new List<ClientReserveComment>();

    public virtual ICollection<Clientlist> Clientlists { get; set; } = new List<Clientlist>();

    public virtual HHost? HHost { get; set; }
}