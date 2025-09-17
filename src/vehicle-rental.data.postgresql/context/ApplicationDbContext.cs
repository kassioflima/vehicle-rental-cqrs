using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using vehicle_rental.data.postgresql.context.configurations;
using vehicle_rental.domain.Domain.auth.entity;
using vehicle_rental.domain.Domain.deliveryPersons.entity;
using vehicle_rental.domain.Domain.motorcycleNotifications.entity;
using vehicle_rental.domain.Domain.motorcycles.entity;
using vehicle_rental.domain.Domain.rentals.entity;

namespace vehicle_rental.data.postgresql.context;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Motorcycle> Motorcycles { get; set; }
    public DbSet<DeliveryPerson> DeliveryPersons { get; set; }
    public DbSet<Rental> Rentals { get; set; }
    public DbSet<MotorcycleNotification> MotorcycleNotifications { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new MotorcycleConfiguration());
        modelBuilder.ApplyConfiguration(new DeliveryPersonConfiguration());
        modelBuilder.ApplyConfiguration(new RentalConfiguration());
        modelBuilder.ApplyConfiguration(new MotorcycleNotificationConfiguration());
        
        // Apply Identity configurations
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new IdentityRoleConfiguration());
        modelBuilder.ApplyConfiguration(new IdentityUserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new IdentityUserClaimConfiguration());
        modelBuilder.ApplyConfiguration(new IdentityUserLoginConfiguration());
        modelBuilder.ApplyConfiguration(new IdentityUserTokenConfiguration());
        modelBuilder.ApplyConfiguration(new IdentityRoleClaimConfiguration());
    }
}
