namespace Shared.Domain.Exceptions;

public sealed class ReglaNegocioException(string codigo, string mensaje) 
    : Exception(mensaje)
{
    public string Codigo { get; } = codigo;
}