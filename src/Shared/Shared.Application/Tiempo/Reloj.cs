namespace Shared.Application.Tiempo;

public sealed class Reloj(
    TimeProvider proveedor,
    TimeZoneInfo zonaHoraria)
{
    public DateTimeOffset AhoraUtc => proveedor.GetUtcNow();
    public DateTimeOffset AhoraLocal => TimeZoneInfo.ConvertTime(AhoraUtc, zonaHoraria);

    public DateOnly HoyUtc => DateOnly.FromDateTime(AhoraUtc.UtcDateTime);
    public DateOnly HoyLocal => DateOnly.FromDateTime(AhoraLocal.DateTime);


    public DateTimeOffset ConvertirAInicioDiaUtc(DateOnly fechaLocal)
    {
        var inicioDiaLocal = fechaLocal.ToDateTime(
            TimeOnly.MinValue,
            DateTimeKind.Unspecified);

        var inicioDiaUtc = TimeZoneInfo.ConvertTimeToUtc(
            inicioDiaLocal,
            zonaHoraria);

        return new DateTimeOffset(inicioDiaUtc);
    }
}