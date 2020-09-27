using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace qrmenuapp.Models
{
    public partial class QrMenuContext : DbContext
    {
        public QrMenuContext()
        {
        }

        public QrMenuContext(DbContextOptions<QrMenuContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Categorias> Categorias { get; set; }
        public virtual DbSet<Empresas> Empresas { get; set; }
        public virtual DbSet<ItemUserLike> ItemUserLike { get; set; }
        public virtual DbSet<Items> Items { get; set; }
        public virtual DbSet<Monedas> Monedas { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=LEX-PC\\PCLEX;Database=QrMenu;User Id=sa;Password=123;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Categorias>(entity =>
            {
                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Empresas>(entity =>
            {
                entity.HasKey(e => e.Name)
                    .HasName("PK_MenuItems");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DescripcionName)
                    .IsRequired()
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.UrlImagen)
                    .HasMaxLength(600)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ItemUserLike>(entity =>
            {
                entity.HasKey(e => new { e.ItemId, e.UserId });

                entity.HasIndex(e => e.ItemId)
                    .HasName("IX_ItemUserLike");
            });

            modelBuilder.Entity<Items>(entity =>
            {
                entity.HasIndex(e => e.EmpresaName)
                    .HasName("IX_Items");

                entity.Property(e => e.Descripcion)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.EmpresaName)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.HasIva).HasColumnName("hasIva");

                entity.Property(e => e.Price).HasColumnType("money");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.UrlImagen)
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.HasOne(d => d.Categoria)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.CategoriaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Items_Categorias");

                entity.HasOne(d => d.EmpresaNameNavigation)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.EmpresaName)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Items_Empresas");

                entity.HasOne(d => d.Moneda)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.MonedaId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Items_Monedas");
            });

            modelBuilder.Entity<Monedas>(entity =>
            {
                entity.Property(e => e.Descripcion)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
