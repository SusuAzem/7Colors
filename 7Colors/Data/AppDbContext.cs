using _7Colors.Models;

using Microsoft.EntityFrameworkCore;

using System.Reflection.Metadata;

namespace _7Colors.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Image> Images { get; set; }
        public DbSet<Post>  Posts { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<SpecialTag> SpecialTags { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.EnableSensitiveDataLogging();           
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
                .HasOne(p=>p.ProductType)
                .WithMany(pt=>pt.Products)
                .HasForeignKey(p => p.TypeId)
                .IsRequired();

            modelBuilder.Entity<Product>()
               .Property(p => p.TypeId)
               .HasDefaultValue(0);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.SpecialTag)
                .WithMany(st=>st.Products)
                .HasForeignKey(p => p.TagId)
                .IsRequired();

            modelBuilder.Entity<Product>()
               .Property(p => p.TagId)
               .HasDefaultValue(0);

            modelBuilder.Entity<Post>()
                .HasMany(g => g.Images)
                .WithOne(i => i.Post)
                .HasForeignKey(i => i.PostId)
                .IsRequired();

            modelBuilder.Entity<Image>()
                .Property(p => p.PostId)
                .HasDefaultValue(0);
                
            base.OnModelCreating(modelBuilder);
        }
    }
}
