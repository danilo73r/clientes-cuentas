using Clientes.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using Wolverine.RabbitMQ;

namespace Clientes.Infrastructure;

public static class Configuration
{
    public static void ConfigurarMensajeria(
        this WolverineOptions options,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("RabbitMQ");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Falta configurar ConnectionStrings:RabbitMQ");

        options.UseRabbitMq(new Uri(connectionString)).AutoProvision();
        options.ListenToRabbitQueue("clientes");
    }

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
    }


}