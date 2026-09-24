namespace Shared.Application.Paginacion;

public sealed record ResultadoPaginado<T>(
    IReadOnlyList<T> Items,
    int TotalRegistros,
    int Pagina,
    int TamanoPagina);
