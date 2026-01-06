using CoffeeShop.DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CoffeeShop.DataLayer.Context;

public partial class AppDbContext : IdentityDbContext<User, IdentityRole, string>
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<City> Cities { get; set; }

    public virtual DbSet<ClientReserveComment> ClientReserveComments { get; set; }

    public virtual DbSet<Clientlist> Clientlists { get; set; }

    public virtual DbSet<Comment> Comments { get; set; }

    public virtual DbSet<DeactiveTime> DeactiveTimes { get; set; }

    public virtual DbSet<HHost> HHosts { get; set; }

    public virtual DbSet<HOption> HOptions { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Residence> Residences { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySql("server=localhost;database=ResidenceDb;user=root;password=@Mysql123456789", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.41-mysql"));

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("admins");

            entity.Property(e => e.Access).HasColumnType("enum('1','2')");

            entity.HasOne(d => d.User).WithOne(p => p.Admin)
                .HasForeignKey<Admin>(d => d.UserId)
                .HasConstraintName("admins_ibfk_1");

            entity.HasMany(d => d.Comments).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "AdminsComment",
                    r => r.HasOne<Comment>().WithMany()
                        .HasForeignKey("CommentId")
                        .HasConstraintName("admins_comment_ibfk_2"),
                    l => l.HasOne<Admin>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("admins_comment_ibfk_1"),
                    j =>
                    {
                        j.HasKey("UserId", "CommentId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("admins_comment");
                        j.HasIndex(new[] { "CommentId" }, "CommentId");
                        j.IndexerProperty<string>("UserId").HasColumnName("UserID");
                    });
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("aspnetusers");

            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex").IsUnique();

            entity.Property(e => e.CreateDate).HasDefaultValueSql("'0001-01-01'");
            entity.Property<string>(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.LockoutEnd).HasMaxLength(6);
            entity.Property<string>(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property<string>(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.PhoneNumber2)
                .HasMaxLength(15)
                .HasDefaultValueSql("''");
            entity.Property<string>(e => e.UserName).HasMaxLength(256);
            entity.Property<string>(e => e.FirstName).HasMaxLength(70);
            entity.Property<string>(e => e.LastName).HasMaxLength(70);
        });

        modelBuilder.Entity<City>(entity =>
        {
            entity.HasKey(e => e.CityId).HasName("PRIMARY");

            entity.ToTable("city");

            entity.Property(e => e.CityName).HasMaxLength(30);
        });

        modelBuilder.Entity<ClientReserveComment>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.CommentId })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("client_reserve_comment");

            entity.HasIndex(e => e.CommentId, "CommentId");

            entity.HasIndex(e => e.ReservationId, "ReservationId");

            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.Comment).WithMany(p => p.ClientReserveComments)
                .HasForeignKey(d => d.CommentId)
                .HasConstraintName("client_reserve_comment_ibfk_2");

            entity.HasOne(d => d.Reservation).WithMany(p => p.ClientReserveComments)
                .HasForeignKey(d => d.ReservationId)
                .HasConstraintName("client_reserve_comment_ibfk_3");

            entity.HasOne(d => d.User).WithMany(p => p.ClientReserveComments)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("client_reserve_comment_ibfk_1");
        });

        modelBuilder.Entity<Clientlist>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.Pin })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("clientlist");

            entity.Property(e => e.Pin)
                .HasMaxLength(10)
                .HasColumnName("PIN");
            entity.Property(e => e.FirstName).HasMaxLength(30);
            entity.Property(e => e.LastName).HasMaxLength(30);

            entity.HasOne(d => d.User).WithMany(p => p.Clientlists)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("clientlist_ibfk_1");
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasKey(e => e.CommentId).HasName("PRIMARY");

            entity.ToTable("comments");

            entity.HasIndex(e => e.ResidenceId, "ResidenceId");

            entity.Property(e => e.CommentDescription).HasMaxLength(500);
            entity.Property(e => e.CommentStatus).HasColumnType("enum('Ok','Not ok','Waiting')");
            entity.Property(e => e.CreateDate).HasDefaultValueSql("curdate()");

            entity.HasOne(d => d.Residence).WithMany(p => p.Comments)
                .HasForeignKey(d => d.ResidenceId)
                .HasConstraintName("comments_ibfk_1");
        });

        modelBuilder.Entity<DeactiveTime>(entity =>
        {
            entity.HasKey(e => new { e.ResidenceId, e.StartTime })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("deactive_time");

            entity.Property(e => e.StartTime);
            entity.Property(e => e.DisableDescription).HasMaxLength(500);

            entity.HasOne(d => d.Residence).WithMany(p => p.DeactiveTimes)
                .HasForeignKey(d => new { d.ResidenceId })
                .HasConstraintName("deactive_time_ibfk_1");
        });

        modelBuilder.Entity<HHost>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("h_host");

            entity.Property(e => e.AccNumber)
                .HasMaxLength(16)
                .HasColumnName("Acc_Number");

            entity.HasOne(d => d.User).WithOne(p => p.HHost)
                .HasForeignKey<HHost>(d => d.UserId)
                .HasConstraintName("h_host_ibfk_1");
        });

        modelBuilder.Entity<HOption>(entity =>
        {
            entity.HasKey(e => e.OptionName).HasName("PRIMARY");

            entity.ToTable("h_option");

            entity.Property(e => e.OptionName)
                .HasMaxLength(20)
                .HasColumnName("Option_Name");
            entity.Property(e => e.OptionDescription)
                .HasMaxLength(500)
                .HasColumnName("Option_Description");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PayId).HasName("PRIMARY");

            entity.ToTable("payment");

            entity.Property(e => e.CreatePay).HasDefaultValueSql("curdate()");
            entity.Property(e => e.Price).HasPrecision(7, 2);
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PRIMARY");

            entity.ToTable("reservation");

            entity.HasIndex(e => e.PayId, "PayId");

            entity.HasIndex(e => e.ResidenceId, "ResidenceId");

            entity.Property(e => e.AmountPaid).HasPrecision(7, 2);
            entity.Property(e => e.DateOfStart).HasDefaultValueSql("curdate()");
            entity.Property(e => e.Situation).HasColumnType("enum('unpaid','pending_approval','approved','cancelled','in_stay')");

            entity.HasOne(d => d.Pay).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.PayId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("reservation_ibfk_2");

            entity.HasOne(d => d.Residence).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.ResidenceId)
                .HasConstraintName("reservation_ibfk_1");

            entity.HasMany(d => d.Clientlists).WithMany(p => p.Reservations)
                .UsingEntity<Dictionary<string, object>>(
                    "ClientlistReserve",
                    r => r.HasOne<Clientlist>().WithMany()
                        .HasForeignKey("UserId", "Pin")
                        .HasConstraintName("clientlist_reserve_ibfk_2"),
                    l => l.HasOne<Reservation>().WithMany()
                        .HasForeignKey("ReservationId")
                        .HasConstraintName("clientlist_reserve_ibfk_1"),
                    j =>
                    {
                        j.HasKey("ReservationId", "UserId", "Pin")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });
                        j.ToTable("clientlist_reserve");
                        j.HasIndex(new[] { "UserId", "Pin" }, "UserId");
                        j.IndexerProperty<string>("Pin")
                            .HasMaxLength(10)
                            .HasColumnName("PIN");
                    });
        });

        modelBuilder.Entity<Residence>(entity =>
        {
            entity.HasKey(e => e.ResidenceId).HasName("PRIMARY");

            entity.ToTable("residence");

            entity.HasIndex(e => e.CityId, "CityId");

            entity.HasIndex(e => e.UserId, "UserId");

            entity.Property(e => e.CreateDate).HasDefaultValueSql("curdate()");
            entity.Property(e => e.ResidenceName).HasMaxLength(50);
            entity.Property(e => e.ResidenceType).HasColumnType("enum('single','double','suite','vip')");
            entity.Property(e => e.Situation).HasColumnType("enum('active','inactive')");
            entity.Property(e => e.Star).HasColumnType("enum('1','2','3','4','5')");
            entity.Property(e => e.Street).HasMaxLength(50);
            entity.Property(e => e.MainImage).HasMaxLength(70);
            entity.Property(e => e.Price).HasPrecision(7, 2);
            entity.Property(e => e.Description).HasMaxLength(200);

            entity.HasOne(d => d.City).WithMany(p => p.Residences)
                .HasForeignKey(d => d.CityId)
                .HasConstraintName("residence_ibfk_2");

            entity.HasOne(d => d.User).WithMany(p => p.Residences)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("residence_ibfk_1");

            entity.HasMany(d => d.OptionNames).WithMany(p => p.Residences)
                .UsingEntity<Dictionary<string, object>>(
                    "OptionResidence",
                    r => r.HasOne<HOption>().WithMany()
                        .HasForeignKey("OptionName")
                        .HasConstraintName("option_residence_ibfk_2"),
                    l => l.HasOne<Residence>().WithMany()
                        .HasForeignKey("ResidenceId")
                        .HasConstraintName("option_residence_ibfk_1"),
                    j =>
                    {
                        j.HasKey("ResidenceId", "OptionName")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("option_residence");
                        j.HasIndex(new[] { "OptionName" }, "Option_Name");
                        j.IndexerProperty<string>("OptionName")
                            .HasMaxLength(20)
                            .HasColumnName("Option_Name");
                    });
        });
    }
}