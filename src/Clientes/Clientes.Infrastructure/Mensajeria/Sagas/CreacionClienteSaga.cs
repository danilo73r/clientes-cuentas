using Clientes.Domain.Entities;
using Clientes.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;
using Shared.Application.Mensajeria.Contratos;
using Wolverine;
using Wolverine.Attributes;
using Wolverine.Persistence;
using Wolverine.Persistence.Sagas;

namespace Clientes.Infrastructure.Mensajeria.Sagas;

public sealed class CreacionClienteSaga : Saga
{
    public Guid Id { get; private set; }
    public Guid ClienteId { get; private set; }
    public bool EstadoEsperado { get; private set; }
    public long VersionEsperada { get; private set; }

    private CreacionClienteSaga() { }

    public CreacionClienteSaga(Cliente cliente)
    {
        Id = cliente.OperacionPendienteId
            ?? throw new InvalidOperationException("El cliente no tiene una operación de creación pendiente");
        ClienteId = cliente.Id;
        EstadoEsperado = cliente.Estado;
        VersionEsperada = cliente.Version;
    }

    [Transactional(Mode = TransactionMiddlewareMode.Lightweight)]
    public async Task Handle(
        [SagaIdentityFrom(nameof(ProyeccionClienteCreadaEvent.OperacionId))] ProyeccionClienteCreadaEvent mensaje,
        ClientesDbContext dbContext,
        CancellationToken cancellationToken)
    {
        if (mensaje.OperacionId != Id
            || mensaje.ClienteId != ClienteId
            || mensaje.Estado != EstadoEsperado
            || mensaje.Version != VersionEsperada)
            return;

        var cliente = await dbContext.Clientes.SingleOrDefaultAsync(
            cliente => cliente.Id == ClienteId,
            cancellationToken);

        if (cliente is null
            || cliente.OperacionPendienteId != Id
            || cliente.Estado != EstadoEsperado
            || cliente.Version != VersionEsperada)
            return;

        cliente.ConfirmarOperacion(Id, EstadoEsperado);
        MarkCompleted();
        // Wolverine guarda el cliente y elimina la saga en el mismo SaveChangesAsync.
    }

    public static void NotFound(ProyeccionClienteCreadaEvent mensaje)
    {
        // Ignorar confirmaciones de operaciones ya completadas o desconocidas.
    }
}
