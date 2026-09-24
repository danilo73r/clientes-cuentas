using Cuentas.Application.ProyeccionesClientes;
using Microsoft.Extensions.DependencyInjection;
using Shared.Application.Mensajeria.Contratos;

namespace Cuentas.Infrastructure.Mensajeria;

public sealed class CrearProyeccionClienteHandler(IServiceScopeFactory scopeFactory)
{
    public async Task Handle(CrearProyeccionClienteCommand mensaje, CancellationToken cancellationToken)
    {
        // Cada intento usa un contexto y un outbox nuevos, incluso en los reintentos de Wolverine.
        await using var scope = scopeFactory.CreateAsyncScope();
        var casoDeUso = scope.ServiceProvider.GetRequiredService<CrearProyeccionCliente>();
        await casoDeUso.EjecutarAsync(mensaje, cancellationToken);
    }
}
