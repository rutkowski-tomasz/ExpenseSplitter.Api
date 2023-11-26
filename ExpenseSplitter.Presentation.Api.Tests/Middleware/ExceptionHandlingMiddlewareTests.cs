using ExpenseSplitter.Application.Exceptions;
using ExpenseSplitter.Presentation.Api.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ExpenseSplitter.Presentation.Api.Tests.Middleware;

public class ExceptionHandlingMiddlewareTests
{
    private readonly Mock<RequestDelegate> _nextMock;
    private readonly Mock<ILogger<ExceptionHandlingMiddleware>> _loggerMock;
    private readonly ExceptionHandlingMiddleware _middleware;

    public ExceptionHandlingMiddlewareTests()
    {
        _nextMock = new Mock<RequestDelegate>();
        _loggerMock = new Mock<ILogger<ExceptionHandlingMiddleware>>();
        _middleware = new ExceptionHandlingMiddleware(_nextMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task InvokeAsync_WhenNoException_ShouldNotChangeResponse()
    {
        var context = new DefaultHttpContext();

        await _middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task InvokeAsync_WhenValidationExceptionOccurs_ShouldReturnBadRequest()
    {
        var context = new DefaultHttpContext();
        var validationException = new ValidationException(new List<ValidationError> {
            new ("Property1", "Message1")
        });
        _nextMock.Setup(n => n(context)).ThrowsAsync(validationException);

        await _middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
    }

    [Fact]
    public async Task InvokeAsync_WhenGenericExceptionOccurs_ShouldReturnInternalServerError()
    {
        var context = new DefaultHttpContext();
        _nextMock.Setup(n => n(context)).ThrowsAsync(new Exception());

        await _middleware.InvokeAsync(context);

        context.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
    }
}
