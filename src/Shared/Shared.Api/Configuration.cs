using Microsoft.Extensions.DependencyInjection;
using Shared.Api.Exceptions;

namespace Shared.Api;

public static class Configuration
{
    public static IServiceCollection AddSharedApi(
        this IServiceCollection services)
    {
        ExceptionHandlerConfig(services);

        return services;
    }

    private static void ExceptionHandlerConfig(IServiceCollection services)
    {
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
    }

}