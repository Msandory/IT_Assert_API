using Inventory_System_API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Inventory_System_API.Data
{
    public class DataContex : IdentityDbContext<IdentityUser>
    {
        private readonly IConfiguration _configuration;

        public DataContex(IConfiguration configuration) : base()
        {
            _configuration = configuration;
        }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string connectionString = _configuration.GetConnectionString("IneventoryData");
                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        public DbSet<Owner> Owners { get; set; }
        public DbSet<Laptop> Laptops { get; set; }
        public DbSet<Desktop> Desktops { get; set; }
        public DbSet<Mobilephones> Mobilephones { get; set; }
        public DbSet<Tablets> Tablets { get; set; }
        public DbSet<ComputerCount> ComputerCounts {  get; set; }
        public DbSet<Licence> Licences { get; set; }
        public DbSet<Logins> Logins { get; set; }

        //New Devices
        public DbSet<PhysicalServer> PhysicalServers { get; set; }
        public DbSet<Switch> Switches { get; set; }
        public DbSet<Printer> Printers { get; set; }
        public DbSet<Firewall> Firewalls { get; set; }
        public DbSet<WifiController> WifiControllers { get; set; }
        public DbSet<AccessPoint> AccessPoints { get; set; }
        public DbSet<StorageDevice> StorageDevices { get; set; }
        public DbSet<Turnstile> Turnstiles { get; set; }
        public DbSet<BiometricDevice> BiometricDevices { get; set; }
        public DbSet<Toner>Toners { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Conversion for DateOnly
            modelBuilder.Entity<Desktop>()
         .Property(e => e.DOP)
         .HasConversion(
             v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
             v => v.HasValue ? DateOnly.FromDateTime(v.Value) : (DateOnly?)null
         );

            modelBuilder.Entity<Laptop>()
                .Property(e => e.DOP)
                .HasConversion(
                    v => v.HasValue ? v.Value.ToDateTime(TimeOnly.MinValue) : (DateTime?)null,
                    v => v.HasValue ? DateOnly.FromDateTime(v.Value) : (DateOnly?)null
                );


            // Entity relationships
            modelBuilder.Entity<Owner>()
                .HasMany(o => o.Laptop)
                .WithOne(l => l.Owner)
                .HasForeignKey(l => l.OwnerId);

            modelBuilder.Entity<Owner>()
                .HasMany(o => o.Desktop)
                .WithOne(d => d.Owner)
                .HasForeignKey(d => d.OwnerId);

            modelBuilder.Entity<Owner>()
                .HasMany(o => o.Mobilephones)
                .WithOne(m => m.Owner)
                .HasForeignKey(m => m.OwnerId);

            modelBuilder.Entity<Owner>()
                .HasMany(o => o.Tablets)
                .WithOne(t => t.Owner)
                .HasForeignKey(t => t.OwnerId);
        }
    
    }
}
