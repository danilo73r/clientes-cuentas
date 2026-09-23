namespace Shared.Application.Mensajeria;

public sealed record CrearProyeccionCliente(
    Guid OperacionId,
    Guid ClienteId,
    bool Estado,
    long Version);
