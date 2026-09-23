namespace Shared.Application.Mensajeria;

public sealed record ProyeccionClienteCreada(
    Guid OperacionId,
    Guid ClienteId,
    bool Estado,
    long Version);
