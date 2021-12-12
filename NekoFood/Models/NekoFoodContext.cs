using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace NekoFood.Models
{
    public partial class NekoFoodContext : DbContext
    {
        public NekoFoodContext()
        {
        }

        public NekoFoodContext(DbContextOptions<NekoFoodContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Bento> Bentos { get; set; } = null!;
        public virtual DbSet<BentoGroup> BentoGroups { get; set; } = null!;
        public virtual DbSet<BentoOrder> BentoOrders { get; set; } = null!;
        public virtual DbSet<BentoShop> BentoShops { get; set; } = null!;
        public virtual DbSet<UserAccount> UserAccounts { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Name=ConnectionStrings:DefaultConnection");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bento>(entity =>
            {
                entity.ToTable("Bento");

                entity.Property(e => e.Name).HasMaxLength(30);

                entity.Property(e => e.ShopGuid)
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<BentoGroup>(entity =>
            {
                entity.ToTable("BentoGroup");

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Creator).HasMaxLength(30);

                entity.Property(e => e.ExpireTime).HasColumnType("datetime");

                entity.Property(e => e.GroupGuid)
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Name).HasMaxLength(30);

                entity.Property(e => e.ShopGuid)
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<BentoOrder>(entity =>
            {
                entity.ToTable("BentoOrder");

                entity.Property(e => e.BentoName).HasMaxLength(30);

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.GroupGuid)
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.Payer).HasMaxLength(30);

                entity.Property(e => e.Remark).HasMaxLength(30);

                entity.Property(e => e.ShopGuid)
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<BentoShop>(entity =>
            {
                entity.ToTable("BentoShop");

                entity.Property(e => e.Address).HasMaxLength(30);

                entity.Property(e => e.Name).HasMaxLength(30);

                entity.Property(e => e.Phone)
                    .HasMaxLength(10)
                    .IsUnicode(false);

                entity.Property(e => e.ShopGuid)
                    .HasMaxLength(32)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UserAccount>(entity =>
            {
                entity.ToTable("UserAccount");

                entity.Property(e => e.Name).HasMaxLength(30);

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.UserGroup).HasMaxLength(10);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
