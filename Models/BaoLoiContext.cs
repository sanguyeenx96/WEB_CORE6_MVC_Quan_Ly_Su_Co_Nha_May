using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace BaoLoi.Models;

public partial class BaoloiContext : DbContext
{
    public BaoloiContext()
    {
    }

    public BaoloiContext(DbContextOptions<BaoloiContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Dulieu> Dulieus { get; set; }

    public virtual DbSet<Dulieusuachua> Dulieusuachuas { get; set; }

    public virtual DbSet<Layout> Layouts { get; set; }

    public virtual DbSet<Layoutkaizen> Layoutkaizens { get; set; }

    public virtual DbSet<Lichsubaoloi> Lichsubaolois { get; set; }

    public virtual DbSet<Lichsubaoloikaizen> Lichsubaoloikaizens { get; set; }

    public virtual DbSet<Listjig> Listjigs { get; set; }

    public virtual DbSet<MigrationHistory> MigrationHistories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=192.168.1.254;User Id=sa;Password=123;Initial Catalog=Baoloi;Trusted_connection=false;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Dulieu>(entity =>
        {
            entity.ToTable("dulieu");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cell)
                .HasMaxLength(100)
                .HasColumnName("cell");
            entity.Property(e => e.Doisach)
                .HasMaxLength(100)
                .HasColumnName("doisach");
            entity.Property(e => e.Ghichuthem)
                .HasMaxLength(100)
                .HasColumnName("ghichuthem");
            entity.Property(e => e.Hientuongloi)
                .HasMaxLength(100)
                .HasColumnName("hientuongloi");
            entity.Property(e => e.Hinhanh)
                .IsUnicode(false)
                .HasColumnName("hinhanh");
            entity.Property(e => e.Majig).HasColumnName("majig");
            entity.Property(e => e.Model)
                .HasMaxLength(100)
                .HasColumnName("model");
            entity.Property(e => e.Ngaygio)
                .HasColumnType("date")
                .HasColumnName("ngaygio");
            entity.Property(e => e.Nguoidamnhiem)
                .HasMaxLength(100)
                .HasColumnName("nguoidamnhiem");
            entity.Property(e => e.Nguyennhan)
                .HasMaxLength(100)
                .HasColumnName("nguyennhan");
            entity.Property(e => e.Quantrong)
                .HasMaxLength(100)
                .HasColumnName("quantrong");
            entity.Property(e => e.Station)
                .HasMaxLength(100)
                .HasColumnName("station");
            entity.Property(e => e.Tenjig)
                .HasMaxLength(100)
                .HasColumnName("tenjig");
            entity.Property(e => e.Thoigiansuachua).HasColumnName("thoigiansuachua");
            entity.Property(e => e.Trangthaihientai)
                .HasMaxLength(100)
                .HasColumnName("trangthaihientai");
            entity.Property(e => e.Vandeanhhuong)
                .HasMaxLength(100)
                .HasColumnName("vandeanhhuong");
            entity.Property(e => e.XacNhanSuaChua)
                .HasMaxLength(100)
                .HasColumnName("xacNhanSuaChua");
        });

        modelBuilder.Entity<Dulieusuachua>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__dulieusu__3213E83F44645C8E");

