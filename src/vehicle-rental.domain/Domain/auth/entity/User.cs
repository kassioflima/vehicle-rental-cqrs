using Microsoft.AspNetCore.Identity;
using vehicle_rental.domain.Domain.auth.enums;
using vehicle_rental.domain.shared.entity;

namespace vehicle_rental.domain.Domain.auth.entity;

public class User : IdentityUser<Guid>
{
    public EUserRole Role { get; private set; }
    public Guid? DeliveryPersonId { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Constructor for Entity Framework
    protected User() { }

    public User(string email, EUserRole role, Guid? deliveryPersonId = null)
    {
        Id = Guid.NewGuid();
        Email = email;
        UserName = email; // Set UserName to email for Identity
        Role = role;
        DeliveryPersonId = deliveryPersonId;
    }

    public void UpdatePassword(string passwordHash)
    {
        PasswordHash = passwordHash;
    }

    public void Deactivate()
    {
        IsActive = false;
    }

    public void Activate()
    {
        IsActive = true;
    }
}
