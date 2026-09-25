using FluentValidation;

namespace Clientes.Application.CambiarContrasenas;

public sealed class CambiarContrasenaClienteValidator : AbstractValidator<CambiarContrasenaClienteInputDto>
{
    public CambiarContrasenaClienteValidator()
    {
        RuleFor(x => x.NuevaContrasena)
            .NotEmpty()
            .WithMessage("La nueva contraseña es obligatoria");

        RuleFor(x => x.Version)
            .GreaterThan(0)
            .WithMessage("La versión debe ser mayor a cero");
    }
}
