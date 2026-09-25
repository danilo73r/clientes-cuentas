using Clientes.Application.Contratos;
using FluentValidation;
using Shared.Application.Persistencia;
using Shared.Application.Tiempo;
using Shared.Domain.Exceptions;

namespace Clientes.Application.ActualizarClientes;

public sealed class ActualizarCliente(
    IClienteRepository repository,
    IUnitOfWork unitOfWork,
    Reloj reloj,
    IValidator<ActualizarClienteInputDto> validador)
{
    public async Task EjecutarAsync(
        Guid id,
        ActualizarClienteInputDto solicitud,
        CancellationToken cancellationToken)
    {
        await validador.ValidateAndThrowAsync(solicitud, cancellationToken);

        var cliente = await repository.ObtenerPorIdAsync(id, cancellationToken)
            ?? throw new NoEncontradoException("El cliente no existe");

        if (cliente.Version != solicitud.Version)
            throw new ConflictoException("El cliente fue modificado por otra operación");

        cliente.ActualizarDatos(
            solicitud.Nombre,
            solicitud.Genero,
            solicitud.FechaNacimiento,
            solicitud.Identificacion,
            solicitud.Direccion,
            solicitud.Telefono,
            reloj.FechaActualLocal);

        var esDuplicado = await repository.ExisteIdentificacionEnOtroClienteAsync(
            cliente.Id,
            cliente.Identificacion,
            cancellationToken);

        if (esDuplicado)
            throw new ConflictoException("Identificación duplicada");

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
