using Shared.Application.Tiempo;

namespace Shared.Application.Identidad;

public sealed class GeneradorId(Reloj reloj)
{
    public Guid Crear() => Guid.CreateVersion7(reloj.AhoraUtc);
}