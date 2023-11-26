using ExpenseSplitter.Application.Settlements.GetSettlement;
using ExpenseSplitter.Domain.Settlements;
using ExpenseSplitter.Application.Settlements.GetAllSettlements;

namespace ExpenseSplitter.Application.UnitTests.Settlements;
public class GetSettlementQueryHandlerTests
{
    private readonly Mock<ISettlementRepository> settlementRepositoryMock;
    private readonly GetSettlementQueryHandler handler;

    public GetSettlementQueryHandlerTests()
    {
        settlementRepositoryMock = new Mock<ISettlementRepository>();
        handler = new GetSettlementQueryHandler(settlementRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldMapSettlementsToDto()
    {
        var settlement = new Fixture()
            .Build<Settlement>()
            .FromFactory((string name) => Settlement.Create(name).Value)
            .Create();

        settlementRepositoryMock
            .Setup(x => x.GetByIdAsync(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(settlement);

        var query = new Fixture().Create<GetSettlementQuery>();

        var response = await handler.Handle(query, default);

        response.IsSuccess.Should().BeTrue();
        response.Value.Name.Should().Be(settlement.Name);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSettlementWithTheIdDoesntExist()
    {
        var query = new Fixture().Create<GetSettlementQuery>();

        var response = await handler.Handle(query, default);

        response.IsFailure.Should().BeTrue();
        response.Error.Code.Should().Be(SettlementErrors.NotFound.Code);
    }
}

