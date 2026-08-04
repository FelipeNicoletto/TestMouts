using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.WebApi;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration;

public class ApiWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    public readonly PostgreSqlContainer DbContainer;

    public ApiWebApplicationFactory()
    {
        DbContainer = new PostgreSqlBuilder("postgres:13")
            .Build();
    }

    public async Task InitializeAsync()
    {
        await DbContainer.StartAsync();

        Environment.SetEnvironmentVariable("ConnectionStrings:DefaultConnection", DbContainer.GetConnectionString());

        await InitDb();
    }

    private async Task InitDb()
    {
        var context = new DefaultContext(
            new DbContextOptionsBuilder<DefaultContext>()
                .UseNpgsql(DbContainer.GetConnectionString()).Options);

        await context.Database.MigrateAsync();
    }

    async Task IAsyncLifetime.DisposeAsync()
    {
        await DbContainer.DisposeAsync();
    }
}
