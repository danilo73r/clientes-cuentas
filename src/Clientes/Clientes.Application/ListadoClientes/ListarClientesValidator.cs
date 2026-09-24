using FluentValidation;

namespace Clientes.Application.ListadoClientes;

public sealed class ListarClientesValidator : AbstractValidator<ListarClientesInputDto>
{
    public ListarClientesValidator()
    {
        RuleFor(x => x.Pagina)
            .GreaterThanOrEqualTo(1)
            .WithMessage("La página debe ser mayor o igual a 1");

        RuleFor(x => x.TamanoPagina)
            .InclusiveBetween(1, 100)
            .WithMessage("El tamaño de página debe estar entre 1 y 100");

        RuleFor(x => x.Pagina)
            .Must((solicitud, pagina) =>
                ((long)pagina - 1) * solicitud.TamanoPagina <= int.MaxValue)
            .When(x => x.Pagina > 0 && x.TamanoPagina is >= 1 and <= 100)
            .WithMessage("La página solicitada supera el límite permitido");
    }
}
