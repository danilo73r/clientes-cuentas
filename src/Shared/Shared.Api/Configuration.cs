using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Routing;
using Shared.Api.Exceptions;

namespace Shared.Api;

public static class Configuration
{
    public static IServiceCollection AddSharedApi(
        this IServiceCollection services)
    {
        ExceptionHandlerConfig(services);
        EnumSerializationConfig(services);

        return services;
    }

    private static void ExceptionHandlerConfig(IServiceCollection services)
    {
        services.Configure<RouteHandlerOptions>(options
            => options.ThrowOnBadRequest = true);
            
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
    }

    private static void EnumSerializationConfig(IServiceCollection services)
    {
        services.ConfigureHttpJsonOptions(options =>
        {
            options.SerializerOptions.Converters.Add(
                new JsonStringEnumConverter(allowIntegerValues: false));
        });
    }
}