using Clientes.Domain.Enums;
using Shared.Domain.Exceptions;

namespace Clientes.Domain.Entities;

public sealed class Cliente : Persona
{
    public string Contrasena { get; private set; } = string.Empty;
    public bool Estado { get; private set; }
    public long Version { get; private set; }
    public Guid? OperacionPendienteId { get; private set; }

    private Cliente() { } // convención de efcore

    public Cliente(
        Guid id,
        string nombre,
        Genero genero,
        DateOnly fechaNacimientoLocal,
        string identificacion,
        string direccion,
        string telefono,
        string contrasena,
        DateOnly fechaActualLocal,
        Guid operacionCreacionId)
        : base(
            id,
            nombre,
            genero,
            fechaNacimientoLocal,
            identificacion,
            direccion,
            telefono,
            fechaActualLocal)
    {
        Contrasena = ValidarContrasena(contrasena);
        OperacionPendienteId = ValidarOperacionId(operacionCreacionId);
        Estado = true;
        Version = 1;
    }

    public void ActualizarDatos(
        string nombre,
        Genero genero,
        DateOnly fechaNacimientoLocal,
        string identificacion,
        string direccion,
        string telefono,
        DateOnly fechaActualLocal)
    {
        AsegurarQueNoTieneOperacionPendiente();
        ActualizarDatosPersonales(
            nombre,
            genero,
            fechaNacimientoLocal,
            identificacion,
            direccion,
            telefono,
            fechaActualLocal);
        IncrementarVersion();
    }

    public void CambiarContrasena(string contrasena)
    {
        AsegurarQueNoTieneOperacionPendiente();
        Contrasena = ValidarContrasena(contrasena);
        IncrementarVersion();
    }

    public void SolicitarDesactivacion(Guid operacionId)
        => SolicitarCambioEstado(false, operacionId);

    public void SolicitarActivacion(Guid operacionId)
        => SolicitarCambioEstado(true, operacionId);

    private void SolicitarCambioEstado(bool nuevoEstado, Guid operacionId)
    {
        AsegurarQueNoTieneOperacionPendiente();
        ValidarOperacionId(operacionId);

        if (Estado == nuevoEstado)
            throw new ConflictoException(nuevoEstado
                ? "El cliente ya está activo."
                : "El cliente ya está inactivo.");

        Estado = nuevoEstado;
        OperacionPendienteId = operacionId;
        IncrementarVersion();
    }


    public bool ConfirmarOperacion(Guid operacionId, bool estadoConfirmado)
    {
        ValidarOperacionId(operacionId);

        if (OperacionPendienteId != operacionId)
            return false;

        if (Estado != estadoConfirmado)
            throw new ConflictoException("El estado confirmado es distinto a lo solicitado");


        OperacionPendienteId = null;
        IncrementarVersion();
        return true;
    }


    private void AsegurarQueNoTieneOperacionPendiente()
    {
        if (OperacionPendienteId.HasValue)
            throw new ConflictoException("El cliente tiene una operación pendiente");
    }

    private static Guid ValidarOperacionId(Guid operacionId)
    {
        if (operacionId == Guid.Empty)
            throw new ValidacionException("El Id de una operación es obligatorio");

        return operacionId;
    }

    private static string ValidarContrasena(string contrasena)
    {
        if (string.IsNullOrWhiteSpace(contrasena))
            throw new ValidacionException("La contraseña es obligatoria");

        return contrasena;
    }

    private void IncrementarVersion()
    {
        Version = checked(Version + 1);
    }
}