using Logitun.Core.Entities;
using Logitun.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

public static class DbSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var rolesToSeed = new[] { "ROLE_DRIVER", "ROLE_ADMIN" };

        foreach (var roleName in rolesToSeed)
        {
            if (!await context.AuthRoles.AnyAsync(r => r.Name == roleName))
            {
                context.AuthRoles.Add(new Role { Name = roleName });
            }
        }

        await context.SaveChangesAsync();
    }
}
