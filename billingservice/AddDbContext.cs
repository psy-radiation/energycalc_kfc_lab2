using Microsoft.EntityFrameworkCore;

namespace billingservice;

public class AddDbContext : DbContext
{
    public DbSet<MeterReading> MeterReadings { get; set; }

    public AddDbContext(DbContextOptions<AddDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MeterReading>().HasIndex(m => m.MeterId);
    }
}
