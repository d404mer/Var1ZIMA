using System;
using System.Collections.Generic;

namespace UCHEBKA.Models;

public partial class Event
{
    public long EventId { get; set; }

    public string? EventTitle { get; set; }

    public DateTime? EventStartTime { get; set; }

    public int? EventDuration { get; set; }

    public string? EventLogoUrl { get; set; }



    public virtual ICollection<EventEventType> EventEventTypes { get; set; } = new List<EventEventType>();

    public virtual ICollection<EventJury> EventJuries { get; set; } = new List<EventJury>();

    public virtual ICollection<SectionEvent> SectionEvents { get; set; } = new List<SectionEvent>();

    public virtual ICollection<UserEvent> UserEvents { get; set; } = new List<UserEvent>();
}
