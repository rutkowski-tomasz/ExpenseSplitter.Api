using ExpenseSplitter.Application.Settlements.CreateSettlement;
using ExpenseSplitter.Domain.Abstractions;
using ExpenseSplitter.Domain.Settlements;

namespace ExpenseSplitter.Application.UnitTests.Settlements;

public class CreateSettlementCommandHandlerTests
{
    private readonly Mock<ISettlementRepository> settlementRepositoryMock;
    private readonly Mock<IUnitOfWork> unitOfWorkMock;
    private readonly CreateSettlementCommandHandler handler;

    public CreateSettlementCommandHandlerTests()
    {
        settlementRepositoryMock = new Mock<ISettlementRepository>();
        unitOfWorkMock = new Mock<IUnitOfWork>();

        handler = new CreateSettlementCommandHandler(
            settlementRepositoryMock.Object,
            unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldAddNewSettlementToRepository()
    {
        var command = new Fixture().Create<CreateSettlementCommand>();

        var response = await handler.Handle(command, default);

        response.IsSuccess.Should().BeTrue();
        response.Value.Should().NotBeEmpty();

        settlementRepositoryMock.Verify(x => x.Add(It.Is<Settlement>(y => y.Name == command.Name)), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenNameIsEmpty()
    {
        var command = new Fixture()
            .Build<CreateSettlementCommand>()
            .With(x => x.Name, string.Empty)
            .Create();

        var response = await handler.Handle(command, default);

        response.IsFailure.Should().BeTrue();
        response.Error.Code.Should().Be(SettlementErrors.EmptyName.Code);

        settlementRepositoryMock.Verify(x => x.Add(It.Is<Settlement>(y => y.Name == command.Name)), Times.Never);
    }
}

