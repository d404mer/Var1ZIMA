using System;
using System.Collections.Generic;

namespace UCHEBKA.Models;

/// <summary>
/// Модель связи мероприятия с жюри
/// </summary>
public partial class EventJury
{
    public long EventJuryId { get; set; }

    public long? FkEventId { get; set; }

    public long? FkActivityId { get; set; }

    public long? FkJuryId { get; set; }


    public virtual Event? FkEvent { get; set; }

    public virtual User? FkJury { get; set; }
}
