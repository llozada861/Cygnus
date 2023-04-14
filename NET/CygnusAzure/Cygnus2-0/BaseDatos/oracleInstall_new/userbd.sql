create table cy_userbd
(
    user_        varchar2(50),
    password_    varchar2(1000),
    basedatos   varchar2(100),
    servidor    varchar2(100),
    puerto      varchar2(100)
);

GRANT DELETE, INSERT, SELECT, UPDATE ON cy_userbd TO SYSTEM_OBJ_PRIVS_ROLE;
CREATE OR REPLACE PUBLIC SYNONYM cy_userbd FOR FLEX.cy_userbd;

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
        
        execute IMMEDIATE 'grant select on cy_userbd to '||sbUser;
        
    end loop;
    close cuUsers;
end;        