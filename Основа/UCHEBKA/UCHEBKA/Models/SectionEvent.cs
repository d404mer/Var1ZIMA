using System;
using System.Collections.Generic;

namespace UCHEBKA.Models;

/// <summary>
/// Модель связи секции с мероприятием
/// </summary>
public partial class SectionEvent
{
    public long SecEventId { get; set; }

    public long? FkSecId { get; set; }

    public long? FkEventId { get; set; }

    public virtual Event? FkEvent { get; set; }

    public virtual Section? FkSec { get; set; }
}
