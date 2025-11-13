using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace UCHEBKA.Models;

public partial class UchebnayaLeto2025Context : DbContext
{
    public UchebnayaLeto2025Context()
    {
    }

    public UchebnayaLeto2025Context(DbContextOptions<UchebnayaLeto2025Context> options)
        : base(options)
    {
    }






    public virtual DbSet<Event> Events { get; set; }

    public virtual DbSet<EventEventType> EventEventTypes { get; set; }

    public virtual DbSet<EventJury> EventJuries { get; set; }

    public virtual DbSet<EventType> EventTypes { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Section> Sections { get; set; }

    public virtual DbSet<SectionEvent> SectionEvents { get; set; }

    public virtual DbSet<Sex> Sexes { get; set; }

    public virtual DbSet<User> Users { get; set; }


    public virtual DbSet<UserEvent> UserEvents { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserSec> UserSecs { get; set; }

    public virtual DbSet<UserSex> UserSexes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=USER;Database=UchebnayaZIMA;Trusted_Connection=True;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

        modelBuilder.Entity<Event>(entity =>
        {
            entity.HasKey(e => e.EventId).HasName("PK__Event__7944C870E55878B3");

            entity.ToTable("Event");

            entity.Property(e => e.EventId)
                .ValueGeneratedNever()
                .HasColumnName("EventID");
            entity.Property(e => e.EventDuration).HasColumnName("Event_Duration");
            entity.Property(e => e.EventLogoUrl).HasColumnName("Event_logoURL");
            entity.Property(e => e.EventStartTime)
                .HasColumnType("datetime")
                .HasColumnName("Event_Start_Time");
            entity.Property(e => e.EventTitle).HasColumnName("Event_Title");
        });

        modelBuilder.Entity<EventEventType>(entity =>
        {
            entity.HasKey(e => e.EventEvenTypeId).HasName("PK__Event_Ev__E1B0866A3D0758E9");

            entity.ToTable("Event_EventType");

            entity.Property(e => e.EventEvenTypeId)
                .ValueGeneratedNever()
                .HasColumnName("Event_EvenTypeID");
            entity.Property(e => e.FkEvenTypeId).HasColumnName("FK_EvenType_ID");
            entity.Property(e => e.FkEventId).HasColumnName("FK_Event_ID");

            entity.HasOne(d => d.FkEvenType).WithMany(p => p.EventEventTypes)
                .HasForeignKey(d => d.FkEvenTypeId)
                .HasConstraintName("FK_EventEventType_EventType");

            entity.HasOne(d => d.FkEvent).WithMany(p => p.EventEventTypes)
                .HasForeignKey(d => d.FkEventId)
                .HasConstraintName("FK_EventEventType_Event");
        });

        

        modelBuilder.Entity<EventType>(entity =>
        {
            entity.HasKey(e => e.EventTypeId).HasName("PK__EventTyp__A9216B1FABED362B");

            entity.ToTable("EventType");

            entity.Property(e => e.EventTypeId)
                .ValueGeneratedNever()
                .HasColumnName("EventTypeID");
            entity.Property(e => e.EventTypeName).HasColumnName("EventType_Name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A7198897A");

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("RoleID");
            entity.Property(e => e.RoleName)
                .HasMaxLength(30)
                .HasColumnName("Role_Name");
        });

        modelBuilder.Entity<Section>(entity =>
        {
            entity.HasKey(e => e.SecId).HasName("PK__Section__14A3699F8F65F1EC");

            entity.ToTable("Section");

            entity.Property(e => e.SecId)
                .ValueGeneratedNever()
                .HasColumnName("SecID");
            entity.Property(e => e.SecName).HasColumnName("Sec_Name");
        });

        modelBuilder.Entity<SectionEvent>(entity =>
        {
            entity.HasKey(e => e.SecEventId).HasName("PK__Section___CF4E4853B823711E");

            entity.ToTable("Section_Event");

            entity.Property(e => e.SecEventId)
                .ValueGeneratedNever()
                .HasColumnName("Sec_Event_ID");
            entity.Property(e => e.FkEventId).HasColumnName("FK_Event_ID");
            entity.Property(e => e.FkSecId).HasColumnName("FK_Sec_ID");

            entity.HasOne(d => d.FkEvent).WithMany(p => p.SectionEvents)
                .HasForeignKey(d => d.FkEventId)
                .HasConstraintName("FK_SectionEvent_Event");

            entity.HasOne(d => d.FkSec).WithMany(p => p.SectionEvents)
                .HasForeignKey(d => d.FkSecId)
                .HasConstraintName("FK_SectionEvent_Section");
        });

        modelBuilder.Entity<Sex>(entity =>
        {
            entity.HasKey(e => e.SexId).HasName("PK__Sex__75622DB6A0856907");

            entity.ToTable("Sex");

            entity.Property(e => e.SexId)
                .ValueGeneratedNever()
                .HasColumnName("SexID");
            entity.Property(e => e.SexName).HasColumnName("Sex_Name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CCAC157B16E9");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("UserID");
            entity.Property(e => e.UserBirthDay)
                .HasColumnType("datetime")
                .HasColumnName("User_BirthDay");
            entity.Property(e => e.UserEmail).HasColumnName("User_Email");
            entity.Property(e => e.UserLastname).HasColumnName("User_Lastname");
            entity.Property(e => e.UserName).HasColumnName("User_Name");
            entity.Property(e => e.UserPassword).HasColumnName("User_Password");
            entity.Property(e => e.UserPhone).HasColumnName("User_Phone");
            entity.Property(e => e.UserPhoto).HasColumnName("User_Photo");
            entity.Property(e => e.UserSurname).HasColumnName("User_Surname");
        });

      

        modelBuilder.Entity<UserEvent>(entity =>
        {
            entity.HasKey(e => e.UserEventId).HasName("PK__User_Eve__4663C0036CD0D3EA");

            entity.ToTable("User_Event");

            entity.Property(e => e.UserEventId)
                .ValueGeneratedNever()
                .HasColumnName("User_Event_ID");
            entity.Property(e => e.FkEventId).HasColumnName("FK_Event_ID");
            entity.Property(e => e.FkUserId).HasColumnName("FK_User_ID");

            entity.HasOne(d => d.FkEvent).WithMany(p => p.UserEvents)
                .HasForeignKey(d => d.FkEventId)
                .HasConstraintName("FK_UserEvent_Event");

            entity.HasOne(d => d.FkUser).WithMany(p => p.UserEvents)
                .HasForeignKey(d => d.FkUserId)
                .HasConstraintName("FK_UserEvent_User");
        });

        modelBuilder.Entity<UserRole>(entity =>
        {
            entity.HasKey(e => e.UserRoleId).HasName("PK__User_Rol__134E48EC93C851D8");

            entity.ToTable("User_Role");

            entity.Property(e => e.UserRoleId)
                .ValueGeneratedNever()
                .HasColumnName("User_Role_ID");
            entity.Property(e => e.FkRoleId).HasColumnName("FK_Role_ID");
            entity.Property(e => e.FkUserId).HasColumnName("FK_User_ID");

            entity.HasOne(d => d.FkRole).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.FkRoleId)
                .HasConstraintName("FK_UserRole_Role");

            entity.HasOne(d => d.FkUser).WithMany(p => p.UserRoles)
                .HasForeignKey(d => d.FkUserId)
                .HasConstraintName("FK_UserRole_User");
        });

        modelBuilder.Entity<UserSec>(entity =>
        {
            entity.HasKey(e => e.UserSecId).HasName("PK__User_Sec__99DF19A4703F9DD2");

            entity.ToTable("User_Sec");

            entity.Property(e => e.UserSecId)
                .ValueGeneratedNever()
                .HasColumnName("User_Sec_ID");
            entity.Property(e => e.FkSecId).HasColumnName("FK_Sec_ID");
            entity.Property(e => e.FkUserId).HasColumnName("FK_User_ID");

            entity.HasOne(d => d.FkSec).WithMany(p => p.UserSecs)
                .HasForeignKey(d => d.FkSecId)
                .HasConstraintName("FK_UserSec_Section");

            entity.HasOne(d => d.FkUser).WithMany(p => p.UserSecs)
                .HasForeignKey(d => d.FkUserId)
                .HasConstraintName("FK_UserSec_User");
        });

        modelBuilder.Entity<UserSex>(entity =>
        {
            entity.HasKey(e => e.UserSecId).HasName("PK__User_Sex__99DF19A4AB466115");

            entity.ToTable("User_Sex");

            entity.Property(e => e.UserSecId)
                .ValueGeneratedNever()
                .HasColumnName("User_Sec_ID");
            entity.Property(e => e.FkSexId).HasColumnName("FK_Sex_ID");
            entity.Property(e => e.FkUserId).HasColumnName("FK_User_ID");

            entity.HasOne(d => d.FkSex).WithMany(p => p.UserSexes)
                .HasForeignKey(d => d.FkSexId)
                .HasConstraintName("FK_UserSex_Sex");

            entity.HasOne(d => d.FkUser).WithMany(p => p.UserSexes)
                .HasForeignKey(d => d.FkUserId)
                .HasConstraintName("FK_UserSex_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
