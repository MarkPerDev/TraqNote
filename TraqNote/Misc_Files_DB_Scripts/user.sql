---- Table: public."user"

---- DROP TABLE public."user";

--CREATE TABLE public."user"
--(
--  user_id integer NOT NULL DEFAULT nextval(('"user_user_id_seq"'::text)::regclass),
--  user_name text NOT NULL,
--  password bytea NOT NULL,
--  session_data text,
--  first_name text,
--  last_name text,
--  phone text,
--  email_address text,
--  comments text,
--  active boolean DEFAULT true,
--  created_by text,
--  created_on timestamp without time zone,
--  modified_by text,
--  modified_on timestamp without time zone,
--  cell_phone text,
--  password_set_date timestamp without time zone NOT NULL,
--  grace_logins_used smallint DEFAULT 0,
--  consecutive_login_failures smallint NOT NULL DEFAULT 0,
--  autologin boolean NOT NULL DEFAULT false,
--  activation_code text NOT NULL,
--  CONSTRAINT user_pkey PRIMARY KEY (user_id)
--)
--WITH (
--  OIDS=FALSE
--);
--ALTER TABLE public."user"
--  OWNER TO postgres;

---- Index: public.user_user_name

---- DROP INDEX public.user_user_name;

--CREATE UNIQUE INDEX user_user_name
--  ON public."user"
--  USING btree
--  (user_name COLLATE pg_catalog."default");


---- Trigger: user_audit_trig on public."user"

---- DROP TRIGGER user_audit_trig ON public."user";

--CREATE TRIGGER user_audit_trig
--  BEFORE INSERT OR UPDATE OR DELETE
--  ON public."user"
--  FOR EACH ROW
--  EXECUTE PROCEDURE public.user_audit_trig();

