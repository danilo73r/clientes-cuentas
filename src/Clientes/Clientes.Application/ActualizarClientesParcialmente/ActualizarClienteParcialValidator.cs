using FluentValidation;

namespace Clientes.Application.ActualizarClientesParcialmente;

public sealed class ActualizarClienteParcialValidator : AbstractValidator<ActualizarClienteParcialInputDto>
{
    public ActualizarClienteParcialValidator()
    {
        RuleFor(x => x)
            .Must(x => x.TieneCambios())
            .OverridePropertyName("general") // para el json de errores
            .WithMessage("Debe enviar al menos un campo para modificar");

        RuleFor(x => x.Version)
            .GreaterThan(0)
            .WithMessage("La versión debe ser mayor a cero");

        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre no puede ser nulo ni vacío")
            .When(x => x.TieneCampo(nameof(x.Nombre)));

        RuleFor(x => x.Genero)
            .NotNull()
            .WithMessage("El género no puede ser nulo")
            .IsInEnum()
            .WithMessage("El género debe ser válido")
            .When(x => x.TieneCampo(nameof(x.Genero)));

        RuleFor(x => x.FechaNacimiento)
            .NotNull()
            .WithMessage("La fecha de nacimiento no puede ser nula")
            .NotEqual(DateOnly.MinValue)
            .WithMessage("La fecha de nacimiento no puede ser vacía")
            .When(x => x.TieneCampo(nameof(x.FechaNacimiento)));

        RuleFor(x => x.Identificacion)
            .NotEmpty()
            .WithMessage("La identificación no puede ser nula ni vacía")
            .When(x => x.TieneCampo(nameof(x.Identificacion)));

        RuleFor(x => x.Direccion)
            .NotEmpty()
            .WithMessage("La dirección no puede ser nula ni vacía")
            .When(x => x.TieneCampo(nameof(x.Direccion)));

        RuleFor(x => x.Telefono)
            .NotEmpty()
            .WithMessage("El teléfono no puede ser nulo ni vacío")
            .When(x => x.TieneCampo(nameof(x.Telefono)));
    }
}
