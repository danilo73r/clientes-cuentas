dotnet ef dbcontext script `
    --project src/Clientes/Clientes.Infrastructure `
    --startup-project src/Clientes/Clientes.Api `
    --context ClientesDbContext `
    --output "$PSScriptRoot/Clientes.sql" `
    -- --environment Development

if ($LASTEXITCODE -ne 0) { throw "Error generando el SQL." }