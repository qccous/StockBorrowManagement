using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace StockManagement.Models
{
    public partial class StockManagementContext : DbContext
    {
        public StockManagementContext()
        {
        }

        public StockManagementContext(DbContextOptions<StockManagementContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Asset> Assets { get; set; } = null!;
        public virtual DbSet<BorrowingAsset> BorrowingAssets { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server =DESKTOP-O7J5K9T; database = StockManagement;uid=sa;pwd=123456;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Asset>(entity =>
            {
                entity.Property(e => e.AssetName).HasMaxLength(250);

                entity.Property(e => e.Image)
                    .HasMaxLength(250)
                    .HasColumnName("image");

                entity.Property(e => e.Specification).HasMaxLength(250);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Assets)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Assets__Category__398D8EEE");
            });

            modelBuilder.Entity<BorrowingAsset>(entity =>
            {
                entity.ToTable("BorrowingAsset");

                entity.Property(e => e.BorrowDate).HasColumnType("date");

                entity.Property(e => e.DueDate).HasColumnType("date");

                entity.Property(e => e.RetrurnDate).HasColumnType("date");

                entity.HasOne(d => d.Asset)
                    .WithMany(p => p.BorrowingAssets)
                    .HasForeignKey(d => d.AssetId)
                    .HasConstraintName("FK__Borrowing__Asset__2C3393D0");

                entity.HasOne(d => d.Borrower)
                    .WithMany(p => p.BorrowingAssets)
                    .HasForeignKey(d => d.BorrowerId)
                    .HasConstraintName("FK__Borrowing__Borro__412EB0B6");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryName).HasMaxLength(250);
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.RoleName).HasMaxLength(250);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.FirstName).HasMaxLength(250);

                entity.Property(e => e.LastName).HasMaxLength(250);

                entity.Property(e => e.Password).HasMaxLength(250);

                entity.Property(e => e.Username).HasMaxLength(250);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Users__RoleId__4222D4EF");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
