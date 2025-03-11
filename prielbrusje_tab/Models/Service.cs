using System;
using System.Collections.Generic;

namespace prielbrusje_tab.Models;

public partial class Service
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public float PricePerHour { get; set; }

    public virtual ICollection<Order> IdOrders { get; set; } = new List<Order>();
}
