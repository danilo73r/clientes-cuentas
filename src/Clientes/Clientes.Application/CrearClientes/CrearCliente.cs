using Shared.Application.Mensajeria;
using FluentValidation;
using Clientes.Application.Contratos;
using Clientes.Domain.Entities;
using Shared.Application.Identidad;
using Shared.Application.Tiempo;
using Shared.Domain.Exceptions;

namespace Clientes.Application.CrearClientes;

public sealed class CrearCliente(
    IClienteRepository repository,
    IOutbox outbox,
    GeneradorId generadorId,
    Reloj reloj,
    IValidator<CrearClienteInputDto> validador)
{
    public async Task<ClienteCreadoOutputDto> EjecutarAsync(
        CrearClienteInputDto solicitud,
        CancellationToken cancellationToken)
    {
        await validador.ValidateAndThrowAsync(solicitud, cancellationToken);

        var operacionId = GeneradorId.CrearRandomId();
        var cliente = new Cliente(
            id: generadorId.CrearIdSecuencial(),
            nombre: solicitud.Nombre,
            genero: solicitud.Genero,
            fechaNacimientoLocal: solicitud.FechaNacimiento,
            identificacion: solicitud.Identificacion,
            direccion: solicitud.Direccion,
            telefono: solicitud.Telefono,
            contrasena: solicitud.Contrasena,
            fechaActualLocal: reloj.FechaActualLocal,
            operacionCreacionId: operacionId);

        var duplicacionDeIdentificacion = await repository.ExisteIdentificacionAsync(
            cliente.Identificacion,
            cancellationToken);

        if (duplicacionDeIdentificacion)
            throw new ConflictoException("Identificación duplicada");

        repository.Agregar(cliente);

        await outbox.GuardarYPublicarAsync(
            new CrearProyeccionCliente(
                operacionId,
                cliente.Id,
                cliente.Estado,
                cliente.Version),
            cancellationToken);

        return new ClienteCreadoOutputDto(
            cliente.Id,
            cliente.CalcularEdad(reloj.FechaActualLocal),
            cliente.Estado,
            cliente.Version,
            cliente.OperacionPendienteId);
    }
}
