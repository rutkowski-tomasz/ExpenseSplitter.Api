using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using ExpenseSplitter.Api.Domain.Abstractions;
using MediatR;

namespace ExpenseSplitter.Api.Application.Abstractions.Idempotency;

internal static class JsonSerializerConfig
{
    internal static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        Converters = { new ResultJsonConverterFactory() }
    };
}

// Note there is also IdempotentFilter which works on request level
internal sealed class IdempotentBehavior<TRequest, TResponse>(IIdempotencyService service)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
    where TResponse : Result
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var getIdempotencyKey = service.GetIdempotencyKeyFromHeaders();
        if (getIdempotencyKey.IsFailure)
        {
            return await next();
        }
        
        var parsedIdempotencyKey = getIdempotencyKey.Value;
        var (isProcessed, cachedResponse) = await service.GetProcessedRequest<string>(
            parsedIdempotencyKey,
            cancellationToken
        );
        
        if (isProcessed)
        {
            var deserialized = JsonSerializer.Deserialize<TResponse?>(cachedResponse!, JsonSerializerConfig.JsonSerializerOptions);
            return deserialized;
        }
        
        var response = await next();

        var serialized = JsonSerializer.Serialize(response, JsonSerializerConfig.JsonSerializerOptions);
        await service.SaveIdempotentRequest(parsedIdempotencyKey, serialized, cancellationToken);

        return response;
    }
}

public class ResultJsonConverterFactory : JsonConverterFactory
{
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType && 
               typeToConvert.GetGenericTypeDefinition() == typeof(Result<>);
    }

    public override JsonConverter? CreateConverter(
        Type typeToConvert, 
        JsonSerializerOptions options)
    {
        var valueType = typeToConvert.GetGenericArguments()[0];
        
        var converterInstance = (JsonConverter)Activator.CreateInstance(
            typeof(ResultJsonConverter<>).MakeGenericType(valueType),
            BindingFlags.Instance | BindingFlags.Public,
            binder: null,
            args: null,
            culture: null)!;

        return converterInstance;
    }
}

public class ResultJsonConverter<TValue> : JsonConverter<Result<TValue>>
{
    public override Result<TValue> Read(
        ref Utf8JsonReader reader, 
        Type typeToConvert, 
        JsonSerializerOptions options)
    {
        using var jsonDocument = JsonDocument.ParseValue(ref reader);
        var root = jsonDocument.RootElement;

        if (root.TryGetProperty("IsSuccess", out var isSuccessProperty) && 
            root.TryGetProperty("AppError", out var appErrorProperty))
        {
            var isSuccess = isSuccessProperty.GetBoolean();

            if (isSuccess)
            {
                if (root.TryGetProperty("Value", out var valueProperty))
                {
                    var value = valueProperty.ValueKind != JsonValueKind.Null
                        ? JsonSerializer.Deserialize<TValue>(valueProperty.GetRawText(), options)
                        : default;
                    
                    return Result.Success(value!);
                }
                
                return Result.Success<TValue>(default!);
            }

            var appError = JsonSerializer.Deserialize<AppError>(appErrorProperty.GetRawText(), options);
            return Result.Failure<TValue>(appError!);
        }

        throw new JsonException("Invalid Result JSON format");
    }

    public override void Write(
        Utf8JsonWriter writer, 
        Result<TValue> value, 
        JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteBoolean("IsSuccess", value.IsSuccess);
        
        if (value.IsSuccess)
        {
            writer.WritePropertyName("Value");
            JsonSerializer.Serialize(writer, value.Value, options);
        }
        else
        {
            writer.WritePropertyName("AppError");
            JsonSerializer.Serialize(writer, value.AppError, options);
        }
        
        writer.WriteEndObject();
    }
}

