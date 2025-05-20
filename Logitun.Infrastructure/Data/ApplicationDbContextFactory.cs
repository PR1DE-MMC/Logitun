using DotNetEnv;
using Logitun.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;

namespace Logitun.Infrastructure.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var envPath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory())!.FullName, ".env");
            Env.Load(envPath);

            var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException("Connection string not found in environment variables.");
            }

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseNpgsql(connectionString, b => b.MigrationsAssembly("Logitun.Core"));

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
