using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Abstractions.Clock;
using ExpenseSplitter.Api.Application.Settlements.CreateSettlement;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Participants;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.UnitTests.Settlements;

public class CreateSettlementCommandHandlerTests
{
    private readonly Fixture fixture;
    private readonly Mock<ISettlementRepository> settlementRepositoryMock;
    private readonly CreateSettlementCommandHandler handler;

    public CreateSettlementCommandHandlerTests()
    {
        fixture = CustomFixture.Create();
        settlementRepositoryMock = new Mock<ISettlementRepository>();
        Mock<IParticipantRepository> participantRepositoryMock = new();
        Mock<ISettlementUserRepository> settlementUserRepositoryMock = new();
        var userContextMock = new Mock<IUserContext>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var inviteCodeServiceMock = new Mock<IInviteCodeService>();
        var dateTimeProviderMock = new Mock<IDateTimeProvider>();

        handler = new CreateSettlementCommandHandler(
            settlementRepositoryMock.Object,
            participantRepositoryMock.Object,
            settlementUserRepositoryMock.Object,
            userContextMock.Object,
            inviteCodeServiceMock.Object,
            dateTimeProviderMock.Object,
            unitOfWorkMock.Object
        );
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenNameIsEmpty()
    {
        var command = fixture
            .Build<CreateSettlementCommand>()
            .With(x => x.Name, string.Empty)
            .Create();

        var validator = new CreateSettlementCommandValidator();
        var result = await validator.ValidateAsync(command);

        result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Name));
    }

    [Fact]
    public async Task Handle_ShouldAddNewSettlementToRepository()
    {
        var command = fixture.Create<CreateSettlementCommand>();

        var response = await handler.Handle(command, default);

        response.IsSuccess.Should().BeTrue();
        response.Value.Should().NotBeEmpty();

        settlementRepositoryMock.Verify(x => x.Add(It.Is<Settlement>(y => y.Name == command.Name)), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenNameIsEmpty()
    {
        var command = fixture
            .Build<CreateSettlementCommand>()
            .With(x => x.Name, string.Empty)
            .Create();

        var response = await handler.Handle(command, default);

        response.IsFailure.Should().BeTrue();
        response.AppError.Type.Should().Be(SettlementErrors.EmptyName.Type);

        settlementRepositoryMock.Verify(x => x.Add(It.Is<Settlement>(y => y.Name == command.Name)), Times.Never);
    }
}

