using Clientes.Domain.Entities;
using Clientes.Domain.Enums;
using Shared.Domain.Exceptions;
namespace Clientes.UnitTests;

public sealed class ClienteTests
{
    [Fact]
    public void RecharzarCambios_DuranteDesactivacion_DeCliente()
    {
        // Given: solicitar desactivacion de cliente 
        var fechaActual = new DateOnly(2026, 9, 18);
        var operacionCreacionId = Guid.NewGuid();
        var cliente = new Cliente(
            id: Guid.NewGuid(),
            nombre: "Jose",
            genero: Genero.Masculino,
            fechaNacimientoLocal: new DateOnly(1990, 1, 15),
            identificacion: "1234567890",
            direccion: "Quito",
            telefono: "098222333",
            contrasena: "1234",
            fechaActualLocal: fechaActual,
            operacionCreacionId: operacionCreacionId);

        Assert.True(cliente.ConfirmarOperacion(operacionCreacionId, true));
        var operacionDesactivacionId = Guid.NewGuid();
        cliente.SolicitarDesactivacion(operacionDesactivacionId);
        Assert.False(cliente.Estado);
        Assert.Equal(operacionDesactivacionId, cliente.OperacionPendienteId);

        var nombreOriginal = cliente.Nombre;
        var estadoOriginal = cliente.Estado;
        var versionOriginal = cliente.Version;
        var operacionOriginal = cliente.OperacionPendienteId;

        // When: intentar modificar datos
        void actualizar() => cliente.ActualizarDatos(
            nombre: "Nombre modificado",
            genero: cliente.Genero,
            fechaNacimientoLocal: cliente.FechaNacimiento,
            identificacion: cliente.Identificacion,
            direccion: cliente.Direccion,
            telefono: cliente.Telefono,
            fechaActualLocal: fechaActual);

        // Then: se rechaza el cambio de datos
        Assert.Throws<ConflictoException>(actualizar);
        Assert.Equal(nombreOriginal, cliente.Nombre);
        Assert.Equal(estadoOriginal, cliente.Estado);
        Assert.Equal(versionOriginal, cliente.Version);
        Assert.Equal(operacionOriginal, cliente.OperacionPendienteId);
    }
}

