using JasperFx;
using Shared.Application.Mensajeria.Contratos;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;
using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.RabbitMQ;
using Microsoft.Extensions.Hosting;

namespace Clientes.Infrastructure;

public static class HostConfiguration
{
    public static IHostBuilder AddInfrastructure(
        this IHostBuilder host,
        IConfiguration configuration)
    {
        MessagingConfig(host, configuration);

        return host;
    }

    private static void MessagingConfig(IHostBuilder host, IConfiguration configuration)
    {
        host.UseWolverine(options =>
        {
            ConfigurarTransporte(options, configuration);
            ConfigurarPersistencia(options, configuration);
            ConfigurarPublicaciones(options);
        });
    }

    private static void ConfigurarTransporte(WolverineOptions options, IConfiguration configuration)
    {
        var rabbitMQConnectionString = configuration.GetConnectionString("RabbitMQ");

        if (string.IsNullOrWhiteSpace(rabbitMQConnectionString))
            throw new InvalidOperationException("Falta configurar ConnectionStrings:RabbitMQ");

        options.UseRabbitMq(new Uri(rabbitMQConnectionString)).AutoProvision();
        options.ListenToRabbitQueue("clientes");
    }

    private static void ConfigurarPersistencia(WolverineOptions options, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");
        
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Falta configurar ConnectionStrings:Database");

        options.PersistMessagesWithPostgresql(connectionString, "wolverine");
        options.AutoBuildMessageStorageOnStartup = AutoCreate.None;
        options.UseEntityFrameworkCoreTransactions();
    }

    private static void ConfigurarPublicaciones(WolverineOptions options)
    {
        options.PublishMessage<CrearProyeccionClienteCommand>()
            .ToRabbitQueue("cuentas")
            .UseDurableOutbox();
    }
}
