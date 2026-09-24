using FluentValidation;
using Shared.Application.Mensajeria.Contratos;
using Cuentas.Application.ProyeccionesClientes;
using Microsoft.Extensions.DependencyInjection;

namespace Cuentas.Application;

public static class Configuration
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        CrearProyeccionClienteUseCase(services);
        return services;
    }

    private static void CrearProyeccionClienteUseCase(IServiceCollection services)
    {
        services.AddScoped<CrearProyeccionCliente>();
        services.AddScoped<IValidator<CrearProyeccionClienteCommand>, CrearProyeccionClienteValidator>();
    }


}
