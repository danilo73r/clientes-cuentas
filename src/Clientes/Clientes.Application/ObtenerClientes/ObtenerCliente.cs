using Clientes.Application.Contratos;
using Shared.Application.Tiempo;
using Shared.Domain.Exceptions;

namespace Clientes.Application.ObtenerClientes;

public sealed class ObtenerCliente(IClienteRepository repository, Reloj reloj)
{
    public async Task<ClienteOutputDto> EjecutarAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        var cliente = await repository.ObtenerPorIdAsync(id, cancellationToken)
            ?? throw new NoEncontradoException("El cliente no existe");

        return ClienteOutputDto.Desde(cliente, reloj.FechaActualLocal);
    }
}
