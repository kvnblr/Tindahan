using Microsoft.EntityFrameworkCore;
using Api.Entities.Models;

namespace Api.Persistence;

public class ApiContext(DbContextOptions<ApiContext> options)
    : DbContext(options)
{
    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasIndex(p => p.Name).IsUnique();
        });

        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Talong", Price = 10m, Stock = 50 },
            new Product { Id = 2, Name = "Kangkong", Price = 15m, Stock = 21 },
            new Product { Id = 3, Name = "Ampalaya", Price = 20m, Stock = 14 },
            new Product { Id = 4, Name = "Labanos", Price = 18m, Stock = 30 },
            new Product { Id = 5, Name = "Patatas", Price = 22m, Stock = 64 },
            new Product { Id = 6, Name = "Karrots", Price = 21m, Stock = 27 },
            new Product { Id = 7, Name = "Repolyo", Price = 54m, Stock = 20 },
            new Product { Id = 8, Name = "Pechay", Price = 25m, Stock = 115 },
            new Product { Id = 9, Name = "Saba", Price = 11m, Stock = 40 },
            new Product { Id = 10, Name = "Sili", Price = 3m, Stock = 68 }
                );
    }
}
