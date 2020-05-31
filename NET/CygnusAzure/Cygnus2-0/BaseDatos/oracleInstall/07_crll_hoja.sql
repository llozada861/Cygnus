CREATE TABLE ll_hoja 
(   
    codigo      NUMBER(15),
    fecha_ini   DATE,
    fecha_fin   DATE,
    descripcion VARCHAR2(100)
);  

ALTER TABLE ll_hoja ADD CONSTRAINT pk_ll_hoja PRIMARY KEY(codigo);

create index idxll_hoja01 on ll_hoja(fecha_fin);
