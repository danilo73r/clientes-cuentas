using Microsoft.EntityFrameworkCore;

namespace Cuentas.Infrastructure.Persistencia;

public sealed class CuentasDbContext(DbContextOptions<CuentasDbContext> options)
    : DbContext(options);