using Shared.Application.Paginacion;
using Clientes.Domain.Entities;

namespace Clientes.Application.Contratos;

public interface IClienteRepository
{
    Task<bool> ExisteIdentificacionAsync(
        string identificacion,
        CancellationToken cancellationToken);

    Task<Cliente?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<ResultadoPaginado<Cliente>> ListarAsync(
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken);

    void Agregar(Cliente cliente);
}
