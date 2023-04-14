SET SERVEROUTPUT ON
/*
  
   Autor:        Llozada
   Fecha:        16-01-2019 
   Descripcion:  Script para creacion de usuarios SQL_<LOGIN_RED>                 

*/
DECLARE
    ---Archivos planos de salida
    sbArchivoSalidaDatos              VARCHAR2 (200);
    iflFileHandle_out                 PKG_EPM_GestionArchivos.TyRcArchivo;     -- Tipo archivo del sistema
    
    ---Archivos planos de salida
    sbArchivoSalidaDatos_err          VARCHAR2 (200);
    iflFileHandle_err                 PKG_EPM_GestionArchivos.TyRcArchivo;     -- Tipo archivo del sistema

    -- Se definen las variables para calcular el tiempo que demora el procedimiento.
    dtTiempoInicia     TIMESTAMP(9);
    dtTiempoTermina    TIMESTAMP(9);
    dtTiempoUtilizado  INTERVAL DAY TO SECOND(9);

    sbNumeroOC         VARCHAR2(20) := 'RESULTADO';
    sbSepara           VARCHAR2(1)  := '|';
    sbSepaFile         VARCHAR2(1)  := '|';
    sbRuta             VARCHAR2(100) := '/output/traza';
    sbUsuarioSql       VARCHAR2(50);

    nuTotal             PLS_INTEGER := 0;
    nuOk                PLS_INTEGER := 0;
    nuErr               PLS_INTEGER := 0;
    nuCicloEsp          PLS_INTEGER := 0;
    
    sbCreaUser          VARCHAR2(2000) := 'CREATE USER %usuario IDENTIFIED BY %pass DEFAULT TABLESPACE USERS TEMPORARY TABLESPACE TEMP PROFILE DEFAULT';
    sbTableSpace        VARCHAR2(200)  := 'GRANT UNLIMITED TABLESPACE TO %usuario';
    
    TYPE tyUsuarios IS TABLE OF VARCHAR2(40) ;     
        
    TYPE typermisos IS TABLE OF VARCHAR2(400);                                 
    tblPermisos typermisos  := typermisos(
                                             --usuarios de creacion de objetos
                                            --'GRANT ACCESO_OBJETOS TO %usuario',
                                            --'GRANT DESCRIBE_OBJETO TO %usuario',
                                            --'GRANT CONSULTA_TODAS_LAS_TABLAS TO %usuario',
                                            'GRANT CREATE TYPE TO %usuario',
                                            'GRANT CREATE VIEW TO %usuario',
                                            'GRANT CREATE TABLE TO %usuario',
                                            'GRANT ALTER SESSION TO %usuario',
                                            'GRANT CREATE SESSION TO %usuario',
                                            'GRANT CREATE SYNONYM TO %usuario',
                                            'GRANT CREATE TRIGGER TO %usuario',
                                            'GRANT CREATE SEQUENCE TO %usuario',
                                            'GRANT CREATE PROCEDURE TO %usuario',
                                            'grant SELECT ANY DICTIONARY to %usuario',
                                            'GRANT SELECT_CATALOG_ROLE,CONSULTA_TODAS_LAS_TABLAS,ROL_AUDITORIA_OPEN,CONSULTA_OPENSIRIUS TO %usuario'
                                     );
  
      CURSOR CuExistUsuario
      (
        isbUsuario IN VARCHAR2
      )
      IS
      SELECT COUNT(1) 
        FROM dba_users
       WHERE username = isbUsuario; 
      
      CURSOR cuTables
      (
        isbUsuaSQL IN VARCHAR2
      )
      IS
      select 'GRANT SELECT, DELETE, INSERT, UPDATE ON ' || owner || '.' || table_name ||' TO '||isbUsuaSQL AS sentencia
        from dba_tables tb
       where owner like 'FLEX%'
         AND IOT_NAME IS NULL 
         AND NOT EXISTS (SELECT 1 FROM dba_external_tables WHERE OWNER LIKE 'FLEX' AND TABLE_NAME = tb.table_name );

      CURSOR cuTablesView
      (
        isbUsuaSQL IN VARCHAR2
      )
      IS     
      SELECT 'GRANT SELECT ON ' || owner || '.' || object_name ||' TO '||isbUsuaSQL AS sentencia
        FROM dba_objects
       WHERE owner like 'FLEX%' 
         AND object_type in ('TABLE','VIEW');
         
      CURSOR cuPaPrFu
      (
        isbUsuaSQL IN VARCHAR2
      )
      IS 
      select 'GRANT EXECUTE ON ' || owner || '.' || object_name ||' TO '||isbUsuaSQL AS sentencia
        from dba_objects
       where owner like 'FLEX%' 
         and object_type in ('PROCEDURE','FUNCTION','PACKAGE');

      sbSentencia  VARCHAR2(4000);
      nuExiste     NUMBER;

    sbPass    varchar2(100);
