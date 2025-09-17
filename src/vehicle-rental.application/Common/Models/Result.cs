namespace vehicle_rental.application.Common.Models;

public class Result<T>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public T? Value { get; }
    public Error? Error { get; }

    private Result(bool isSuccess, T? value, Error? error)
    {
        IsSuccess = isSuccess;
        Value = value;
        Error = error;
    }

    public static Result<T> Success(T value) => new(true, value, null);
    public static Result<T> Failure(Error error) => new(false, default, error);
    public static Result<T> Failure(string code, string message) => new(false, default, new Error(code, message));
}

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error? Error { get; }

    private Result(bool isSuccess, Error? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static Result Success() => new(true, null);
    public static Result Failure(Error error) => new(false, error);
    public static Result Failure(string code, string message) => new(false, new Error(code, message));
}

public record Error(string Code, string Message)
{
    public static Error NotFound(string resource) => new("NOT_FOUND", $"{resource} não encontrado");
    public static Error AlreadyExists(string resource) => new("ALREADY_EXISTS", $"{resource} já existe");
    public static Error InvalidOperation(string message) => new("INVALID_OPERATION", message);
    public static Error ValidationFailed(string message) => new("VALIDATION_FAILED", message);
    public static Error Unauthorized(string message) => new("UNAUTHORIZED", message);
    public static Error Forbidden(string message) => new("FORBIDDEN", message);
    public static Error Conflict(string message) => new("CONFLICT", message);
}
