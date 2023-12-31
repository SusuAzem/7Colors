using _7Colors.Models;

using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using System.Reflection.Metadata;

namespace _7Colors.Data
{
    public class AppDbContext : DbContext, IDataProtectionKeyContext
    {
        public AppDbContext()
        {
        }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {
        }        

        public DbSet<Image> Images { get; set; }
        public DbSet<Post>  Posts { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<SpecialTag> SpecialTags { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }
        public DbSet<ShoppingCartLine> ShoppingCartLines { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(p => {
                p.HasOne(p => p.ProductType).WithMany(pt => pt.Products)
                    .HasForeignKey(p => p.TypeId).IsRequired();

                p.Property(p => p.TypeId).HasDefaultValue(0);

                p.Property(p => p.Price).HasPrecision(10, 2);

                p.HasOne(p => p.SpecialTag).WithMany(st => st.Products)
                .HasForeignKey(p => p.TagId).IsRequired();

                p.Property(p => p.TagId).HasDefaultValue(0);
            });

            modelBuilder.Entity<Post>(p => { 
                p.HasMany(g => g.Images) .WithOne(i => i.Post)
                .HasForeignKey(i => i.PostId).IsRequired();
                p.HasData(new Post { Id = -1, Created = new(), Title = "بدون موضوع", Description = "نص"});
            });


            modelBuilder.Entity<Image>()
                .Property(p => p.PostId)
                .HasDefaultValue(0);


            modelBuilder.Entity<OrderHeader>(o => { 
                o.HasOne(o => o.User).WithMany(u => u.Orders)
                    .HasForeignKey(o => o.UserNameIdentifier).IsRequired();

                o.HasMany(o => o.OrderItems).WithOne(i => i.OrderHeader)
                .HasForeignKey(o => o.OrderHeaderId).IsRequired();
            });

            modelBuilder.Entity<OrderItem>(o => { 

                o.Property(o=>o.OrderHeaderId).IsRequired();
                o.HasOne(o => o.Product).WithMany().HasForeignKey(o => o.ProductId);
                
            });

            modelBuilder.Entity<ShoppingCartLine>(s => {
                s.Property(s=>s.ProductId).IsRequired();
                s.HasOne(s => s.Product).WithMany().HasForeignKey(o => o.ProductId);

                s.Property(s => s.UserNameIdentifier).IsRequired();
                s.HasOne(s => s.User).WithMany().HasForeignKey(o => o.UserNameIdentifier);

            });           
            base.OnModelCreating(modelBuilder);
        }
    }
    //public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    //{
    //    public AppDbContext CreateDbContext(string[] args)
    //    {
    //        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
    //        optionsBuilder.UseSqlServer("Data Source=sql2022-001.adaptivewebhosting.com,1433;Initial Catalog=sevencol_Db;User Id=7cadmin;Password=SqlS12345*;TrustServerCertificate=True;Trusted_Connection=true;encrypt=false;Integrated Security=false");

    //        return new AppDbContext(optionsBuilder.Options);
    //    }
    //}
}
