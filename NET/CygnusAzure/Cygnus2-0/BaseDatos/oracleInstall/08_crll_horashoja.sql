CREATE TABLE ll_horashoja
(
    codigo NUMBER(15),
    id_hoja NUMBER(15), 
    fecha_registro DATE,
    usuario VARCHAR2(50),
    requerimiento NUMBER(15),
	lunes NUMBER(3),
	martes NUMBER(3),
	miercoles NUMBER(3),
	jueves NUMBER(3),
	viernes NUMBER(3),
	sabado NUMBER(3),
	domingo NUMBER(3)
);

ALTER TABLE ll_horashoja ADD(fecha_actualiza DATE);
ALTER TABLE ll_horashoja ADD(observacion varchar2(4000));

ALTER TABLE ll_horashoja ADD CONSTRAINT pk_ll_horashojas PRIMARY KEY(codigo);
ALTER TABLE ll_horashoja ADD CONSTRAINT fk_ll_horashoja02 Foreign KEY(requerimiento) REFERENCES ll_requerimiento(codigo);
ALTER TABLE ll_horashoja ADD CONSTRAINT fk_ll_horashoja FOREIGN KEY(ID_hoja) REFERENCES ll_hoja(codigo);

create index idxll_horashoja01 on ll_horashoja(requerimiento);
create index idxll_horashoja02 on ll_horashoja(requerimiento,usuario,id_hoja);
create index idxll_horashoja03 on ll_horashoja(id_hoja,usuario);

ALTER TABLE ll_horashoja MODIFY (lunes NUMBER(4,1));
ALTER TABLE ll_horashoja MODIFY (martes NUMBER(4,1));
ALTER TABLE ll_horashoja MODIFY (miercoles NUMBER(4,1));
ALTER TABLE ll_horashoja MODIFY (jueves NUMBER(4,1));
ALTER TABLE ll_horashoja MODIFY (viernes NUMBER(4,1));
ALTER TABLE ll_horashoja MODIFY (sabado NUMBER(4,1));
ALTER TABLE ll_horashoja MODIFY (domingo NUMBER(4,1));
