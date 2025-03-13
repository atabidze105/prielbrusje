using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using prielbrusje_tab.Models;

namespace prielbrusje_tab.Context;

public partial class User011Context : DbContext
{
    public User011Context()
    {
    }

    public User011Context(DbContextOptions<User011Context> options)
        : base(options)
    {
    }

    public virtual DbSet<ClientInfo> ClientInfos { get; set; }

    public virtual DbSet<LoginHistory> LoginHistories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Status> Statuses { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseNpgsql("Host=89.110.53.87:5522;Database=user011;Username=user011;password=83292");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClientInfo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("client_info_pk");

            entity.ToTable("client_info", "prielbrusje");

            entity.HasIndex(e => e.Code, "client_info_unique").IsUnique();

            entity.HasIndex(e => e.PassportSerie, "client_info_unique_1").IsUnique();

            entity.HasIndex(e => e.PassportCode, "client_info_unique_2").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Address)
                .HasColumnType("character varying")
                .HasColumnName("address");
            entity.Property(e => e.Code)
                .HasColumnType("character varying")
                .HasColumnName("code");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.PassportCode)
                .HasColumnType("character varying")
                .HasColumnName("passport_code");
            entity.Property(e => e.PassportSerie)
                .HasColumnType("character varying")
                .HasColumnName("passport_serie");
        });

        modelBuilder.Entity<LoginHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("login_history_pk");

            entity.ToTable("login_history", "prielbrusje");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.IsValid).HasColumnName("is_valid");
            entity.Property(e => e.LoginDateTime)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("login_date_time");
        });

        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("order_pk");

            entity.ToTable("order", "prielbrusje");

            entity.HasIndex(e => e.Code, "order_unique").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Code)
                .HasColumnType("character varying")
                .HasColumnName("code");
            entity.Property(e => e.DateClose).HasColumnName("date_close");
            entity.Property(e => e.DateTimeOrder)
                .HasColumnType("timestamp without time zone")
                .HasColumnName("date_time_order");
            entity.Property(e => e.IdClient).HasColumnName("id_client");
            entity.Property(e => e.IdStatus).HasColumnName("id_status");
            entity.Property(e => e.RentTime).HasColumnName("rent_time");

            entity.HasOne(d => d.IdClientNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdClient)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_client_info_fk");

            entity.HasOne(d => d.IdStatusNavigation).WithMany(p => p.Orders)
                .HasForeignKey(d => d.IdStatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("order_status_fk");

            entity.HasMany(d => d.IdServices).WithMany(p => p.IdOrders)
                .UsingEntity<Dictionary<string, object>>(
                    "OrderService",
                    r => r.HasOne<Service>().WithMany()
                        .HasForeignKey("IdService")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("order_service_service_fk"),
                    l => l.HasOne<Order>().WithMany()
                        .HasForeignKey("IdOrder")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("order_service_order_fk"),
                    j =>
                    {
                        j.HasKey("IdOrder", "IdService").HasName("order_service_pk");
                        j.ToTable("order_service", "prielbrusje");
                        j.IndexerProperty<int>("IdOrder").HasColumnName("id_order");
                        j.IndexerProperty<int>("IdService").HasColumnName("id_service");
                    });
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("role_pk");

            entity.ToTable("role", "prielbrusje");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("service_pk");

            entity.ToTable("service", "prielbrusje");

            entity.HasIndex(e => e.Code, "service_unique").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Code)
                .HasColumnType("character varying")
                .HasColumnName("code");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.PricePerHour).HasColumnName("price_per_hour");
        });

        modelBuilder.Entity<Status>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("status_pk");

            entity.ToTable("status", "prielbrusje");

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("user_pk");

            entity.ToTable("user", "prielbrusje");

            entity.HasIndex(e => e.Login, "user_unique").IsUnique();

            entity.HasIndex(e => e.Password, "user_unique_1").IsUnique();

            entity.Property(e => e.Id)
                .UseIdentityAlwaysColumn()
                .HasColumnName("id");
            entity.Property(e => e.IdClientInfo).HasColumnName("id_client_info");
            entity.Property(e => e.IdRole).HasColumnName("id_role");
            entity.Property(e => e.Lastname)
                .HasColumnType("character varying")
                .HasColumnName("lastname");
            entity.Property(e => e.Login)
                .HasColumnType("character varying")
                .HasColumnName("login");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasColumnType("character varying")
                .HasColumnName("password");
            entity.Property(e => e.Patronymic)
                .HasColumnType("character varying")
                .HasColumnName("patronymic");
            entity.Property(e => e.Photo)
                .HasColumnType("character varying")
                .HasColumnName("photo");

            entity.HasOne(d => d.IdClientInfoNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdClientInfo)
                .HasConstraintName("user_client_info_fk");

            entity.HasOne(d => d.IdRoleNavigation).WithMany(p => p.Users)
                .HasForeignKey(d => d.IdRole)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_role_fk");

            entity.HasMany(d => d.IdLogins).WithMany(p => p.IdUsers)
                .UsingEntity<Dictionary<string, object>>(
                    "UserLogin",
                    r => r.HasOne<LoginHistory>().WithMany()
                        .HasForeignKey("IdLogin")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("user_login_login_history_fk"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("IdUser")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("user_login_user_fk"),
                    j =>
                    {
                        j.HasKey("IdUser", "IdLogin").HasName("user_login_pk");
                        j.ToTable("user_login", "prielbrusje");
                        j.IndexerProperty<int>("IdUser").HasColumnName("id_user");
                        j.IndexerProperty<int>("IdLogin").HasColumnName("id_login");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
