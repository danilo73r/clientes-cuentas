namespace Cuentas.Domain.Entities;

public sealed class ProyeccionCliente
{
    public Guid ClienteId { get; private set; }
    public bool Estado { get; private set; }
    public long Version { get; private set; }

    private ProyeccionCliente() { }

    public ProyeccionCliente(Guid clienteId, bool estado, long version)
    {
        if (clienteId == Guid.Empty)
            throw new ArgumentException("El cliente es obligatorio");
            
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(version);

        ClienteId = clienteId;
        Estado = estado;
        Version = version;
    }

    public bool EsVersionAnteriorOIgual(long version)
        => version <= Version;

    public void ActualizarSiEsMasReciente(bool estado, long version)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(version);

        if (EsVersionAnteriorOIgual(version))
            return;

        Estado = estado;
        Version = version;
    }
}
