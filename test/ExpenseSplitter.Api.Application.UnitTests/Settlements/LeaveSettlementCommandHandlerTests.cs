using ExpenseSplitter.Api.Application.Settlements.LeaveSettlement;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.UnitTests.Settlements;

public class LeaveSettlementCommandHandlerTests
{
    private readonly Fixture fixture;
    private readonly Mock<ISettlementUserRepository> settlementUserRepositoryMock;
    private readonly LeaveSettlementCommandHandler handler;

    public LeaveSettlementCommandHandlerTests()
    {
        fixture = CustomFixture.Create();
        settlementUserRepositoryMock = new Mock<ISettlementUserRepository>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        settlementUserRepositoryMock
            .Setup(x => x.GetBySettlementId(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.Create<SettlementUser>());

        handler = new LeaveSettlementCommandHandler(
            settlementUserRepositoryMock.Object,
            unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenSettlementIdIsEmpty()
    {
        var command = fixture
            .Build<LeaveSettlementCommand>()
            .With(x => x.SettlemetId, Guid.Empty)
            .Create();

        var validator = new LeaveSettlementCommandValidator();
        var result = await validator.ValidateAsync(command);

        result.Errors.Should().Contain(x => x.PropertyName == nameof(command.SettlemetId));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSettlementWithInviteCodeDoesNotExist()
    {
        settlementUserRepositoryMock
            .Setup(x => x.GetBySettlementId(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((SettlementUser) null!);

        var command = fixture.Create<LeaveSettlementCommand>();

        var response = await handler.Handle(command, default);

        response.IsFailure.Should().BeTrue();
        response.AppError.Type.Should().Be(SettlementErrors.Forbidden.Type);
    }

    [Fact]
    public async Task Handle_ShoulSuccess()
    {
        var command = fixture.Create<LeaveSettlementCommand>();

        var response = await handler.Handle(command, default);

        response.IsSuccess.Should().BeTrue();
    }
}
