using ExpenseSplitter.Api.Application.Settlements.GetAllSettlements;
using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.Application.UnitTests.Settlements;

public class GetAllSettlementsQueryHandlerTests
{
    private readonly Fixture fixture = CustomFixture.Create();
    private readonly ISettlementRepository settlementRepository = Substitute.For<ISettlementRepository>();
    private readonly GetSettlementsQueryHandler handler;

    public GetAllSettlementsQueryHandlerTests()
    {
        handler = new GetSettlementsQueryHandler(settlementRepository);
    }

    [Fact]
    public async Task Handle_ShouldMapSettlementsToDto()
    {
        var settlements = fixture.CreateMany<Settlement>(2).ToList();
        var query = fixture.Create<GetAllSettlementsQuery>();
    
        settlementRepository
            .GetPaged(query.Page, query.PageSize, Arg.Any<CancellationToken>())
            .Returns(settlements);

        var response = await handler.Handle(query, default);

        response.IsSuccess.Should().BeTrue();
        response.Value.Settlements.Should().HaveCount(2);

        var firstSettlement = settlements[0];
        response.Value.Settlements.First().Id.Should().Be(firstSettlement.Id.Value);
        response.Value.Settlements.First().Name.Should().Be(firstSettlement.Name);
    }
}
