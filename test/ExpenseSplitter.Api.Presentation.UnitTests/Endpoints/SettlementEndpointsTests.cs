using ExpenseSplitter.Api.Application.Settlements.CreateSettlement;
using ExpenseSplitter.Api.Application.Settlements.GetAllSettlements;
using ExpenseSplitter.Api.Application.Settlements.GetSettlement;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Presentation.Settlements;
using ExpenseSplitter.Api.Presentation.Settlements.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ExpenseSplitter.Api.Presentation.UnitTests.Endpoints;

public class SettlementEndpointsTests
{
    private readonly Mock<ISender> senderMock;

    public SettlementEndpointsTests()
    {
        senderMock = new Mock<ISender>();
    }

    // [Fact]
    // public async Task GetAllSettlements_ShouldReturnOk_WhenSettlementsExist()
    // {
    //     var settlements = new Fixture().Create<GetAllSettlementsQueryResult>();
    //     senderMock
    //         .Setup(x => x.Send(It.IsAny<GetAllSettlementsQuery>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(Result.Success(settlements));

    //     var result = await SettlementEndpoints.GetAllSettlements(senderMock.Object, CancellationToken.None, 1, 20);

    //     result.StatusCode.Should().Be(200);
    //     result.Value!.Settlements.Should().HaveCount(settlements.Settlements.Count());
    // }

    // [Fact]
    // public async Task GetSettlement_ShouldReturnOk_WhenSettlementExists()
    // {
    //     var settlement = new Fixture().Create<GetSettlementQueryResult>();
    //     senderMock
    //         .Setup(x => x.Send(It.IsAny<GetSettlementQuery>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(Result.Success(settlement));

    //     var result = await SettlementEndpoints.GetSettlement(Guid.NewGuid(), senderMock.Object, CancellationToken.None);

    //     var castedResult = result.Result as Ok<GetSettlementQueryResult>;
    //     castedResult!.StatusCode.Should().Be(200);
    //     castedResult.Value!.Id.Should().Be(settlement.Id);
    // }

    // [Fact]
    // public async Task CreateSettlement_ShouldReturnOk_WhenCreationIsSuccessful()
    // {
    //     var settlementId = Guid.NewGuid();
    //     var request = new Fixture().Create<CreateSettlementRequest>();
    //     senderMock
    //         .Setup(x => x.Send(It.IsAny<CreateSettlementCommand>(), It.IsAny<CancellationToken>()))
    //         .ReturnsAsync(Result.Success(settlementId));

    //     var result = await SettlementEndpoints.CreateSettlement(
    //         request,
    //         senderMock.Object,
    //         CancellationToken.None
    //     );

    //     var castedResult = result.Result as Ok<Guid>;
    //     castedResult!.StatusCode.Should().Be(200);
    //     castedResult.Value.Should().Be(settlementId);
    // }
}
