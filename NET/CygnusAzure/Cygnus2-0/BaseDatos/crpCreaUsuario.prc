CREATE OR REPLACE   PROCEDURE pCreaUsuario
    (
        isbUsuario       IN  VARCHAR2,
        isbCodigo        IN  VARCHAR2,
        isbEmail         IN  VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    )
    IS
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
                                                'grant SELECT ANY DICTIONARY to %usuario'
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
             AND IOT_NAME IS NULL;
             --AND NOT EXISTS (SELECT 1 FROM dba_external_tables WHERE OWNER LIKE 'FLEX' AND TABLE_NAME = tb.table_name );

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
          
          TYPE tyUsuarios IS TABLE OF VARCHAR2(40) ;

          --Lista de nombre campos
          tblUsuarios tyUsuarios; 
    BEGIN
        onuErrorCode := 0;
        osbErrorMessage := '';
        
        sbUsuarioSql := isbUsuario;
            
        OPEN CuExistUsuario(sbUsuarioSql);
          FETCH CuExistUsuario INTO nuExiste;          
        CLOSE CuExistUsuario;
            
        IF nuExiste = 0 THEN
           sbSentencia := REPLACE(sbCreaUser,'%usuario',sbUsuarioSql);
           sbSentencia := REPLACE(sbSentencia,'%pass',isbCodigo);
           EXECUTE IMMEDIATE sbSentencia;
           sbSentencia := REPLACE(sbTableSpace,'%usuario',sbUsuarioSql);
           EXECUTE IMMEDIATE sbSentencia;
        END IF;        
        
        DBMS_OUTPUT.PUT_LINE ('USUARIO:' || sbUsuarioSql );
        DBMS_OUTPUT.PUT_LINE ('ASIGNACION PERMISOS CREACION OBJETOS'); 
        FOR idx IN 1 .. tblPermisos.COUNT LOOP
        BEGIN     
            sbSentencia := REPLACE(tblPermisos(idx),'%usuario',sbUsuarioSql);
            EXECUTE IMMEDIATE sbSentencia;
        EXCEPTION
            WHEN OTHERS THEN
                 RAISE;
        END;                
        END LOOP; 

        DBMS_OUTPUT.PUT_LINE ('ASIGNACION PERMISOS TABLAS'); 
        
        FOR rctables IN cuTables(sbUsuarioSql) LOOP
        BEGIN    
            EXECUTE IMMEDIATE rctables.sentencia;            
        EXCEPTION
            WHEN OTHERS THEN
                 RAISE;
        END;  
        END LOOP; 

        DBMS_OUTPUT.PUT_LINE ('ASIGNACION PERMISOS CONSULTA TABLAS Y VISTAS'); 
        
        FOR rctables IN cuTablesView(sbUsuarioSql) LOOP
        BEGIN    
            EXECUTE IMMEDIATE rctables.sentencia;
            
        EXCEPTION
            WHEN OTHERS THEN
                 RAISE;
        END;  
        END LOOP;

        DBMS_OUTPUT.PUT_LINE ('ASIGNACION PERMISOS PACKAGE,PROCEDURES,FUNCTIONS'); 
         
        FOR rctables IN cuPaPrFu(sbUsuarioSql) LOOP
        BEGIN    
            EXECUTE IMMEDIATE rctables.sentencia;
            
        EXCEPTION
            WHEN OTHERS THEN
                 RAISE;
        END;  
        END LOOP;           

        DBMS_OUTPUT.PUT_LINE ('TERMINA PROCESO'); 
        
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;
    END pCreaUsuario;
/
