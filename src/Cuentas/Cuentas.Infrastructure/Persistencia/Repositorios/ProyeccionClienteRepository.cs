using Cuentas.Application.Contratos;
using Cuentas.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Cuentas.Infrastructure.Persistencia.Repositorios;

public sealed class ProyeccionClienteRepository(CuentasDbContext dbContext)
    : IProyeccionClienteRepository
{
    public Task<bool> ExisteAsync(
        Guid clienteId,
        CancellationToken cancellationToken)
    {
        return dbContext.ProyeccionesClientes.AnyAsync(
            cliente => cliente.ClienteId == clienteId,
            cancellationToken);
    }

    public Task<ProyeccionCliente?> ObtenerAsync(
        Guid clienteId,
        CancellationToken cancellationToken)
    {
        return dbContext.ProyeccionesClientes.SingleOrDefaultAsync(
            cliente => cliente.ClienteId == clienteId,
            cancellationToken);
    }


    public void Agregar(ProyeccionCliente proyeccion)
        => dbContext.ProyeccionesClientes.Add(proyeccion);
}
