using CosmicSpaceWebsiteDll;
using Microsoft.EntityFrameworkCore;

namespace CosmicSpaceWebsiteApi.Database
{
    public partial class cosmicspaceContext : DbContext
    {
        public cosmicspaceContext()
        {
        }

        public cosmicspaceContext(DbContextOptions<cosmicspaceContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Ammunitions> Ammunitions { get; set; }
        public virtual DbSet<Enemies> Enemies { get; set; }
        public virtual DbSet<Enemymap> Enemymap { get; set; }
        public virtual DbSet<Gamesettings> Gamesettings { get; set; }
        public virtual DbSet<Itemreward> Itemreward { get; set; }
        public virtual DbSet<Items> Items { get; set; }
        public virtual DbSet<Itemtypes> Itemtypes { get; set; }
        public virtual DbSet<Maps> Maps { get; set; }
        public virtual DbSet<Pilotresources> Pilotresources { get; set; }
        public virtual DbSet<Pilots> Pilots { get; set; }
        public virtual DbSet<Pilotsitems> Pilotsitems { get; set; }
        public virtual DbSet<Pilottask> Pilottask { get; set; }
        public virtual DbSet<Pilottaskquest> Pilottaskquest { get; set; }
        public virtual DbSet<Portalpositions> Portalpositions { get; set; }
        public virtual DbSet<Portals> Portals { get; set; }
        public virtual DbSet<Prefabs> Prefabs { get; set; }
        public virtual DbSet<PrefabsTypes> PrefabsTypes { get; set; }
        public virtual DbSet<Questmap> Questmap { get; set; }
        public virtual DbSet<Quests> Quests { get; set; }
        public virtual DbSet<Questtypes> Questtypes { get; set; }
        public virtual DbSet<Rewards> Rewards { get; set; }
        public virtual DbSet<Rockets> Rockets { get; set; }
        public virtual DbSet<Ships> Ships { get; set; }
        public virtual DbSet<Taskquest> Taskquest { get; set; }
        public virtual DbSet<Tasks> Tasks { get; set; }
        public virtual DbSet<Users> Users { get; set; }
        public virtual DbSet<Userslog> Userslog { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseMySQL("server=localhost;port=3306;user=root;database=cosmicspace");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.6-servicing-10079");

            modelBuilder.Entity<Ammunitions>(entity =>
            {
                entity.HasKey(e => e.Ammunitionid);

                entity.ToTable("ammunitions", "cosmicspace");

                entity.Property(e => e.Ammunitionid)
                    .HasColumnName("ammunitionid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Ammunitionname)
                    .IsRequired()
                    .HasColumnName("ammunitionname")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Basedamage)
                    .HasColumnName("basedamage")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Isammunition)
                    .HasColumnName("isammunition")
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Metalprice).HasColumnName("metalprice");

                entity.Property(e => e.Multiplierenemy)
                    .HasColumnName("multiplierenemy")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Multiplierplayer)
                    .HasColumnName("multiplierplayer")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Scrapprice).HasColumnName("scrapprice");

