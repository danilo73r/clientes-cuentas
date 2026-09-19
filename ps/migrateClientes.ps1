# Parametro opcional para el nombre de la migración
param([string]$Prefijo = "Clientes")

$NombreMigracion = "${Prefijo}_$(Get-Date -Format 'yyyyMMddHHmmssfff')"

dotnet ef migrations add $NombreMigracion `
    --project src/Clientes/Clientes.Infrastructure `
    --startup-project src/Clientes/Clientes.Api `
    --context ClientesDbContext `
    --output-dir Persistencia/Migrations `
    -- --environment Development

if ($LASTEXITCODE -ne 0) { throw "Error creando la migración." }

dotnet ef migrations script 0 `
    --project src/Clientes/Clientes.Infrastructure `
    --startup-project src/Clientes/Clientes.Api `
    --context ClientesDbContext `
    --output "$PSScriptRoot/Clientes.sql" `
    -- --environment Development

if ($LASTEXITCODE -ne 0) { throw "Error generando el SQL." }