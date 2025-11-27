using System;
using System.Collections.Generic;

namespace UCHEBKA.Models;

/// <summary>
/// Модель связи пользователя с секцией
/// </summary>
public partial class UserSec
{
    public long UserSecId { get; set; }

    public long? FkUserId { get; set; }

    public long? FkSecId { get; set; }

    public virtual Section? FkSec { get; set; }

    public virtual User? FkUser { get; set; }
}
