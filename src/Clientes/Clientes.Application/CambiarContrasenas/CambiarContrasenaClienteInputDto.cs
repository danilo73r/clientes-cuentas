namespace Clientes.Application.CambiarContrasenas;

public sealed record CambiarContrasenaClienteInputDto(
    string NuevaContrasena,
    long Version);
