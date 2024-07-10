namespace ExpenseSplitter.Api.Application.Abstractions.Etag;

public interface IEtagService
{
    bool HasIfMatchConflict(object value);
    bool HasIfNoneMatchConflict(object value);
    void AttachEtagToResponse(object value);
}
