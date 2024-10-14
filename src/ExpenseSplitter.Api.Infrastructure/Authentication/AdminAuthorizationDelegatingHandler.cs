using System.Net.Http.Headers;
using System.Net.Http.Json;
using ExpenseSplitter.Api.Infrastructure.Authentication.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace ExpenseSplitter.Api.Infrastructure.Authentication;

public sealed class AdminAuthorizationDelegatingHandler : DelegatingHandler
{
    private readonly KeycloakOptions keycloakOptions;

    public AdminAuthorizationDelegatingHandler(IOptions<KeycloakOptions> keycloakOptions)
    {
        this.keycloakOptions = keycloakOptions.Value;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var authorizationToken = await GetAdminToken(cancellationToken);

        request.Headers.Authorization = new AuthenticationHeaderValue(
            JwtBearerDefaults.AuthenticationScheme,
            authorizationToken.AccessToken);

        var httpResponseMessage = await base.SendAsync(request, cancellationToken);

        return httpResponseMessage;
    }

    private async Task<AuthorizationToken> GetAdminToken(CancellationToken cancellationToken)
    {
        var authorizationRequestParameters = new KeyValuePair<string, string>[]
        {
            new("client_id", keycloakOptions.AdminClientId),
            new("client_secret", keycloakOptions.AdminClientSecret),
            new("scope", "openid email"),
            new("grant_type", "client_credentials")
        };

        var authorizationRequestContent = new FormUrlEncodedContent(authorizationRequestParameters);

        using var authorizationRequest = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri(keycloakOptions.TokenUrl)
        );
        authorizationRequest.Content = authorizationRequestContent;

        var authorizationResponse = await base.SendAsync(authorizationRequest, cancellationToken);

        authorizationResponse.EnsureSuccessStatusCode();

        return await authorizationResponse.Content.ReadFromJsonAsync<AuthorizationToken>(cancellationToken) ??
               throw new ApplicationException();
    }
}