            entity.ToTable("dulieusuachua");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cell)
                .HasMaxLength(100)
                .HasColumnName("cell");
            entity.Property(e => e.Doisach)
                .HasMaxLength(100)
                .HasColumnName("doisach");
            entity.Property(e => e.Ghichuthem)
                .HasMaxLength(100)
                .HasColumnName("ghichuthem");
            entity.Property(e => e.Hientuongloi)
                .HasMaxLength(100)
                .HasColumnName("hientuongloi");
            entity.Property(e => e.Model)
                .HasMaxLength(100)
                .HasColumnName("model");
            entity.Property(e => e.Ngaygio)
                .HasMaxLength(100)
                .HasColumnName("ngaygio");
            entity.Property(e => e.Nguoidamnhiem)
                .HasMaxLength(100)
                .HasColumnName("nguoidamnhiem");
            entity.Property(e => e.Nguyennhan)
                .HasMaxLength(100)
                .HasColumnName("nguyennhan");
            entity.Property(e => e.Station)
                .HasMaxLength(100)
                .HasColumnName("station");
            entity.Property(e => e.Tenjig)
                .HasMaxLength(100)
                .HasColumnName("tenjig");
            entity.Property(e => e.Thoigiansuachua).HasColumnName("thoigiansuachua");
            entity.Property(e => e.Trangthaihientai)
                .HasMaxLength(100)
                .HasColumnName("trangthaihientai");
            entity.Property(e => e.Vandeanhhuong)
                .HasMaxLength(100)
                .HasColumnName("vandeanhhuong");
            entity.Property(e => e.XacNhanSuaChua)
                .HasMaxLength(30)
                .HasColumnName("xacNhanSuaChua");
        });

        modelBuilder.Entity<Layout>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__layout__3213E83FDD4E8E44");

            entity.ToTable("layout");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cell)
                .HasMaxLength(100)
                .HasColumnName("cell");
            entity.Property(e => e.Model)
                .HasMaxLength(100)
                .HasColumnName("model");
            entity.Property(e => e.Station)
                .HasMaxLength(100)
                .HasColumnName("station");
            entity.Property(e => e.Status)
                .HasMaxLength(100)
                .HasDefaultValueSql("(N'OK')")
                .HasColumnName("status");
            entity.Property(e => e.Thoigianbaoloi)
                .HasMaxLength(100)
                .HasColumnName("thoigianbaoloi");
        });

        modelBuilder.Entity<Layoutkaizen>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("layoutkaizen");

            entity.Property(e => e.Cell)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("cell");
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Model)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("model");
            entity.Property(e => e.Station)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("station");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("status");
            entity.Property(e => e.Thoigianbaoloi)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("thoigianbaoloi");
        });

        modelBuilder.Entity<Lichsubaoloi>(entity =>
        {
            entity.ToTable("lichsubaoloi");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cell)
                .HasMaxLength(50)
                .HasColumnName("cell");
            entity.Property(e => e.Model)
                .HasMaxLength(50)
                .HasColumnName("model");
            entity.Property(e => e.Station)
                .HasMaxLength(50)
                .HasColumnName("station");
            entity.Property(e => e.Thoigian)
                .HasMaxLength(100)
                .HasColumnName("thoigian");
        });

        modelBuilder.Entity<Lichsubaoloikaizen>(entity =>
        {
            entity.ToTable("lichsubaoloikaizen");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
            entity.Property(e => e.Cell)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("cell");
            entity.Property(e => e.Model)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("model");
            entity.Property(e => e.Station)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("station");
            entity.Property(e => e.Thoigian)
                .HasMaxLength(10)
                .IsFixedLength()
                .HasColumnName("thoigian");
        });

        modelBuilder.Entity<Listjig>(entity =>
        {
            entity.ToTable("listjig");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Cell)
                .HasMaxLength(50)
                .HasColumnName("cell");
            entity.Property(e => e.Jigname).HasColumnName("jigname");
            entity.Property(e => e.Jigno).HasColumnName("jigno");
            entity.Property(e => e.Model)
                .HasMaxLength(50)
                .HasColumnName("model");
            entity.Property(e => e.Quantrong)
                .HasMaxLength(50)
                .HasColumnName("quantrong");
            entity.Property(e => e.Station)
                .HasMaxLength(200)
                .HasColumnName("station");
        });

        modelBuilder.Entity<MigrationHistory>(entity =>
        {
            entity.HasKey(e => new { e.MigrationId, e.ContextKey }).HasName("PK_dbo.__MigrationHistory");

            entity.ToTable("__MigrationHistory");

            entity.Property(e => e.MigrationId).HasMaxLength(150);
            entity.Property(e => e.ContextKey).HasMaxLength(300);
            entity.Property(e => e.ProductVersion).HasMaxLength(32);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
