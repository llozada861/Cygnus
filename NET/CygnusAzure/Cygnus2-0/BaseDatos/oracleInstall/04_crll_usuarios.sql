create table ll_usuarios
(
    usuario varchar2(100) PRIMARY KEY,
    rol     varchar2(2000)
);

alter table ll_usuarios add email varchar2(100);
ALTER TABLE ll_usuarios ADD(usuarioAzure VARCHAR2(250));

GRANT DELETE, INSERT, SELECT, UPDATE ON ll_usuarios TO SYSTEM_OBJ_PRIVS_ROLE;
