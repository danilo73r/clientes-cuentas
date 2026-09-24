namespace Shared.Application.Mensajeria.Contratos;

public sealed record ProyeccionClienteCreadaEvent(
    Guid OperacionId,
    Guid ClienteId,
    bool Estado,
    long Version);
