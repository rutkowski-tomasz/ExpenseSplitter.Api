using System.Net.Http.Json;
using ExpenseSplitter.Api.Application.Abstractions.Authentication;
using ExpenseSplitter.Api.Domain.Abstractions;
using ExpenseSplitter.Api.Infrastructure.Authentication.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ExpenseSplitter.Api.Infrastructure.Authentication;

internal sealed class AuthenticationService(
    HttpClient client,
    ILogger<AuthenticationService> logger,
    IOptions<KeycloakOptions> keycloakOptions
) : IAuthenticationService
{
    private static readonly AppError RegistrationAppError = new(
        ErrorType.BadRequest,
        "An error occurred during user registration"
    );

    private const string PasswordCredentialType = "password";

    private readonly KeycloakOptions keycloakOptions = keycloakOptions.Value;

    public async Task<Result<string>> RegisterAsync(
        string email,
        string password,
        CancellationToken cancellationToken
    )
    {
        var userRepresentationModel = UserRepresentationModel.Create(email);

        userRepresentationModel.Credentials =
        [
            new CredentialRepresentationModel
            {
                Value = password,
                Temporary = false,
                Type = PasswordCredentialType
            }
        ];

        var response = await client.PostAsJsonAsync(
            keycloakOptions.AdminUsersPath,
            userRepresentationModel,
            cancellationToken
        );

        if (response.IsSuccessStatusCode)
        {
            return ExtractIdentityIdFromLocationHeader(response);
        }

        logger.LogWarning(
            "Keycloak returned non-success response {StatusCode} {Content}",
            response.StatusCode,
            await response.Content.ReadAsStringAsync(cancellationToken)
        );

        return Result.Failure<string>(RegistrationAppError);

    }

    private static string ExtractIdentityIdFromLocationHeader(
        HttpResponseMessage httpResponseMessage)
    {
        const string usersSegmentName = "users/";

        var locationHeader = httpResponseMessage.Headers.Location?.PathAndQuery
            ?? throw new InvalidOperationException("Location header can't be null");

        var userSegmentValueIndex = locationHeader.IndexOf(
            usersSegmentName,
            StringComparison.InvariantCultureIgnoreCase);

        var userIdentityId = locationHeader[(userSegmentValueIndex + usersSegmentName.Length)..];

        return userIdentityId;
    }
}
