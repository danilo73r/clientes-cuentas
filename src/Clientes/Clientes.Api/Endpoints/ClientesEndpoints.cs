using Clientes.Application.CrearClientes;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Clientes.Api.Endpoints;

public static class ClientesEndpoints
{
    public static IEndpointRouteBuilder MapClientesEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var clientes = endpoints.MapGroup("/clientes").WithTags("Clientes");

        clientes.MapPost("", CrearAsync)
            .WithName("CrearCliente")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status409Conflict);

        clientes.MapGet("/{id:guid}", Obtener)
            .WithName("ObtenerCliente")
            .ProducesProblem(StatusCodes.Status501NotImplemented);

        return endpoints;
    }

    private static async Task<CreatedAtRoute<ClienteCreadoOutputDto>> CrearAsync(
        CrearClienteInputDto solicitud,
        CrearCliente casoDeUso,
        CancellationToken cancellationToken)
    {
        var cliente = await casoDeUso.EjecutarAsync(solicitud, cancellationToken);
        return TypedResults.CreatedAtRoute(
            cliente,
            routeName: "ObtenerCliente",
            routeValues: new { id = cliente.Id });
    }

    private static ProblemHttpResult Obtener(Guid id)
    {
        // TODO: devolver el cliente 
        return TypedResults.Problem(
            statusCode: StatusCodes.Status501NotImplemented,
            title: "pendiente");
    }
}
