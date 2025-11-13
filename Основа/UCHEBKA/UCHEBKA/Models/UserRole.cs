using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace UCHEBKA.Models;

public partial class UserRole
{
    public long UserRoleId { get; set; }

    public long? FkUserId { get; set; }

    public long? FkRoleId { get; set; }

    public virtual Role? FkRole { get; set; }

    public virtual User? FkUser { get; set; }
}
