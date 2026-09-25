using FluentValidation;

namespace Clientes.Application.ActualizarClientes;

public sealed class ActualizarClienteValidator : AbstractValidator<ActualizarClienteInputDto>
{
    public ActualizarClienteValidator()
    {
        
        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre es obligatorio");
        
        RuleFor(x => x.Genero)
            .IsInEnum()
            .WithMessage("El género no es válido");
        
        RuleFor(x => x.FechaNacimiento)
            .NotEmpty()
            .WithMessage("La fecha de nacimiento es obligatoria");
        
        RuleFor(x => x.Identificacion)
            .NotEmpty()
            .WithMessage("La identificación es obligatoria");
        
        RuleFor(x => x.Direccion)
            .NotEmpty()
            .WithMessage("La dirección es obligatoria");
        
        RuleFor(x => x.Telefono)
            .NotEmpty()
            .WithMessage("El teléfono es obligatorio");
        
        RuleFor(x => x.Version)
            .GreaterThan(0)
            .WithMessage("La versión debe ser mayor a cero");
    }
}
