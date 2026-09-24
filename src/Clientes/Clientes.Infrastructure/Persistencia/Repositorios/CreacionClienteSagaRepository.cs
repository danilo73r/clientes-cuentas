using Clientes.Application.Contratos;
using Clientes.Domain.Entities;
using Clientes.Infrastructure.Mensajeria.Sagas;

namespace Clientes.Infrastructure.Persistencia.Repositorios;

public sealed class CreacionClienteSagaRepository(ClientesDbContext dbContext)
    : ICreacionClienteSagaRepository
{
    public void Agregar(Cliente cliente)
        => dbContext.CreacionesClientes.Add(new CreacionClienteSaga(cliente));
}
