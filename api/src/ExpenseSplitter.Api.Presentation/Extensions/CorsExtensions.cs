namespace ExpenseSplitter.Api.Presentation.Extensions;

public static class CorsExtensions
{
    public static void AddCorsPolicy(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalhost8080", policy =>
            {
                policy
                    .WithOrigins("http://localhost:8080", "https://localhost:8080")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });
    }
} 
