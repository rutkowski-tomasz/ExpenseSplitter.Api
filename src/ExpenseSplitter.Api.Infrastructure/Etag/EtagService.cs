using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ExpenseSplitter.Api.Application.Abstractions.Etag;
using Microsoft.AspNetCore.Http;

namespace ExpenseSplitter.Api.Infrastructure.Etag;

public class EtagService(
    IHttpContextAccessor httpContextAccessor
) : IEtagService
{
    public const string ETagHeader = "ETag";
    public const string IfMatchHeader = "If-Match";
    public const string IfNoneMatchHeader = "If-None-Match";

    public bool HasIfMatchConflict(object value)
    {
        var hasIfMatch = httpContextAccessor.HttpContext!.Request.Headers.TryGetValue(IfMatchHeader, out var ifMatch);
        if (!hasIfMatch)
        {
            return false;
        }

        var etag = GenerateEtag(value);
        var isMismatch = etag != ifMatch;

        return isMismatch;
    }

    public bool HasIfNoneMatchConflict(object value)
    {
        var hasIfNoneMatch = httpContextAccessor.HttpContext!.Request.Headers.TryGetValue(IfNoneMatchHeader, out var ifNoneMatch);
        if (!hasIfNoneMatch)
        {
            return false;
        }

        var etag = GenerateEtag(value);
        var isMatch = etag == ifNoneMatch;

        return isMatch;
    }

    public void AttachEtagToResponse(object value)
    {
        var etag = GenerateEtag(value);
        httpContextAccessor.HttpContext!.Response.Headers[ETagHeader] = etag;
    }

    private string GenerateEtag(object value)
    {
        var serialized = JsonSerializer.Serialize(value);
        var bytes = Encoding.UTF8.GetBytes(serialized);
        var hash = SHA256.HashData(bytes);
        var base64 = Convert.ToBase64String(hash);
        return base64;
    }
}
