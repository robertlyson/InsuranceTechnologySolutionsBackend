using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Auditing;

public static class AuditExtensions
{
    public static void AddAuditing(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddScoped<IAuditer, Auditer>();
        serviceCollection.AddHostedService<AuditHostedService>();
    }
}