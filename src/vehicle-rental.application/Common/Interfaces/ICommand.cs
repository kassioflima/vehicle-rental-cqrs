using MediatR;
using vehicle_rental.application.Common.Models;

namespace vehicle_rental.application.Common.Interfaces;

public interface ICommand<TResponse> : IRequest<Result<TResponse>>
{
}

public interface ICommand : IRequest<Result>
{
}

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
}

public interface IQuery : IRequest<Result>
{
}
