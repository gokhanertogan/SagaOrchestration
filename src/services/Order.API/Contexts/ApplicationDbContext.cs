using Microsoft.EntityFrameworkCore;

namespace Order.API.Contexts;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<Entities.Order> Orders { get; set; }
    public DbSet<Entities.OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
}