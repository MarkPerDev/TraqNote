---- Table: public.user_audit

---- DROP TABLE public.user_audit;

--CREATE TABLE public.user_audit
--(
--  id integer NOT NULL DEFAULT nextval('user_audit_id_seq'::regclass),
--  action_timestamp timestamp without time zone NOT NULL DEFAULT now(),
--  actor text NOT NULL DEFAULT "session_user"(),
--  action text NOT NULL,
--  old_record hstore,
--  new_record hstore,
--  CONSTRAINT user_audit_pkey PRIMARY KEY (id)
--)
--WITH (
--  OIDS=FALSE
--);
--ALTER TABLE public.user_audit
--  OWNER TO postgres;
