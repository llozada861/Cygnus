column dt new_value vdt
column db new_value vdb
select to_char(sysdate,'yyyymmdd_hh24miss') dt, sys_context('userenv','db_name') db from dual;
spool &vdb._logSAO<numero_oc>_&vdt..txt
set serveroutput on
set define off
set timing on
execute dbms_application_info.set_action('APLICANDO SAO<numero_oc>');
prompt Inicio Proceso!!
select to_char(sysdate,'yyyy-mm-dd hh:mi:ss p.m.') fecha_inicio from dual;

prompt "------------------------------------------------------"
prompt " APLICANDO <nombre_aplica> "
prompt "------------------------------------------------------"

prompt "Aplicando SAO<numero_oc>/<objeto>"
@./SAO<numero_oc>/<objeto>

prompt "------------------------------------------------------"
prompt " FIN DE APLICA <nombre_aplica> "
prompt "------------------------------------------------------"

select to_char(sysdate,'yyyy-mm-dd hh:mi:ss p.m.') fecha_fin from dual;
prompt Fin Proceso!!
set timing off
set serveroutput off
set define on
spool off
