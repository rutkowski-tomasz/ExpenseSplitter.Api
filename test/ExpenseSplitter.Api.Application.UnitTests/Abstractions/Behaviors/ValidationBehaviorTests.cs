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
        var requestMock = new Mock<IBaseCommand>();
        var nextDelegateMock = new Mock<RequestHandlerDelegate<int>>();
        nextDelegateMock.Setup(x => x()).ReturnsAsync(1337);

        var result = await behavior.Handle(requestMock.Object, nextDelegateMock.Object, default);

        result.Should().Be(1337);
        nextDelegateMock.Verify(x => x(), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowValidationException_WhenValidationFails()
    {
        var validatorMock = new Mock<IValidator<IBaseCommand>>();
        var validationFailures = new List<ValidationFailure>
        {
            new("Property1", "Error message 1")
        };

        validatorMock.Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                        .Returns(new ValidationResult(validationFailures));

        var validators = new List<IValidator<IBaseCommand>> { validatorMock.Object };
        var behavior = new ValidationBehavior<IBaseCommand, int>(validators);
        var requestMock = new Mock<IBaseCommand>();
        var nextDelegateMock = new Mock<RequestHandlerDelegate<int>>();

        var action = () => behavior.Handle(requestMock.Object, nextDelegateMock.Object, default);

        await action.Should()
            .ThrowAsync<Exceptions.ValidationException>()
            .Where(x => x.Errors.Single().PropertyName == "Property1");
    }

    [Fact]
    public async Task Handle_ShouldProceedToNextDelegate_WhenValidationPasses()
    {
        var validatorMock = new Mock<IValidator<IBaseCommand>>();
        validatorMock.Setup(x => x.Validate(It.IsAny<IValidationContext>()))
                        .Returns(new ValidationResult());

        var validators = new List<IValidator<IBaseCommand>> { validatorMock.Object };
        var behavior = new ValidationBehavior<IBaseCommand, int>(validators);
        var requestMock = new Mock<IBaseCommand>();
        var nextDelegateMock = new Mock<RequestHandlerDelegate<int>>();
        nextDelegateMock.Setup(x => x()).ReturnsAsync(1337);

        var result = await behavior.Handle(requestMock.Object, nextDelegateMock.Object, default);

        result.Should().Be(1337);
        nextDelegateMock.Verify(x => x(), Times.Once);
    }
}
