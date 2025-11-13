using System;
using System.Collections.Generic;

namespace UCHEBKA.Models;

public partial class UserEvent
{
    public long UserEventId { get; set; }

    public long? FkUserId { get; set; }

    public long? FkEventId { get; set; }

    public virtual Event? FkEvent { get; set; }

    public virtual User? FkUser { get; set; }
}
