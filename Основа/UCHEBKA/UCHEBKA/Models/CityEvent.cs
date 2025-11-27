using System;
using System.Collections.Generic;

namespace UCHEBKA.Models;

/// <summary>
/// Модель связи города с мероприятием
/// </summary>
public partial class CityEvent
{
    public long CityEventId { get; set; }

    public long? FkEventId { get; set; }

    public long? FkCityId { get; set; }

    public virtual City? FkCity { get; set; }

    public virtual Event? FkEvent { get; set; }
}
