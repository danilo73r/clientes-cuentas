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

CREATE TABLE "CreacionesClientes" (
    "Id" uuid NOT NULL,
    "ClienteId" uuid NOT NULL,
    "EstadoEsperado" boolean NOT NULL,
    "VersionEsperada" bigint NOT NULL,
    "Version" integer NOT NULL,
    CONSTRAINT "PK_CreacionesClientes" PRIMARY KEY ("Id")
);


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


CREATE UNIQUE INDEX "IX_CreacionesClientes_ClienteId" ON "CreacionesClientes" ("ClienteId");


CREATE UNIQUE INDEX "UX_Personas_Identificacion" ON "Personas" ("Identificacion");

DO LANGUAGE plpgsql $tran$
BEGIN

DO $$
BEGIN      IF NOT EXISTS (SELECT 1 FROM pg_catalog.pg_namespace WHERE nspname = 'wolverine') THEN
        BEGIN
          EXECUTE 'CREATE SCHEMA IF NOT EXISTS wolverine';
        EXCEPTION
          WHEN duplicate_schema THEN NULL;
          WHEN unique_violation THEN NULL;
        END;
      END IF;

END
$$;


CREATE TABLE IF NOT EXISTS wolverine.wolverine_outgoing_envelopes (
    id              uuid                        NOT NULL,
    owner_id        integer                     NOT NULL,
    destination     varchar                     NOT NULL,
    deliver_by      timestamp with time zone    NULL,
    body            bytea                       NOT NULL,
    attempts        integer                     NULL DEFAULT 0,
    message_type    varchar                     NOT NULL,
CONSTRAINT pkey_wolverine_outgoing_envelopes_id PRIMARY KEY (id)
);

CREATE INDEX idx_wolverine_outgoing_envelopes_owner ON wolverine.wolverine_outgoing_envelopes USING btree (owner_id) WHERE (owner_id <> 0);

CREATE INDEX idx_wolverine_outgoing_envelopes_recover ON wolverine.wolverine_outgoing_envelopes USING btree (destination) WHERE (owner_id = 0);
CREATE TABLE IF NOT EXISTS wolverine.wolverine_incoming_envelopes (
    id                uuid                        NOT NULL,
    status            varchar                     NOT NULL,
    owner_id          integer                     NOT NULL,
    execution_time    timestamp with time zone    NULL DEFAULT NULL,
    attempts          integer                     NULL DEFAULT 0,
    body              bytea                       NOT NULL,
    message_type      varchar                     NOT NULL,
    received_at       varchar                     NULL,
    keep_until        timestamp with time zone    NULL,
CONSTRAINT pkey_wolverine_incoming_envelopes_id PRIMARY KEY (id)
);

CREATE INDEX idx_wolverine_incoming_envelopes_owner ON wolverine.wolverine_incoming_envelopes USING btree (owner_id) WHERE (owner_id <> 0);

CREATE INDEX idx_wolverine_incoming_envelopes_recover ON wolverine.wolverine_incoming_envelopes USING btree (received_at) WHERE (status = 'Incoming' AND owner_id = 0);

CREATE INDEX idx_wolverine_incoming_envelopes_keep_until ON wolverine.wolverine_incoming_envelopes USING btree (keep_until) WHERE (status = 'Handled');
CREATE TABLE IF NOT EXISTS wolverine.wolverine_dead_letters (
    id                   uuid                        NOT NULL,
    execution_time       timestamp with time zone    NULL DEFAULT NULL,
    body                 bytea                       NOT NULL,
    message_type         varchar                     NOT NULL,
    received_at          varchar                     NULL,
    source               varchar                     NULL,
    exception_type       varchar                     NULL,
    exception_message    varchar                     NULL,
    sent_at              timestamp with time zone    NULL,
    replayable           boolean                     NULL,
CONSTRAINT pkey_wolverine_dead_letters_id PRIMARY KEY (id)
);

