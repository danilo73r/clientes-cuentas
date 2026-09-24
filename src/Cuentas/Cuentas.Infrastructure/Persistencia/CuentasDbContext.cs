using Cuentas.Domain.Entities;
using Shared.Application.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace Cuentas.Infrastructure.Persistencia;

public sealed class CuentasDbContext(DbContextOptions<CuentasDbContext> options)
    : DbContext(options), IUnitOfWork
{
    public DbSet<ProyeccionCliente> ProyeccionesClientes => Set<ProyeccionCliente>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(CuentasDbContext).Assembly);
    }
}
