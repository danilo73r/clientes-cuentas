using Clientes.Domain.Entities;

namespace Clientes.Application.Contratos;

public interface ICreacionClienteSagaRepository
{
    void Agregar(Cliente cliente);
}
