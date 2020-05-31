DECLARE
    sbUsuario VARCHAR2(50);
    
    CURSOR cuUsuarios
    IS
        select username
        from all_users
        where username in (select usuario from ll_usuarios);
BEGIN
    OPEN cuUsuarios;
    LOOP
        BEGIN
            sbUsuario := null;
            FETCH cuUsuarios INTO sbUsuario;
            IF(cuUsuarios%NOTFOUND) THEN
                exit;
            END IF;
            --Grants
            execute immediate 'grant execute on pkg_utilmark to '||sbUsuario; 
            execute immediate 'grant select, insert on ll_logapli to '||sbUsuario;
            execute immediate 'grant select on ll_credmark to '||sbUsuario;
        EXCEPTION
            WHEN OTHERS THEN
                dbms_output.put_line(SQLERRM);
        END;
    END LOOP;
    CLOSE cuUsuarios;
	
	execute immediate 'CREATE OR REPLACE PUBLIC SYNONYM pkg_utilmark FOR pkg_utilmark'; 
	execute immediate 'CREATE OR REPLACE PUBLIC SYNONYM ll_logapli FOR ll_logapli';
	execute immediate 'CREATE OR REPLACE PUBLIC SYNONYM ll_credmark FOR ll_credmark';
END;
/
/*select * from dba_tab_privs where table_name = 'LL_CREDMARK'

--PERMISOS todos los usuarios
grant execute on pkg_utilmark to sql_llozada 
grant all on ll_logapli to sql_llozada
grant all on ll_credmark to sql_llozada


/
CREATE OR REPLACE PUBLIC SYNONYM pkg_utilmark FOR flex.pkg_utilmark; 
CREATE OR REPLACE PUBLIC SYNONYM ll_logapli FOR flex.ll_logapli;
/
select * from dba_users
where username like 'SQL%'
and account_status = 'OPEN'
/

Usuarios SQL:

'SQL_ABETANB',
'SQL_CVILLARR',
'SQL_ARIVERIV',
'SQL_ETACHEJI',
'SQL_EHENAOHE',
'SQL_IOSORIOF',
'SQL_JRINCONG',
'SQL_JMORALEM',
'SQL_JMORENRI',
'SQL_JHERNASI',
'SQL_JSUAREZM',
'SQL_JACOSTAC',
'SQL_JTABATAB',
'SQL_JVEGAPER',
'SQL_JMARMARI',
'SQL_JARISTZ',
'SQL_JCATUCHE',
'SQL_JJARAMVA',
'SQL_JRADAG',
'SQL_LANGELES',
'SQL_LBLANQUI',
'SQL_LOCAMPRE',
'SQL_LLOZADA',
'SQL_LMACIASJ',
'SQL_MCASTANC',
'SQL_MALVARAS',
'SQL_OSUAREZO',
'SQL_ROLARTEP',
'SQL_TRUIZDIA',
'SQL_VJIMENEC',
'SQL_ZMORENOR',
'SQL_JVELAV'
*/