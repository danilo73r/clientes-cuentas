using Shared.Domain.Exceptions;
using Shared.Application.Paginacion;
using Clientes.Application.Contratos;
using Clientes.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clientes.Infrastructure.Persistencia.Repositorios;

public sealed class ClienteRepository(ClientesDbContext context)
    : IClienteRepository
{
    public Task<bool> ExisteIdentificacionAsync(
        string identificacion,
        CancellationToken cancellationToken)
    {
        return context.Clientes.AnyAsync(
            cliente => cliente.Identificacion == identificacion,
            cancellationToken);
    }

    public Task<Cliente?> ObtenerPorIdAsync(
        Guid id,
        CancellationToken cancellationToken,
        bool asNoTracking = false)
    {
        var consulta = asNoTracking
            ? context.Clientes.AsNoTracking()
            : context.Clientes;

        return consulta.SingleOrDefaultAsync(
            cliente => cliente.Id == id, cancellationToken);
    }

    public async Task<ResultadoPaginado<Cliente>> ListarAsync(
        int pagina,
        int tamanoPagina,
        CancellationToken cancellationToken)
    {
        var consulta = context.Clientes.AsNoTracking();
        var totalRegistros = await consulta.CountAsync(cancellationToken);
        var totalPaginas = (int)Math.Ceiling((double)totalRegistros / tamanoPagina);
        var ultimaPagina = Math.Max(1, totalPaginas);

        if (pagina > ultimaPagina)
            throw new ValidacionException($"La página no existe. La última es {ultimaPagina}");

        var clientes = await consulta
            .OrderBy(cliente => cliente.Id)
            .Skip(checked((pagina - 1) * tamanoPagina))
            .Take(tamanoPagina)
            .ToListAsync(cancellationToken);

        return new ResultadoPaginado<Cliente>(
            clientes, totalRegistros, pagina, tamanoPagina);
    }

    public Task<bool> ExisteIdentificacionEnOtroClienteAsync(
        Guid id,
        string identificacion,
        CancellationToken cancellationToken)
    {
        return context.Clientes.AnyAsync(
            cliente => cliente.Id != id && cliente.Identificacion == identificacion,
            cancellationToken);
    }

    public void Agregar(Cliente cliente)
    {
        context.Clientes.Add(cliente);
    }

}
