using ExpenseSplitter.Api.Application.Settlements.GetAllSettlements;
using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.Application.UnitTests.Settlements;

public class GetAllSettlementsQueryHandlerTests
{
    private readonly Fixture fixture;
    private readonly Mock<ISettlementRepository> settlementRepositoryMock;
    private readonly GetSettlementsQueryHandler handler;

    public GetAllSettlementsQueryHandlerTests()
    {
        fixture = CustomFixture.Create();
        settlementRepositoryMock = new Mock<ISettlementRepository>();
        handler = new GetSettlementsQueryHandler(settlementRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldMapSettlementsToDto()
    {
        var settlements = fixture.CreateMany<Settlement>(2).ToList();
        var query = fixture.Create<GetAllSettlementsQuery>();
    
        settlementRepositoryMock
            .Setup(x => x.GetPaged(query.Page, query.PageSize, It.IsAny<CancellationToken>()))
            .ReturnsAsync(settlements);

        var response = await handler.Handle(query, default);

        response.IsSuccess.Should().BeTrue();
        response.Value.Settlements.Should().HaveCount(2);

        var firstSettlement = settlements.First();
        response.Value.Settlements.First().Id.Should().Be(firstSettlement.Id.Value);
        response.Value.Settlements.First().Name.Should().Be(firstSettlement.Name);
    }
}
