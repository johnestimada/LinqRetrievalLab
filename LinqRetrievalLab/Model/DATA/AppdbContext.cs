using LinqRetrievalLab.Models.Domain;
using Microsoft.EntityFrameworkCore;

namespace LinqRetrievalLab.Models.DATA;

    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }


        public DbSet<Product> Product => Set<Product>();


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
