using FluentValidation;
using Shared.Application.Mensajeria.Contratos;

namespace Cuentas.Application.ProyeccionesClientes;

public sealed class CrearProyeccionClienteValidator : AbstractValidator<CrearProyeccionClienteCommand>
{
    public CrearProyeccionClienteValidator()
    {
        RuleFor(mensaje => mensaje.ClienteId)
            .NotEmpty().WithMessage("El cliente es obligatorio");

        RuleFor(mensaje => mensaje.OperacionId)
            .NotEmpty().WithMessage("La operación es obligatoria");

        RuleFor(mensaje => mensaje.Version)
            .GreaterThan(0).WithMessage("La versión debe ser mayor que cero");
    }
}
