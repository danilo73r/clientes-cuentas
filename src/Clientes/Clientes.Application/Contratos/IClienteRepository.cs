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
        CancellationToken cancellationToken,
        bool asNoTracking = false);

    Task<ResultadoPaginado<Cliente>> ListarAsync(
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken);

    Task<bool> ExisteIdentificacionEnOtroClienteAsync(
        Guid id,
        string identificacion,
        CancellationToken cancellationToken);

    void Agregar(Cliente cliente);
}
