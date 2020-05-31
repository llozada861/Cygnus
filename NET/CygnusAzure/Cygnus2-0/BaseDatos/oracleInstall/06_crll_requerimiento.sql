CREATE TABLE ll_requerimiento 
(
    codigo NUMBER(15),  
    descripcion VARCHAR2(200),
    id_azure NUMBER(15)
);

ALTER TABLE ll_requerimiento ADD CONSTRAINT pk_ll_requerimiento PRIMARY KEY(codigo);

ALTER TABLE ll_requerimiento ADD(estado VARCHAR2(20));

ALTER TABLE ll_requerimiento ADD(fecha_actualiza DATE);

ALTER TABLE ll_requerimiento ADD(usuario varchar2(100));

ALTER TABLE ll_requerimiento ADD(completado NUMBER);

create index idxll_requerimiento01 on ll_requerimiento(usuario);

ALTER TABLE ll_requerimiento ADD(fecha_display DATE);

ALTER TABLE ll_requerimiento ADD(fecha_registro DATE);

create index idxll_requerimiento02 on ll_requerimiento(id_azure,usuario);
create index idxll_requerimiento03 on ll_requerimiento(fecha_display);

ALTER TABLE ll_requerimiento ADD(hist_usuario NUMBER(15));
