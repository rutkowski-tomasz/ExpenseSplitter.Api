using System.Diagnostics.CodeAnalysis;
using ExpenseSplitter.Api.Application.Users.GetLoggedInUser;
using ExpenseSplitter.Api.Application.Users.LoginUser;
using ExpenseSplitter.Api.Application.Users.RegisterUser;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Presentation.User.Models;
using MediatR;
using Microsoft.AspNetCore.Http.HttpResults;

namespace ExpenseSplitter.Api.Presentation.User;

public static class UserEndpoints
{
    [ExcludeFromCodeCoverage]
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder builder)
    {
        var routeGroupBuilder = builder.MapGroup("user");

        routeGroupBuilder.MapPost("register", Register).AllowAnonymous();

        routeGroupBuilder.MapPost("login", Login).AllowAnonymous();

        routeGroupBuilder.MapGet("me", GetLoggedInUser).RequireAuthorization();

        return builder;
    }

    public static async Task<Results<Ok<Guid>, BadRequest>> Register(
        RegisterUserRequest request,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var command = new RegisterUserCommand(
            request.Email,
            request.Nickname,
            request.Password
        );

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.BadRequest();
    }

    public static async Task<Results<Ok<LoginUserResult>, BadRequest<Error>>> Login(
        LoginUserRequest request,
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var command = new LoginUserCommand(request.Email, request.Password);

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.BadRequest(result.Error);
    }
    
    public static async Task<Results<Ok<GetLoggedInUserQueryResult>, BadRequest>> GetLoggedInUser(
        ISender sender,
        CancellationToken cancellationToken
    )
    {
        var command = new GetLoggedInUserQuery();

        var result = await sender.Send(command, cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.Value) : TypedResults.BadRequest();
    }
}
