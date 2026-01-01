using System;
using System.Collections.Generic;

namespace CoffeeShop.DataLayer.Entities;

public partial class City
{
    public int CityId { get; set; }

    public string CityName { get; set; } = null!;

    public virtual ICollection<Residence> Residences { get; set; } = new List<Residence>();
}
