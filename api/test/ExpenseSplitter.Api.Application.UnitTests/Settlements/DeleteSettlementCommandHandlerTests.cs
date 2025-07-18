using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Abstractions.Etag;
using ExpenseSplitter.Api.Application.Settlements.DeleteSettlement;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Application.UnitTests.Settlements;

public class DeleteSettlementCommandHandlerTests
{
    private readonly Fixture fixture = CustomFixture.Create();
    private readonly ISettlementRepository settlementRepository = Substitute.For<ISettlementRepository>();
    private readonly IUserContext userContext = Substitute.For<IUserContext>();
    private readonly IUnitOfWork unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IEtagService etagService = Substitute.For<IEtagService>();
    private readonly DeleteSettlementCommandHandler handler;
    private readonly Settlement settlement;

    public DeleteSettlementCommandHandlerTests()
    {
        settlement = fixture.Create<Settlement>();
        userContext.UserId.Returns(settlement.CreatorUserId);

        settlementRepository
            .GetById(settlement.Id, Arg.Any<CancellationToken>())
            .Returns(settlement);

        handler = new DeleteSettlementCommandHandler(
            settlementRepository,
            userContext,
            unitOfWork,
            etagService
        );
    }

    [Fact]
    public async Task Validate_ShouldFail_WhenSettlementIdIsEmpty()
    {
        var command = fixture
            .Build<DeleteSettlementCommand>()
            .With(x => x.Id, Guid.Empty)
            .Create();

        var validator = new DeleteSettlementCommandValidator();
        var result = await validator.ValidateAsync(command);

        result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Id));
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenSettlementWitIdDoesNotExist()
    {
        settlementRepository
            .GetById(settlement.Id, Arg.Any<CancellationToken>())
            .Returns((Settlement) default);

        var command = BuildCommand();

        var response = await handler.Handle(command, default);

        response.IsFailure.Should().BeTrue();
        response.AppError.Type.Should().Be(SettlementErrors.NotFound.Type);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserIsNotSettlementCreator()
    {
        userContext.UserId.Returns(UserId.New());

        var command = BuildCommand();

        var response = await handler.Handle(command, default);

        response.IsFailure.Should().BeTrue();
        response.AppError.Type.Should().Be(SettlementErrors.Forbidden.Type);
    }

    [Fact]
    public async Task Handle_ShouldSuccess()
    {
        var command = BuildCommand();

        var response = await handler.Handle(command, default);

        response.IsSuccess.Should().BeTrue();
    }

    private DeleteSettlementCommand BuildCommand()
    {
        return fixture
            .Build<DeleteSettlementCommand>()
            .With(x => x.Id, settlement.Id.Value)
            .Create();
    }
}
