using Cuentas.Domain.Entities;

namespace Cuentas.Application.Contratos;

public interface IProyeccionClienteRepository
{
    Task<bool> ExisteAsync(
        Guid clienteId,
        CancellationToken cancellationToken);

    Task<ProyeccionCliente?> ObtenerAsync(
        Guid clienteId,
        CancellationToken cancellationToken);
        
    void Agregar(ProyeccionCliente proyeccion);
}
