using ExpenseSplitter.Domain.Abstractions;
using NetArchTest.Rules;

namespace ExpenseSplitter.ArchitectureTests;

public class DomainTests
{
    [Fact]
    public void Test()
    {
        var result = Types
            .InAssembly(Assemblies.Domain)
            .That()
            .ImplementInterface(typeof(IDomainEvent))
            .Should()
            .HaveNameEndingWith("DomainEvent")
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
