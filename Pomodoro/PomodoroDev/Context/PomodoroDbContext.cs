using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Entities;

namespace WebApplication1.Context;

public partial class PomodoroDbContext : DbContext
{
    public PomodoroDbContext(DbContextOptions<PomodoroDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ciclo> Ciclos { get; set; }

    public virtual DbSet<HistoricoMigraco> HistoricoMigracoes { get; set; }

    public virtual DbSet<Tarefa> Tarefas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ciclo>(entity =>
        {
            entity.HasOne(d => d.Tarefa).WithMany(p => p.Ciclos)
                .HasForeignKey(d => d.TarefaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Tarefa");
        });

        modelBuilder.Entity<HistoricoMigraco>(entity =>
        {
            entity.HasKey(e => e.Nome).HasName("PK_HistoricoMigracao");

            entity.Property(e => e.Nome).HasMaxLength(200);
            entity.Property(e => e.AplicadoEm).HasDefaultValueSql("(sysutcdatetime())", "DF_HistoricoMigracoes_AplicadoEm");
            entity.Property(e => e.AplicadoPor)
                .HasMaxLength(128)
                .HasDefaultValueSql("(suser_sname())", "DF_HistoricoMigracoes_Por");
        });

        modelBuilder.Entity<Tarefa>(entity =>
        {
            entity.Property(e => e.Cor)
                .HasMaxLength(7)
                .IsUnicode(false)
                .HasDefaultValue("#4b5563", "DF_Tarefas_Cor");
            entity.Property(e => e.NomeDaTarefa)
                .HasMaxLength(100)
                .IsUnicode(false);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
