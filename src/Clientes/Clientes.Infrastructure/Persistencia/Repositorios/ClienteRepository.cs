using Clientes.Application.Contratos;
using Clientes.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Infrastructure.Persistencia.Repositorios;

public sealed class ClienteRepository(ClientesDbContext context)
    : IClienteRepository
{
    public Task<bool> ExisteIdentificacionAsync(
        string identificacion,
        CancellationToken cancellationToken)
    {
        return context.Clientes.AnyAsync(
            cliente => cliente.Identificacion == identificacion,
            cancellationToken);
    }

    public void Agregar(Cliente cliente)
    {
        context.Clientes.Add(cliente);
    }

}
