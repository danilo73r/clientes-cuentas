using Shared.Domain.Exceptions;
using Wolverine.ErrorHandling;
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
            ConfigurarHandlers(options);
            ConfigurarPublicaciones(options);
            ConfigurarReintentos(options);
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

    private static void ConfigurarHandlers(WolverineOptions options)
    {
        var assembly = typeof(Mensajeria.Sagas.CreacionClienteSaga).Assembly;
        options.Discovery.IncludeAssembly(assembly);
    }

    private static void ConfigurarPublicaciones(WolverineOptions options)
    {
        options.PublishMessage<CrearProyeccionClienteCommand>()
            .ToRabbitQueue("cuentas")
            .UseDurableOutbox();
    }

    private static void ConfigurarReintentos(WolverineOptions options)
    {
        // Dos confirmaciones simultáneas: el perdedor reintenta y encuentra la saga completada.
        // Esto solo aplica a mensajes, no a solicitudes HTTP.
        var pausa = TimeSpan.FromMilliseconds(500);
        options.OnException<ConflictoException>()
            .RetryWithCooldown(pausa, pausa, pausa);
    }

}
