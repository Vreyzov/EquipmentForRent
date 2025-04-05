using Microsoft.EntityFrameworkCore;

namespace EquipmentForRent.Models
{
    public partial class EquipmentRentContext : DbContext
    {
        public EquipmentRentContext()
        {
        }

        public EquipmentRentContext(DbContextOptions<EquipmentRentContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<Equipment> Equipment { get; set; }
        public virtual DbSet<Lessor> Lessors { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<DefaultImage> DefaultImages { get; set; }
        public object Equipments { get; internal set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=PC;Database=EquipmentRent;TrustServerCertificate=True;Trusted_Connection=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>(entity =>
            {
                entity.HasKey(e => e.ClientId).HasName("PK__Client__E67E1A04541D0855");
                entity.ToTable("Client");
                entity.HasIndex(e => e.Login, "UQ__Client__5E55825B2F9A3948").IsUnique();

                entity.Property(e => e.ClientId).HasColumnName("ClientID");
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.Login).HasMaxLength(50);
                entity.Property(e => e.Password).HasMaxLength(255);
                entity.Property(e => e.Phone).HasMaxLength(15);
            });

            modelBuilder.Entity<Lessor>(entity =>
            {
                entity.HasKey(e => e.LessorId).HasName("PK__Lessor__BB9D81E7192D8589");
                entity.ToTable("Lessor");
                entity.HasIndex(e => e.Login, "UQ__Lessor__5E55825B3D8D5AC8").IsUnique();

                entity.Property(e => e.LessorId).HasColumnName("LessorID");
                entity.Property(e => e.Email).HasMaxLength(100);
                entity.Property(e => e.FirstName).HasMaxLength(100);
                entity.Property(e => e.LastName).HasMaxLength(100);
                entity.Property(e => e.Login).HasMaxLength(50);
                entity.Property(e => e.Password).HasMaxLength(255);
                entity.Property(e => e.Phone).HasMaxLength(15);
            });

            modelBuilder.Entity<DefaultImage>(entity =>
            {
                entity.HasKey(e => e.ImageID).HasName("PK__DefaultImage__3F77A23D");
                entity.Property(e => e.ImageName).HasColumnName("ImageName");
                entity.Property(e => e.ImageData).HasColumnName("ImageData");
            });

            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.ReviewId).HasName("PK__Review__D7D8F9223A1F82BC");
                entity.Property(e => e.ClientId).HasColumnName("ClientID");
                entity.Property(e => e.LessorId).HasColumnName("LessorID");
                entity.Property(e => e.Rating).HasColumnName("Rating");
                entity.Property(e => e.ReviewText).HasColumnName("ReviewText");
                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.ClientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Review__ClientID__5D6D4F8B");

                entity.HasOne(d => d.Lessor)
                    .WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.LessorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Review__LessorID__5E5F0003");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.HasKey(e => e.OrderId).HasName("PK__Order__C3905BAF4F37B495");
                entity.ToTable("Order");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");
                entity.Property(e => e.ClientId).HasColumnName("ClientID");
                entity.Property(e => e.EndDate).HasColumnType("datetime");
                entity.Property(e => e.EquipmentId).HasColumnName("EquipmentID");
                entity.Property(e => e.LessorId).HasColumnName("LessorID");
                entity.Property(e => e.OrderDate)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");
                entity.Property(e => e.StartDate).HasColumnType("datetime");
                entity.Property(e => e.TotalCost).HasColumnType("decimal(10, 2)");

                entity.HasOne(d => d.Client).WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ClientId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order__ClientID__412EB0B6");

                entity.HasOne(d => d.Equipment).WithMany(p => p.Orders)
                    .HasForeignKey(d => d.EquipmentId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order__Equipment__4316F928");

                entity.HasOne(d => d.Lessor).WithMany(p => p.Orders)
                    .HasForeignKey(d => d.LessorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order__LessorID__4222D4EF");
            });
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
