namespace ExpenseSplitter.Api.Domain.Abstractions;

public record Error(ErrorType Type, string Description)
{
    public static readonly Error None = new(ErrorType.None, string.Empty);
}
