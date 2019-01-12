using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CosmicSpaceService.Models
{
    public partial class CosmicSpaceContext : DbContext
    {
        public CosmicSpaceContext()
        {
        }

        public CosmicSpaceContext(DbContextOptions<CosmicSpaceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Pilot> Pilots { get; set; }
        public virtual DbSet<Ship> Ships { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Map> Maps { get; set; }
        public virtual DbSet<Enemy> Enemies { get; set; }
        public virtual DbSet<EnemyMap> EnemiesMaps { get; set; }
        public virtual DbSet<Ammunition> Ammunitions { get; set; }
        public virtual DbSet<AmmunitionPilot> AmmunitionsPilots { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=DESKTOP-TI6I264;Database=CosmicSpace;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.0-rtm-35687");

            modelBuilder.Entity<Reward>(entity =>
            {
                entity.HasIndex(e => new { e.RewardId })
                    .HasName("UC_Reward")
                    .IsUnique();
            });

            modelBuilder.Entity<Map>(entity =>
            {
                entity.HasIndex(e => new { e.MapId, e.Name })
                    .HasName("UC_Map")
                    .IsUnique();

                entity.Property(e => e.RequiredLevel).HasDefaultValueSql("((1))");
                entity.Property(e => e.IsPvp).HasDefaultValueSql("((0))");
            });
            
            modelBuilder.Entity<EnemyMap>(entity =>
            {
                entity.HasKey(e => new { e.MapId, e.EnemyId });

                entity
                    .HasOne<Map>(e => e.Map)
                    .WithMany(e => e.EnemiesMaps)
                    .HasForeignKey(e => e.MapId);
                
                entity
                    .HasOne<Enemy>(e => e.Enemy)
                    .WithMany(e => e.EnemiesMaps)
                    .HasForeignKey(e => e.EnemyId);
            });

            modelBuilder.Entity<Ammunition>(entity =>
            {
                entity.HasIndex(e => new { e.AmmunitionId, e.Name })
                    .HasName("UC_Ammunition")
                    .IsUnique();

                entity.Property(e => e.MultiplierEnemy).HasDefaultValueSql("((1))");
                entity.Property(e => e.MultiplierPilot).HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<AmmunitionPilot>(entity =>
            {
                entity.HasKey(e => new { e.AmmunitionId, e.PilotId });

                entity
                    .HasOne<Ammunition>(e => e.Ammunition)
                    .WithOne(e => e.AmmunitionsPilots);

                entity
                    .HasOne<Pilot>(e => e.Pilot)
                    .WithMany(e => e.Ammunitions)
                    .HasForeignKey(e => e.PilotId);
            });

            modelBuilder.Entity<Ship>(entity =>
            {
                entity.HasIndex(e => new { e.ShipId, e.Name })
                    .HasName("UC_Ship")
                    .IsUnique();

                entity.HasOne<Reward>(e => e.Reward)
                .WithOne();

                entity.Property(e => e.RequiredLevel).HasDefaultValueSql("((1))");
                entity.Property(e => e.ScrapPrice).HasDefaultValueSql("((0))");
                entity.Property(e => e.MetalPrice).HasDefaultValueSql("((0))");
                entity.Property(e => e.Lasers).HasDefaultValueSql("((1))");
                entity.Property(e => e.Generators).HasDefaultValueSql("((1))");
                entity.Property(e => e.Extras).HasDefaultValueSql("((1))");
                entity.Property(e => e.BasicSpeed).HasDefaultValueSql("((50))");
                entity.Property(e => e.BasicCargo).HasDefaultValueSql("((100))");
                entity.Property(e => e.BasicHitpoints).HasDefaultValueSql("((1000))");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => new { e.UserId, e.Username, e.Password, e.Email })
                    .HasName("UC_User")
                    .IsUnique();

                entity.HasOne<Pilot>(e => e.Pilot)
                .WithOne(e => e.User)
                .HasForeignKey<Pilot>(e => e.UserId);
            });

            modelBuilder.Entity<Pilot>(entity =>
            {
                entity.HasKey(e => new { e.UserId });

                entity.HasIndex(e => new { e.Nickname })
                    .HasName("UC_Pilot")
                    .IsUnique();

                entity.HasOne<Map>(e => e.Map)
                .WithOne();

                entity.HasOne<Ship>(e => e.Ship)
                .WithOne();

                entity.Property(e => e.PositionX).HasDefaultValueSql("((100))");
                entity.Property(e => e.PositionY).HasDefaultValueSql("((-100))");
                entity.Property(e => e.Experience).HasDefaultValueSql("((1000))");
                entity.Property(e => e.Level).HasDefaultValueSql("((1))");
                entity.Property(e => e.Scrap).HasDefaultValueSql("((0))");
                entity.Property(e => e.Metal).HasDefaultValueSql("((0))");
                entity.Property(e => e.Hitpoints).HasDefaultValueSql("((1000))");
                entity.Property(e => e.Shields).HasDefaultValueSql("((0))");
                entity.Property(e => e.IsDead).HasDefaultValueSql("((0))");

                entity.Property(e => e.MapId).HasDefaultValueSql("((1))");
                entity.Property(e => e.ShipId).HasDefaultValueSql("((1))");
            });
        }
    }
}