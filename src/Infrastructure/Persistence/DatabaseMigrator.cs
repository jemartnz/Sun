using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence;

public static class DatabaseMigrator
{
    public static async Task ApplyMigrationsAsync(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<AppDbContext>>();

        try
        {
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            var pending = pendingMigrations.ToList();

            if (pending.Count > 0)
            {
                logger.LogInformation(
                    "Aplicando {Count} migración(es) pendiente(s): {Migrations}",
                    pending.Count,
                    string.Join(", ", pending));

                await context.Database.MigrateAsync();

                logger.LogInformation("Migraciones aplicadas correctamente.");
            }
            else
            {
                logger.LogInformation("La base de datos está actualizada. No hay migraciones pendientes.");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error al aplicar migraciones. La API no puede iniciar sin base de datos.");
            throw;
        }
    }
}
