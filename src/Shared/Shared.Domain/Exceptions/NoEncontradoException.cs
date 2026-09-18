namespace Shared.Domain.Exceptions;

public sealed class NoEncontradoException(string mensaje)
    : Exception(mensaje);