\connect postgres

-- elimina las bases si existen
-- force cierra las conexiones activas para poder eliminar
 
DROP DATABASE IF EXISTS :"clientes" WITH (FORCE);
DROP DATABASE IF EXISTS :"cuentas" WITH (FORCE);