using System;
using System.Collections.Generic;

namespace CoffeeShop.DataLayer.Entities;

public partial class HOption
{
    public string OptionName { get; set; } = null!;

    public string OptionDescription { get; set; } = null!;

    public virtual ICollection<Residence> Residences { get; set; } = new List<Residence>();
}
