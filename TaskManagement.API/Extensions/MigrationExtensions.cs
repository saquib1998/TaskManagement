using Microsoft.EntityFrameworkCore;
using TaskManagement.API.Data;

namespace TaskManagement.API.Extensions
{
    public static class MigrationExtensions
    {
        public static void ApplyMigrations(this IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();

            using var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            context.Database.Migrate();
        }
    }
}
