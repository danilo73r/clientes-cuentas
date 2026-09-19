CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL,
    CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId")
);

START TRANSACTION;
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

INSERT INTO "__EFMigrationsHistory" ("MigrationId", "ProductVersion")
VALUES ('20260919004612_Inicial_20260918194606914', '10.0.4');

COMMIT;

