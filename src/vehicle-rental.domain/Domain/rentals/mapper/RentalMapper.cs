using vehicle_rental.domain.Domain.rentals.dtos;
using vehicle_rental.domain.Domain.rentals.entity;

namespace vehicle_rental.domain.Domain.rentals.mapper;

public static class RentalMapper
{
    public static RentalDto? MapToDto(this Rental rental)
    {
        if (rental == null) return null;
        return new RentalDto
        {
            Id = rental.Id,
            DeliveryPersonId = rental.DeliveryPersonId,
            MotorcycleId = rental.MotorcycleId,
            Plan = rental.Plan,
            StartDate = rental.StartDate,
            EndDate = rental.EndDate,
            ExpectedEndDate = rental.ExpectedEndDate,
            DailyRate = rental.DailyRate,
            TotalAmount = rental.TotalAmount,
            FineAmount = rental.FineAmount,
            AdditionalDaysAmount = rental.AdditionalDaysAmount,
            Status = rental.Status,
            CreatedAt = rental.CreatedAt,
            UpdatedAt = rental.UpdatedAt
        };
    }

    public static IEnumerable<RentalDto> MapToDto(this IEnumerable<Rental> rentals)
    {
        if (rentals == null) return [];
        return rentals.Select(rentals => new RentalDto()
        {
            Id = rentals.Id,
            DeliveryPersonId = rentals.DeliveryPersonId,
            MotorcycleId = rentals.MotorcycleId,
            Plan = rentals.Plan,
            StartDate = rentals.StartDate,
            EndDate = rentals.EndDate,
            ExpectedEndDate = rentals.ExpectedEndDate,
            DailyRate = rentals.DailyRate,
            TotalAmount = rentals.TotalAmount,
            FineAmount = rentals.FineAmount,
            AdditionalDaysAmount = rentals.AdditionalDaysAmount,
            Status = rentals.Status,
            CreatedAt = rentals.CreatedAt,
            UpdatedAt = rentals.UpdatedAt
        });
    }
}
