using DeliveryApp.Infrastructure.Adapters.Postgre;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Xunit;

namespace DeliveryApp.IntegrationTests.Infrastructure.Repositories;

public abstract class BaseRepositoryTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _container;
    protected ApplicationDbContext DbContext { get; private set; }

    protected BaseRepositoryTests()
    {
        _container = new PostgreSqlBuilder()
            .WithImage("postgres:14.7")
            .WithDatabase("delivery_test_db")
            .WithUsername("test_user")
            .WithPassword("test_password")
            .WithCleanUp(true)
            .Build();
    }

    public virtual async Task InitializeAsync()
    {
        await _container.StartAsync();
        
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseNpgsql(
                _container.GetConnectionString(),
                sqlOptions => sqlOptions.MigrationsAssembly("DeliveryApp.Infrastructure"))
            .Options;

        DbContext = new ApplicationDbContext(options);
        await DbContext.Database.MigrateAsync();
    }

    public async Task DisposeAsync()
    {
        if (DbContext != null)
        {
            await DbContext.Database.EnsureDeletedAsync();
        }
        await _container.DisposeAsync();
    }
} 