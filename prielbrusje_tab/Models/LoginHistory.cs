using System;
using System.Collections.Generic;

namespace prielbrusje_tab.Models;

public partial class LoginHistory
{
    public int Id { get; set; }

    public DateTime LoginDateTime { get; set; }

    public bool IsValid { get; set; }

    public virtual ICollection<User> IdUsers { get; set; } = new List<User>();

    public string LoginResult => IsValid == true ? "Успешно" : "Неуспешно" ;
}
