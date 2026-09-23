using Shared.Application.Persistencia;
using Clientes.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Clientes.Infrastructure;

public static class ServicesConfiguration
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        DatabaseConfig(services, configuration);

        return services;
    }

    private static void DatabaseConfig(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Falta configurar ConnectionStrings:Database");

        services.AddDbContext<ClientesDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<IUnitOfWork>(provider =>
            provider.GetRequiredService<ClientesDbContext>());
    }

}
