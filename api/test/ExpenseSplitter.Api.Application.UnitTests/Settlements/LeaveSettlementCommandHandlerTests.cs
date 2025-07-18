using ExpenseSplitter.Api.Application.Settlements.LeaveSettlement;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.UnitTests.Settlements;

public class LeaveSettlementCommandHandlerTests
{
    private readonly Fixture fixture = CustomFixture.Create();
    private readonly ISettlementUserRepository settlementUserRepository = Substitute.For<ISettlementUserRepository>();
    private readonly IUnitOfWork unitOfWorkMock = Substitute.For<IUnitOfWork>();
    private readonly LeaveSettlementCommandHandler handler;

    public LeaveSettlementCommandHandlerTests()
    {
        settlementUserRepository
            .GetBySettlementId(Arg.Any<SettlementId>(), Arg.Any<CancellationToken>())
            .Returns(fixture.Create<SettlementUser>());

        handler = new LeaveSettlementCommandHandler(
            settlementUserRepository,
            unitOfWorkMock
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
        settlementUserRepository
            .GetBySettlementId(Arg.Any<SettlementId>(), Arg.Any<CancellationToken>())
            .Returns((SettlementUser) default);

        var command = fixture.Create<LeaveSettlementCommand>();

        var response = await handler.Handle(command, default);

        response.IsFailure.Should().BeTrue();
        response.AppError.Type.Should().Be(SettlementErrors.Forbidden.Type);
    }

    [Fact]
    public async Task Handle_ShouldSuccess()
    {
        var command = fixture.Create<LeaveSettlementCommand>();

        var response = await handler.Handle(command, default);

        response.IsSuccess.Should().BeTrue();
    }
}
