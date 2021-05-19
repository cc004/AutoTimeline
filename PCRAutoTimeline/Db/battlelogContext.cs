using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace PCRApi.Models.Db
{
    public partial class battlelogContext : DbContext
    {
        public battlelogContext()
        {
        }

        public battlelogContext(DbContextOptions<battlelogContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Battlelog> Battlelogs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlite("data source=Data\\battlelog.db");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Battlelog>(entity =>
            {
                entity.ToTable("battlelog");
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Boss).HasColumnName("boss");

                entity.Property(e => e.Damage)
                    .HasColumnType("integer")
                    .HasColumnName("damage");

                entity.Property(e => e.Frame)
                    .HasColumnType("integer")
                    .HasColumnName("frame");

                entity.Property(e => e.Lap).HasColumnName("lap");

                entity.Property(e => e.Log).HasColumnName("log");

                entity.Property(e => e.Team).HasColumnName("team");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
