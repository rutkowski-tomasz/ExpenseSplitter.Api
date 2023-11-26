﻿using ExpenseSplitter.Application.Settlements.GetAllSettlements;
using ExpenseSplitter.Domain.Settlements;

namespace ExpenseSplitter.Application.UnitTests.Settlements;

public class GetAllSettlementsQueryHandlerTests
{
    private readonly Mock<ISettlementRepository> settlementRepositoryMock;
    private readonly GetSettlementsQueryHandler handler;

    public GetAllSettlementsQueryHandlerTests()
    {
        settlementRepositoryMock = new Mock<ISettlementRepository>();
        handler = new GetSettlementsQueryHandler(settlementRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldMapSettlementsToDto()
    {
        var settlements = new Fixture()
            .Build<Settlement>()
            .FromFactory((string name) => Settlement.Create(name).Value)
            .CreateMany(2);

        settlementRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(settlements);

        var query = new Fixture().Create<GetAllSettlementsQuery>();

        var response = await handler.Handle(query, default);

        response.IsSuccess.Should().BeTrue();
        response.Value.Should().HaveCount(2);

        var firstSettlement = settlements.First();
        response.Value.First().Id.Should().Be(firstSettlement.Id.Value);
        response.Value.First().Name.Should().Be(firstSettlement.Name);
    }
}
