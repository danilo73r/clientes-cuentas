using Clientes.Infrastructure.Persistencia;
using Shared.Application.Mensajeria;
using Wolverine.EntityFrameworkCore;

namespace Clientes.Infrastructure.Mensajeria;

public sealed class OutboxClientes(IDbContextOutbox<ClientesDbContext> outbox) : IOutbox
{
    public async Task GuardarYPublicarAsync<T>(T mensaje, CancellationToken cancellationToken)
        where T : class
    {
        await outbox.PublishAsync(mensaje);
        await outbox.SaveChangesAndFlushMessagesAsync(cancellationToken);
    }
}
