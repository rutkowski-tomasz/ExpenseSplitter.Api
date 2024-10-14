using System.Net;
using System.Net.Http.Json;
using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Infrastructure.Authentication.Models;
using Microsoft.Extensions.Options;

namespace ExpenseSplitter.Api.Infrastructure.Authentication;

internal sealed class JwtService(HttpClient client, IOptions<KeycloakOptions> keycloakOptions)
    : IJwtService
{
    private static readonly Error AuthenticationFailed = new(
        ErrorType.Unauthorized,
        "Invalid email or password"
    );

    private static readonly Error BadGateway = new(
        ErrorType.BadGateway,
        "Can't communicate with identity provider"
    );

    private readonly KeycloakOptions keycloakOptions = keycloakOptions.Value;

    public async Task<Result<string>> GetAccessTokenAsync(
        string email,
        string password,
        CancellationToken cancellationToken
    )
    {
        try
        {
            var authRequestParameters = new KeyValuePair<string, string>[]
            {
                new("client_id", keycloakOptions.AuthClientId),
                new("client_secret", keycloakOptions.AuthClientSecret),
                new("scope", "openid email"),
                new("grant_type", "password"),
                new("username", email),
                new("password", password)
            };

            using var authorizationRequestContent = new FormUrlEncodedContent(authRequestParameters);

            var response = await client.PostAsync("", authorizationRequestContent, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                return Result.Failure<string>(AuthenticationFailed);
            }

            response.EnsureSuccessStatusCode();

            var authorizationToken = await response.Content.ReadFromJsonAsync<AuthorizationToken>(cancellationToken: cancellationToken);

            return authorizationToken?.AccessToken ?? Result.Failure<string>(AuthenticationFailed);
        }
        catch (HttpRequestException)
        {
            return Result.Failure<string>(BadGateway);
        }
    }
}
