using ExpenseSplitter.Api.Application.Abstractions.Cqrs;
using FluentValidation;
using MediatR;
using NetArchTest.Rules;

namespace ExpenseSplitter.Api.ArchitectureTests;

public class ApplicationTests
{
    [Fact]
    public void Queries_ShouldHaveQueryEnding()
    {
        var result = Types
          .InAssembly(Assemblies.Application)
          .That()
          .ImplementInterface(typeof(IQuery<>))
          .Should()
          .HaveNameEndingWith("Query")
          .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Commands_ShouldHaveCommandEnding()
    {
        var result = Types
          .InAssembly(Assemblies.Application)
          .That()
          .ImplementInterface(typeof(ICommand<>))
          .Should()
          .HaveNameEndingWith("Command")
          .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void QueryHandlers_ShouldHaveQueryHandlerEnding()
    {
        var result = Types
          .InAssembly(Assemblies.Application)
          .That()
          .ImplementInterface(typeof(IQueryHandler<,>))
          .Should()
          .HaveNameEndingWith("QueryHandler")
          .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void CommandHandlers_ShouldHaveCommandHandlerEnding()
    {
        var result = Types
          .InAssembly(Assemblies.Application)
          .That()
          .ImplementInterface(typeof(ICommandHandler<>))
          .Or()
          .ImplementInterface(typeof(ICommandHandler<,>))
          .Should()
          .HaveNameEndingWith("CommandHandler")
          .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Validators_ShouldHaveValidatorEnding()
    {
        var result = Types
          .InAssembly(Assemblies.Application)
          .That()
          .Inherit(typeof(AbstractValidator<>))
          .Should()
          .HaveNameEndingWith("Validator")
          .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void Behaviors_ShouldHaveBehaviorEnding()
    {
        var result = Types
            .InAssembly(Assemblies.Application)
            .That()
            .ImplementInterface(typeof(IPipelineBehavior<,>))
            .Should()
            .HaveNameMatching("Behavior")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void DomainEventHandlers_ShouldHaveDomainEventHandlerEnding()
    {
        var result = Types
            .InAssembly(Assemblies.Application)
            .That()
            .ImplementInterface(typeof(IDomainEventHandler<>))
            .Should()
            .HaveNameEndingWith("DomainEventHandler")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}

