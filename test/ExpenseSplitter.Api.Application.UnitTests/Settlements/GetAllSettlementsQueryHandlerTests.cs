﻿using ExpenseSplitter.Api.Application.Settlements.GetAllSettlements;
using ExpenseSplitter.Api.Domain.Settlements;

namespace ExpenseSplitter.Api.Application.UnitTests.Settlements;

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
            .FromFactory((string name, string inviteCode) => Settlement.Create(name, inviteCode).Value)
            .CreateMany(2)
            .ToArray();

        settlementRepositoryMock
            .Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(settlements.ToList());

        var query = new Fixture().Create<GetAllSettlementsQuery>();

        var response = await handler.Handle(query, default);

        response.IsSuccess.Should().BeTrue();
        response.Value.Settlements.Should().HaveCount(2);

        var firstSettlement = settlements.First();
        response.Value.Settlements.First().Id.Should().Be(firstSettlement.Id.Value);
        response.Value.Settlements.First().Name.Should().Be(firstSettlement.Name);
    }
}
