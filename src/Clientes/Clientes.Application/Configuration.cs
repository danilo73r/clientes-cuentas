using Clientes.Application.CambiarContrasenas;
using Clientes.Application.ActualizarClientes;
using Clientes.Application.ListadoClientes;
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
        CrearClienteUseCase(services);
        ActualizarClienteUseCase(services);
        ObtenerClienteUseCase(services);
        ListarClientesUseCase(services);
        CambiarContrasenaClienteUseCase(services);
        return services;
    }

    private static void CrearClienteUseCase(IServiceCollection services)
    {
        services.AddTransient<CrearCliente>();
        services.AddScoped<IValidator<CrearClienteInputDto>, CrearClienteValidator>();
    }

    private static void ActualizarClienteUseCase(IServiceCollection services)
    {
        services.AddTransient<ActualizarCliente>();
        services.AddScoped<IValidator<ActualizarClienteInputDto>, ActualizarClienteValidator>();
    }

    private static void ObtenerClienteUseCase(IServiceCollection services)
    {
        services.AddTransient<ObtenerCliente>();
    }

    private static void ListarClientesUseCase(IServiceCollection services)
    {
        services.AddTransient<ListarClientes>();
        services.AddScoped<IValidator<ListarClientesInputDto>, ListarClientesValidator>();
    }
    
    private static void CambiarContrasenaClienteUseCase(IServiceCollection services)
    {
        services.AddTransient<CambiarContrasenaCliente>();
        services.AddScoped<IValidator<CambiarContrasenaClienteInputDto>, CambiarContrasenaClienteValidator>();
    }

}
