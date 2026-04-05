--
-- PostgreSQL database dump
--

-- Dumped from database version 16.2
-- Dumped by pg_dump version 16.2

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: semenova_schema; Type: SCHEMA; Schema: -; Owner: semenova_app
--

CREATE SCHEMA semenova_schema;


ALTER SCHEMA semenova_schema OWNER TO semenova_app;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: partner_types; Type: TABLE; Schema: semenova_schema; Owner: postgres
--

CREATE TABLE semenova_schema.partner_types (
    id integer NOT NULL,
    name character varying(100) NOT NULL
);


ALTER TABLE semenova_schema.partner_types OWNER TO postgres;

--
-- Name: partner_types_id_seq; Type: SEQUENCE; Schema: semenova_schema; Owner: postgres
--

CREATE SEQUENCE semenova_schema.partner_types_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE semenova_schema.partner_types_id_seq OWNER TO postgres;

--
-- Name: partner_types_id_seq; Type: SEQUENCE OWNED BY; Schema: semenova_schema; Owner: postgres
--

ALTER SEQUENCE semenova_schema.partner_types_id_seq OWNED BY semenova_schema.partner_types.id;


--
-- Name: partners; Type: TABLE; Schema: semenova_schema; Owner: postgres
--

CREATE TABLE semenova_schema.partners (
    id integer NOT NULL,
    type_id integer NOT NULL,
    name character varying(200) NOT NULL,
    legal_adress character varying(500),
    inn character varying(10),
    director_fullname character varying(200),
    phone character varying(20),
    email character varying(100),
    logo_path character varying(500),
    rating integer DEFAULT 0 NOT NULL,
    CONSTRAINT partners_rating_check CHECK ((rating >= 0))
);


ALTER TABLE semenova_schema.partners OWNER TO postgres;

--
-- Name: partners_id_seq; Type: SEQUENCE; Schema: semenova_schema; Owner: postgres
--

CREATE SEQUENCE semenova_schema.partners_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE semenova_schema.partners_id_seq OWNER TO postgres;

--
-- Name: partners_id_seq; Type: SEQUENCE OWNED BY; Schema: semenova_schema; Owner: postgres
--

ALTER SEQUENCE semenova_schema.partners_id_seq OWNED BY semenova_schema.partners.id;


--
-- Name: products; Type: TABLE; Schema: semenova_schema; Owner: postgres
--

CREATE TABLE semenova_schema.products (
    id integer NOT NULL,
    article character varying(50) NOT NULL,
    name character varying(200) NOT NULL,
    description text,
    unit character varying(20) NOT NULL,
    price numeric(10,2) NOT NULL,
    CONSTRAINT products_price_check CHECK ((price > (0)::numeric))
);


ALTER TABLE semenova_schema.products OWNER TO postgres;

--
-- Name: products_id_seq; Type: SEQUENCE; Schema: semenova_schema; Owner: postgres
--

CREATE SEQUENCE semenova_schema.products_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE semenova_schema.products_id_seq OWNER TO postgres;

--
-- Name: products_id_seq; Type: SEQUENCE OWNED BY; Schema: semenova_schema; Owner: postgres
--

ALTER SEQUENCE semenova_schema.products_id_seq OWNED BY semenova_schema.products.id;


--
-- Name: sales_history; Type: TABLE; Schema: semenova_schema; Owner: postgres
--

CREATE TABLE semenova_schema.sales_history (
    id integer NOT NULL,
    partner_id integer NOT NULL,
    product_id integer NOT NULL,
    quantity integer NOT NULL,
    sale_date date NOT NULL,
    CONSTRAINT sales_history_quantity_check CHECK ((quantity > 0))
);


ALTER TABLE semenova_schema.sales_history OWNER TO postgres;

--
-- Name: sales_history_id_seq; Type: SEQUENCE; Schema: semenova_schema; Owner: postgres
--

CREATE SEQUENCE semenova_schema.sales_history_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE semenova_schema.sales_history_id_seq OWNER TO postgres;

--
-- Name: sales_history_id_seq; Type: SEQUENCE OWNED BY; Schema: semenova_schema; Owner: postgres
--

ALTER SEQUENCE semenova_schema.sales_history_id_seq OWNED BY semenova_schema.sales_history.id;


--
-- Name: partner_types id; Type: DEFAULT; Schema: semenova_schema; Owner: postgres
--

ALTER TABLE ONLY semenova_schema.partner_types ALTER COLUMN id SET DEFAULT nextval('semenova_schema.partner_types_id_seq'::regclass);


--
-- Name: partners id; Type: DEFAULT; Schema: semenova_schema; Owner: postgres
--

