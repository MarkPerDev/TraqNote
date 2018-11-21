-- Table: public.post

-- DROP TABLE public.post;

CREATE TABLE public.post
(
  id integer NOT NULL DEFAULT nextval(('"post_id_seq"'::text)::regclass),
  title text,
  content text,
  topic_id integer DEFAULT '-1'::integer,
  created_on date,
  modified_on date,
  CONSTRAINT users_pkey PRIMARY KEY (id),
  CONSTRAINT topic_id_fkey FOREIGN KEY (topic_id)
      REFERENCES public.topic (id) MATCH SIMPLE
      ON UPDATE NO ACTION ON DELETE NO ACTION
)
WITH (
  OIDS=FALSE
);
ALTER TABLE public.post
  OWNER TO postgres;

CREATE SEQUENCE post_id_seq
start 1
increment 1;