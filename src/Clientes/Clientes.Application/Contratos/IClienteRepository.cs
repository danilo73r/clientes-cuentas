using Clientes.Domain.Entities;

namespace Clientes.Application.Contratos;

public interface IClienteRepository
{
    Task<bool> ExisteIdentificacionAsync(
        string identificacion,
        CancellationToken cancellationToken);

    void Agregar(Cliente cliente);
}