ALTER TABLE ONLY semenova_schema.partners ALTER COLUMN id SET DEFAULT nextval('semenova_schema.partners_id_seq'::regclass);


--
-- Name: products id; Type: DEFAULT; Schema: semenova_schema; Owner: postgres
--

ALTER TABLE ONLY semenova_schema.products ALTER COLUMN id SET DEFAULT nextval('semenova_schema.products_id_seq'::regclass);


--
-- Name: sales_history id; Type: DEFAULT; Schema: semenova_schema; Owner: postgres
--

ALTER TABLE ONLY semenova_schema.sales_history ALTER COLUMN id SET DEFAULT nextval('semenova_schema.sales_history_id_seq'::regclass);


--
-- Data for Name: partner_types; Type: TABLE DATA; Schema: semenova_schema; Owner: postgres
--

COPY semenova_schema.partner_types (id, name) FROM stdin;
1	розничный магазн
2	оптовый магазин
3	интернет-магазин
4	дистрибьютор
\.


--
-- Data for Name: partners; Type: TABLE DATA; Schema: semenova_schema; Owner: postgres
--

COPY semenova_schema.partners (id, type_id, name, legal_adress, inn, director_fullname, phone, email, logo_path, rating) FROM stdin;
1	1	магазин уютный дом	г. москва ул. ленина 10	7701123456	петров петр перович	84951234567	utdon@mail.ru	\N	85
2	3	ооо строймаркет	г. москва ул. пушкина 10	7708923456	иванов иван сергеевич	84951278567	info@stroymarket.ru	\N	92
3	2	ип-напольные покрытие	г. екатеринбург  ул. малышева 10	669874	сергеева мария константиновна	8469878567	m_serger@yandex.ru	\N	78
4	4	ввввв		1254789654	ввввввв	8965478952	test@hah.ru		4
\.


--
-- Data for Name: products; Type: TABLE DATA; Schema: semenova_schema; Owner: postgres
--

COPY semenova_schema.products (id, article, name, description, unit, price) FROM stdin;
1	vp-lam-001	ламинат дуб 	ламинат 32 класс, толщина 8 мм	м2	850.00
2	vp-lam-002	ламинат ореховый 	ламинат 33 класс, толщина 12 мм	м2	1200.00
3	vp-rar-001	паркетная доска ясень	массивная доска 15 мм	м2	2550.00
4	vp-rar-002	паркетная доска дуб 	массивная доска 15 мм, термообработка	м2	3250.00
5	vp-pup-001	вавиловая плитка 	плитка пвх	м2	900.00
6	vp-pup-002	вавиловый ламинат 	камень-керамика	м2	1100.00
\.


--
-- Data for Name: sales_history; Type: TABLE DATA; Schema: semenova_schema; Owner: postgres
--

COPY semenova_schema.sales_history (id, partner_id, product_id, quantity, sale_date) FROM stdin;
1	1	1	1500	2025-01-15
2	1	2	800	2025-01-20
3	1	3	200	2025-02-10
4	1	1	1200	2025-02-25
5	1	2	600	2025-03-05
6	2	1	5000	2025-01-10
7	2	2	3500	2025-01-15
8	2	4	1000	2025-02-01
9	2	5	2000	2025-02-15
10	2	1	4500	2025-03-01
11	3	1	300	2025-01-25
12	3	3	50	2025-02-05
13	3	5	150	2025-02-20
14	4	5	15	2026-03-15
\.


--
-- Name: partner_types_id_seq; Type: SEQUENCE SET; Schema: semenova_schema; Owner: postgres
--

SELECT pg_catalog.setval('semenova_schema.partner_types_id_seq', 4, true);


--
-- Name: partners_id_seq; Type: SEQUENCE SET; Schema: semenova_schema; Owner: postgres
--

SELECT pg_catalog.setval('semenova_schema.partners_id_seq', 4, true);


--
-- Name: products_id_seq; Type: SEQUENCE SET; Schema: semenova_schema; Owner: postgres
--

SELECT pg_catalog.setval('semenova_schema.products_id_seq', 6, true);


--
-- Name: sales_history_id_seq; Type: SEQUENCE SET; Schema: semenova_schema; Owner: postgres
--

SELECT pg_catalog.setval('semenova_schema.sales_history_id_seq', 14, true);


--
-- Name: partner_types partner_types_pkey; Type: CONSTRAINT; Schema: semenova_schema; Owner: postgres
--

ALTER TABLE ONLY semenova_schema.partner_types
    ADD CONSTRAINT partner_types_pkey PRIMARY KEY (id);


--
-- Name: partners partners_inn_key; Type: CONSTRAINT; Schema: semenova_schema; Owner: postgres
--

