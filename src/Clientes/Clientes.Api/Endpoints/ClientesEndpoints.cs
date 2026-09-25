using Clientes.Application.CambiarContrasenas;
using Clientes.Application.ActualizarClientes;
using Clientes.Application.ListadoClientes;
using Shared.Application.Paginacion;
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

        clientes.MapPut("/{id:guid}", ActualizarAsync)
            .WithName("ActualizarCliente")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

        clientes.MapGet("/{id:guid}", ObtenerAsync)
            .WithName("ObtenerCliente")
            .ProducesProblem(StatusCodes.Status404NotFound);

        clientes.MapGet("", ListarAsync)
            .WithName("ListarClientes")
            .ProducesProblem(StatusCodes.Status400BadRequest);

        clientes.MapPost("/{id:guid}/contrasena", CambiarContrasenaAsync)
            .WithName("CambiarContrasenaCliente")
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .ProducesProblem(StatusCodes.Status409Conflict);

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

    private static async Task<NoContent> ActualizarAsync(
        Guid id,
        ActualizarClienteInputDto solicitud,
        ActualizarCliente casoDeUso,
        CancellationToken cancellationToken)
    {
        await casoDeUso.EjecutarAsync(id, solicitud, cancellationToken);
        return TypedResults.NoContent();
    }

    private static async Task<Ok<ClienteOutputDto>> ObtenerAsync(
        Guid id,
        ObtenerCliente casoDeUso,
        CancellationToken cancellationToken)
    {
        var cliente = await casoDeUso.EjecutarAsync(id, cancellationToken);
        return TypedResults.Ok(cliente);
    }

    private static async Task<Ok<ResultadoPaginado<ListarClientesOutputDto>>> ListarAsync(
        ListarClientes casoDeUso,
        CancellationToken cancellationToken,
        int pagina = 1,
        int tamanoPagina = 20)
    {
        var solicitud = new ListarClientesInputDto(pagina, tamanoPagina);
        var resultado = await casoDeUso.EjecutarAsync(solicitud, cancellationToken);
        return TypedResults.Ok(resultado);
    }

    private static async Task<NoContent> CambiarContrasenaAsync(
        Guid id,
        CambiarContrasenaClienteInputDto solicitud,
        CambiarContrasenaCliente casoDeUso,
        CancellationToken cancellationToken)
    {
        await casoDeUso.EjecutarAsync(id, solicitud, cancellationToken);
        return TypedResults.NoContent();
    }
}
