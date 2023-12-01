using ExpenseSplitter.Api.Domain.Users;

namespace ExpenseSplitter.Api.Application.Abstractions.Authentication;

public interface IUserContext
{
    UserId UserId { get; }
}