SET appinfo ON
SET echo OFF
SET serveroutput ON
SET timing OFF
SET verify OFF
SET heading OFF 
SET feedback ON

COLUMN file_script new_val file_script;
COLUMN instance_name new_val instance_name;
COLUMN fecha_exec new_val fecha_exec;
COLUMN spool_file new_val spool_file;
COLUMN usuario_exec new_val usuario_exec;
COLUMN usuario_os new_val usuario_os;
DEFINE OC=WOXXX

SELECT SUBSTR(SYS_CONTEXT('USERENV','MODULE'),INSTR(SYS_CONTEXT('USERENV','MODULE'),'@')+2) file_script,
       SYS_CONTEXT('USERENV', 'DB_NAME') instance_name,
       TO_CHAR(SYSDATE, 'DD-MM-YYYY HH24:MI:SS') fecha_exec,
       USER usuario_exec,
       SYS_CONTEXT('USERENV', 'OS_USER') usuario_os,
       '&OC'||'_'||'apl_'||USER||'_'||SUBSTR(SYS_CONTEXT('USERENV', 'DB_NAME'),1,4)||
       '_'||TO_CHAR(SYSDATE, 'YYYYMMDD_HH24MISS')  spool_file  FROM DUAL;

SPOOL &spool_file..log
PROMPT
PROMPT =========================================
PROMPT  ****   Información de Ejecución    ****
PROMPT =========================================
PROMPT Archivo ejecutado: &file_script
PROMPT Instancia        : &instance_name 
PROMPT Fecha ejecución  : &fecha_exec
PROMPT Usuario DB       : &usuario_exec
PROMPT Usuario O.S      : &usuario_os
PROMPT OC               : &OC
PROMPT ========================================= 
PROMPT
PROMPT **** Aplica de objetos **** 

PROMPT INICIA PROCESO ....
prompt "Aplicando /01_crll_credmark.sql"
@./01_crll_credmark.sql

prompt "Aplicando /02_crll_logapli.sql"
@./02_crll_logapli.sql

prompt "Aplicando /03_crll_objetosbl.sql"
@./03_crll_objetosbl.sql

prompt "Aplicando /04_crll_usuarios.sql"
@./04_crll_usuarios.sql

prompt "Aplicando /05_crll_version.sql"
@./05_crll_version.sql

prompt "Aplicando /06_crll_requerimiento.sql"
@./06_crll_requerimiento.sql

prompt "Aplicando /07_crll_hoja.sql"
@./07_crll_hoja.sql

prompt "Aplicando /08_crll_horashoja.sql"
@./08_crll_horashoja.sql

prompt "Aplicando /08_crll_estandar.sql"
@./08_crll_estandar.sql

prompt "Aplicando /09_crSequences.sql"
@./09_crSequences.sql

prompt "Aplicando /11_crseq_ll_logapli.sql"
@./11_crseq_ll_logapli.sql

prompt "Aplicando /12_pkg_utilmark.sql"
@./12_pkg_utilmark.sql

prompt "Aplicando /p_DC_GeneraAudit.sql.sql"
@./p_DC_GeneraAudit.sql.sql

prompt "Aplicando /14_Grants_usuarios_epm_cygnus.sql"
@./14_Grants_usuarios_epm_cygnus.sql

prompt "Aplicando /15_insepm_parametrCY_GRUPO_CORREO.sql"
@./15_insepm_parametrCY_GRUPO_CORREO.sql

prompt "Aplicando /16_insepm_parametrEXC_MENSAJE_MARK.sql"
@./16_insepm_parametrEXC_MENSAJE_MARK.sql

prompt "Aplicando /17_reiniciaSecuencia.sql"
@./17_reiniciaSecuencia.sql

prompt "Aplicando /18_insll_credmark.sql"
@./18_insll_credmark.sql
commit;

prompt "Aplicando /19_insll_usuarios.sql"
@./19_insll_usuarios.sql
commit;

prompt "Aplicando /20_insll_objetosbl.sql"
@./20_insll_objetosbl.sql
commit;

prompt "Aplicando /21_inssa_user.sql"
@./21_inssa_user.sql
commit;

prompt "Aplicando /22_insge_person.sql"
@./22_insge_person.sql
commit;

prompt "Aplicando /23_insll_hojas.sql"
@./23_insll_hojas.sql
commit;

prompt "Aplicando /24_insll_requerimiento.sql"
@./24_insll_requerimiento.sql
commit;

prompt "Aplicando /25_insll_horashoja.sql"
@./25_insll_horashoja.sql
commit;

prompt "Aplicando /26_crea_usua_sql_asig_permisos_mvm.sql"
@./26_crea_usua_sql_asig_permisos_mvm.sql

PROMPT **** Termina aplica de objetos **** 

SPOOL OFF
SET SERVEROUTPUT OFF