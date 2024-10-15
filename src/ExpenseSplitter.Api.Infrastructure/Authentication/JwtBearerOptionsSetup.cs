using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace ExpenseSplitter.Api.Infrastructure.Authentication;

internal sealed class JwtBearerOptionsSetup(IOptions<AuthenticationOptions> authenticationOptions)
    : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly AuthenticationOptions authenticationOptions = authenticationOptions.Value;

    public void Configure(JwtBearerOptions options)
    {
        options.Audience = authenticationOptions.Audience;
        options.MetadataAddress = $"{authenticationOptions.BaseUrl}/{authenticationOptions.MetadataUrlPath}";
        options.RequireHttpsMetadata = authenticationOptions.RequireHttpsMetadata;
        options.TokenValidationParameters.ValidIssuer = $"{authenticationOptions.BaseUrl}/{authenticationOptions.ValidIssuerPath}";
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }
}
