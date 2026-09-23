using Microsoft.EntityFrameworkCore;
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
        var connectionString = configuration.GetConnectionString("RabbitMQ");

        if (string.IsNullOrWhiteSpace(connectionString))
            throw new InvalidOperationException("Falta configurar ConnectionStrings:RabbitMQ");

        host.UseWolverine(options =>
        {
            options.UseRabbitMq(new Uri(connectionString)).AutoProvision();
            options.ListenToRabbitQueue("clientes");
        });
    }
}