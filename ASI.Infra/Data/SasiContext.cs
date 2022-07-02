using System;
using ASI.Infra.Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace ASI.Infra.Data
{
    public partial class SasiContext : DbContext
    {
        public SasiContext()
        {
        }

        public SasiContext(DbContextOptions<SasiContext> options)
            : base(options)
        {
        }

        public virtual DbSet<DadosTreinamento> DadosTreinamento { get; set; }
        public virtual DbSet<Modelo> Modelo { get; set; }
        public virtual DbSet<ModeloTreinado> ModeloTreinado { get; set; }
        public virtual DbSet<ParametrosModelo> ParametrosModelo { get; set; }
        public virtual DbSet<Treinamento> Treinamento { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=.;Database=sasi;User Id=sa;Password=Senha12345;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DadosTreinamento>(entity =>
            {
                entity.Property(e => e.Dados)
                    .IsRequired()
                    .HasColumnType("text");

                entity.HasOne(d => d.ParametrosModelo)
                    .WithMany(p => p.DadosTreinamento)
                    .HasForeignKey(d => d.ParametrosModeloId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DadosTreinamento_ParametrosModelo");
            });

            modelBuilder.Entity<Modelo>(entity =>
            {
                entity.HasIndex(e => e.Id)
                    .HasName("IX_Modelo")
                    .IsUnique();

                entity.Property(e => e.DataUltimoTreinamento).HasColumnType("datetime");

                entity.Property(e => e.FrequenciaTreinamentos)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Nome)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<ModeloTreinado>(entity =>
            {
                entity.Property(e => e.Dados).IsRequired();

                entity.HasOne(d => d.ParametrosModelo)
                    .WithMany(p => p.ModeloTreinado)
                    .HasForeignKey(d => d.ParametrosModeloId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ModeloTreinado_ParametrosModelo");
            });

            modelBuilder.Entity<ParametrosModelo>(entity =>
            {
                entity.Property(e => e.CamposEntrada)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.CamposSaida)
                    .IsRequired()
                    .HasColumnType("text");

                entity.Property(e => e.Data)
                    .IsRequired()
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.Modelo)
                    .WithMany(p => p.ParametrosModelo)
                    .HasForeignKey(d => d.ModeloId)
                    .HasConstraintName("FK_ParametrosModelo_Modelo");
            });

            modelBuilder.Entity<Treinamento>(entity =>
            {
                entity.HasIndex(e => e.Id)
                    .HasName("IX_Treinamento");

                entity.Property(e => e.Data).HasColumnType("datetime");

                entity.HasOne(d => d.Modelo)
                    .WithMany(p => p.Treinamento)
                    .HasForeignKey(d => d.ModeloId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Treinamento_Modelo");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
