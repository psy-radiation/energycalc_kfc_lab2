using Microsoft.EntityFrameworkCore;

public class DatabaseContext : DbContext
{
    public DbSet<MeterRecord> MeterRecords { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=meter_data.db");
}

public class MeterRecord
{
    public int Id { get; set; }
    public string MeterId { get; set; }
    public double Day { get; set; }
    public double Night { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.Now;
}
