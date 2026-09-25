using Clientes.Application.Contratos;
using FluentValidation;
using Shared.Application.Persistencia;
using Shared.Domain.Exceptions;

namespace Clientes.Application.CambiarContrasenas;

public sealed class CambiarContrasenaCliente(
    IClienteRepository repository,
    IUnitOfWork unitOfWork,
    IValidator<CambiarContrasenaClienteInputDto> validador)
{
    public async Task EjecutarAsync(
        Guid id,
        CambiarContrasenaClienteInputDto solicitud,
        CancellationToken cancellationToken)
    {
        await validador.ValidateAndThrowAsync(solicitud, cancellationToken);

        var cliente = await repository.ObtenerPorIdAsync(id, cancellationToken)
            ?? throw new NoEncontradoException("El cliente no existe");

        if (cliente.Version != solicitud.Version)
            throw new ConflictoException("El cliente fue modificado por otra operación");

        cliente.CambiarContrasena(solicitud.NuevaContrasena);

        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
