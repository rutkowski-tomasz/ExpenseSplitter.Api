using ExpenseSplitter.Application.Settlements.CreateSettlement;
using ExpenseSplitter.Application.Settlements.GetAllSettlements;
using ExpenseSplitter.Application.Settlements.GetSettlement;
using ExpenseSplitter.Domain.Abstractions;
using ExpenseSplitter.Presentation.Api.Endpoints;
using ExpenseSplitter.Presentation.Api.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ExpenseSplitter.Presentation.Api.Tests.Endpoints;

public class SettlementEndpointsTests
{
    private readonly Mock<ISender> senderMock;

    public SettlementEndpointsTests()
    {
        senderMock = new Mock<ISender>();
    }

    [Fact]
    public async Task GetAllSettlements_ShouldReturnOk_WhenSettlementsExist()
    {
        var settlements = new Fixture().CreateMany<GetSettlementResponse>(2);
        senderMock
            .Setup(x => x.Send(It.IsAny<GetAllSettlementsQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(settlements));

        var result = await SettlementEndpoints.GetAllSettlements(senderMock.Object, CancellationToken.None);

        var castedResult = result.Result as Ok<IEnumerable<GetSettlementResponse>>;
        castedResult!.StatusCode.Should().Be(200);
        castedResult.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetSettlement_ShouldReturnOk_WhenSettlementExists()
    {
        var settlement = new Fixture().Create<GetSettlementResponse>();
        senderMock
            .Setup(x => x.Send(It.IsAny<GetSettlementQuery>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(settlement));

        var result = await SettlementEndpoints.GetSettlement(Guid.NewGuid(), senderMock.Object, CancellationToken.None);

        var castedResult = result.Result as Ok<GetSettlementResponse>;
        castedResult!.StatusCode.Should().Be(200);
        castedResult.Value!.Id.Should().Be(settlement.Id);
    }

    [Fact]
    public async Task CreateSettlement_ShouldReturnOk_WhenCreationIsSuccessful()
    {
        var settlementId = Guid.NewGuid();
        var request = new Fixture().Create<CreateSettlementRequest>();
        senderMock
            .Setup(x => x.Send(It.IsAny<CreateSettlementCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result.Success(settlementId));

        var result = await SettlementEndpoints.CreateSettlement(request, senderMock.Object, CancellationToken.None);

        var castedResult = result.Result as Ok<Guid>;
        castedResult!.StatusCode.Should().Be(200);
        castedResult.Value!.Should().Be(settlementId);
    }
}
