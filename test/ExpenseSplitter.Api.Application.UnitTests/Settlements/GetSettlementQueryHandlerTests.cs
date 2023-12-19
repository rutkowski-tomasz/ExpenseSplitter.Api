using ExpenseSplitter.Api.Application.Settlements.GetSettlement;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;
using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Application.UnitTests.Settlements;
public class GetSettlementQueryHandlerTests
{
    private readonly Fixture fixture;
    private readonly Mock<ISettlementRepository> settlementRepositoryMock;
    private readonly Mock<ISettlementUserRepository> settlementUserRepositoryMock;
    private readonly GetSettlementQueryHandler handler;

    public GetSettlementQueryHandlerTests()
    {
        fixture = CustomFixutre.Create();
        settlementRepositoryMock = new Mock<ISettlementRepository>();
        settlementUserRepositoryMock = new Mock<ISettlementUserRepository>();
        var participantRepositoryMock = new Mock<IParticipantRepository>();

        handler = new GetSettlementQueryHandler(
            settlementRepositoryMock.Object,
            settlementUserRepositoryMock.Object,
            participantRepositoryMock.Object
        );
    }

    [Fact]
    public async Task Handle_ShouldMapSettlementsToDto()
    {
        var settlement = fixture.Create<Settlement>();

        settlementRepositoryMock
            .Setup(x => x.GetById(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(settlement);

        settlementUserRepositoryMock
            .Setup(x => x.CanUserAccessSettlement(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var query = fixture.Create<GetSettlementQuery>();

        var response = await handler.Handle(query, default);

        response.IsSuccess.Should().BeTrue();
        response.Value.Name.Should().Be(settlement.Name);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSettlementWithTheIdDoesntExist()
    {
        var query = fixture.Create<GetSettlementQuery>();

        settlementUserRepositoryMock
            .Setup(x => x.CanUserAccessSettlement(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var response = await handler.Handle(query, default);

        response.IsFailure.Should().BeTrue();
        response.Error.Code.Should().Be(SettlementErrors.NotFound.Code);
    }
}

