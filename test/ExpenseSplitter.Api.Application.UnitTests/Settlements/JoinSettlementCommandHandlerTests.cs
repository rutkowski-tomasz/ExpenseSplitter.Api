using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Settlements.JoinSettlement;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.UnitTests.Settlements;

public class JoinSettlementCommandHandlerTests
{
    private readonly Fixture fixture = CustomFixture.Create();

    private readonly ISettlementRepository settlementRepository = Substitute.For<ISettlementRepository>();
    private readonly ISettlementUserRepository settlementUserRepository = Substitute.For<ISettlementUserRepository>();
    private readonly IUserContext userContext = Substitute.For<IUserContext>();
    private readonly IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly JoinSettlementCommandHandler handler;

    public JoinSettlementCommandHandlerTests()
    {
        settlementRepository
            .GetByInviteCode(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(fixture.Create<Settlement>());

        settlementUserRepository
            .CanUserAccessSettlement(Arg.Any<SettlementId>(), Arg.Any<CancellationToken>())
            .Returns(false);

        handler = new JoinSettlementCommandHandler(
            settlementRepository,
            settlementUserRepository,
            userContext,
            unitOfWork
        );
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenInviteCodeIsEmpty()
    {
        var command = fixture
            .Build<JoinSettlementCommand>()
            .With(x => x.InviteCode, string.Empty)
            .Create();

        var validator = new JoinSettlementCommandValidator();
        var result = await validator.ValidateAsync(command);

        result.Errors.Should().Contain(x => x.PropertyName == nameof(command.InviteCode));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSettlementWithInviteCodeDoesNotExist()
    {
        settlementRepository
            .GetByInviteCode(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns((Settlement) default);

        var command = fixture.Create<JoinSettlementCommand>();

        var response = await handler.Handle(command, default);

        response.IsFailure.Should().BeTrue();
        response.AppError.Type.Should().Be(SettlementErrors.NotFound.Type);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserAlreadyJoinedSettlement()
    {
        settlementUserRepository
            .CanUserAccessSettlement(Arg.Any<SettlementId>(), Arg.Any<CancellationToken>())
            .Returns(true);

        var command = fixture.Create<JoinSettlementCommand>();

        var response = await handler.Handle(command, default);

        response.IsFailure.Should().BeTrue();
        response.AppError.Type.Should().Be(SettlementUserErrors.AlreadyJoined.Type);
    }

    [Fact]
    public async Task Handle_ShouldSuccess()
    {
        var command = fixture.Create<JoinSettlementCommand>();

        var response = await handler.Handle(command, default);

        response.IsSuccess.Should().BeTrue();
        response.Value.Should().NotBeEmpty();
    }
}
