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

    -- Inicia clientes

CREATE TABLE "Personas" (
    "Id" uuid NOT NULL,
    "Nombre" text NOT NULL,
    "Genero" text NOT NULL,
    "FechaNacimiento" date NOT NULL,
    "Identificacion" text NOT NULL,
    "Direccion" text NOT NULL,
    "Telefono" text NOT NULL,
    CONSTRAINT "PK_Personas" PRIMARY KEY ("Id")
);


CREATE TABLE "Clientes" (
    "Id" uuid NOT NULL,
    "Contrasena" text NOT NULL,
    "Estado" boolean NOT NULL,
    "Version" bigint NOT NULL,
    "OperacionPendienteId" uuid,
    CONSTRAINT "PK_Clientes" PRIMARY KEY ("Id"),
    CONSTRAINT "FK_Clientes_Personas_Id" FOREIGN KEY ("Id") REFERENCES "Personas" ("Id") ON DELETE CASCADE
);


CREATE UNIQUE INDEX "UX_Personas_Identificacion" ON "Personas" ("Identificacion");

    -- Termina clientes

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

    -- Inicia cuentas



    -- Termina cuentas

    \connect postgres
\endif
