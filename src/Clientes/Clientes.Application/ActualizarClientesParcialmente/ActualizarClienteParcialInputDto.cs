using System.Text.Json.Serialization;
using Clientes.Domain.Enums;

namespace Clientes.Application.ActualizarClientesParcialmente;

[JsonUnmappedMemberHandling(JsonUnmappedMemberHandling.Disallow)]
public sealed class ActualizarClienteParcialInputDto
{
    private readonly HashSet<string> camposEnviados = [];

    public long Version { get; init; }

    // El init se ejecuta solo cuando el deserializador instancia el objeto con la propiedad
    // y se instancia con esa propiedad solo cuando el Json lo tiene (incluso con null)
    public string? Nombre
    {
        get;
        init { field = value; camposEnviados.Add(nameof(Nombre)); }
    }

    public Genero? Genero
    {
        get;
        init { field = value; camposEnviados.Add(nameof(Genero)); }
    }

    public DateOnly? FechaNacimiento
    {
        get;
        init { field = value; camposEnviados.Add(nameof(FechaNacimiento)); }
    }

    public string? Identificacion
    {
        get;
        init { field = value; camposEnviados.Add(nameof(Identificacion)); }
    }

    public string? Direccion
    {
        get;
        init { field = value; camposEnviados.Add(nameof(Direccion)); }
    }

    public string? Telefono
    {
        get;
        init { field = value; camposEnviados.Add(nameof(Telefono)); }
    }

    public bool TieneCampo(string nombre) => camposEnviados.Contains(nombre);
    public bool TieneCambios() => camposEnviados.Count > 0;
}