ALTER TABLE ONLY semenova_schema.partners
    ADD CONSTRAINT partners_inn_key UNIQUE (inn);


--
-- Name: partners partners_pkey; Type: CONSTRAINT; Schema: semenova_schema; Owner: postgres
--

ALTER TABLE ONLY semenova_schema.partners
    ADD CONSTRAINT partners_pkey PRIMARY KEY (id);


--
-- Name: products products_article_key; Type: CONSTRAINT; Schema: semenova_schema; Owner: postgres
--

ALTER TABLE ONLY semenova_schema.products
    ADD CONSTRAINT products_article_key UNIQUE (article);


--
-- Name: products products_pkey; Type: CONSTRAINT; Schema: semenova_schema; Owner: postgres
--

ALTER TABLE ONLY semenova_schema.products
    ADD CONSTRAINT products_pkey PRIMARY KEY (id);


--
-- Name: sales_history sales_history_pkey; Type: CONSTRAINT; Schema: semenova_schema; Owner: postgres
--

ALTER TABLE ONLY semenova_schema.sales_history
    ADD CONSTRAINT sales_history_pkey PRIMARY KEY (id);


--
-- Name: idx_partner_inn; Type: INDEX; Schema: semenova_schema; Owner: postgres
--

CREATE INDEX idx_partner_inn ON semenova_schema.partners USING btree (inn);


--
-- Name: idx_partner_name; Type: INDEX; Schema: semenova_schema; Owner: postgres
--

CREATE INDEX idx_partner_name ON semenova_schema.partners USING btree (name);


--
-- Name: idx_sales_date; Type: INDEX; Schema: semenova_schema; Owner: postgres
--

CREATE INDEX idx_sales_date ON semenova_schema.sales_history USING btree (sale_date);


--
-- Name: idx_sales_partner; Type: INDEX; Schema: semenova_schema; Owner: postgres
--

CREATE INDEX idx_sales_partner ON semenova_schema.sales_history USING btree (partner_id);


--
-- Name: partners partners_type_id_fkey; Type: FK CONSTRAINT; Schema: semenova_schema; Owner: postgres
--

ALTER TABLE ONLY semenova_schema.partners
    ADD CONSTRAINT partners_type_id_fkey FOREIGN KEY (type_id) REFERENCES semenova_schema.partner_types(id);


--
-- Name: sales_history sales_history_partner_id_fkey; Type: FK CONSTRAINT; Schema: semenova_schema; Owner: postgres
--

ALTER TABLE ONLY semenova_schema.sales_history
    ADD CONSTRAINT sales_history_partner_id_fkey FOREIGN KEY (partner_id) REFERENCES semenova_schema.partners(id);


--
-- Name: sales_history sales_history_product_id_fkey; Type: FK CONSTRAINT; Schema: semenova_schema; Owner: postgres
--

ALTER TABLE ONLY semenova_schema.sales_history
    ADD CONSTRAINT sales_history_product_id_fkey FOREIGN KEY (product_id) REFERENCES semenova_schema.products(id);


--
-- Name: TABLE partner_types; Type: ACL; Schema: semenova_schema; Owner: postgres
--

GRANT ALL ON TABLE semenova_schema.partner_types TO semenova_app;


--
-- Name: SEQUENCE partner_types_id_seq; Type: ACL; Schema: semenova_schema; Owner: postgres
--

GRANT ALL ON SEQUENCE semenova_schema.partner_types_id_seq TO semenova_app;


--
-- Name: TABLE partners; Type: ACL; Schema: semenova_schema; Owner: postgres
--

GRANT ALL ON TABLE semenova_schema.partners TO semenova_app;


--
-- Name: SEQUENCE partners_id_seq; Type: ACL; Schema: semenova_schema; Owner: postgres
--

GRANT ALL ON SEQUENCE semenova_schema.partners_id_seq TO semenova_app;


--
-- Name: TABLE products; Type: ACL; Schema: semenova_schema; Owner: postgres
--

GRANT ALL ON TABLE semenova_schema.products TO semenova_app;


--
-- Name: SEQUENCE products_id_seq; Type: ACL; Schema: semenova_schema; Owner: postgres
--

GRANT ALL ON SEQUENCE semenova_schema.products_id_seq TO semenova_app;


--
-- Name: TABLE sales_history; Type: ACL; Schema: semenova_schema; Owner: postgres
--

GRANT ALL ON TABLE semenova_schema.sales_history TO semenova_app;


--
-- Name: SEQUENCE sales_history_id_seq; Type: ACL; Schema: semenova_schema; Owner: postgres
--

GRANT ALL ON SEQUENCE semenova_schema.sales_history_id_seq TO semenova_app;


--
-- PostgreSQL database dump complete
--

