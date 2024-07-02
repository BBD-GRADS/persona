DO
$do$
BEGIN
   IF NOT EXISTS (
      SELECT 
      FROM   pg_catalog.pg_database 
      WHERE  datname = 'persona_db') THEN
      EXECUTE 'CREATE DATABASE persona_db';
   END IF;
END
$do$;