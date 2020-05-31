create table ll_version
(
    version     varchar2(50) PRIMARY KEY,
    fecha_ini   date,
    fecha_fin   date,
    objeto      blob
);

GRANT DELETE, INSERT, SELECT, UPDATE ON ll_version TO SYSTEM_OBJ_PRIVS_ROLE;