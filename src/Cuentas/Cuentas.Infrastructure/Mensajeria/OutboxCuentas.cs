using Cuentas.Infrastructure.Persistencia;
using Shared.Application.Mensajeria;
using Wolverine.EntityFrameworkCore;

namespace Cuentas.Infrastructure.Mensajeria;

public sealed class OutboxCuentas(IDbContextOutbox<CuentasDbContext> outbox) : IOutbox
{
    public async Task GuardarYPublicarAsync<T>(T mensaje, CancellationToken cancellationToken)
        where T : class
    {
        await outbox.PublishAsync(mensaje);
        await outbox.SaveChangesAndFlushMessagesAsync(cancellationToken);
    }
}
