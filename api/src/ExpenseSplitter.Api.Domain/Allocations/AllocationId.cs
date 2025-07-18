namespace ExpenseSplitter.Api.Domain.Allocations;

public record AllocationId(Guid Value)
{
    public static AllocationId New() => new(Guid.CreateVersion7());
}
