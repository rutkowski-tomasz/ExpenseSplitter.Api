using ExpenseSplitter.Api.Application.Abstractions.Behaviors;
using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace ExpenseSplitter.Api.Application.UnitTests.Abstractions.Behaviors;

public class ValidationBehaviorTests
{
    [Fact]
    public async Task Handle_ShouldProceedToNextDelegate_WhenNoValidators()
    {
        var validators = Enumerable.Empty<IValidator<IBaseCommand>>();
        var behavior = new ValidationBehavior<IBaseCommand, int>(validators);
        var request = Substitute.For<IBaseCommand>();
        var nextDelegate = Substitute.For<RequestHandlerDelegate<int>>();
        nextDelegate(Arg.Any<CancellationToken>()).Returns(Task.FromResult(1337));

        var result = await behavior.Handle(request, nextDelegate, default);

        result.Should().Be(1337);
        await nextDelegate.Received(1)(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
    {
        var validator = Substitute.For<IValidator<IBaseCommand>>();
        var validationFailures = new List<ValidationFailure>
        {
            new("Property1", "Error message 1")
        };

        validator
            .ValidateAsync(Arg.Any<IValidationContext>())
            .Returns(new ValidationResult(validationFailures));

        var validators = new List<IValidator<IBaseCommand>> { validator };
        var behavior = new ValidationBehavior<IBaseCommand, int>(validators);
        var request = Substitute.For<IBaseCommand>();
        var nextDelegate = Substitute.For<RequestHandlerDelegate<int>>();

        var action = () => behavior.Handle(request, nextDelegate, default);

        await action.Should()
            .ThrowAsync<Exceptions.ValidationException>()
            .Where(x => x.Errors.Single().PropertyName == "Property1");
    }

    [Fact]
    public async Task Handle_ShouldProceedToNextDelegate_WhenValidationPasses()
    {
        var validator = Substitute.For<IValidator<IBaseCommand>>();
        validator
            .ValidateAsync(Arg.Any<IValidationContext>())
            .Returns(new ValidationResult());

        var validators = new List<IValidator<IBaseCommand>> { validator };
        var behavior = new ValidationBehavior<IBaseCommand, int>(validators);
        var request = Substitute.For<IBaseCommand>();
        var nextDelegate = Substitute.For<RequestHandlerDelegate<int>>();
        nextDelegate(Arg.Any<CancellationToken>()).Returns(Task.FromResult(1337));

        var result = await behavior.Handle(request, nextDelegate, default);

        result.Should().Be(1337);
        await nextDelegate.Received(1)(Arg.Any<CancellationToken>());
    }
}
