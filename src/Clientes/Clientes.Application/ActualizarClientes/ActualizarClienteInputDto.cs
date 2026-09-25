using Clientes.Domain.Enums;

namespace Clientes.Application.ActualizarClientes;

public sealed record ActualizarClienteInputDto(
    string Nombre,
    Genero Genero,
    DateOnly FechaNacimiento,
    string Identificacion,
    string Direccion,
    string Telefono,
    long Version);
