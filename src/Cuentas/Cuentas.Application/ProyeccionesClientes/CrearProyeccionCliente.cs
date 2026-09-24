using FluentValidation;
using Cuentas.Application.Contratos;
using Cuentas.Domain.Entities;
using Shared.Application.Mensajeria;
using Shared.Application.Mensajeria.Contratos;

namespace Cuentas.Application.ProyeccionesClientes;

public sealed class CrearProyeccionCliente(
    IProyeccionClienteRepository repository,
    IOutbox outbox,
    IValidator<CrearProyeccionClienteCommand> validator)
{
    public async Task EjecutarAsync(CrearProyeccionClienteCommand mensaje, CancellationToken cancellationToken)
    {
        await validator.ValidateAndThrowAsync(mensaje, cancellationToken);

        if (await repository.ExisteAsync(mensaje.ClienteId, cancellationToken))
            return;

        repository.Agregar(new ProyeccionCliente(
            mensaje.ClienteId,
            mensaje.Estado,
            mensaje.Version));

        await outbox.GuardarYPublicarAsync(
            new ProyeccionClienteCreadaEvent(
                mensaje.OperacionId,
                mensaje.ClienteId,
                mensaje.Estado,
                mensaje.Version),
            cancellationToken);
    }
}
