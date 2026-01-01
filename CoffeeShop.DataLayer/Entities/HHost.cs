using CoffeeShop.DataLayer.Entities;

namespace CoffeeShop.DataLayer.Entities;

public partial class HHost
{
    public string UserId { get; set; } = null!;

    public string AccNumber { get; set; } = null!;

    public virtual ICollection<Residence> Residences { get; set; } = new List<Residence>();

    public virtual User User { get; set; } = null!;
}