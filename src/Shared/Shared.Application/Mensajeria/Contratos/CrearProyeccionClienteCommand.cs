namespace Shared.Application.Mensajeria.Contratos;

public sealed record CrearProyeccionClienteCommand(
    Guid OperacionId,
    Guid ClienteId,
    bool Estado,
    long Version);