BEGIN

    --almacena la hora de inicio del proceso
    dtTiempoInicia := SYSTIMESTAMP;
    
    sbUsuarioSql := '[PAR_USUARIO_SQL]';
    sbPass       := '[PAR_PASS_SQL]';
    
    sbArchivoSalidaDatos      := sbUsuarioSql||'_CREA_USUA_'||TO_CHAR(SYSDATE,'yyyymmdd_hh24miss')||'.txt';
    iflFileHandle_out         := PKG_EPM_GestionArchivos.fflAbrirArchivo (sbRuta, sbArchivoSalidaDatos, 'w');
    sbArchivoSalidaDatos_err  := sbUsuarioSql||'_CREA_USUA_ERR_'||TO_CHAR(SYSDATE,'yyyymmdd_hh24miss')||'.txt';
    iflFileHandle_err         := PKG_EPM_GestionArchivos.fflAbrirArchivo (sbRuta, sbArchivoSalidaDatos_err, 'w');

    PKG_EPM_GestionArchivos.PUT_LINE(iflFileHandle_out, 'CREACION USUARIO SQL Y PERMISOS'); 
        
    OPEN CuExistUsuario(sbUsuarioSql);
      FETCH CuExistUsuario INTO nuExiste;          
    CLOSE CuExistUsuario;
    
    IF nuExiste = 0 THEN
       sbSentencia := REPLACE(sbCreaUser,'%usuario',sbUsuarioSql);
       sbSentencia := REPLACE(sbSentencia,'%pass',sbPass);
       EXECUTE IMMEDIATE sbSentencia;
       sbSentencia := REPLACE(sbTableSpace,'%usuario',sbUsuarioSql);
       EXECUTE IMMEDIATE sbSentencia;
       PKG_EPM_GestionArchivos.PUT_LINE(iflFileHandle_out,sbSentencia||';');
    END IF;    

    PKG_EPM_GestionArchivos.PUT_LINE(iflFileHandle_out, 'USUARIO:'||sbUsuarioSql); 
    DBMS_OUTPUT.PUT_LINE ('USUARIO:' || sbUsuarioSql );
    DBMS_OUTPUT.PUT_LINE ('ASIGNACION PERMISOS CREACION OBJETOS'); 
    FOR idx IN 1 .. tblPermisos.COUNT LOOP
    BEGIN     
        sbSentencia := REPLACE(tblPermisos(idx),'%usuario',sbUsuarioSql);
        PKG_EPM_GestionArchivos.PUT_LINE(iflFileHandle_out,sbSentencia||';'); 
        EXECUTE IMMEDIATE sbSentencia;
    EXCEPTION
        WHEN OTHERS THEN
             PKG_EPM_GestionArchivos.PUT_LINE (iflFileHandle_err, 'ERROR|'||SQLERRM||'|'||sbSentencia||';');
    END;                
    END LOOP; 

    DBMS_OUTPUT.PUT_LINE ('ASIGNACION PERMISOS TABLAS'); 
    PKG_EPM_GestionArchivos.PUT_LINE(iflFileHandle_out,'ASIGNACION PERMISOS TABLAS'); 
    FOR rctables IN cuTables(sbUsuarioSql) LOOP
    BEGIN    
        EXECUTE IMMEDIATE rctables.sentencia;
    
    EXCEPTION
        WHEN OTHERS THEN
             PKG_EPM_GestionArchivos.PUT_LINE (iflFileHandle_err, 'ERROR|'||SQLERRM||'|'||rctables.sentencia);
    END;  
    END LOOP; 

    DBMS_OUTPUT.PUT_LINE ('ASIGNACION PERMISOS CONSULTA TABLAS Y VISTAS'); 
    PKG_EPM_GestionArchivos.PUT_LINE(iflFileHandle_out,'ASIGNACION PERMISOS CONSULTA TABLAS Y VISTAS'); 
    FOR rctables IN cuTablesView(sbUsuarioSql) LOOP
    BEGIN    
        EXECUTE IMMEDIATE rctables.sentencia;
    
    EXCEPTION
        WHEN OTHERS THEN
             PKG_EPM_GestionArchivos.PUT_LINE (iflFileHandle_err, 'ERROR|'||SQLERRM||'|'||rctables.sentencia);
    END;  
    END LOOP;

    DBMS_OUTPUT.PUT_LINE ('ASIGNACION PERMISOS PACKAGE,PROCEDURES,FUNCTIONS'); 
    PKG_EPM_GestionArchivos.PUT_LINE(iflFileHandle_out,'ASIGNACION PERMISOS PACKAGE,PROCEDURES,FUNCTIONS'); 
    FOR rctables IN cuPaPrFu(sbUsuarioSql) LOOP
        BEGIN    
            EXECUTE IMMEDIATE rctables.sentencia;
        
        EXCEPTION
            WHEN OTHERS THEN
                 PKG_EPM_GestionArchivos.PUT_LINE (iflFileHandle_err, '*ERROR|'||SQLERRM||'|'||rctables.sentencia);
        END;  
    END LOOP;          

    dtTiempoTermina   := SYSTIMESTAMP ;
    dtTiempoUtilizado := dtTiempoTermina - dtTiempoInicia ;

    PKG_EPM_GestionArchivos.PUT_LINE (iflFileHandle_out,'TERMINA PROCESO');
    PKG_EPM_GestionArchivos.PUT_LINE (iflFileHandle_out, 'Hora de Finalización:'||TO_CHAR(dtTiempoTermina,'DD-MM-YYYY HH24:MI:SS'));
    PKG_EPM_GestionArchivos.PUT_LINE (iflFileHandle_out, 'Tiempo de Ejecucion['||dtTiempoUtilizado||']');
    PKG_EPM_GestionArchivos.PUT_LINE (iflFileHandle_out,'Fin Archivo');

    PKG_EPM_GestionArchivos.fclose(iflFileHandle_out);
    PKG_EPM_GestionArchivos.fclose(iflFileHandle_err);
    DBMS_OUTPUT.PUT_LINE ('TERMINA PROCESO'); 
    
EXCEPTION
    WHEN OTHERS THEN
        PKG_EPM_GestionArchivos.PUT_LINE(iflFileHandle_err,'ERROR - General: '||SQLERRM);
        PKG_EPM_GestionArchivos.PUT_LINE(iflFileHandle_err,'Ultima Linea procesada:'||nuTotal);
        PKG_EPM_GestionArchivos.PUT_LINE (iflFileHandle_err, 'Fin de Archivo');
        PKG_EPM_GestionArchivos.FCLOSE(iflFileHandle_out);
        PKG_EPM_GestionArchivos.fclose(iflFileHandle_err);
END;
/  
