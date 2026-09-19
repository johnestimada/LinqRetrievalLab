using LinqRetrievalLab.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace LinqRetrievalLab.Models.DATA;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Product> Product => Set<Product>();

    public DbSet<Category> Category => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Category>()
            .HasMany(c => c.Products)
            .WithOne(p => p.Category)
            .HasForeignKey(p => p.CategoryId);

        // Prepopulate Categories
        modelBuilder.Entity<Category>().HasData(
            new Category
            {
                Id = 1,
                Name = "Electronics"
            },
            new Category
            {
                Id = 2,
                Name = "Computer Accessories"
            },
            new Category
            {
                Id = 3,
                Name = "Office Supplies"
            }
        );

        // Prepopulate Products
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 45000,
                Stock = 10,
                CategoryId = 1
            },
            new Product
            {
                Id = 2,
                Name = "Smartphone",
                Price = 25000,
                Stock = 15,
                CategoryId = 1
            },
            new Product
            {
                Id = 3,
                Name = "Keyboard",
                Price = 1500,
                Stock = 25,
                CategoryId = 2
            },
            new Product
            {
                Id = 4,
                Name = "Mouse",
                Price = 800,
                Stock = 30,
                CategoryId = 2
            },
            new Product
            {
                Id = 5,
                Name = "Notebook",
                Price = 100,
                Stock = 50,
                CategoryId = 3
            }
        );
    }
}