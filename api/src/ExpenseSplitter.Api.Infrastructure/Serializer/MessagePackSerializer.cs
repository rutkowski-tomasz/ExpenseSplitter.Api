using MessagePack;

namespace ExpenseSplitter.Api.Infrastructure.Serializer;

public class MessagePackSerializer : ISerializer
{
    private static readonly MessagePackSerializerOptions SerializerOptions
        = MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray);

    public T Deserialize<T>(byte[] serializedBytes, CancellationToken cancellationToken)
    {
        var deserialized = MessagePack.MessagePackSerializer.Deserialize<T>(serializedBytes, SerializerOptions, cancellationToken);
        return deserialized;
    }

    public byte[] Serialize<T>(T result, CancellationToken cancellationToken)
    {
        var serializedResult = MessagePack.MessagePackSerializer.Serialize(result, SerializerOptions, cancellationToken);
        return serializedResult;
    }
}
