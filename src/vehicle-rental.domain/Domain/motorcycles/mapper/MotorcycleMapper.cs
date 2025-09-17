using vehicle_rental.domain.Domain.motorcycles.dtos;
using vehicle_rental.domain.Domain.motorcycles.entity;

namespace vehicle_rental.domain.Domain.motorcycles.mapper
{
    public static class MotorcycleMapper
    {
        public static MotorcycleDto? MapToDto(this Motorcycle motorcycle)
        {
            if (motorcycle == null) return default;
            return new MotorcycleDto
            {
                Id = motorcycle.Id,
                Year = motorcycle.Year!,
                Model = motorcycle.Model!,
                LicensePlate = motorcycle.LicensePlate!
            };
        }

        public static IEnumerable<MotorcycleDto> MapToDto(this IEnumerable<Motorcycle> motorcycles)
        {
            if (motorcycles == null) return Enumerable.Empty<MotorcycleDto>();
            return motorcycles.Select(motorcycle => new MotorcycleDto()
            {
                Id = motorcycle.Id,
                Year = motorcycle.Year!,
                Model = motorcycle.Model!,
                LicensePlate = motorcycle.LicensePlate!,
                CreatedAt = motorcycle.CreatedAt,
                UpdatedAt = motorcycle.UpdatedAt
            });
        }
    }
}
