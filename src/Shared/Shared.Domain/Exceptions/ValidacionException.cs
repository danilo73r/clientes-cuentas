namespace Shared.Domain.Exceptions;

public sealed class ValidacionException(string mensaje)
    : Exception(mensaje);