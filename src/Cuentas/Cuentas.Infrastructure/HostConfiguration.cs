using Wolverine.ErrorHandling;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using JasperFx;
using Shared.Application.Mensajeria.Contratos;
using Wolverine.EntityFrameworkCore;
using Wolverine.Postgresql;
using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.RabbitMQ;
using Microsoft.Extensions.Hosting;

namespace Cuentas.Infrastructure;

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
            ConfigurarHandlers(options);
            ConfigurarTransporte(options, configuration);
            ConfigurarPersistencia(options, configuration);
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
        options.ListenToRabbitQueue("cuentas");
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
        var assembly = typeof(Mensajeria.CrearProyeccionClienteHandler).Assembly;
        options.Discovery.IncludeAssembly(assembly);
    }

    private static void ConfigurarPublicaciones(WolverineOptions options)
    {
        options.PublishMessage<ProyeccionClienteCreadaEvent>()
            .ToRabbitQueue("clientes")
            .UseDurableOutbox();
    }

    private static void ConfigurarReintentos(WolverineOptions options)
    {
        options.OnException<FluentValidation.ValidationException>()
            .MoveToErrorQueue();

        // si 2 mensajes leen la version 1 (por ejemplo), uno funciona y otro
        // falla, el reintento permite al que fallo tomar la ultima version
        // pero ya solo por lo que no habrá error, a menos que su version sea menor. 
        var pausa = TimeSpan.FromMilliseconds(500);
        options.OnException<DbUpdateConcurrencyException>()
            .RetryWithCooldown(pausa, pausa, pausa);

        // si 2 mensajes con el mismo clientId intentan crear la proyeccion
        // una funciona y otra falla (unique), el reintento le permite saber
        // al que falló que ya ha sido creado y así ser ignorado gracefully
        options.OnException<DbUpdateException>(exception => 
                exception.InnerException is PostgresException
                {
                    SqlState: PostgresErrorCodes.UniqueViolation,
                    ConstraintName: "PK_ProyeccionesClientes"
                })
            .RetryWithCooldown(pausa, pausa, pausa);
    }
}
