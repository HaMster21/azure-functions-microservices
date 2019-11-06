using CataBot.Domain.Model;
using Microsoft.EntityFrameworkCore;

namespace CataBot.Catalogs.Data
{
    public class CatalogContext : DbContext
    {
        public CatalogContext(DbContextOptions<CatalogContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Catalog>().HasKey(catalog => catalog.ID);
            modelBuilder.Entity<Catalog>().HasIndex(catalog => catalog.Name);
        }

        public virtual DbSet<Catalog> Catalogs { get; }
    }
}
