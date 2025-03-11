using System;
using System.Collections.Generic;

namespace prielbrusje_tab.Models;

public partial class Order
{
    public int Id { get; set; }

    public string Code { get; set; } = null!;

    public DateTime DateTimeOrder { get; set; }

    public DateOnly? DateClose { get; set; }

    public TimeOnly RentTime { get; set; }

    public int IdStatus { get; set; }

    public int IdClient { get; set; }

    public virtual ClientInfo IdClientNavigation { get; set; } = null!;

    public virtual Status IdStatusNavigation { get; set; } = null!;

    public virtual ICollection<Service> IdServices { get; set; } = new List<Service>();
}
