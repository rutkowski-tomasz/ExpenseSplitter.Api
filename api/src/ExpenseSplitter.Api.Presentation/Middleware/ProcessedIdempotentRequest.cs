using MessagePack;

namespace ExpenseSplitter.Api.Presentation.Middleware;

[MessagePackObject]
public class ProcessedIdempotentRequest(int statusCode, object? response)
{
    [Key(0)]
    public int StatusCode { get; } = statusCode;

    [Key(1)]
    public object? Response { get; } = response;
}
