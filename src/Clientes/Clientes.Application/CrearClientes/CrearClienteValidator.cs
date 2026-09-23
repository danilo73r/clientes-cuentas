using FluentValidation;

namespace Clientes.Application.CrearClientes;

public sealed class CrearClienteValidator : AbstractValidator<CrearClienteInputDto>
{
    public CrearClienteValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre es obligatorio");

        RuleFor(x => x.Genero)
            .NotEmpty()
            .WithMessage("El género es obligatorio")
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
        
        RuleFor(x => x.Contrasena)
            .NotEmpty()
            .WithMessage("La contraseña es obligatoria");
    }
}
