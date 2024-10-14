using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Settlements.JoinSettlement;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.UnitTests.Settlements;

public class JoinSettlementCommandHandlerTests
{
    private readonly Fixture fixture;
    private readonly Mock<ISettlementRepository> settlementRepositoryMock;
    private readonly Mock<ISettlementUserRepository> settlementUserRepositoryMock;
    private readonly JoinSettlementCommandHandler handler;

    public JoinSettlementCommandHandlerTests()
    {
        fixture = CustomFixture.Create();
        settlementRepositoryMock = new Mock<ISettlementRepository>();
        settlementUserRepositoryMock = new Mock<ISettlementUserRepository>();
        var userContextMock = new Mock<IUserContext>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();

        settlementRepositoryMock
            .Setup(x => x.GetByInviteCode(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fixture.Create<Settlement>());

        settlementUserRepositoryMock
            .Setup(x => x.CanUserAccessSettlement(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        handler = new JoinSettlementCommandHandler(
            settlementRepositoryMock.Object,
            settlementUserRepositoryMock.Object,
            userContextMock.Object,
            unitOfWorkMock.Object
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
        settlementRepositoryMock
            .Setup(x => x.GetByInviteCode(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Settlement) null!);

        var command = fixture.Create<JoinSettlementCommand>();

        var response = await handler.Handle(command, default);

        response.IsFailure.Should().BeTrue();
        response.Error.Type.Should().Be(SettlementErrors.NotFound.Type);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserAlreadyJoinedSettlement()
    {
        settlementUserRepositoryMock
            .Setup(x => x.CanUserAccessSettlement(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = fixture.Create<JoinSettlementCommand>();

        var response = await handler.Handle(command, default);

        response.IsFailure.Should().BeTrue();
        response.Error.Type.Should().Be(SettlementUserErrors.AlreadyJoined.Type);
    }

    [Fact]
    public async Task Handle_ShoulSuccess()
    {
        var command = fixture.Create<JoinSettlementCommand>();

        var response = await handler.Handle(command, default);

        response.IsSuccess.Should().BeTrue();
        response.Value.Should().NotBeEmpty();
    }
}
