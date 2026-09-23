using FluentValidation;
using Clientes.Application.CrearClientes;
using Microsoft.Extensions.DependencyInjection;

namespace Clientes.Application;

public static class Configuration
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        CrearClienteUseCase(services);
        return services;
    }

    private static void CrearClienteUseCase(IServiceCollection services)
    {
        services.AddTransient<CrearCliente>();
        services.AddScoped<IValidator<CrearClienteInputDto>, CrearClienteValidator>();
    }

}
