-- Table: public.topic

-- DROP TABLE public.topic;

CREATE TABLE public.topic
(
  id integer NOT NULL DEFAULT nextval(('"topic_id_seq"'::text)::regclass),
  topic_name text,
  created_on date,
  modified_on date,
  CONSTRAINT topic_pkey PRIMARY KEY (id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.topic
  OWNER TO postgres;


CREATE SEQUENCE topic_id_seq
start 1
increment 1;
