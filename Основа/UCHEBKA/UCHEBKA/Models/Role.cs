using System;
using System.Collections.Generic;

namespace UCHEBKA.Models;

/// <summary>
/// Модель роли пользователя
/// </summary>
public partial class Role
{
    public long RoleId { get; set; }

    public string? RoleName { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
