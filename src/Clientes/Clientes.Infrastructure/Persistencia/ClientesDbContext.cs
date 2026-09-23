using Shared.Application.Persistencia;
using Clientes.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Shared.Domain.Exceptions;

namespace Clientes.Infrastructure.Persistencia;

public sealed class ClientesDbContext(DbContextOptions<ClientesDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<Cliente> Clientes => Set<Cliente>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ClientesDbContext).Assembly);
    }

    public override async Task<int> SaveChangesAsync(
        bool acceptAllChangesOnSuccess,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await base.SaveChangesAsync(
                acceptAllChangesOnSuccess,
                cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            throw new ConflictoException(
                "El cliente fue modificado por otra operación, " +
                "consulta otra vez e intenta de nuevo.");
        }
        catch (DbUpdateException exception)
            when (exception.InnerException is PostgresException
            {
                SqlState: PostgresErrorCodes.UniqueViolation,
                ConstraintName: "UX_Personas_Identificacion"
            })
        {
            throw new ConflictoException("Identificación duplicada.");
        }
    }
}