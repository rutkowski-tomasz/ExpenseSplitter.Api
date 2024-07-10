using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Application.Abstractions.Etag;
using ExpenseSplitter.Api.Application.Settlements.DeleteSettlement;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Domain.Settlements;
using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Application.UnitTests.Settlements;

public class DeleteSettlementCommandHandlerTests
{
    private readonly Fixture fixture;
    private readonly Mock<ISettlementRepository> settlementRepositoryMock;
    private readonly Mock<IUserContext> userContext;
    private readonly DeleteSettlementCommandHandler handler;

    public DeleteSettlementCommandHandlerTests()
    {
        fixture = CustomFixutre.Create();
        settlementRepositoryMock = new Mock<ISettlementRepository>();
        userContext = new Mock<IUserContext>();
        var unitOfWorkMock = new Mock<IUnitOfWork>();
        var etagServiceMock = new Mock<IEtagService>();

        var settlement = fixture.Create<Settlement>();
        userContext.Setup(x => x.UserId).Returns(settlement.CreatorUserId);

        settlementRepositoryMock
            .Setup(x => x.GetById(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(settlement);

        handler = new DeleteSettlementCommandHandler(
            settlementRepositoryMock.Object,
            userContext.Object,
            unitOfWorkMock.Object,
            etagServiceMock.Object
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
        settlementRepositoryMock
            .Setup(x => x.GetById(It.IsAny<SettlementId>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Settlement) null!);

        var command = fixture.Create<DeleteSettlementCommand>();

        var response = await handler.Handle(command, default);

        response.IsFailure.Should().BeTrue();
        response.Error.Type.Should().Be(SettlementErrors.NotFound.Type);
    }

    [Fact]
    public async Task Handle_ShouldFail_WhenUserIsNotSettlementCreator()
    {
        userContext.Setup(x => x.UserId).Returns(UserId.New());

        var command = fixture.Create<DeleteSettlementCommand>();

        var response = await handler.Handle(command, default);

        response.IsFailure.Should().BeTrue();
        response.Error.Type.Should().Be(SettlementErrors.Forbidden.Type);
    }

    [Fact]
    public async Task Handle_ShoulSuccess()
    {
        var command = fixture.Create<DeleteSettlementCommand>();

        var response = await handler.Handle(command, default);

        response.IsSuccess.Should().BeTrue();
    }
}
