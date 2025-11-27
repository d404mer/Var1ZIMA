using System;
using System.Collections.Generic;

namespace UCHEBKA.Models;

/// <summary>
/// Модель пользователя системы
/// </summary>
public partial class User
{
    public long UserId { get; set; }

    public string? UserName { get; set; }

    public string? UserSurname { get; set; }

    public string? UserLastname { get; set; }

    public string? UserEmail { get; set; }

    public DateTime? UserBirthDay { get; set; }

    public string? UserPhone { get; set; }

    public string? UserPhoto { get; set; }

    public string? UserPassword { get; set; }

    public virtual ICollection<EventJury> EventJuries { get; set; } = new List<EventJury>();


    public virtual ICollection<UserEvent> UserEvents { get; set; } = new List<UserEvent>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

    public virtual ICollection<UserSec> UserSecs { get; set; } = new List<UserSec>();

    public virtual ICollection<UserSex> UserSexes { get; set; } = new List<UserSex>();
}
