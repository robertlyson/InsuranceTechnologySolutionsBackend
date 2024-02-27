using Infrastructure.Auditing;
using Microsoft.EntityFrameworkCore;

namespace Claims;

public static class MigrationExtensions
{
    public static void RunMigrations(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuditContext>();
        context.Database.Migrate();
    }
}