using System;
using System.Collections.Generic;

namespace UCHEBKA.Models;

public partial class Sex
{
    public long SexId { get; set; }

    public string? SexName { get; set; }

    public virtual ICollection<UserSex> UserSexes { get; set; } = new List<UserSex>();
}
