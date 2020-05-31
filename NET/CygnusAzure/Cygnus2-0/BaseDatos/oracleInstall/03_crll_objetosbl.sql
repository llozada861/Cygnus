create table ll_objetosbl
(
    codigo number(15) PRIMARY KEY,
    objeto varchar2(100),
    orden  varchar2(100),
    usuario varchar2(100),
    bloqueado varchar2(1),
    fecha_bloqueo date,
    fecha_liberacion date,
    fecha_registro date,
    owner varchar2(50),
    fecha_est_lib date
);

create index idxll_objetosbl01 on ll_objetosbl(objeto, bloqueado);
create index idxll_objetosbl02 on ll_objetosbl(orden);
create index idxll_objetosbl03 on ll_objetosbl(usuario, bloqueado);
create index idxll_objetosbl04 on ll_objetosbl(fecha_bloqueo);
create index idxll_objetosbl05 on ll_objetosbl(objeto,orden, bloqueado);
create index idxll_objetosbl06 on ll_objetosbl(objeto,orden,owner, bloqueado);
create index idxll_objetosbl07 on ll_objetosbl(objeto,owner, bloqueado);
create index idxll_objetosbl08 on ll_objetosbl(usuario,objeto, owner);

CREATE SEQUENCE seq_ll_objetosbl
  MINVALUE 1
  START WITH 1
  INCREMENT BY 1
  NOCACHE;

alter table ll_objetosbl add (owner varchar2(50));
alter table ll_objetosbl add (fecha_est_lib date);