CREATE INDEX idx_wolverine_dead_letters_replayable ON wolverine.wolverine_dead_letters USING btree (replayable) WHERE (replayable = true);
CREATE TABLE IF NOT EXISTS wolverine.wolverine_nodes (
    id              uuid                        NOT NULL,
    node_number     serial                      NOT NULL,
    description     varchar                     NOT NULL,
    uri             varchar                     NOT NULL,
    started         timestamp with time zone    NOT NULL DEFAULT now(),
    health_check    timestamp with time zone    NOT NULL DEFAULT now(),
    version         varchar                     NULL,
    capabilities    text[]                      NULL,
CONSTRAINT pkey_wolverine_nodes_id PRIMARY KEY (id)
);
CREATE TABLE IF NOT EXISTS wolverine.wolverine_node_assignments (
    id         varchar                     NOT NULL,
    node_id    uuid                        NULL,
    started    timestamp with time zone    NOT NULL DEFAULT now(),
CONSTRAINT pkey_wolverine_node_assignments_id PRIMARY KEY (id)
);

ALTER TABLE wolverine.wolverine_node_assignments
ADD CONSTRAINT fkey_wolverine_node_assignments_node_id FOREIGN KEY(node_id)
REFERENCES wolverine.wolverine_nodes(id)ON DELETE CASCADE
;

CREATE TABLE IF NOT EXISTS wolverine.wolverine_control_queue (
    id              uuid                        NOT NULL,
    message_type    varchar                     NOT NULL,
    node_id         uuid                        NOT NULL,
    body            bytea                       NOT NULL,
    posted          timestamp with time zone    NOT NULL DEFAULT NOW(),
    expires         timestamp with time zone    NULL,
CONSTRAINT pkey_wolverine_control_queue_id PRIMARY KEY (id)
);
CREATE TABLE IF NOT EXISTS wolverine.wolverine_node_records (
    id             serial                      NOT NULL,
    node_number    integer                     NOT NULL,
    event_name     varchar                     NOT NULL,
    timestamp      timestamp with time zone    NOT NULL DEFAULT now(),
    description    varchar                     NULL,
CONSTRAINT pkey_wolverine_node_records_id PRIMARY KEY (id)
);
CREATE TABLE IF NOT EXISTS wolverine.wolverine_agent_restrictions (
    id      uuid       NOT NULL,
    uri     varchar    NOT NULL,
    type    varchar    NOT NULL,
    node    integer    NOT NULL DEFAULT 0,
CONSTRAINT pkey_wolverine_agent_restrictions_id PRIMARY KEY (id)
);

END;
$tran$;

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

CREATE TABLE "ProyeccionesClientes" (
    "ClienteId" uuid NOT NULL,
    "Estado" boolean NOT NULL,
    "Version" bigint NOT NULL,
    CONSTRAINT "PK_ProyeccionesClientes" PRIMARY KEY ("ClienteId")
);

DO LANGUAGE plpgsql $tran$
BEGIN

DO $$
BEGIN      IF NOT EXISTS (SELECT 1 FROM pg_catalog.pg_namespace WHERE nspname = 'wolverine') THEN
        BEGIN
          EXECUTE 'CREATE SCHEMA IF NOT EXISTS wolverine';
        EXCEPTION
          WHEN duplicate_schema THEN NULL;
          WHEN unique_violation THEN NULL;
        END;
      END IF;

END
$$;


CREATE TABLE IF NOT EXISTS wolverine.wolverine_outgoing_envelopes (
    id              uuid                        NOT NULL,
    owner_id        integer                     NOT NULL,
    destination     varchar                     NOT NULL,
    deliver_by      timestamp with time zone    NULL,
    body            bytea                       NOT NULL,
    attempts        integer                     NULL DEFAULT 0,
    message_type    varchar                     NOT NULL,
CONSTRAINT pkey_wolverine_outgoing_envelopes_id PRIMARY KEY (id)
);

CREATE INDEX idx_wolverine_outgoing_envelopes_owner ON wolverine.wolverine_outgoing_envelopes USING btree (owner_id) WHERE (owner_id <> 0);

CREATE INDEX idx_wolverine_outgoing_envelopes_recover ON wolverine.wolverine_outgoing_envelopes USING btree (destination) WHERE (owner_id = 0);
CREATE TABLE IF NOT EXISTS wolverine.wolverine_incoming_envelopes (
    id                uuid                        NOT NULL,
    status            varchar                     NOT NULL,
    owner_id          integer                     NOT NULL,
    execution_time    timestamp with time zone    NULL DEFAULT NULL,
    attempts          integer                     NULL DEFAULT 0,
    body              bytea                       NOT NULL,
    message_type      varchar                     NOT NULL,
    received_at       varchar                     NULL,
    keep_until        timestamp with time zone    NULL,
CONSTRAINT pkey_wolverine_incoming_envelopes_id PRIMARY KEY (id)
);

