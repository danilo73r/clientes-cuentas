using Clientes.Application.ObtenerClientes;
using FluentValidation;
using Clientes.Application.CrearClientes;
using Microsoft.Extensions.DependencyInjection;

namespace Clientes.Application;

public static class Configuration
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        ObtenerClienteUseCase(services);
        CrearClienteUseCase(services);
        return services;
    }

    private static void ObtenerClienteUseCase(IServiceCollection services)
    {
        services.AddTransient<ObtenerCliente>();
    }

    private static void CrearClienteUseCase(IServiceCollection services)
    {
        services.AddTransient<CrearCliente>();
        services.AddScoped<IValidator<CrearClienteInputDto>, CrearClienteValidator>();
    }

}
