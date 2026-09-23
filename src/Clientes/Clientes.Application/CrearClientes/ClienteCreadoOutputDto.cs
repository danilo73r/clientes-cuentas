namespace Clientes.Application.CrearClientes;

public sealed record ClienteCreadoOutputDto(
    Guid Id,
    int Edad,
    bool Estado,
    long Version,
    Guid? OperacionPendienteId);
