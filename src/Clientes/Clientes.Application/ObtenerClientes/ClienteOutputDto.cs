using Clientes.Domain.Enums;

namespace Clientes.Application.ObtenerClientes;

public sealed record ClienteOutputDto(
    Guid Id,
    string Nombre,
    Genero Genero,
    DateOnly FechaNacimiento,
    int Edad,
    string Identificacion,
    string Direccion,
    string Telefono,
    bool Estado,
    long Version,
    Guid? OperacionPendienteId);
