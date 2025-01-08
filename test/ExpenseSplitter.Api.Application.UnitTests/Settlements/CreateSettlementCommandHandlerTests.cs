using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Abstractions.Clock;
using ExpenseSplitter.Api.Application.Settlements.CreateSettlement;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.SettlementUsers;

namespace ExpenseSplitter.Api.Application.UnitTests.Settlements;

public class CreateSettlementCommandHandlerTests
{
    private readonly Fixture fixture = CustomFixture.Create();
    private readonly ISettlementRepository settlementRepository = Substitute.For<ISettlementRepository>();
    private readonly ISettlementUserRepository settlementUserRepository = Substitute.For<ISettlementUserRepository>();
    private readonly IUserContext userContext = Substitute.For<IUserContext>();
    private readonly IInviteCodeService inviteCodeService = Substitute.For<IInviteCodeService>();
    private readonly IDateTimeProvider dateTimeProvider = Substitute.For<IDateTimeProvider>();
    private readonly IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
    
    private readonly CreateSettlementCommandHandler handler;

    public CreateSettlementCommandHandlerTests()
    {
        handler = new CreateSettlementCommandHandler(
            settlementRepository,
            settlementUserRepository,
            userContext,
            inviteCodeService,
            dateTimeProvider,
            unitOfWork
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

        settlementRepository
            .Received(1)
            .Add(Arg.Is<Settlement>(y => y.Name == command.Name));
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

        settlementRepository
            .DidNotReceive()
            .Add(Arg.Is<Settlement>(y => y.Name == command.Name));
    }
}

