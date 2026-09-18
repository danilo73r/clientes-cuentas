namespace Shared.Domain.Exceptions;

public sealed class ConflictoException(string mensaje)
    : Exception(mensaje);