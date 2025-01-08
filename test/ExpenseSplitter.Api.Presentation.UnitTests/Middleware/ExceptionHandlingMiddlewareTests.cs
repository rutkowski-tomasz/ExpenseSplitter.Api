using ExpenseSplitter.Api.Application.Exceptions;
using ExpenseSplitter.Api.Presentation.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute.ExceptionExtensions;

namespace ExpenseSplitter.Api.Presentation.UnitTests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private readonly RequestDelegate next = Substitute.For<RequestDelegate>();
    private readonly ILogger<ExceptionHandlingMiddleware> logger =
        Substitute.For<ILogger<ExceptionHandlingMiddleware>>();
    private readonly ExceptionHandlingMiddleware middleware;

    public ExceptionHandlingMiddlewareTests()
    {
        middleware = new ExceptionHandlingMiddleware(next, logger);
    }

    [Fact]
    public async Task InvokeAsync_ShouldNotChangeResponse_WhenNoException()
    {
        var context = new DefaultHttpContext();

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturnBadRequest_WhenValidationExceptionOccurs()
    {
        var context = new DefaultHttpContext();
        var validationException = new ValidationException(new List<ValidationError> {
            new ("Property1", "Message1")
        });
        next.Invoke(context).ThrowsAsync(validationException);

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturnInternalServerError_WhenGenericExceptionOccurs()
    {
        var context = new DefaultHttpContext();
        next.Invoke(context).ThrowsAsync(new Exception());

        await middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }
}
