using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using vehicle_rental.domain.Domain.auth.entity;
using vehicle_rental.domain.Domain.auth.enums;

namespace vehicle_rental.application.Services;

public interface IUserInitializationService
{
    Task InitializeAsync();
}

public class UserInitializationService : IUserInitializationService
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly ILogger<UserInitializationService> _logger;

    public UserInitializationService(
        UserManager<User> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        ILogger<UserInitializationService> logger)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    public async Task InitializeAsync()
    {
        await CreateRolesAsync();
        await CreateDefaultUsersAsync();
    }

    private async Task CreateRolesAsync()
    {
        var roles = Enum.GetNames<EUserRole>();

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole<Guid>(role));
                _logger.LogInformation("Role {Role} criada com sucesso", role);
            }
        }
    }

    private async Task CreateDefaultUsersAsync()
    {
        // Create default administrator
        var adminEmail = "admin@vehicle-rental.com";
        var adminUser = await _userManager.FindByEmailAsync(adminEmail);
        
        if (adminUser == null)
        {
            adminUser = new User(adminEmail, EUserRole.Administrator);
            var result = await _userManager.CreateAsync(adminUser, "Admin123!");
            
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, EUserRole.Administrator.ToString());
                _logger.LogInformation("Usuário administrador padrão criado: {Email}", adminEmail);
            }
            else
            {
                _logger.LogError("Falha ao criar usuário administrador: {Errors}", 
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // Create default delivery person
        var deliveryEmail = "entregador@vehicle-rental.com";
        var deliveryUser = await _userManager.FindByEmailAsync(deliveryEmail);
        
        if (deliveryUser == null)
        {
            deliveryUser = new User(deliveryEmail, EUserRole.DeliveryPerson);
            var result = await _userManager.CreateAsync(deliveryUser, "Entregador123!");
            
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(deliveryUser, EUserRole.DeliveryPerson.ToString());
                _logger.LogInformation("Usuário entregador padrão criado: {Email}", deliveryEmail);
            }
            else
            {
                _logger.LogError("Falha ao criar usuário entregador: {Errors}", 
                    string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}
