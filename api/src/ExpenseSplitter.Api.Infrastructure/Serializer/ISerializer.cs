namespace ExpenseSplitter.Api.Infrastructure.Serializer;

public interface ISerializer
{
    T Deserialize<T>(byte[] serializedBytes, CancellationToken cancellationToken);
    byte[] Serialize<T>(T result, CancellationToken cancellationToken);
}
