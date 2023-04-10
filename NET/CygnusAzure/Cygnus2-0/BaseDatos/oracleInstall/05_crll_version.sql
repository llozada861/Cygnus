create table ll_version
(
    version     varchar2(50) PRIMARY KEY,
    fecha_ini   date,
    fecha_fin   date,
    objeto      blob
);

GRANT DELETE, INSERT, SELECT, UPDATE ON ll_version TO SYSTEM_OBJ_PRIVS_ROLE;
CREATE OR REPLACE PUBLIC SYNONYM ll_version FOR FLEX.ll_version;

declare
    sbUser varchar2(2000);
    cursor cuUsers
    is
        select username from dba_users where username like 'SQL_%'
        and account_status = 'OPEN';
begin
    open cuUsers;
    loop
        fetch cuUsers into sbUser;
        exit when cuUsers%NOTFOUND;
        
        execute IMMEDIATE 'grant select, insert on ll_version to '||sbUser;
        
    end loop;
    close cuUsers;
end;        