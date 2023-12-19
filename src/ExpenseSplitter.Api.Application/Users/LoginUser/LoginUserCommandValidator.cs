using System.Diagnostics.CodeAnalysis;
using FluentValidation;

namespace ExpenseSplitter.Api.Application.Users.LoginUser;

[ExcludeFromCodeCoverage]
public sealed class LoginUserCommandValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty();

        RuleFor(x => x.Password).NotEmpty();
    }
}
