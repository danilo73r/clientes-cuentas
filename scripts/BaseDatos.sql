\connect postgres


-- Clientes
-- crear base y tablas solo si no existe
SELECT NOT EXISTS (
    SELECT 1 FROM pg_database WHERE datname = :'clientes'
) AS crear_clientes
\gset

\if :crear_clientes
    CREATE DATABASE :"clientes";

    \connect :clientes

    -- aquí va el SQL de las migraciones de clientes

    \connect postgres
\endif


-- Cuentas
-- crear base y tablas solo si no existe
SELECT NOT EXISTS (
    SELECT 1 FROM pg_database WHERE datname = :'cuentas'
) AS crear_cuentas
\gset

\if :crear_cuentas
    CREATE DATABASE :"cuentas";

    \connect :cuentas

    -- aquí va el SQL de las migraciones de cuentas

    \connect postgres
\endif