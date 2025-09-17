using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using vehicle_rental.data.postgresql.context;

namespace vehicle_rental.integration.tests.Infrastructure;

public class IntegrationTestBase
{
    protected ApplicationDbContext CreateDbContext(string databaseName = "TestDatabase")
    {
        var services = new ServiceCollection();
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseInMemoryDatabase(databaseName);
            options.EnableSensitiveDataLogging();
        });

        var serviceProvider = services.BuildServiceProvider();
        var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

        // Ensure database is created
        context.Database.EnsureCreated();

        return context;
    }
}