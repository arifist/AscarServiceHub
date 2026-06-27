/*
 * MSSQL / Entity Framework Core — Aktif etmek için:
 * 1. AscarServiceHub.csproj içindeki EF Core paket yorumlarını kaldırın.
 * 2. appsettings.json'a "ConnectionStrings:DefaultConnection" ekleyin.
 * 3. Program.cs'de AddSingleton<JsonDataStore>() yerine AddDbContext<AppDbContext>() kullanın.
 * 4. Tüm Razor sayfaları ve servislerde @inject JsonDataStore Store yerine @inject AppDbContext Db kullanın.
 * 5. `dotnet ef migrations add InitialCreate` ve `dotnet ef database update` çalıştırın.
 *
 * ────────────────────────────────────────────────────────────────────────────

using Microsoft.EntityFrameworkCore;
using AscarServiceHub.Models;

namespace AscarServiceHub.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<ServiceRecord> ServiceRecords => Set<ServiceRecord>();
    public DbSet<DamagePhoto> DamagePhotos => Set<DamagePhoto>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceItem> InvoiceItems => Set<InvoiceItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.FullName).IsRequired().HasMaxLength(150);
            e.Property(c => c.Phone).IsRequired().HasMaxLength(20);
            e.HasMany(c => c.Vehicles).WithOne(v => v.Customer).HasForeignKey(v => v.CustomerId);
        });

        modelBuilder.Entity<Vehicle>(e =>
        {
            e.HasKey(v => v.Id);
            e.Property(v => v.Plate).IsRequired().HasMaxLength(20);
            e.Property(v => v.Brand).IsRequired().HasMaxLength(60);
            e.Property(v => v.Model).IsRequired().HasMaxLength(60);
            e.HasIndex(v => v.Plate).IsUnique();
            e.HasMany(v => v.ServiceRecords).WithOne(r => r.Vehicle).HasForeignKey(r => r.VehicleId);
        });

        modelBuilder.Entity<ServiceRecord>(e =>
        {
            e.HasKey(r => r.Id);
            e.Property(r => r.RecordNumber).IsRequired().HasMaxLength(30);
            e.Property(r => r.EstimatedCost).HasColumnType("decimal(18,2)");
            e.Property(r => r.FinalCost).HasColumnType("decimal(18,2)");
            e.HasMany(r => r.Photos).WithOne(p => p.ServiceRecord).HasForeignKey(p => p.ServiceRecordId);
            e.HasOne(r => r.Invoice).WithOne(i => i.ServiceRecord).HasForeignKey<Invoice>(i => i.ServiceRecordId);
        });

        modelBuilder.Entity<DamagePhoto>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.FileName).IsRequired().HasMaxLength(200);
        });

        modelBuilder.Entity<Invoice>(e =>
        {
            e.HasKey(i => i.Id);
            e.Property(i => i.TaxRate).HasColumnType("decimal(5,4)");
            e.HasMany(i => i.Items).WithOne().HasForeignKey(item => item.Id);
        });

        modelBuilder.Entity<InvoiceItem>(e =>
        {
            e.HasKey(item => item.Id);
            e.Property(item => item.UnitPrice).HasColumnType("decimal(18,2)");
            e.Property(item => item.Quantity).HasColumnType("decimal(10,3)");
        });
    }
}

// Program.cs'de kullanım:
// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseSqlServer(connectionString));
//
// Migrations:
// using (var scope = app.Services.CreateScope())
// {
//     var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//     db.Database.Migrate();
// }

*/
