using Clientes.Application.Contratos;
using FluentValidation;
using Shared.Application.Paginacion;
using Shared.Application.Tiempo;

namespace Clientes.Application.ListadoClientes;

public sealed class ListarClientes(
    IClienteRepository repository,
    IValidator<ListarClientesInputDto> validador)
{
    public async Task<ResultadoPaginado<ListarClientesOutputDto>> EjecutarAsync(
        ListarClientesInputDto solicitud,
        CancellationToken cancellationToken)
    {
        await validador.ValidateAndThrowAsync(solicitud, cancellationToken);

        var resultado = await repository.ListarAsync(
            solicitud.Pagina,
            solicitud.TamanoPagina,
            cancellationToken);

        return new ResultadoPaginado<ListarClientesOutputDto>(
            [.. resultado.Items.Select(ListarClientesOutputDto.Desde)],
            resultado.TotalRegistros,
            resultado.Pagina,
            resultado.TamanoPagina);
    }
}
