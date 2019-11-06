using CataBot.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace CataBot.Products.Data
{
    public class ProductsContext : DbContext
    {
        public ProductsContext(DbContextOptions<ProductsContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>().HasKey(product => product.ID);
            modelBuilder.Entity<Product>().HasIndex(product => product.Name);
            modelBuilder.Entity<Product>().HasIndex(product => product.GTIN);
        }

        public virtual DbSet<Product> Products { get; }
    }
}
