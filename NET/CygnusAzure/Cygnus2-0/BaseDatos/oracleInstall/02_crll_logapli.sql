create table ll_logapli
(
    codigo NUMBER(15) PRIMARY KEY,
    fecha_registro DATE,
    objeto VARCHAR2(1000),
    maquina VARCHAR2(100),
    usuario VARCHAR2(100),
    tipo  VARCHAR2(100)
);

create index idxll_logapli01 ON ll_logapli(usuario);

create index idxll_logapli02 ON ll_logapli(objeto);

create index idxll_logapli03 ON ll_logapli(maquina);

create index idxll_logapli04 ON ll_logapli(objeto, usuario);

create index idxll_logapli05 ON ll_logapli(fecha_registro);

alter table ll_logapli add (accion VARCHAR2(1),objetos_inv NUMBER(15));

alter table ll_logapli add(esquema varchar2(100));

--alter table ll_logapli drop column objetos_inv