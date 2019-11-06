using System;
using System.Collections.Generic;
using System.Text;
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

            modelBuilder.Entity<Model.Product>().HasKey(product => product.ID);
            modelBuilder.Entity<Model.Product>().HasIndex(product => product.Name);
            modelBuilder.Entity<Model.Product>().HasIndex(product => product.GTIN);
        }

        public virtual DbSet<Model.Product> Products { get; }
    }
}
