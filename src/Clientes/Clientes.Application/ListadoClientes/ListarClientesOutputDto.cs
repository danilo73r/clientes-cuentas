using Clientes.Domain.Entities;
using Clientes.Domain.Enums;

namespace Clientes.Application.ListadoClientes;

public sealed record ListarClientesOutputDto(
    Guid Id,
    string Nombre,
    Genero Genero,
    DateOnly FechaNacimiento,
    string Identificacion,
    string Direccion,
    string Telefono,
    bool Estado)
{
    public static ListarClientesOutputDto Desde(Cliente cliente)
        => new(
            cliente.Id,
            cliente.Nombre,
            cliente.Genero,
            cliente.FechaNacimiento,
            cliente.Identificacion,
            cliente.Direccion,
            cliente.Telefono,
            cliente.Estado);
}
