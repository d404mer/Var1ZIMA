using System;
using System.Collections.Generic;

namespace UCHEBKA.Models;

/// <summary>
/// Модель страны
/// </summary>
public partial class Country
{
    public long CountryCode { get; set; }

    public string? CountryName { get; set; }

    public string? CountryNameEn { get; set; }

    public string? CountryCode2 { get; set; }

    public virtual ICollection<UserCountry> UserCountries { get; set; } = new List<UserCountry>();
}
