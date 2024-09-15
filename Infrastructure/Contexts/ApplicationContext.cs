using System;
using System.Collections.Generic;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Contexts;

public partial class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Feature> Features { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<State> States { get; set; }

    public virtual DbSet<TaskJob> TaskJobs { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Feature>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Features__3213E83F5659DBBA");

            entity.ToTable("Feature");

            entity.Property(e => e.Id)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Role__3213E83F8542B091");

            entity.ToTable("Role");

            entity.Property(e => e.Id)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");

            entity.HasMany(d => d.Features).WithMany(p => p.Roles)
                .UsingEntity<Dictionary<string, object>>(
                    "RoleFeature",
                    r => r.HasOne<Feature>().WithMany()
                        .HasForeignKey("FeatureId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__RoleFeatu__featu__5AEE82B9"),
                    l => l.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__RoleFeatu__roleI__59FA5E80"),
                    j =>
                    {
                        j.HasKey("RoleId", "FeatureId").HasName("PK__RoleFeat__E2D757A11753B7BA");
                        j.ToTable("RoleFeature");
                    });
        });

        modelBuilder.Entity<State>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__State__3213E83F6A3EF3B8");

            entity.ToTable("State");

            entity.Property(e => e.Id)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<TaskJob>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Task__3213E83F247E34EA");

            entity.ToTable("TaskJob");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Name)
                .HasMaxLength(150)
                .HasColumnName("name");
            entity.Property(e => e.StateId)
                .HasMaxLength(30)
                .IsUnicode(false)
                .HasColumnName("stateId");
            entity.Property(e => e.UserId).HasColumnName("userId");

            entity.HasOne(d => d.State).WithMany(p => p.TaskJobs)
                .HasForeignKey(d => d.StateId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Task__stateId__60A75C0F");

            entity.HasOne(d => d.User).WithMany(p => p.TaskJobs)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Task__userId__5FB337D6");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("password");

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UserRole",
                    r => r.HasOne<Role>().WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserRole__roleId__5165187F"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__UserRole__userId__5070F446"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId").HasName("PK__UserRole__7743989D562F0610");
                        j.ToTable("UserRole");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
