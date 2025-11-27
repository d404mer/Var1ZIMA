using System;
using System.Collections.Generic;

namespace UCHEBKA.Models;

/// <summary>
/// Модель города
/// </summary>
public partial class City
{
    public long CityId { get; set; }

    public string? CityName { get; set; }

    public string? CityUrl { get; set; }

    public virtual ICollection<CityEvent> CityEvents { get; set; } = new List<CityEvent>();
}
