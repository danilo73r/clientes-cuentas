using Clientes.Domain.Enums;
using Shared.Domain.Exceptions;

namespace Clientes.Domain.Entities;

public abstract class Persona
{
    public Guid Id { get; private set; }
    public string Nombre { get; private set; } = string.Empty;
    public Genero Genero { get; private set; }
    public DateOnly FechaNacimiento { get; private set; }
    public string Identificacion { get; private set; } = string.Empty;
    public string Direccion { get; private set; } = string.Empty;
    public string Telefono { get; private set; } = string.Empty;

    protected Persona() { } // convención de efcore

    protected Persona(
        Guid id,
        string nombre,
        Genero genero,
        DateOnly fechaNacimientoLocal,
        string identificacion,
        string direccion,
        string telefono,
        DateOnly fechaActualLocal)
    {
        if (id == Guid.Empty)
            throw new ValidacionException("El Id es obligatorio.");

        if (!Enum.IsDefined(genero))
            throw new ValidacionException("El género no es válido.");

        if (fechaNacimientoLocal > fechaActualLocal)
            throw new ValidacionException("La fecha de nacimiento no puede ser del futuro.");

        Id = id;
        Genero = genero;
        FechaNacimiento = fechaNacimientoLocal;

        Nombre = NoVacío(nombre, "nombre");
        Identificacion = NoVacío(identificacion, "identificación");
        Direccion = NoVacío(direccion, "dirección");
        Telefono = NoVacío(telefono, "teléfono");
    }

    public int CalcularEdad(DateOnly fechaActualLocal)
    {
        if (fechaActualLocal < FechaNacimiento)
            throw new ValidacionException("La fecha actual menor a la fecha de nacimiento");

        var edad = fechaActualLocal.Year - FechaNacimiento.Year;

        if (fechaActualLocal < FechaNacimiento.AddYears(edad))
            edad--;

        return edad;
    }

    protected static string NoVacío(string valor, string campo)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ValidacionException($"El campo {campo} es obligatorio.");

        return valor.Trim();
    }
}