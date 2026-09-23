using Shared.Application.Tiempo;

namespace Shared.Application.Identidad;

public sealed class GeneradorId(Reloj reloj)
{
    public Guid CrearIdSecuencial() => Guid.CreateVersion7(reloj.AhoraUtc);
    public static Guid CrearRandomId() => Guid.NewGuid();
}