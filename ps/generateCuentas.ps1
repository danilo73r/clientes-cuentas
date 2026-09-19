dotnet ef dbcontext script `
    --project src/Cuentas/Cuentas.Infrastructure `
    --startup-project src/Cuentas/Cuentas.Api `
    --context CuentasDbContext `
    --output "$PSScriptRoot/Cuentas.sql" `
    -- --environment Development

if ($LASTEXITCODE -ne 0) { throw "Error generando el SQL." }