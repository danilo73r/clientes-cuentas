namespace Shared.Application.Mensajeria;

public interface IOutbox
{
    Task GuardarYPublicarAsync<T>(T mensaje, CancellationToken cancellationToken)
        where T : class;
}
