using vehicle_rental.domain.Domain.auth.enums;

namespace vehicle_rental.domain.Domain.auth.dtos;

public record LoginRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

public record LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public UserInfoDto User { get; set; } = null!;
}

public record UserInfoDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public EUserRole Role { get; set; }
    public Guid? DeliveryPersonId { get; set; }
}

public record CreateUserDto
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public EUserRole Role { get; set; }
    public Guid? DeliveryPersonId { get; set; }
}
