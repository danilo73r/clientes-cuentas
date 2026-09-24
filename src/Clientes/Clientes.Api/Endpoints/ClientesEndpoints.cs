using Clientes.Application.ObtenerClientes;
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

        clientes.MapGet("/{id:guid}", ObtenerAsync)
            .WithName("ObtenerCliente")
            .ProducesProblem(StatusCodes.Status404NotFound);

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

    private static async Task<Ok<ClienteOutputDto>> ObtenerAsync(
        Guid id,
        ObtenerCliente casoDeUso,
        CancellationToken cancellationToken)
    {
        var cliente = await casoDeUso.EjecutarAsync(id, cancellationToken);
        return TypedResults.Ok(cliente);
    }
}
