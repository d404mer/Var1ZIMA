using System;
using System.Collections.Generic;

namespace UCHEBKA.Models;

public partial class Section
{
    public long SecId { get; set; }

    public string? SecName { get; set; }

    public virtual ICollection<SectionEvent> SectionEvents { get; set; } = new List<SectionEvent>();

    public virtual ICollection<UserSec> UserSecs { get; set; } = new List<UserSec>();
}