CREATE INDEX idx_wolverine_incoming_envelopes_owner ON wolverine.wolverine_incoming_envelopes USING btree (owner_id) WHERE (owner_id <> 0);

CREATE INDEX idx_wolverine_incoming_envelopes_recover ON wolverine.wolverine_incoming_envelopes USING btree (received_at) WHERE (status = 'Incoming' AND owner_id = 0);

CREATE INDEX idx_wolverine_incoming_envelopes_keep_until ON wolverine.wolverine_incoming_envelopes USING btree (keep_until) WHERE (status = 'Handled');
CREATE TABLE IF NOT EXISTS wolverine.wolverine_dead_letters (
    id                   uuid                        NOT NULL,
    execution_time       timestamp with time zone    NULL DEFAULT NULL,
    body                 bytea                       NOT NULL,
    message_type         varchar                     NOT NULL,
    received_at          varchar                     NULL,
    source               varchar                     NULL,
    exception_type       varchar                     NULL,
    exception_message    varchar                     NULL,
    sent_at              timestamp with time zone    NULL,
    replayable           boolean                     NULL,
CONSTRAINT pkey_wolverine_dead_letters_id PRIMARY KEY (id)
);

CREATE INDEX idx_wolverine_dead_letters_replayable ON wolverine.wolverine_dead_letters USING btree (replayable) WHERE (replayable = true);
CREATE TABLE IF NOT EXISTS wolverine.wolverine_nodes (
    id              uuid                        NOT NULL,
    node_number     serial                      NOT NULL,
    description     varchar                     NOT NULL,
    uri             varchar                     NOT NULL,
    started         timestamp with time zone    NOT NULL DEFAULT now(),
    health_check    timestamp with time zone    NOT NULL DEFAULT now(),
    version         varchar                     NULL,
    capabilities    text[]                      NULL,
CONSTRAINT pkey_wolverine_nodes_id PRIMARY KEY (id)
);
CREATE TABLE IF NOT EXISTS wolverine.wolverine_node_assignments (
    id         varchar                     NOT NULL,
    node_id    uuid                        NULL,
    started    timestamp with time zone    NOT NULL DEFAULT now(),
CONSTRAINT pkey_wolverine_node_assignments_id PRIMARY KEY (id)
);

ALTER TABLE wolverine.wolverine_node_assignments
ADD CONSTRAINT fkey_wolverine_node_assignments_node_id FOREIGN KEY(node_id)
REFERENCES wolverine.wolverine_nodes(id)ON DELETE CASCADE
;

CREATE TABLE IF NOT EXISTS wolverine.wolverine_control_queue (
    id              uuid                        NOT NULL,
    message_type    varchar                     NOT NULL,
    node_id         uuid                        NOT NULL,
    body            bytea                       NOT NULL,
    posted          timestamp with time zone    NOT NULL DEFAULT NOW(),
    expires         timestamp with time zone    NULL,
CONSTRAINT pkey_wolverine_control_queue_id PRIMARY KEY (id)
);
CREATE TABLE IF NOT EXISTS wolverine.wolverine_node_records (
    id             serial                      NOT NULL,
    node_number    integer                     NOT NULL,
    event_name     varchar                     NOT NULL,
    timestamp      timestamp with time zone    NOT NULL DEFAULT now(),
    description    varchar                     NULL,
CONSTRAINT pkey_wolverine_node_records_id PRIMARY KEY (id)
);
CREATE TABLE IF NOT EXISTS wolverine.wolverine_agent_restrictions (
    id      uuid       NOT NULL,
    uri     varchar    NOT NULL,
    type    varchar    NOT NULL,
    node    integer    NOT NULL DEFAULT 0,
CONSTRAINT pkey_wolverine_agent_restrictions_id PRIMARY KEY (id)
);

END;
$tran$;

    -- Termina cuentas

    \connect postgres
\endif
