using System.Net;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

namespace ExpenseSplitter.Api.Presentation.Filters;

public class EtagFilter : IEndpointFilter
{
    private const string ETagHeader = "ETag";
    private const string IfNoneMatchHeader = "If-None-Match";
    private const string salt = "etag-salt-for-hash-function";

    public virtual async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next
    )
    {
        var request = context.HttpContext.Request;
        var result = await next(context);
        var response = context.HttpContext.Response;

        if (request.Method != HttpMethod.Get.Method || response.StatusCode != (int)HttpStatusCode.OK)
        {
            return result;
        }

        var resultValue = GetResultValue(result);
        if (resultValue is null)
        {
            return result;
        }

        var etag = ComputeWithHashFunction(resultValue);
        response.Headers[ETagHeader] = etag;

        if (request.Headers.TryGetValue(IfNoneMatchHeader, out var incomingEtag) && incomingEtag == etag)
        {
            return TypedResults.StatusCode((int)HttpStatusCode.NotModified);
        }

        return result;
    }

    private object? GetResultValue(object? result)
    {
        if (result is not null
            && result is INestedHttpResult nestedHttpResult
            && nestedHttpResult.Result is IValueHttpResult valueHttpResult)
        {
            return valueHttpResult.Value;
        }

        return null;
    }

    private string ComputeWithHashFunction(object value)
    {
        var serialized = JsonSerializer.Serialize(value);

        var valueBytes = KeyDerivation.Pbkdf2(
            password: serialized,
            salt: Encoding.UTF8.GetBytes(salt),
            prf: KeyDerivationPrf.HMACSHA512,
            iterationCount: 10000,
            numBytesRequested: 256 / 8
        );

        return Convert.ToBase64String(valueBytes);
    }
}
