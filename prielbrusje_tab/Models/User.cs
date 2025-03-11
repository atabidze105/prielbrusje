using System;
using System.Collections.Generic;

namespace prielbrusje_tab.Models;

public partial class User
{
    public int Id { get; set; }

    public string Lastname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Patronymic { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int IdRole { get; set; }

    public int? IdClientInfo { get; set; }

    public virtual ClientInfo? IdClientInfoNavigation { get; set; }

    public virtual Role IdRoleNavigation { get; set; } = null!;

    public virtual ICollection<LoginHistory> IdLogins { get; set; } = new List<LoginHistory>();
}
