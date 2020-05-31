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
DEFINE OC=Fenix

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
prompt "Aplicando /crll_credmark.sql"
@./crll_credmark.sql

prompt "Aplicando /crll_logapli.sql"
@./crll_logapli.sql

prompt "Aplicando /crll_objetosbl.sql"
@./crll_objetosbl.sql

prompt "Aplicando /crll_usuarios.sql"
@./crll_usuarios.sql

prompt "Aplicando /crll_version.sql"
@./crll_version.sql

prompt "Aplicando /01_crll_requerimiento.sql"
@./01_crll_requerimiento.sql

prompt "Aplicando /02_crll_hoja.sql"
@./02_crll_hoja.sql

prompt "Aplicando /03_crll_horashoja.sql"
@./03_crll_horashoja.sql

prompt "Aplicando /crSequences.sql"
@./crSequences.sql

prompt "Aplicando /crsequencehoja.sql"
@./crsequencehoja.sql

prompt "Aplicando /crseq_ll_logapli.sql"
@./crseq_ll_logapli.sql

prompt "Aplicando /inshojas.sql"
@./inshojas.sql

prompt "Aplicando /pkg_utilmark_fenix.sql"
@./pkg_utilmark_fenix.sql

prompt "Aplicando /Grants_usuarios_epm_cygnus.sql"
@./Grants_usuarios_epm_cygnus.sql

PROMPT **** Termina aplica de objetos **** 

SPOOL OFF
SET SERVEROUTPUT OFF
