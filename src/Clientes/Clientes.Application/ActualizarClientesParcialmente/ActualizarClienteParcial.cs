using Clientes.Application.Contratos;
using FluentValidation;
using Shared.Application.Persistencia;
using Shared.Application.Tiempo;
using Shared.Domain.Exceptions;

namespace Clientes.Application.ActualizarClientesParcialmente;

public sealed class ActualizarClienteParcial(
    IClienteRepository repository,
    IUnitOfWork unitOfWork,
    Reloj reloj,
    IValidator<ActualizarClienteParcialInputDto> validador)
{
    public async Task EjecutarAsync(
        Guid id,
        ActualizarClienteParcialInputDto solicitud,
        CancellationToken cancellationToken)
    {
        await validador.ValidateAndThrowAsync(solicitud, cancellationToken);

        var cliente = await repository.ObtenerPorIdAsync(id, cancellationToken)
            ?? throw new NoEncontradoException("El cliente no existe");

        if (cliente.Version != solicitud.Version)
            throw new ConflictoException("El cliente fue modificado por otra operación");

        cliente.ActualizarDatos(
            solicitud.Nombre ?? cliente.Nombre,
            solicitud.Genero ?? cliente.Genero,
            solicitud.FechaNacimiento ?? cliente.FechaNacimiento,
            solicitud.Identificacion ?? cliente.Identificacion,
            solicitud.Direccion ?? cliente.Direccion,
            solicitud.Telefono ?? cliente.Telefono,
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
