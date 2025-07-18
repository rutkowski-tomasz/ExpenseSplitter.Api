using ExpenseSplitter.Api.Domain.Abstractions;

namespace ExpenseSplitter.Api.Application.Exceptions;

public sealed class ConcurrencyException : Exception
{
    public static readonly AppError ConcurrencyAppError = new(
        ErrorType.ServerInternalError,
        "Concurrency occurred at database level, please retry."
    );

    public ConcurrencyException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