                entity.Property(e => e.Skillid)
                    .HasColumnName("skillid")
                    .HasColumnType("tinyint(3) unsigned");
            });

            modelBuilder.Entity<Enemies>(entity =>
            {
                entity.HasKey(e => e.Enemyid);

                entity.ToTable("enemies", "cosmicspace");

                entity.HasIndex(e => e.Ammunitionid)
                    .HasName("ammunitionid");

                entity.HasIndex(e => e.Prefabid)
                    .HasName("prefabname");

                entity.HasIndex(e => e.Rewardid)
                    .HasName("rewardid");

                entity.HasIndex(e => e.Rocketid)
                    .HasName("rocketid");

                entity.Property(e => e.Enemyid)
                    .HasColumnName("enemyid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Ammunitionid)
                    .HasColumnName("ammunitionid")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValueSql("100");

                entity.Property(e => e.Damage)
                    .HasColumnName("damage")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Enemyname)
                    .IsRequired()
                    .HasColumnName("enemyname")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Hitpoints)
                    .HasColumnName("hitpoints")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Isaggressive)
                    .HasColumnName("isaggressive")
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Prefabid)
                    .HasColumnName("prefabid")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Rewardid)
                    .HasColumnName("rewardid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Rocketid)
                    .HasColumnName("rocketid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Shields)
                    .HasColumnName("shields")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Shotdistance)
                    .HasColumnName("shotdistance")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("30");

                entity.Property(e => e.Speed)
                    .HasColumnName("speed")
                    .HasColumnType("smallint(5)")
                    .HasDefaultValueSql("20");

                entity.HasOne(d => d.Ammunition)
                    .WithMany(p => p.EnemiesAmmunition)
                    .HasForeignKey(d => d.Ammunitionid)
                    .HasConstraintName("enemies_ibfk_3");

                entity.HasOne(d => d.Prefab)
                    .WithMany(p => p.Enemies)
                    .HasForeignKey(d => d.Prefabid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("enemies_ibfk_2");

                entity.HasOne(d => d.Reward)
                    .WithMany(p => p.Enemies)
                    .HasForeignKey(d => d.Rewardid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("enemies_ibfk_1");

                entity.HasOne(d => d.Rocket)
                    .WithMany(p => p.EnemiesRocket)
                    .HasForeignKey(d => d.Rocketid)
                    .HasConstraintName("enemies_ibfk_4");
            });

            modelBuilder.Entity<Enemymap>(entity =>
            {
                entity.ToTable("enemymap", "cosmicspace");

                entity.HasIndex(e => e.Enemyid)
                    .HasName("enemyid");

                entity.HasIndex(e => e.Mapid)
                    .HasName("mapid");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Count)
                    .HasColumnName("count")
                    .HasColumnType("smallint(6)")
                    .HasDefaultValueSql("50");

                entity.Property(e => e.Enemyid)
                    .HasColumnName("enemyid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Mapid)
                    .HasColumnName("mapid")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Enemy)
                    .WithMany(p => p.Enemymap)
                    .HasForeignKey(d => d.Enemyid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("enemymap_ibfk_1");

                entity.HasOne(d => d.Map)
                    .WithMany(p => p.Enemymap)
                    .HasForeignKey(d => d.Mapid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("enemymap_ibfk_2");
            });

            modelBuilder.Entity<Gamesettings>(entity =>
            {
                entity.ToTable("gamesettings", "cosmicspace");

                entity.HasIndex(e => e.Key)
                    .HasName("key")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasColumnName("key")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Value)
                    .HasColumnName("value")
                    .HasColumnType("int(11)");
            });

            modelBuilder.Entity<Itemreward>(entity =>
            {
                entity.ToTable("itemreward", "cosmicspace");

                entity.HasIndex(e => e.Itemid)
                    .HasName("itemid");

                entity.HasIndex(e => e.Rewardid)
                    .HasName("rewardid");

                entity.Property(e => e.Itemrewardid)
                    .HasColumnName("itemrewardid")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Chance)
                    .HasColumnName("chance")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1000");

                entity.Property(e => e.Itemid)
                    .HasColumnName("itemid")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Rewardid)
                    .HasColumnName("rewardid")
                    .HasColumnType("int(11) unsigned");

                entity.Property(e => e.Upgradelevel)
                    .HasColumnName("upgradelevel")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.Itemreward)
                    .HasForeignKey(d => d.Itemid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("itemreward_ibfk_1");

                entity.HasOne(d => d.Reward)
                    .WithMany(p => p.Itemreward)
                    .HasForeignKey(d => d.Rewardid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("itemreward_ibfk_2");
            });

            modelBuilder.Entity<Items>(entity =>
            {
                entity.HasKey(e => e.Itemid);

                entity.ToTable("items", "cosmicspace");

                entity.HasIndex(e => e.Itemtypeid)
                    .HasName("itemtypeid");

                entity.Property(e => e.Itemid)
                    .HasColumnName("itemid")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.GeneratorShield)
                    .HasColumnName("generator_shield")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.GeneratorShieldDivision)
                    .HasColumnName("generator_shield_division")
                    .HasDefaultValueSql("0.7");

                entity.Property(e => e.GeneratorShieldRepair)
                    .HasColumnName("generator_shield_repair")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("20");

                entity.Property(e => e.GeneratorSpeed).HasColumnName("generator_speed");

                entity.Property(e => e.Itemtypeid)
                    .HasColumnName("itemtypeid")
                    .HasColumnType("int(11)");

                entity.Property(e => e.LaserDamagePve)
                    .HasColumnName("laser_damage_pve")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.LaserDamagePvp)
                    .HasColumnName("laser_damage_pvp")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.LaserShotdispersion)
                    .HasColumnName("laser_shotdispersion")
                    .HasDefaultValueSql("0.15");

                entity.Property(e => e.LaserShotrange)
                    .HasColumnName("laser_shotrange")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("50");

                entity.Property(e => e.Metalprice).HasColumnName("metalprice");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Prefabname)
                    .IsRequired()
                    .HasColumnName("prefabname")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Requiredlevel)
                    .HasColumnName("requiredlevel")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Scrapprice).HasColumnName("scrapprice");

                entity.HasOne(d => d.Itemtype)
                    .WithMany(p => p.Items)
                    .HasForeignKey(d => d.Itemtypeid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("items_ibfk_1");
            });

            modelBuilder.Entity<Itemtypes>(entity =>
            {
                entity.HasKey(e => e.Itemtypeid);

                entity.ToTable("itemtypes", "cosmicspace");

                entity.Property(e => e.Itemtypeid)
                    .HasColumnName("itemtypeid")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Itemtypename)
                    .IsRequired()
                    .HasColumnName("itemtypename")
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Maps>(entity =>
            {
                entity.HasKey(e => e.Mapid);

                entity.ToTable("maps", "cosmicspace");

                entity.Property(e => e.Mapid)
                    .HasColumnName("mapid")
                    .HasColumnType("int(11) unsigned");

                entity.Property(e => e.Ispvp)
                    .HasColumnName("ispvp")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Mapname)
                    .IsRequired()
                    .HasColumnName("mapname")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Requiredlevel)
                    .HasColumnName("requiredlevel")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");
            });

            modelBuilder.Entity<Pilotresources>(entity =>
            {
                entity.HasKey(e => e.Pilotresourceid);

                entity.ToTable("pilotresources", "cosmicspace");

                entity.HasIndex(e => e.Userid)
                    .HasName("userid");

                entity.Property(e => e.Pilotresourceid)
                    .HasColumnName("pilotresourceid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Ammunition0)
                    .HasColumnName("ammunition0")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValueSql("500");

                entity.Property(e => e.Ammunition1)
                    .HasColumnName("ammunition1")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Ammunition2)
                    .HasColumnName("ammunition2")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Ammunition3)
                    .HasColumnName("ammunition3")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Rocket0)
                    .HasColumnName("rocket0")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Rocket1)
                    .HasColumnName("rocket1")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Rocket2)
                    .HasColumnName("rocket2")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Userid)
                    .HasColumnName("userid")
                    .HasColumnType("bigint(20) unsigned");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Pilotresources)
                    .HasPrincipalKey(p => p.Userid)
                    .HasForeignKey(d => d.Userid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("pilotresources_ibfk_3");
            });

            modelBuilder.Entity<Pilots>(entity =>
            {
                entity.HasKey(e => e.Pilotid);

                entity.ToTable("pilots", "cosmicspace");

                entity.HasIndex(e => e.Ammunitionid)
                    .HasName("ammunitionid");

                entity.HasIndex(e => e.Mapid)
                    .HasName("mapid");

                entity.HasIndex(e => e.Nickname)
                    .HasName("nickname")
                    .IsUnique();

                entity.HasIndex(e => e.Rocketid)
                    .HasName("rocketid");

                entity.HasIndex(e => e.Shipid)
                    .HasName("shipid");

                entity.HasIndex(e => e.Userid)
                    .HasName("userid")
                    .IsUnique();

                entity.Property(e => e.Pilotid)
                    .HasColumnName("pilotid")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Achievement)
                    .HasColumnName("achievement")
                    .HasColumnType("mediumtext");

                entity.Property(e => e.Ammunitionid)
                    .HasColumnName("ammunitionid")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValueSql("100");

                entity.Property(e => e.Experience)
                    .HasColumnName("experience")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Hitpoints)
                    .HasColumnName("hitpoints")
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("1000");

                entity.Property(e => e.Isdead)
                    .HasColumnName("isdead")
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Killerby)
                    .HasColumnName("killerby")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Level)
                    .HasColumnName("level")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Mapid)
                    .HasColumnName("mapid")
                    .HasColumnType("int(11) unsigned")
                    .HasDefaultValueSql("100");

                entity.Property(e => e.Metal)
                    .HasColumnName("metal")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Nickname)
                    .HasColumnName("nickname")
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Positionx)
                    .HasColumnName("positionx")
                    .HasDefaultValueSql("100");

                entity.Property(e => e.Positiony)
                    .HasColumnName("positiony")
                    .HasDefaultValueSql("-100");

                entity.Property(e => e.Rocketid)
                    .HasColumnName("rocketid")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValueSql("104");

                entity.Property(e => e.Scrap)
                    .HasColumnName("scrap")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Shields)
                    .HasColumnName("shields")
                    .HasColumnType("bigint(20)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Shipid)
                    .HasColumnName("shipid")
                    .HasColumnType("int(11) unsigned")
                    .HasDefaultValueSql("100");

                entity.Property(e => e.Userid)
                    .HasColumnName("userid")
                    .HasColumnType("bigint(20) unsigned");

                entity.HasOne(d => d.Ammunition)
                    .WithMany(p => p.PilotsAmmunition)
                    .HasForeignKey(d => d.Ammunitionid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("pilots_ibfk_4");

                entity.HasOne(d => d.Map)
                    .WithMany(p => p.Pilots)
                    .HasForeignKey(d => d.Mapid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("pilots_ibfk_2");

                entity.HasOne(d => d.Rocket)
                    .WithMany(p => p.PilotsRocket)
                    .HasForeignKey(d => d.Rocketid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("pilots_ibfk_5");

                entity.HasOne(d => d.Ship)
                    .WithMany(p => p.Pilots)
                    .HasForeignKey(d => d.Shipid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("pilots_ibfk_3");

                entity.HasOne(d => d.User)
                    .WithOne(p => p.Pilots)
                    .HasForeignKey<Pilots>(d => d.Userid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("pilots_ibfk_1");
            });

            modelBuilder.Entity<Pilotsitems>(entity =>
            {
                entity.HasKey(e => e.Relationid);

                entity.ToTable("pilotsitems", "cosmicspace");

                entity.HasIndex(e => e.Itemid)
                    .HasName("itemid");

                entity.HasIndex(e => e.Userid)
                    .HasName("pilotid");

                entity.Property(e => e.Relationid)
                    .HasColumnName("relationid")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Isequipped)
                    .HasColumnName("isequipped")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Issold)
                    .HasColumnName("issold")
                    .HasColumnType("tinyint(4)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Itemid)
                    .HasColumnName("itemid")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Upgradelevel)
                    .HasColumnName("upgradelevel")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Userid)
                    .HasColumnName("userid")
                    .HasColumnType("bigint(20) unsigned");

                entity.HasOne(d => d.Item)
                    .WithMany(p => p.Pilotsitems)
                    .HasForeignKey(d => d.Itemid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("pilotsitems_ibfk_2");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Pilotsitems)
                    .HasPrincipalKey(p => p.Userid)
                    .HasForeignKey(d => d.Userid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("pilotsitems_ibfk_1");
            });

            modelBuilder.Entity<Pilottask>(entity =>
            {
                entity.ToTable("pilottask", "cosmicspace");

                entity.HasIndex(e => e.Pilotid)
                    .HasName("pilotid");

                entity.HasIndex(e => e.Taskid)
                    .HasName("taskid");

                entity.Property(e => e.Pilottaskid)
                    .HasColumnName("pilottaskid")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Enddate).HasColumnName("enddate");

                entity.Property(e => e.Pilotid)
                    .HasColumnName("pilotid")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Startdate)
                    .HasColumnName("startdate")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Taskid)
                    .HasColumnName("taskid")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Pilot)
                    .WithMany(p => p.Pilottask)
                    .HasPrincipalKey(p => p.Userid)
                    .HasForeignKey(d => d.Pilotid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("pilottask_ibfk_1");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.Pilottask)
                    .HasForeignKey(d => d.Taskid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("pilottask_ibfk_2");
            });

            modelBuilder.Entity<Pilottaskquest>(entity =>
            {
                entity.ToTable("pilottaskquest", "cosmicspace");

                entity.HasIndex(e => e.Pilottaskid)
                    .HasName("pilottaskid");

                entity.HasIndex(e => e.Questid)
                    .HasName("questid");

                entity.Property(e => e.Pilottaskquestid)
                    .HasColumnName("pilottaskquestid")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Isdone)
                    .HasColumnName("isdone")
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Pilottaskid)
                    .HasColumnName("pilottaskid")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Progress)
                    .HasColumnName("progress")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Questid)
                    .HasColumnName("questid")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Pilottask)
                    .WithMany(p => p.Pilottaskquest)
                    .HasForeignKey(d => d.Pilottaskid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("pilottaskquest_ibfk_2");

                entity.HasOne(d => d.Quest)
                    .WithMany(p => p.Pilottaskquest)
                    .HasForeignKey(d => d.Questid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("pilottaskquest_ibfk_1");
            });

            modelBuilder.Entity<Portalpositions>(entity =>
            {
                entity.HasKey(e => e.Portalpositionid);

                entity.ToTable("portalpositions", "cosmicspace");

                entity.HasIndex(e => e.Name)
                    .HasName("name")
                    .IsUnique();

                entity.HasIndex(e => e.Portalpositionid)
                    .HasName("portalpositionid")
                    .IsUnique();

                entity.Property(e => e.Portalpositionid)
                    .HasColumnName("portalpositionid")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Positionx).HasColumnName("positionx");

                entity.Property(e => e.Positiony).HasColumnName("positiony");
            });

            modelBuilder.Entity<Portals>(entity =>
            {
                entity.HasKey(e => e.Portalid);

                entity.ToTable("portals", "cosmicspace");

                entity.HasIndex(e => e.Mapid)
                    .HasName("mapid");

                entity.HasIndex(e => e.Portalpositionid)
                    .HasName("portalpositionid");

                entity.HasIndex(e => e.Prefabid)
                    .HasName("prefabid");

                entity.HasIndex(e => e.TargetMapid)
                    .HasName("target_mapid");

                entity.HasIndex(e => e.TargetPortalpositionid)
                    .HasName("target_portalpositionid");

                entity.Property(e => e.Portalid)
                    .HasColumnName("portalid")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Mapid)
                    .HasColumnName("mapid")
                    .HasColumnType("int(11) unsigned");

                entity.Property(e => e.Portalpositionid)
                    .HasColumnName("portalpositionid")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Prefabid)
                    .HasColumnName("prefabid")
                    .HasColumnType("int(11)");

                entity.Property(e => e.TargetMapid)
                    .HasColumnName("target_mapid")
                    .HasColumnType("int(11) unsigned");

                entity.Property(e => e.TargetPortalpositionid)
                    .HasColumnName("target_portalpositionid")
                    .HasColumnType("int(11)");

                entity.HasOne(d => d.Map)
                    .WithMany(p => p.PortalsMap)
                    .HasForeignKey(d => d.Mapid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("portals_ibfk_2");

                entity.HasOne(d => d.Portalposition)
                    .WithMany(p => p.PortalsPortalposition)
                    .HasForeignKey(d => d.Portalpositionid)
                    .HasConstraintName("portals_ibfk_4");

                entity.HasOne(d => d.Prefab)
                    .WithMany(p => p.Portals)
                    .HasForeignKey(d => d.Prefabid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("portals_ibfk_1");

                entity.HasOne(d => d.TargetMap)
                    .WithMany(p => p.PortalsTargetMap)
                    .HasForeignKey(d => d.TargetMapid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("portals_ibfk_3");

                entity.HasOne(d => d.TargetPortalposition)
                    .WithMany(p => p.PortalsTargetPortalposition)
                    .HasForeignKey(d => d.TargetPortalpositionid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("portals_ibfk_5");
            });

            modelBuilder.Entity<Prefabs>(entity =>
            {
                entity.HasKey(e => e.Prefabid);

                entity.ToTable("prefabs", "cosmicspace");

                entity.HasIndex(e => e.Prefabname)
                    .HasName("prefabname")
                    .IsUnique();

                entity.HasIndex(e => e.Prefabtypeid)
                    .HasName("prefab_type_id");

                entity.Property(e => e.Prefabid)
                    .HasColumnName("prefabid")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.Property(e => e.Prefabname)
                    .IsRequired()
                    .HasColumnName("prefabname")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Prefabtypeid)
                    .HasColumnName("prefabtypeid")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");

                entity.HasOne(d => d.Prefabtype)
                    .WithMany(p => p.Prefabs)
                    .HasForeignKey(d => d.Prefabtypeid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("prefabs_ibfk_1");
            });

            modelBuilder.Entity<PrefabsTypes>(entity =>
            {
                entity.HasKey(e => e.Prefabtypeid);

                entity.ToTable("prefabs_types", "cosmicspace");

                entity.Property(e => e.Prefabtypeid)
                    .HasColumnName("prefabtypeid")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Prefabtypename)
                    .IsRequired()
                    .HasColumnName("prefabtypename")
                    .HasMaxLength(100)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Questmap>(entity =>
            {
                entity.ToTable("questmap", "cosmicspace");

                entity.HasIndex(e => e.Mapid)
                    .HasName("mapid");

                entity.HasIndex(e => e.Questid)
                    .HasName("questid");

                entity.Property(e => e.Questmapid)
                    .HasColumnName("questmapid")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Mapid)
                    .HasColumnName("mapid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Questid)
                    .HasColumnName("questid")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Map)
                    .WithMany(p => p.Questmap)
                    .HasForeignKey(d => d.Mapid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("questmap_ibfk_1");

                entity.HasOne(d => d.Quest)
                    .WithMany(p => p.Questmap)
                    .HasForeignKey(d => d.Questid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("questmap_ibfk_2");
            });

            modelBuilder.Entity<Quests>(entity =>
            {
                entity.HasKey(e => e.Questid);

                entity.ToTable("quests", "cosmicspace");

                entity.HasIndex(e => e.Questtypeid)
                    .HasName("questtypeid");

                entity.Property(e => e.Questid)
                    .HasColumnName("questid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Quantity)
                    .HasColumnName("quantity")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Questtypeid)
                    .HasColumnName("questtypeid")
                    .HasColumnType("tinyint(10) unsigned");

                entity.Property(e => e.Targetid)
                    .HasColumnName("targetid")
                    .HasColumnType("bigint(20) unsigned");

                entity.HasOne(d => d.Questtype)
                    .WithMany(p => p.Quests)
                    .HasForeignKey(d => d.Questtypeid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("quests_ibfk_2");
            });

            modelBuilder.Entity<Questtypes>(entity =>
            {
                entity.HasKey(e => e.Questtypeid);

                entity.ToTable("questtypes", "cosmicspace");

                entity.Property(e => e.Questtypeid)
                    .HasColumnName("questtypeid")
                    .HasColumnType("tinyint(3) unsigned")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Questtypename)
                    .IsRequired()
                    .HasColumnName("questtypename")
                    .HasMaxLength(500)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Rewards>(entity =>
            {
                entity.HasKey(e => e.Rewardid);

                entity.ToTable("rewards", "cosmicspace");

                entity.HasIndex(e => e.Ammunitionid)
                    .HasName("ammunitionid");

                entity.Property(e => e.Rewardid)
                    .HasColumnName("rewardid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.AmmunitionQuantity)
                    .HasColumnName("ammunition_quantity")
                    .HasColumnType("bigint(20)");

                entity.Property(e => e.Ammunitionid)
                    .HasColumnName("ammunitionid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Experience)
                    .HasColumnName("experience")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Metal).HasColumnName("metal");

                entity.Property(e => e.Scrap).HasColumnName("scrap");

                entity.HasOne(d => d.Ammunition)
                    .WithMany(p => p.Rewards)
                    .HasForeignKey(d => d.Ammunitionid)
                    .HasConstraintName("rewards_ibfk_1");
            });

            modelBuilder.Entity<Rockets>(entity =>
            {
                entity.HasKey(e => e.Rocketid);

                entity.ToTable("rockets", "cosmicspace");

                entity.Property(e => e.Rocketid)
                    .HasColumnName("rocketid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Damage)
                    .HasColumnName("damage")
                    .HasColumnType("int(10) unsigned")
                    .HasDefaultValueSql("1000");

                entity.Property(e => e.Metalprice).HasColumnName("metalprice");

                entity.Property(e => e.Rocketname)
                    .IsRequired()
                    .HasColumnName("rocketname")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Scrapprice).HasColumnName("scrapprice");

                entity.Property(e => e.Skillid)
                    .HasColumnName("skillid")
                    .HasColumnType("tinyint(3) unsigned");
            });

            modelBuilder.Entity<Ships>(entity =>
            {
                entity.HasKey(e => e.Shipid);

                entity.ToTable("ships", "cosmicspace");

                entity.HasIndex(e => e.Prefabid)
                    .HasName("prefabname");

                entity.HasIndex(e => e.Rewardid)
                    .HasName("rewardid");

                entity.HasIndex(e => e.Shipname)
                    .HasName("shipname")
                    .IsUnique();

                entity.Property(e => e.Shipid)
                    .HasColumnName("shipid")
                    .HasColumnType("int(11) unsigned");

                entity.Property(e => e.Cargo)
                    .HasColumnName("cargo")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValueSql("100");

                entity.Property(e => e.Extras)
                    .HasColumnName("extras")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Generators)
                    .HasColumnName("generators")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Hitpoints)
                    .HasColumnName("hitpoints")
                    .HasColumnType("bigint(20) unsigned")
                    .HasDefaultValueSql("1000");

                entity.Property(e => e.Lasers)
                    .HasColumnName("lasers")
                    .HasColumnType("tinyint(3) unsigned")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Metalprice).HasColumnName("metalprice");

                entity.Property(e => e.Prefabid)
                    .HasColumnName("prefabid")
                    .HasColumnType("int(11)");

                entity.Property(e => e.Requiredlevel)
                    .HasColumnName("requiredlevel")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Rewardid)
                    .HasColumnName("rewardid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Scrapprice).HasColumnName("scrapprice");

                entity.Property(e => e.Shipname)
                    .IsRequired()
                    .HasColumnName("shipname")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Speed)
                    .HasColumnName("speed")
                    .HasColumnType("smallint(5) unsigned")
                    .HasDefaultValueSql("50");

                entity.HasOne(d => d.Prefab)
                    .WithMany(p => p.Ships)
                    .HasForeignKey(d => d.Prefabid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ships_ibfk_2");

                entity.HasOne(d => d.Reward)
                    .WithMany(p => p.Ships)
                    .HasForeignKey(d => d.Rewardid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("ships_ibfk_1");
            });

            modelBuilder.Entity<Taskquest>(entity =>
            {
                entity.ToTable("taskquest", "cosmicspace");

                entity.HasIndex(e => e.Questid)
                    .HasName("questid");

                entity.HasIndex(e => e.Taskid)
                    .HasName("taskid");

                entity.Property(e => e.Taskquestid)
                    .HasColumnName("taskquestid")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Questid)
                    .HasColumnName("questid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Taskid)
                    .HasColumnName("taskid")
                    .HasColumnType("int(10) unsigned");

                entity.HasOne(d => d.Quest)
                    .WithMany(p => p.Taskquest)
                    .HasForeignKey(d => d.Questid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("taskquest_ibfk_1");

                entity.HasOne(d => d.Task)
                    .WithMany(p => p.Taskquest)
                    .HasForeignKey(d => d.Taskid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("taskquest_ibfk_2");
            });

            modelBuilder.Entity<Tasks>(entity =>
            {
                entity.HasKey(e => e.Taskid);

                entity.ToTable("tasks", "cosmicspace");

                entity.HasIndex(e => e.Rewardid)
                    .HasName("rewardid");

                entity.Property(e => e.Taskid)
                    .HasColumnName("taskid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Level)
                    .HasColumnName("level")
                    .HasColumnType("int(11)")
                    .HasDefaultValueSql("1");

                entity.Property(e => e.Rewardid)
                    .HasColumnName("rewardid")
                    .HasColumnType("int(10) unsigned");

                entity.Property(e => e.Taskname)
                    .IsRequired()
                    .HasColumnName("taskname")
                    .HasMaxLength(300)
                    .IsUnicode(false);

                entity.HasOne(d => d.Reward)
                    .WithMany(p => p.Tasks)
                    .HasForeignKey(d => d.Rewardid)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("tasks_ibfk_1");
            });

            modelBuilder.Entity<Users>(entity =>
            {
                entity.HasKey(e => e.Userid);

                entity.ToTable("users", "cosmicspace");

                entity.HasIndex(e => e.Email)
                    .HasName("email")
                    .IsUnique();

                entity.HasIndex(e => e.Usernamehash)
                    .HasName("usernamehash")
                    .IsUnique();

                entity.Property(e => e.Userid)
                    .HasColumnName("userid")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Acceptrules)
                    .HasColumnName("acceptrules")
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Ban)
                    .HasColumnName("ban")
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Emailnewsletter)
                    .HasColumnName("emailnewsletter")
                    .HasColumnType("tinyint(1)")
                    .HasDefaultValueSql("0");

                entity.Property(e => e.Passwordhash)
                    .IsRequired()
                    .HasColumnName("passwordhash")
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.Registerdate).HasColumnName("registerdate");

                entity.Property(e => e.Usernamehash)
                    .IsRequired()
                    .HasColumnName("usernamehash")
                    .HasMaxLength(128)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Userslog>(entity =>
            {
                entity.ToTable("userslog", "cosmicspace");

                entity.HasIndex(e => e.Userid)
                    .HasName("userid");

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .HasColumnType("bigint(20) unsigned");

                entity.Property(e => e.Action)
                    .IsRequired()
                    .HasColumnName("action")
                    .HasMaxLength(100)
                    .IsUnicode(false);

                entity.Property(e => e.Datetime).HasColumnName("datetime");

                entity.Property(e => e.Host)
                    .IsRequired()
                    .HasColumnName("host")
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Result)
                    .HasColumnName("result")
                    .HasColumnType("tinyint(1)");

                entity.Property(e => e.Useragent)
                    .IsRequired()
                    .HasColumnName("useragent")
                    .HasMaxLength(500)
                    .IsUnicode(false);

                entity.Property(e => e.Userid)
                    .HasColumnName("userid")
                    .HasColumnType("bigint(20) unsigned");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Userslog)
                    .HasForeignKey(d => d.Userid)
                    .HasConstraintName("userslog_ibfk_1");
            });
        }
    }
}
