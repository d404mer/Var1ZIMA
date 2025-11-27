using System;
using System.Collections.Generic;

namespace UCHEBKA.Models;

/// <summary>
/// Модель типа мероприятия
/// </summary>
public partial class EventType
{
    public long EventTypeId { get; set; }

    public string? EventTypeName { get; set; }

    public virtual ICollection<EventEventType> EventEventTypes { get; set; } = new List<EventEventType>();
}
