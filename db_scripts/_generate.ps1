$ErrorActionPreference = "Stop"
Push-Location (Split-Path $PSScriptRoot -Parent)
try {
    $template = Get-Content "$PSScriptRoot/_template.sql" -Raw
    $bloqueClientes = [regex]::Matches($template, "(?s)-- Inicia clientes\b.*?-- Termina clientes\b")
    $bloqueCuentas = [regex]::Matches($template, "(?s)-- Inicia cuentas\b.*?-- Termina cuentas\b")
    if ($bloqueClientes.Count -ne 1 -or $bloqueCuentas.Count -ne 1) {
        throw "_template.sql debe tener un bloque de clientes y otro de cuentas"
    }

    dotnet tool restore
    if ($LASTEXITCODE -ne 0) { throw "No se pudo restaurar dotnet-ef" }

    dotnet ef dbcontext script `
        --project src/Clientes/Clientes.Infrastructure `
        --startup-project src/Clientes/Clientes.Api `
        --context ClientesDbContext `
        --output "$PSScriptRoot/Clientes.sql" `
        -- --environment Development
    if ($LASTEXITCODE -ne 0) { throw "No se pudo generar Clientes.sql" }

    dotnet ef dbcontext script `
        --project src/Cuentas/Cuentas.Infrastructure `
        --startup-project src/Cuentas/Cuentas.Api `
        --context CuentasDbContext `
        --output "$PSScriptRoot/Cuentas.sql" `
        -- --environment Development
    if ($LASTEXITCODE -ne 0) { throw "No se pudo generar Cuentas.sql" }

    dotnet run --project src/Clientes/Clientes.Api --no-build -- db-dump "$PSScriptRoot/Clientes.wolverine.sql"
    if ($LASTEXITCODE -ne 0) { throw "No se pudo generar Clientes.wolverine.sql" }

    dotnet run --project src/Cuentas/Cuentas.Api --no-build -- db-dump "$PSScriptRoot/Cuentas.wolverine.sql"
    if ($LASTEXITCODE -ne 0) { throw "No se pudo generar Cuentas.wolverine.sql" }

    # insertar ambos SQL en el template
    $clientesSql = [System.IO.File]::ReadAllText("$PSScriptRoot/Clientes.sql").TrimEnd()
    $clientesSql += "`n`n" + [System.IO.File]::ReadAllText("$PSScriptRoot/Clientes.wolverine.sql").TrimEnd()
    $cuentasSql = [System.IO.File]::ReadAllText("$PSScriptRoot/Cuentas.sql").TrimEnd()
    $cuentasSql += "`n`n" + [System.IO.File]::ReadAllText("$PSScriptRoot/Cuentas.wolverine.sql").TrimEnd()
    $template = $template.Replace($bloqueClientes[0].Value, "-- Inicia clientes`n`n$clientesSql`n`n    -- Termina clientes")
    $template = $template.Replace($bloqueCuentas[0].Value, "-- Inicia cuentas`n`n$cuentasSql`n`n    -- Termina cuentas")

    # guardar resultado y borrar sql temporales
    Set-Content "$PSScriptRoot/BaseDatos.sql" $template.TrimEnd() -Encoding utf8
    Write-Host "BaseDatos.sql generado" -ForegroundColor Green
}
finally {
    Remove-Item -LiteralPath @(
        "$PSScriptRoot/Clientes.sql"
        "$PSScriptRoot/Cuentas.sql"
        "$PSScriptRoot/Clientes.wolverine.sql"
        "$PSScriptRoot/Cuentas.wolverine.sql"
    ) -ErrorAction SilentlyContinue
    Pop-Location
}
