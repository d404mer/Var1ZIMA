using System;
using System.Collections.Generic;

namespace UCHEBKA.Models;

/// <summary>
/// Модель активности/деятельности
/// </summary>
public partial class Activity
{
    public long ActivityId { get; set; }

    public string? ActivityName { get; set; }

    public int? ActivityScore { get; set; }

    public virtual ICollection<ActivityEvent> ActivityEvents { get; set; } = new List<ActivityEvent>();

    public virtual ICollection<EventJury> EventJuries { get; set; } = new List<EventJury>();
}
