using Microsoft.EntityFrameworkCore;

namespace Clientes.Infrastructure.Persistencia;

public sealed class ClientesDbContext(DbContextOptions<ClientesDbContext> options)
    : DbContext(options);