using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Identidad;
using Shared.Application.Tiempo;

namespace Shared.Application;

public static class Configuration
{
    public static IServiceCollection AddSharedApplication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        GeneradorGuidConfig(services);
        TimezoneConfig(services, configuration);

        return services;
    }

    private static void GeneradorGuidConfig(IServiceCollection services)
    {
        services.AddSingleton<GeneradorId>();
    }

    private static void TimezoneConfig(IServiceCollection services, IConfiguration configuration)
    {
        var zonaId = configuration["Tiempo:ZonaHoraria"];

        if (string.IsNullOrWhiteSpace(zonaId))
            throw new InvalidOperationException("Falta configurar Tiempo:ZonaHoraria");

        var zonaHoraria = TimeZoneInfo.FindSystemTimeZoneById(zonaId);

        services.AddSingleton(TimeProvider.System);
        services.AddSingleton(provider => new Reloj(
                provider.GetRequiredService<TimeProvider>(),
                zonaHoraria));
    }
}