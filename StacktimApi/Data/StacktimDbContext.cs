using Microsoft.EntityFrameworkCore;
using StacktimApi.Models;

namespace StacktimApi.Data
{
    public partial class StacktimDbContext : DbContext
    {
        public StacktimDbContext()
        {
        }

        public StacktimDbContext(DbContextOptions<StacktimDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Player> Players { get; set; }
        public virtual DbSet<Team> Teams { get; set; }
        public virtual DbSet<TeamPlayer> TeamPlayers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // WARNING : To protect potentially sensitive information in your connection string,
            // move it out of source code. Use appsettings.json and the Name= syntax instead.
            optionsBuilder.UseSqlServer("Server=localhost\\SQLEXPRESS02;Database=StacktimDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Player>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Players__3214EC07590333DA");

                entity.HasIndex(e => e.Email, "UQ_Players_Email").IsUnique();
                entity.HasIndex(e => e.Pseudo, "UQ_Players_Pseudo").IsUnique();

                entity.Property(e => e.Email)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Pseudo)
                    .HasMaxLength(50)
                    .IsUnicode(false);
                entity.Property(e => e.Rank)
                    .HasMaxLength(20)
                    .IsUnicode(false);
                entity.Property(e => e.RegistrationDate).HasDefaultValueSql("(getdate())");
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.HasKey(e => e.Id).HasName("PK__Teams__3214EC0750703524");

                entity.HasIndex(e => e.Name, "UQ_Teams_Name").IsUnique();
                entity.HasIndex(e => e.Tag, "UQ_Teams_Tag").IsUnique();

                entity.Property(e => e.CreationDate).HasDefaultValueSql("(getdate())");
                entity.Property(e => e.Name)
                    .HasMaxLength(100)
                    .IsUnicode(false);
                entity.Property(e => e.Tag)
                    .HasMaxLength(3)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.HasOne<Player>()
                     .WithMany()
                     .HasForeignKey(e => e.CaptainId)
                     .HasConstraintName("FK_Teams_Players_Captain")
                     .OnDelete(DeleteBehavior.Restrict); // empêche la suppression d’un joueur capitaine
            });


            modelBuilder.Entity<TeamPlayer>(entity =>
            {
                entity.HasKey(e => new { e.TeamId, e.PlayerId });

                entity.Property(e => e.JoinDate).HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.Player).WithMany(p => p.TeamPlayers)
                    .HasForeignKey(d => d.PlayerId)
                    .HasConstraintName("FK_TeamPlayers_Players");

                entity.HasOne(d => d.Team).WithMany(p => p.TeamPlayers)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK_TeamPlayers_Teams");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
