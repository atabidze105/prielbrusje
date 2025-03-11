using System;
using System.Collections.Generic;

namespace prielbrusje_tab.Models;

public partial class ClientInfo
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public string PassportSerie { get; set; } = null!;

    public string PassportCode { get; set; } = null!;

    public DateOnly DateOfBirth { get; set; }

    public string Address { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
