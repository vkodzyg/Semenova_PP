using Microsoft.EntityFrameworkCore;

namespace semenova_library
{
    public class AppDbContext : DbContext
    {
        public DbSet<Partners> Partner { get; set; }
        public DbSet<Partner_Types> PartnerTypes { get; set; }
        public DbSet<Product> Product { get; set; }
        public DbSet<SalesHistory> SalesHistories { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = "Host=localhost;Port=5432;Database=semenova_ind_db;Username=semenova_app;Password=123456789";
            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("semenova_schema");

            modelBuilder.Entity<Partners>(entity =>
            {
                entity.ToTable("partners");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Inn)
                    .HasMaxLength(12);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20);

                entity.Property(e => e.Email)
                    .HasMaxLength(100);

                entity.Property(e => e.LogoPath)
                    .HasMaxLength(500);

                entity.Property(e => e.Rating)
                    .IsRequired();

                entity.HasOne(e => e.Type)
                    .WithMany(t => t.Partners)
                    .HasForeignKey(e => e.TypeId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.Name);
                entity.HasIndex(e => e.Inn).IsUnique();
            });

            modelBuilder.Entity<Partner_Types>(entity =>
            {
                entity.ToTable("partner_types");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Article)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(e => e.Unit)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Price)
                    .IsRequired()
                    .HasColumnType("decimal(10,2)");

                entity.HasIndex(e => e.Article).IsUnique();
            });

            modelBuilder.Entity<SalesHistory>(entity =>
            {
                entity.ToTable("sales_history");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Quantity)
                    .IsRequired();

                entity.Property(e => e.SaleDate)
                    .IsRequired();

                entity.HasOne(e => e.Partner)
                    .WithMany(p => p.SalesHistories)
                    .HasForeignKey(e => e.PartnerId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(e => e.Product)
                    .WithMany(p => p.SalesHistories)
                    .HasForeignKey(e => e.ProductId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(e => e.PartnerId);
                entity.HasIndex(e => e.SaleDate);
            });
        }
    }
}