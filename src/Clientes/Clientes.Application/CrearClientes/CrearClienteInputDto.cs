using Clientes.Domain.Enums;

namespace Clientes.Application.CrearClientes;

public sealed record CrearClienteInputDto(
    string Nombre,
    Genero Genero,
    DateOnly FechaNacimiento,
    string Identificacion,
    string Direccion,
    string Telefono,
    string Contrasena);
