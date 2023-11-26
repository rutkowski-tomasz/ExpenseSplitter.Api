using NetArchTest.Rules;

namespace ExpenseSplitter.ArchitectureTests;

public class ProjectDependenciesTests
{
    [Fact]
    public void DomainLayer_ShouldNotHaveDependencyOnOtherLayers()
    {
        var result = Types.InAssembly(Assemblies.Domain)
            .Should()
            .NotHaveDependencyOnAny(new string[]
            {
                Assemblies.Application.GetName().Name!,
                Assemblies.Infrastructure.GetName().Name!,
                Assemblies.Presentation.GetName().Name!,
            })
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }

    [Fact]
    public void ApplicationLayer_ShouldNotHaveDependencyOnInfrastructureAndPresentationLayer()
    {
        var result = Types.InAssembly(Assemblies.Application)
            .Should()
            .NotHaveDependencyOnAny(new string[]
            {
                Assemblies.Infrastructure.GetName().Name!,
                Assemblies.Presentation.GetName().Name!,
            })
            .GetResult();

        result.IsSuccessful.Should().BeTrue();
    }
}
