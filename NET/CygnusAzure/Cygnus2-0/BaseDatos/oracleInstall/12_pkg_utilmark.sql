CREATE OR REPLACE PACKAGE pkg_utilmark
AS
    TYPE TYREFCURSOR IS REF CURSOR;
    
    PROCEDURE pInicializaError
    (
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    );
    
    PROCEDURE pCreaParametro
    (
        isbParamId      IN VARCHAR2,
        isbDescrip      IN VARCHAR2,
        isbValor        IN VARCHAR2,
        isbTipo         IN VARCHAR2,
        isbFuncion      IN VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    );
    
    PROCEDURE pCreaMensaje
    (
        isbCodigo       IN VARCHAR2, 
        isbDescrip      IN VARCHAR2,
        isbCausa        IN VARCHAR2,
        isbSolucion     IN VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    );
    
    PROCEDURE pObtCodigoMensaje
    (
        osbCodigoMens   OUT VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    );
    
    PROCEDURE pObtErrores
    (
        isbObjeto       IN  VARCHAR2,
        isbUsuario      IN  VARCHAR2,
        onuRefCursor    OUT tyRefCursor,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    );
    
    PROCEDURE pObtBackup
    (
        onuSeqObjBl     OUT NUMBER,
        onuSeqLogap     OUT NUMBER,
        onuSeqRq        OUT NUMBER,
        onuSeqHH        OUT NUMBER,
        onuSeqNeg       OUT NUMBER,
        oRefCredma      OUT tyRefCursor,
        oRefUsuari      OUT tyRefCursor,
        oRefObjsbl      OUT tyRefCursor,
        oRefUsersO      OUT tyRefCursor,
        oRefPerssO      OUT tyRefCursor,
        oRefHojas       OUT tyRefCursor,
        oRefRq          OUT tyRefCursor,
        oRefHorasH    OUT tyRefCursor,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    );
    
    PROCEDURE pInsLog
    (
        isbObjeto       IN  VARCHAR2,
        isbMaquina      IN  VARCHAR2,
        isbUsuario      IN  VARCHAR2,
        isbTipo         IN  VARCHAR2,
        isbAccion       IN  VARCHAR2,
        inuCantObjsI    IN  NUMBER,
        isbOwner        IN  VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    );
    
    PROCEDURE pGuardaCodigo
    (
        isbUsuario       IN  VARCHAR2,
        isbCodigo         IN  VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    );
    
    PROCEDURE pObtieneCodigo
    (
        isbUsuario      IN  VARCHAR2,
        osbCodigo       OUT VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    );
    
    PROCEDURE pValidaUsuarioApl
    (
        isbObjeto       IN VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    );
    
    PROCEDURE pValidaObjEsquema
    (
        isbObjeto       IN VARCHAR2,
        isbUsuario      IN VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    );
    
    PROCEDURE pObtCantObjsInvalidos
    (
        onuCantObjetos  OUT NUMBER,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    );
    
    PROCEDURE pObtConsultaObjetos
    (
        isbNombreObj     IN VARCHAR2,
        onuRefCursor    OUT tyRefCursor,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    );
    
    PROCEDURE pBloqueaObjeto
    (
        isbNombreObj     IN VARCHAR2,
        isbOwnerObj      IN VARCHAR2,
        isbNumCaso       IN VARCHAR2,
        isbUsuario       IN VARCHAR2,
        isbFechaLib      IN VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    );
    
    PROCEDURE pObtObjetosBloqueados
    (
        isbUsuario     IN VARCHAR2,
        onuRefCursor    OUT tyRefCursor,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2 
    ); 
    
    PROCEDURE pDesbloqueaObjeto
    (
        isbNombreObj     IN VARCHAR2,
        isbOwnerObj      IN VARCHAR2,
        isbUsuario       IN VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    );    
    
    PROCEDURE pObtObjetosBloqTodos
    (
        isbObjeto       IN VARCHAR2,
        onuRefCursor    OUT tyRefCursor,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2 
    );   
    
    PROCEDURE pGenerapktbl
    (
        isbTabla IN VARCHAR2,
        isbOwner IN VARCHAR2,
        isOrder  IN VARCHAR2,
        oclFile  OUT CLOB,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2 
    );
    
    PROCEDURE pObtRol
    (
        isbUsuario      IN  VARCHAR2,
        osbRol          OUT VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    );
    
    PROCEDURE pGuardaRol
    (
        isbUsuario       IN  VARCHAR2,
        isbCodigo        IN  VARCHAR2,
        isbEmail         IN  VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    );
    
    PROCEDURE pActualizaFecha
    (
        isbNombreObj    IN  VARCHAR2,
        isbOwnerObj     IN  VARCHAR2,
        isbUsuario      IN  VARCHAR2,
        isbFechaLib     IN  VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    );
    
    PROCEDURE pObtCodigoVersion
    (
        osbVersion      OUT VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    );
    
    PROCEDURE pObtCodigoSql
    (
        isbVersion      in VARCHAR2,
        onuRefCursor    OUT Sys_Refcursor,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    );    
    
    PROCEDURE pObtGrupoCorreo
    (
        osbGrupoCorreo  OUT VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    );
        
    PROCEDURE pActualizaCorreo
    (
        isbUsuario       IN  VARCHAR2,
        isbCorreo        IN  VARCHAR2,
        isbUsuarioAzure  IN VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    );    
        
    PROCEDURE pObtAreaAzure
    (
        isbUsuario       IN VARCHAR2,
        osbCorreo       OUT VARCHAR2,
        osbUsuarioAzure OUT VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2 
    );
END pkg_utilmark;
/
CREATE OR REPLACE PACKAGE BODY pkg_utilmark
AS           
    csbBloqueadoSI CONSTANT VARCHAR2(1) := 'Y';
    csbBloqueadoNO CONSTANT VARCHAR2(1) := 'N';
    
    PROCEDURE pInicializaError
    (
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    )
    IS
    BEGIN
        onuErrorCode := 0;
        osbErrorMessage := NULL;
    END pInicializaError;
    
    /*Crea Parámetro*/
    PROCEDURE pCreaParametro
    (
        isbParamId      IN VARCHAR2,
        isbDescrip      IN VARCHAR2,
        isbValor        IN VARCHAR2,
        isbTipo         IN VARCHAR2,
        isbFuncion      IN VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    )
    IS
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        
        INSERT INTO FLEX.epm_parametr (PARAMETER_ID, DESCRIPTION, VALUE, VAL_FUNCTION, MODULE_ID, DATA_TYPE, ALLOW_UPDATE)
         VALUES
         (
             isbParamId,
             isbDescrip,
             isbValor,
             isbFuncion,
             -99,
             isbTipo,
             'Y'
         );
         
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;
    END pCreaParametro;
    
    /*Crea Mensaje*/
    PROCEDURE pCreaMensaje
    (
        isbCodigo       IN VARCHAR2, 
        isbDescrip      IN VARCHAR2,
        isbCausa        IN VARCHAR2,
        isbSolucion     IN VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    )
    IS
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        
        --realiza la insercción del mensaje EPM - CUZ 
        INSERT INTO flex.mensaje (menscodi,mensdesc,mensdivi,mensmodu,menscaus,mensposo ) 
        VALUES
        (
            isbCodigo,
            isbDescrip,
            'EPM',
            'CUZ',
            isbCausa,
            isbSolucion
        );
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;    
    END pCreaMensaje;
    
    /*Obtiene código mensaje*/
    PROCEDURE pObtCodigoMensaje
    (
        osbCodigoMens   OUT VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    )   
    IS
        sbValoresExcluir epm_parametr.value%type := pkg_epm_boparametr.fsbget('EXC_MENSAJE_MARK');
        nuCodigo         mensaje.menscodi%type := 0;
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
    
        osbCodigoMens := NULL;
        
        SELECT max(menscodi) INTO nuCodigo
        FROM flex.mensaje 
        WHERE mensdivi = 'EPM'
        AND mensmodu = 'CUZ'
        AND menscodi < 900196;
        
        IF(NVL(nuCodigo,0) > 0) THEN
            nuCodigo := nuCodigo + 1;
            osbCodigoMens := to_char(nuCodigo);
        END IF;
        
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM; 
    END pObtCodigoMensaje;
    
    PROCEDURE pObtCodigoVersion
    (
        osbVersion      OUT VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    )   
    IS
        CURSOR cuDatos
        IS
            SELECT version
            FROM flex.ll_version
            ORDER BY fecha_ini desc;
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
    
        osbVersion := NULL;
        
        OPEN cuDatos;
        FETCH cuDatos INTO osbVersion;
        CLOSE cuDatos;
        
        IF(osbVersion IS null) THEN
            osbVersion := '';
        END IF;
        
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM; 
    END pObtCodigoVersion;
    
    PROCEDURE pObtCodigoSql
    (
        isbVersion      in VARCHAR2,
        onuRefCursor    OUT Sys_Refcursor,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    )   
    IS
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
    
        OPEN onuRefCursor FOR
            SELECT objeto
            FROM flex.ll_version
            where version >= isbVersion
            order by version;
        
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM; 
    END pObtCodigoSql;    
    
    PROCEDURE pObtGrupoCorreo
    (
        osbGrupoCorreo  OUT VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    )  
    IS
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        
        osbGrupoCorreo := null;
        osbGrupoCorreo := pkg_epm_boparametr.fsbget('CY_GRUPO_CORREO');
        
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM; 
    END pObtGrupoCorreo;    
    
    /*Obtiene errores de aplicación*/
    PROCEDURE pObtErrores
    (
        isbObjeto       IN  VARCHAR2,
        isbUsuario      IN  VARCHAR2,
        onuRefCursor    OUT tyRefCursor,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    )
    IS
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        
        OPEN onuRefCursor FOR
            SELECT 
                   NAME ,
                   TYPE ,
                   LINE ,
                   TEXT 
            FROM   all_errors 
            WHERE  name = isbObjeto
            AND    owner IN (SELECT usuario FROM FLEX.ll_credmark);
            
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;             
    END pObtErrores;
    
    PROCEDURE pObtBackup
    (
        onuSeqObjBl     OUT NUMBER,
        onuSeqLogap     OUT NUMBER,
        onuSeqRq        OUT NUMBER,
        onuSeqHH        OUT NUMBER,
        onuSeqNeg       OUT NUMBER,
        oRefCredma      OUT tyRefCursor,
        oRefUsuari      OUT tyRefCursor,
        oRefObjsbl      OUT tyRefCursor,
        oRefUsersO      OUT tyRefCursor,
        oRefPerssO      OUT tyRefCursor,
        oRefHojas       OUT tyRefCursor,
        oRefRq          OUT tyRefCursor,
        oRefHorasH      OUT tyRefCursor,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    )
    IS
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        
        onuSeqObjBl := 0;
        onuSeqLogap := 0;
        onuSeqRq := 0;
        onuSeqHH := 0;
        onuSeqNeg := 0;
        
        OPEN oRefCredma FOR
            SELECT 'INSERT INTO flex.ll_credmark (usuario,codigo) values ('''||usuario||''','''||codigo||''');' valor
            FROM flex.ll_credmark;
                    
        OPEN oRefUsuari FOR
            SELECT 'INSERT INTO flex.ll_usuarios (usuario,rol,email,usuarioazure) values ('''||usuario||''','''||rol||''','''||email||''','''||usuarioazure||''');' valor
            FROM flex.ll_usuarios;
                        
        OPEN oRefObjsbl FOR
            SELECT 'INSERT INTO flex.ll_objetosbl (codigo,objeto,orden,usuario,bloqueado,fecha_bloqueo,fecha_registro,owner,fecha_est_lib) values ('||codigo||','''||objeto||''','''||orden||''','''||usuario||''','''||bloqueado||''','''||fecha_bloqueo||''','''||fecha_registro||''','''||owner||''','''||fecha_est_lib||''');' valor
            FROM flex.ll_objetosbl
            WHERE bloqueado = 'Y';  
            
        OPEN oRefUsersO FOR
            select 'insert into flex.sa_user (USER_ID, MASK, DB_USER_NAME, OWNER_ID, USER_TYPE_ID, LAST_SUCC_LOGON_DATE, CREATION_DATE, LAST_UPDATE_DATE, LAST_UPDATE_USER, DEFAULT_TABLESPACE, TEMPORARY_TABLESPACE, PATH_REPORT, COMPANY_ID, HAS_MULTISESSION, GUID, COMMON_NAME, TENANT_ID) '||
                   'values ('''||sa.USER_ID||''','''|| MASK||''','''||DB_USER_NAME||''','''||OWNER_ID||''','''||USER_TYPE_ID||''','''||LAST_SUCC_LOGON_DATE||''','''||CREATION_DATE||''','''||LAST_UPDATE_DATE||''','''||LAST_UPDATE_USER||''','''|| DEFAULT_TABLESPACE||''','''||TEMPORARY_TABLESPACE||''','''||PATH_REPORT||''','''||sa.COMPANY_ID||''','''||HAS_MULTISESSION||''','''||GUID||''','''||COMMON_NAME||''','''||TENANT_ID||''');' valor
            from ge_person per,sa_user sa 
            where per.user_id = sa.user_id 
            and mask in (select usuario from flex.ll_usuarios);
       
       OPEN oRefPerssO FOR
            select 'INSERT INTO flex.ge_person (PERSON_ID, PERSONAL_TYPE, NAME_, NUMBER_ID, PHONE_NUMBER, E_MAIL, BEEPER, GEOGRAP_LOCATION_ID, IDENT_TYPE_ID, USER_ID, EMPLOYEE_COMPANY_ID, COMMENT_, FAX_NUMBER, ATEL_PHONE_NUMBER, EXTERNAL_CODE, ADDRESS_ID, BANK_ID, BRANCH_ID, POPUP_NOTI_ENABLE, COMPANY_ID) '||
                   'values ('''|| PERSON_ID||''','''|| PERSONAL_TYPE||''','''||NAME_||''','''||NUMBER_ID||''','''||PHONE_NUMBER||''','''||E_MAIL||''','''||BEEPER||''','''||GEOGRAP_LOCATION_ID||''','''||IDENT_TYPE_ID||''','''||per.USER_ID||''','''||EMPLOYEE_COMPANY_ID||''','''||COMMENT_||''','''||FAX_NUMBER||''','''||ATEL_PHONE_NUMBER||''','''||EXTERNAL_CODE||''','''||ADDRESS_ID||''','''||BANK_ID||''','''||BRANCH_ID||''','''||POPUP_NOTI_ENABLE||''','''||per.COMPANY_ID ||''');' valor
            from ge_person per,sa_user sa 
            where per.user_id = sa.user_id 
            and mask in (select usuario from flex.ll_usuarios);  
            
        OPEN oRefHojas FOR
            SELECT 'INSERT INTO flex.ll_hoja (codigo,fecha_ini,fecha_fin,descripcion) '||
                   'VALUES ('||codigo||','''||to_char(fecha_ini,'dd/mm/yyyy')||''','''||to_char(fecha_fin,'dd/mm/yyyy')||''','''||descripcion||''');' valor
            FROM flex.ll_hoja; 
            
        OPEN oRefRq FOR
            SELECT 'INSERT INTO flex.ll_requerimiento (codigo,descripcion,id_azure,estado,fecha_actualiza,usuario,fecha_display,completado,fecha_registro,hist_usuario) '||
                    'values ('||codigo||','''||descripcion||''','||id_azure||','''||estado||''','''||to_char(nvl(fecha_actualiza,SYSDATE),'dd/mm/yyyy')||''','''||usuario||''','''||to_char(nvl(fecha_display,SYSDATE),'dd/mm/yyyy')||''','''||nvl(completado,0)||''','''||to_char(nvl(fecha_registro,SYSDATE),'dd/mm/yyyy')||''','||nvl(hist_usuario,0)||');' valor
            FROM flex.ll_requerimiento;   
            
        OPEN oRefHorasH FOR
            SELECT 'INSERT INTO flex.ll_horashoja (CODIGO, ID_HOJA, FECHA_REGISTRO, USUARIO, REQUERIMIENTO, LUNES, MARTES, MIERCOLES, JUEVES, VIERNES, SABADO, DOMINGO, FECHA_ACTUALIZA, OBSERVACION) '||
                    'VALUES ('||CODIGO||','||ID_HOJA||','''||to_char(FECHA_REGISTRO,'dd/mm/yyyy')||''','''||USUARIO||''','||REQUERIMIENTO||','''||LUNES||''','''||MARTES||''','''||MIERCOLES||''','''||JUEVES||''','''||VIERNES||''','''||SABADO||''','''||DOMINGO||''','''||to_char(FECHA_ACTUALIZA,'dd/mm/yyyy')||''','''||OBSERVACION||''');' valor
            FROM flex.ll_horashoja 
            WHERE LUNES > 0
            OR MARTES > 0
            OR MIERCOLES > 0
            OR JUEVES > 0
            OR VIERNES > 0
            OR SABADO > 0
            OR DOMINGO > 0
            UNION
            SELECT valor FROM (
            SELECT 'INSERT INTO flex.ll_version (version,fecha_ini,fecha_fin) values ('''||version||''','''||fecha_ini||''','''||fecha_fin||''');' valor
            FROM ll_version
            ORDER BY fecha_ini DESC)
            WHERE rownum < 2;   
        
        SELECT flex.seq_ll_objetosbl.nextval
        INTO onuSeqObjBl
        FROM dual;
        
        SELECT flex.seq_ll_logapli.nextval
        INTO onuSeqLogap
        FROM dual;
        
        SELECT flex.seq_ll_requerimiento.nextval
        INTO onuSeqRq
        FROM dual;
        
        SELECT flex.seq_ll_horashoja.nextval
        INTO onuSeqHH
        FROM dual;
        
        SELECT flex.seq_ll_negativa.nextval
        INTO onuSeqNeg
        FROM dual;
                
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM; 
    END pObtBackup;
    
    /*Log ejecución*/
    PROCEDURE pInsLog
    (
        isbObjeto       IN  VARCHAR2,
        isbMaquina      IN  VARCHAR2,
        isbUsuario      IN  VARCHAR2,
        isbTipo         IN  VARCHAR2,
        isbAccion       IN  VARCHAR2,
        inuCantObjsI    IN  NUMBER,
        isbOwner        IN  VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    )
    IS
        sbUsuario   VARCHAR2(100);
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        
        SELECT user INTO sbUsuario
        FROM DUAL;
        
        INSERT INTO flex.ll_logapli (codigo,fecha_registro,objeto,maquina,usuario,tipo,accion,objetos_inv,esquema)
        VALUES
        (
            flex.seq_ll_logapli.nextval,
            sysdate,
            isbObjeto,
            isbMaquina,
            UPPER(isbUsuario),
            isbTipo,
            isbAccion,
            inuCantObjsI,
            isbOwner
        );
        
        commit;
        
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;     
    END pInsLog;
    
    /*Guarda*/
    PROCEDURE pGuardaCodigo
    (
        isbUsuario       IN  VARCHAR2,
        isbCodigo        IN  VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    )
    IS
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        
        INSERT INTO flex.ll_credmark (usuario, codigo)
        VALUES
        (
            isbUsuario,
            isbCodigo
        );   
                
        COMMIT;
        
    EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN 
            UPDATE flex.ll_credmark
            SET codigo = isbCodigo
            WHERE usuario = isbUsuario;
            commit;
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;     
    END pGuardaCodigo; 
    
    /*Guarda*/
    PROCEDURE pGuardaRol
    (
        isbUsuario       IN  VARCHAR2,
        isbCodigo        IN  VARCHAR2,
        isbEmail         IN  VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    )
    IS
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        
        INSERT INTO flex.ll_usuarios (usuario, rol,email)
        VALUES
        (
            isbUsuario,
            isbCodigo,
            isbEmail
        );  
                
        COMMIT;
        
    EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN 
            UPDATE flex.ll_usuarios
            SET rol = isbCodigo
            WHERE usuario = isbUsuario;
            commit;
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;     
    END pGuardaRol;
    
    /*Obtiene*/
    PROCEDURE pObtieneCodigo
    (
        isbUsuario      IN  VARCHAR2,
        osbCodigo       OUT VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    )
    IS        
        CURSOR cuDatos
        IS
            SELECT codigo
            FROM flex.ll_credmark 
            WHERE usuario = isbUsuario;
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        osbCodigo := NULL;
        
        OPEN cuDatos;
        FETCH cuDatos INTO osbCodigo;
        CLOSE cuDatos;
        
        IF(osbCodigo IS NULL) THEN
            osbCodigo := null;
        END IF;   
        
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;     
    END pObtieneCodigo;  
    
    PROCEDURE pObtRol
    (
        isbUsuario      IN  VARCHAR2,
        osbRol          OUT VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    )
    IS        
        CURSOR cuDatos
        IS
            SELECT rol 
            FROM flex.ll_usuarios 
            WHERE usuario = isbUsuario;
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        osbRol := NULL;
        
        OPEN cuDatos;
        FETCH cuDatos INTO osbRol;
        CLOSE cuDatos;
        
        IF(osbRol IS NULL) THEN
            osbRol := '-';
        END IF;   
        
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;     
    END pObtRol;   
    
    /*Valida usuario*/
    PROCEDURE pValidaUsuarioApl
    (
        isbObjeto       IN VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    )
    IS
        nuAplUser   NUMBER := 0;
        
        CURSOR cuDatos
        IS
            SELECT COUNT(1) 
            FROM 
            (
                SELECT DISTINCT OWNER
                FROM   all_objects 
                WHERE  object_name = upper(isbObjeto)
                AND    OWNER IN (SELECT usuario FROM FLEX.ll_credmark)
            );
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);

        OPEN cuDatos;
        FETCH cuDatos INTO nuAplUser;
        CLOSE cuDatos;    
        
        IF(nvl(nuAplUser,0) > 1) THEN
            onuErrorCode := 1001;
            osbErrorMessage := 'El objeto '||isbObjeto||' se encuentra aplicado en más de un esquema.';
        END IF;
        
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;    
    END pValidaUsuarioApl;
    
    /*Valida usuario*/
    PROCEDURE pValidaObjEsquema
    (
        isbObjeto       IN VARCHAR2,
        isbUsuario      IN VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    )
    IS
        sbUserCursor   VARCHAR2(100) := null;
        sbUserExist    VARCHAR2(100) := null;
        blExObjeto     BOOLEAN := FALSE;
        blDifUser      BOOLEAN := FALSE;
        sbUsuario      VARCHAR2(100);
        sdFechaBl      DATE;
        
        CURSOR cuDatos
        IS
            SELECT DISTINCT OWNER
            FROM   all_objects 
            WHERE  object_name = upper(isbObjeto)
            AND    OWNER IN (SELECT usuario FROM FLEX.ll_credmark);            
            
        CURSOR cuValidaBloqueo
        IS
            SELECT usuario,fecha_bloqueo,orden
            FROM flex.ll_objetosbl
            WHERE bloqueado = 'Y'
            AND owner = isbUsuario
            AND objeto = isbObjeto; 
            
        CURSOR cuValidaBloqueoSql
        (
            isbUserSql varchar2
        )
        IS
            SELECT count(1)
            FROM flex.ll_objetosbl
            WHERE bloqueado = 'Y'
            AND usuario = isbUserSql
            AND objeto = isbObjeto;
            
        CURSOR cuExisteObj
        IS
            SELECT count(1)
            FROM   all_objects 
            WHERE  object_name = upper(isbObjeto)
            AND    OWNER IN (SELECT usuario FROM FLEX.ll_credmark);  
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);

        OPEN cuDatos;
        LOOP 
            sbUserCursor := null;
            FETCH cuDatos INTO sbUserCursor;
            
            IF(cuDatos%NOTFOUND) THEN
                EXIT;
            END IF;
            
            blExObjeto := true;
            
            IF(isbUsuario != sbUserCursor) THEN
                sbUserExist := sbUserCursor;
                blDifUser := TRUE;
            END IF;
        END LOOP;
        CLOSE cuDatos;    
        
        --Existe el objeto
        IF(blExObjeto AND blDifUser) THEN
            onuErrorCode := 1002;
            osbErrorMessage := 'El objeto '||isbObjeto||' solo puede existir en un esquema. Ya existe en '||sbUserExist||'.';
            return;
        END IF;
        
        --Se valida que el objeto no se encuentre bloqueado para el usuario.
        /*OPEN  cuValidaBloqueo;
        FETCH cuValidaBloqueo INTO sbUsuario,sdFechaBl,sbOrden;
        CLOSE cuValidaBloqueo;   
        
        SELECT user INTO sbUsuarioCon FROM DUAL;
              
        IF(sbUsuario IS NOT NULL)THEN            
            
            IF(upper(trim(sbUsuario)) != upper(trim(sbUsuarioCon))) THEN            
                onuErrorCode := 1003;
                osbErrorMessage := 'El objeto ['||isbObjeto||'] se encuentra bloqueado por ['||sbUsuario||'] con la OC ['||sbOrden||'] desde el día ['||sdFechaBl||']';
                return;
           END IF;
        END IF;
        
        OPEN  cuValidaBloqueoSql(sbUsuarioCon);
        FETCH cuValidaBloqueoSql INTO nuCantidad;
        CLOSE cuValidaBloqueoSql; 
        
        IF(nvl(nuCantidad,0) = 0) THEN
            
            OPEN cuExisteObj;
            FETCH cuExisteObj INTO nuExiste;
            CLOSE cuExisteObj;
            
            IF(NVL(nuExiste,0) > 0) THEN            
                onuErrorCode := 1004;
                osbErrorMessage := 'El objeto ['||isbObjeto||'] debe estar bloqueado con su usuario SQL para poder compilarlo en los ambientes controlados.';
            END IF;
        END IF;*/
        
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;    
    END pValidaObjEsquema; 
    
    PROCEDURE pObtCantObjsInvalidos
    (
        onuCantObjetos  OUT NUMBER,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    )
    IS        
        CURSOR cuDatos
        IS            
            select count(distinct name)
            from all_errors 
            where owner in (select usuario
                            from flex.ll_credmark)
            and attribute = 'ERROR'; 
            
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        onuCantObjetos := 0;
        
        OPEN cuDatos;
        FETCH cuDatos INTO onuCantObjetos;
        CLOSE cuDatos;    
        
        --Existe el objeto
        IF(NVL(onuCantObjetos,0) = 0) THEN
            onuCantObjetos := 0;
        END IF;
        
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;    
    END pObtCantObjsInvalidos;  
    
    PROCEDURE pObtConsultaObjetos
    (
        isbNombreObj     IN VARCHAR2,
        onuRefCursor    OUT tyRefCursor,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    )
    IS                
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        
        OPEN onuRefCursor FOR
            SELECT DISTINCT a.owner,a.object_name,nvl(b.bloqueado,'N') bloqueado,
            b.usuario,
            b.fecha_bloqueo,
            b.orden,
            b.fecha_est_lib
            FROM all_objects a, flex.ll_objetosbl b
            WHERE a.object_name = b.objeto(+)
            AND a.object_name LIKE isbNombreObj
            AND a.OWNER IN (
                              SELECT usuario
                              FROM flex.ll_credmark
                          )
            order by fecha_bloqueo desc;        
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;    
    END pObtConsultaObjetos;  
    
    PROCEDURE pActualizaFecha
    (
        isbNombreObj    IN  VARCHAR2,
        isbOwnerObj     IN  VARCHAR2,
        isbUsuario      IN  VARCHAR2,
        isbFechaLib     IN  VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    )
    IS
        dtFecha     DATE;
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);        
        dtFecha := to_date(isbFechaLib);
        
        UPDATE flex.ll_objetosbl
        SET    fecha_est_lib = dtFecha
        WHERE objeto = isbNombreObj
        AND   owner = isbOwnerObj
        AND   bloqueado = csbBloqueadoSI;
        
        COMMIT;
        
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM||'. isbFechaLib: '||isbFechaLib; 
    END pActualizaFecha;
    
    PROCEDURE pBloqueaObjeto
    (
        isbNombreObj     IN VARCHAR2,
        isbOwnerObj      IN VARCHAR2,
        isbNumCaso       IN VARCHAR2,
        isbUsuario       IN VARCHAR2,
        isbFechaLib      IN VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    )
    IS 
        dtFecha     DATE;
        sbOrden     flex.ll_objetosbl.orden%type;
        sbUsuario   flex.ll_objetosbl.usuario%type;
        dtFechaBlo  flex.ll_objetosbl.fecha_bloqueo%type;
        dtFechaLib  flex.ll_objetosbl.fecha_liberacion%type;
        
        CURSOR cuDatos
        IS
            SELECT orden,usuario,fecha_bloqueo,fecha_liberacion
            FROM flex.ll_objetosbl
            WHERE  objeto = isbNombreObj
            AND  owner = isbOwnerObj
            AND bloqueado = csbBloqueadoSI;
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        
        dtFecha := to_date(isbFechaLib);
        
        IF(cuDatos%ISOPEN) THEN
            CLOSE cuDatos;
        END IF;
        
        OPEN cuDatos;
        FETCH cuDatos INTO sbOrden,sbUsuario,dtFechaBlo,dtFechaLib;
        CLOSE cuDatos;
        
        IF(sbOrden IS NOT NULL) THEN
            onuErrorCode := 3001;
            osbErrorMessage := 'El objeto ['||isbOwnerObj||'.'||isbNombreObj||'] se encuentra bloqueado por el usuario ['||sbUsuario||'] con la OC ['||sbOrden||'] desde el día ['||TO_CHAR(dtFechaBlo,'DD/MM/YYYY')||'] con fecha estimada de liberación ['||TO_CHAR(dtFechaLib,'DD/MM/YYYY')||']'; 
            return;
        END IF;
        
        INSERT INTO flex.ll_objetosbl
        (
            codigo,
            objeto,
            orden,
            usuario,
            bloqueado,
            fecha_bloqueo,
            fecha_est_lib,
            fecha_registro,
            owner
        )
        VALUES
        (
            flex.seq_ll_objetosbl.nextval,
            trim(isbNombreObj),
            trim(isbNumCaso),
            trim(isbUsuario),
            csbBloqueadoSI,
            SYSDATE,
            dtFecha,
            SYSDATE,
            trim(isbOwnerObj)
        );   
        
        COMMIT;
      
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM||'. isbFechaLib: '||isbFechaLib;    
    END pBloqueaObjeto;  
    
    PROCEDURE pObtObjetosBloqueados
    (
        isbUsuario     IN VARCHAR2,
        onuRefCursor    OUT tyRefCursor,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2 
    )
    IS
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        
        OPEN onuRefCursor FOR
            SELECT objeto,
                   owner,
                   orden,
                   fecha_bloqueo,
                   to_char(fecha_est_lib,'DD/MM/YYYY') fecha_liberacion
            FROM  flex.ll_objetosbl
            WHERE usuario = isbUsuario
            AND   bloqueado = csbBloqueadoSI;
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;   
    END pObtObjetosBloqueados;
    
    PROCEDURE pDesbloqueaObjeto
    (
        isbNombreObj     IN VARCHAR2,
        isbOwnerObj      IN VARCHAR2,
        isbUsuario       IN VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    )
    IS 
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        
        UPDATE flex.ll_objetosbl
        SET    bloqueado = csbBloqueadoNO,
               fecha_liberacion = sysdate
        WHERE  usuario = isbUsuario
        AND    objeto = isbNombreObj
        AND    owner = isbOwnerObj;
        
        COMMIT;
      
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;    
    END pDesbloqueaObjeto; 
    
    PROCEDURE pObtObjetosBloqTodos
    (
        isbObjeto       IN VARCHAR2,
        onuRefCursor    OUT tyRefCursor,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2 
    )
    IS
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        
        OPEN onuRefCursor FOR
            SELECT objeto,
                   owner,
                   orden,
                   fecha_bloqueo,
                   to_char(fecha_est_lib,'DD/MM/YYYY') fecha_est_liberacion,
                   usuario
            FROM  flex.ll_objetosbl
            WHERE objeto like isbObjeto
            AND   bloqueado = csbBloqueadoSI;
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;   
    END pObtObjetosBloqTodos;  
    
    PROCEDURE pActualizaCorreo
    (
        isbUsuario       IN  VARCHAR2,
        isbCorreo        IN  VARCHAR2,
        isbUsuarioAzure  IN VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    )
    IS
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        
        INSERT INTO flex.ll_usuarios (usuario, rol,email,usuarioAzure)
        VALUES
        (
            isbUsuario,
            NULL,
            isbCorreo,
            isbUsuarioAzure
        ); 
                
        COMMIT;
        
    EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN 
            UPDATE flex.ll_usuarios
            SET email = isbCorreo,
                usuarioAzure = isbUsuarioAzure
            WHERE usuario = isbUsuario;
            commit;
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;     
    END pActualizaCorreo;
    
    PROCEDURE pObtAreaAzure
    (
        isbUsuario       IN VARCHAR2,
        osbCorreo       OUT VARCHAR2,
        osbUsuarioAzure OUT VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2 
    )
    IS
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        
        osbCorreo := '';
        osbUsuarioAzure := '';
                    
        SELECT email,usuarioAzure INTO osbCorreo,osbUsuarioAzure
        FROM flex.ll_usuarios 
        WHERE usuario = isbUsuario;
        
        osbUsuarioAzure := nvl(osbUsuarioAzure,'');
        osbCorreo := nvl(osbCorreo,'');
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;   
    END pObtAreaAzure;
        
    PROCEDURE pGenerapktbl
    (
        isbTabla IN VARCHAR2,
        isbOwner IN VARCHAR2,
        isOrder  IN VARCHAR2,
        oclFile  OUT CLOB,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2 
    )
    IS    
    
        csbTABLA            varchar2( 100 ) := isbTabla;
        csbOWNER            varchar2( 100 ) := isbOwner;
        csbWO               varchar2( 100 ) := isOrder;
        csbEXISTE           constant number := 12202;
        csbNO_EXISTE        constant number := 12201;
        csbRUTA_ARCHIVO     constant varchar2( 200 ) := '/output/traza';
        
        -- Variable para construir funciones de campos
        sbFunction              VARCHAR2(32000);
        -- Parámetros de entrada de llave primaria como argumentos de entrada
        sbParamPK               VARCHAR2(2000);        
        -- Parámetros de entrada de la PK
        sbVariablesPK           VARCHAR2(2000);
        -- Listado de variables de entrada que componen la PK, para imprimirse en mensajes
        sbCamposPKParaOutput    VARCHAR2(2000);
        -- Listado de variables de entrada que componen la PK, para imprimirse en mensajes (Encomillada)
        sbCamposPKParaOutputCom VARCHAR2(2000);
        -- Campos igualados a la variable
        sbWherePorPK            VARCHAR2(2000);                
        
        -- Cascarón para la especificación de una función
        csbSpecTemplateFunction   VARCHAR2(32000) :=
'
    -- Obtención del campo <Campo>
    FUNCTION <Type>Get<Campo>
    (
        <ParamPK>
        INUCACHE    IN  NUMBER  DEFAULT 1
    )
    RETURN <Tabla>.<Campo>%TYPE;
';
        
        -- Cascarón para una función de obtención por campo
        csbTemplateFunction   VARCHAR2(32000) := 
'
    -- Obtención del campo <Campo>
    FUNCTION <Type>Get<Campo>
    (
        <ParamPK>
        INUCACHE    IN  NUMBER  DEFAULT 1
    )
    RETURN <Tabla>.<Campo>%TYPE
    IS
    BEGIN
        pkErrors.Push(''pktbl<Tabla>.<Type>Get<Campo>'');
        
        -- Valida si el dato está en memoria
        
        AccKey
        ( 
            <pk>    INUCACHE 
        );
        
        pkErrors.pop;
        
        -- Obtiene el dato
        RETURN (rc<Tabla>.<Campo>);
    EXCEPTION
        WHEN LOGIN_DENIED THEN
            pkErrors.Pop;
            RAISE LOGIN_DENIED;
    END <Type>Get<Campo>;
';

        -- Cascarón para la especificación de un procedimiento update
        csbSpecTemplateProc   VARCHAR2(32000) :=
'
    -- Actualización del campo <Campo>
    PROCEDURE pUpd<Campo>
    (
        <ParamPK>
        <ParamCampo>
    );
';

        -- Cascarón para un procedimiento de actualización por campo
        csbTemplateUpdate   VARCHAR2(32000) := 
'
    -- Actualización del campo <Campo>
    PROCEDURE pUpd<Campo>
    (
        <ParamPK>
        <ParamCampo>
    )
    IS
    BEGIN
        pkErrors.Push(''pktbl<Tabla>.pUpd<Campo>'');
        
        UPDATE  <Tabla>
        SET     <Campo> = <AliasCampo>
        WHERE   <Condicion>;
        
        IF (SQL%NOTFOUND) THEN
            pkErrors.pop;
            RAISE NO_DATA_FOUND;
        END IF;
        
        -- Actualiza la copia en memoria
        rc<Tabla>.<Campo> := <AliasCampo>;
        
        pkErrors.pop;
        
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_NO_EXISTE );
            pkErrors.ADDSUFFIXTOMESSAGE ( csbTABLA_PK||'||'<sbCamposPKParaOutputSinCom>||'''||'] )'||CHR(39)||');
            RAISE LOGIN_DENIED;
    END pUpd<Campo>;
';
        
        gsbTableCap     varchar2( 100 );  
    
        CURSOR cuPrimaria IS
            select  --+ ordered
                    tcol.table_name, tcol.column_name, tcol.data_type, tcol.data_length, tcol.data_precision, tcol.data_scale,
                    'i' || decode( tcol.data_type, 'NUMBER', 'nu', 'VARCHAR2', 'sb', 'DATE', 'dt', 'BOOLEAN', 'bo', 'CLOB', 'cl', 'BLOB', 'bl', 'XMLTYPE', 'xm' ) || substr( tcol.column_name, 1, 1 ) || lower( substr( tcol.column_name, 2 ) ) alias,
                    'i' || decode( tcol.data_type, 'NUMBER', 'nu', 'VARCHAR2', 'sb', 'DATE', 'dt', 'BOOLEAN', 'bo', 'CLOB', 'cl', 'BLOB', 'bl', 'XMLTYPE', 'xm' ) || substr( tcol.column_name, 1, 1 ) || lower( substr( tcol.column_name, 2 ) ) || ' ' || 'IN' || ' ' || lower( tcol.table_name || '.' || tcol.column_name) || '%TYPE'  param_entrada
            from    all_constraints cons, all_cons_columns ccol, all_tab_columns tcol
            where   cons.table_name = upper( csbTABLA )
            and     cons.constraint_type = 'P'
            and     cons.owner = upper( csbOWNER )
            and     ccol.owner = cons.owner
            and     ccol.constraint_name = cons.constraint_name
            and     ccol.table_name = cons.table_name
            and     tcol.owner = ccol.owner
            and     tcol.table_name = ccol.table_name
            and     tcol.column_name = ccol.column_name
            order by ccol.position;
            
            
        CURSOR cuCampos IS
            select  ------+ ordered
                    tcol.table_name, tcol.column_name, lower( tcol.column_name ) alias, data_type,
                    'i' || decode( data_type, 'NUMBER', 'nu', 'VARCHAR2', 'sb', 'DATE', 'dt', 'BOOLEAN', 'bo', 'CLOB', 'cl', 'BLOB', 'bl', 'XMLTYPE', 'xm' ) || substr( tcol.column_name, 1, 1 ) || lower( substr( tcol.column_name, 2 ) ) param_alias,
                    'i' || decode( data_type, 'NUMBER', 'nu', 'VARCHAR2', 'sb', 'DATE', 'dt', 'BOOLEAN', 'bo', 'CLOB', 'cl', 'BLOB', 'bl', 'XMLTYPE', 'xm' ) || substr( tcol.column_name, 1, 1 ) || lower( substr( tcol.column_name, 2 ) ) || ' ' || 'IN' || ' ' || lower( tcol.table_name || '.' || tcol.column_name) || '%TYPE'  param_entrada
            from    all_tab_columns tcol
            where   tcol.table_name = upper( csbTABLA )
            and     tcol.owner = upper( csbOWNER )
            ORDER BY column_id;
                    
        TYPE tytbPrimaria IS TABLE OF cuPrimaria%rowtype INDEX BY binary_integer;
        TYPE tytbCampos IS TABLE OF cuCampos%rowtype INDEX BY binary_integer;
        
        flArchivo       utl_file.file_type;
        tbPrimaria      tytbPrimaria;
        tbCampos        tytbCampos;
        rcPrimaria      cuPrimaria%rowtype;
        sbExiste        varchar2( 1 );    
        sbCadena        varchar2( 32767 );
        sbUser          varchar2( 20 );
        sbTipoDato      varchar2( 20 );    
        
        FUNCTION fsbTipoDato ( 
            isbTipoDato     IN  VARCHAR2,
            iboIsTable      IN  BOOLEAN DEFAULT FALSE,
            iboIsFunction   IN  BOOLEAN DEFAULT FALSE
        )
        RETURN VARCHAR2
        IS
            sbToken     VARCHAR2(20) := NULL;
            sbPrefijo   VARCHAR2(1) := 'i';
        BEGIN            
                
            -- Si es para una función, inicia con f
            IF (iboIsFunction) THEN
                sbPrefijo := 'f';
            END IF;
        
            -- Si es tipo para una tabla, siempre es tb
            IF (iboIsTable) THEN
                sbToken := sbPrefijo||'tb';
            ELSE
                -- Selecciona según el tipo de dato
                CASE UPPER(isbTipoDato) 
                    WHEN 'NUMBER' THEN
                        sbToken := sbPrefijo||'nu';
                    WHEN 'VARCHAR2' THEN
                        sbToken := sbPrefijo||'sb';
                    WHEN 'CHAR' THEN
                        sbToken := sbPrefijo||'sb';
                    WHEN 'DATE' THEN
                        sbToken := sbPrefijo||'dt';
                    WHEN 'CLOB' THEN
                        sbToken := sbPrefijo||'cl';
                    WHEN 'BLOB' THEN
                        sbToken := sbPrefijo||'bl';
                    WHEN 'XMLTYPE' THEN
                        sbToken := sbPrefijo||'xm';
                END CASE;
            END IF;
            RETURN sbToken;   
        END;
        
        /***************************************************************************
        <Procedure Fuente="Propiedad Intelectual de Empresas Públicas de Medellín">
          <Unidad>fboCampoEnPrimaryKey</Unidad>
          <Descripcion>
                Indica si un campo está en la llave primaria
          </Descripcion>
          <Autor>Diego Fernando Coba - MVM Ingenieria de Software</Autor>
          <Fecha> 03-Mar-2020 </Fecha>
          <Parametros>
            <param nombre="param1" tipo="TYPE" Direccion="In/Out" >
                Descripción
            </param>
            <param nombre="param2" tipo="TYPE" Direccion="In/Out" >
                Descripción
            </param>
            <param nombre="param3" tipo="TYPE" Direccion="In/Out" >
                Descripción
            </param>
          </Parametros>
          <Retorno Nombre="boEstaEnPK" Tipo="BOOLEAN">
                Indica si el campo está en la llave primaria
          </Retorno>
          <Historial>
            <Modificacion Autor="dcoba" Fecha="03-Mar-2020" Inc="">
                Creación.
            </Modificacion>
          </Historial>
        </Procedure>
        ***************************************************************************/
        FUNCTION fboCampoEnPrimaryKey 
        (
            itbLlavePrimaria    IN  tytbPrimaria,
            isbCampo            IN  all_tab_columns.COLUMN_NAME%TYPE
        )
        RETURN BOOLEAN
        IS       
            /* Indica si el campo está en la llave primaria */
            boEstaEnPK          BOOLEAN := FALSE;
            
            nuIndex             BINARY_INTEGER;
            
        BEGIN                                  

            /* Recorre la colección de registros */
            nuIndex := itbLlavePrimaria.FIRST;
            
            LOOP
                EXIT WHEN nuIndex IS NULL;
                
                /* Si el campo está en la PK retorna TRUE */
                IF ( itbLlavePrimaria(nuIndex).column_name = isbCampo ) THEN
                    boEstaEnPK := TRUE;
                    EXIT;
                END IF;
                    
                /* Avanza al siguiente registro */
                nuIndex := itbLlavePrimaria.NEXT(nuIndex);
            
            END LOOP;   
        
            RETURN boEstaEnPK;
                
        EXCEPTION
            WHEN Epm_Errors.EX_CTRLERROR THEN
                RAISE;
            WHEN OTHERS THEN
                Epm_Errors.SetError;
                RAISE Epm_Errors.EX_CTRLERROR;
        END fboCampoEnPrimaryKey; 
        
    BEGIN
        dbms_lob.createtemporary(lob_loc => oclFile, cache => true, dur => dbms_lob.session);
        --oclFile := NULL; --EMPTY_CLOB();
        --DBMS_LOB.APPEND(oclFile, CHR(10)||'PRUEBA');
        onuErrorCode := 0;
        osbErrorMessage := 1;
        
        dbms_output.enable( 1000000 );
    
        BEGIN
            select  'x'
            into    sbExiste
            from    all_tables
            where   table_name = upper( csbTABLA )
            and     owner = upper( csbOWNER )
            ;
        EXCEPTION
            when NO_DATA_FOUND then
                onuErrorCode := 101;
                osbErrorMessage := 'La tabla no existe';
                dbms_output.put_line( 'La tabla no existe' );
                raise;
        END;    
        
        gsbTableCap := upper( substr( csbTABLA, 1, 1 ) ) || lower( substr( csbTABLA, 2 ) );
        
        SELECT  USER usuario_exec
        INTO    sbUser
        FROM    dual;    
        
        if ( cuPrimaria%isopen ) then
            close cuPrimaria;
        end if;
        open cuPrimaria;
        fetch cuPrimaria bulk collect into tbPrimaria;
        close cuPrimaria;
        
        if ( cuCampos%isopen ) then
            close cuCampos;
        end if;
        open cuCampos;
        fetch cuCampos bulk collect into tbCampos;
        close cuCampos;
        
        --flArchivo := utl_file.fopen( csbRUTA_ARCHIVO, 'PKTBL' || upper( csbTABLA ) || '.sql', 'w', 32767 );
        
        DBMS_LOB.APPEND(oclFile,
'CREATE OR REPLACE PACKAGE pktbl' || gsbTableCap || '
IS
/**************************************************************************
    Copyright (c) 2020 EPM - Empresas Públicas de Medellín
    Archivo generado automaticamente.
    '|| sbUser ||' - MVM Ingeniería de Software S.A.S.
            
    Nombre      :   pktbl' || gsbTableCap || '
    Descripción :   Paquete de primer nivel, para la tabla ' || upper( csbTABLA ) || '
    Autor       :   Generador automatico paquetes de primer nivel.
    Fecha       :   ' || to_char( sysdate, 'dd/mm/yyyy' ) || '
    WO          :   ' || csbWO || '
            
    Historial de Modificaciones
    ---------------------------------------------------------------------------
    Fecha         Autor         Descripcion
    =====         =======       ===============================================
***************************************************************************/
                
    --------------------------------------------
    --  Type and Subtypes
    --------------------------------------------');
        
        if( tbCampos.first is not null ) then            
            DBMS_LOB.APPEND(oclFile, CHR(10)||'
    -- Define colecciones de cada columna de la tabla ' || gsbTableCap);
        
            for i in tbCampos.first .. tbCampos.last loop            

                    DBMS_LOB.APPEND(oclFile, '
    TYPE ty'||tbCampos( i ).column_name||' IS TABLE OF '||gsbTableCap||'.'||tbCampos( i ).column_name||'%TYPE INDEX BY BINARY_INTEGER;');

            end loop;
        
            DBMS_LOB.APPEND(oclFile, CHR(10)||'
    -- Define registro de colecciones');

            DBMS_LOB.APPEND(oclFile,'
    TYPE tytb'||gsbTableCap||' IS RECORD
    (');
        
            for i in tbCampos.first .. tbCampos.last loop            
                if ( i <> tbCampos.last ) then
                    sbCadena := sbCadena ||'
        '||tbCampos( i ).column_name||'  ty'||tbCampos( i ).column_name||',';
                else
                    sbCadena := sbCadena ||'
        '||tbCampos( i ).column_name||'  ty'||tbCampos( i ).column_name;
                end if;
            END loop;
            
            DBMS_LOB.APPEND(oclFile, sbCadena||'
    );');
        end if;
    
    DBMS_LOB.APPEND(oclFile,'
    --------------------------------------------
    -- Constants
    --------------------------------------------
        
    --------------------------------------------
    -- Variables
    --------------------------------------------');
        
        
        if( tbPrimaria.first is not null ) then
            DBMS_LOB.APPEND(oclFile, CHR(10)||'
    -- Cursor para accesar ' || gsbTableCap || '
    CURSOR cu' || gsbTableCap || '
    (');
        
            for i in tbPrimaria.first .. tbPrimaria.last loop
                sbCadena := tbPrimaria( i ).param_entrada;
                if ( i <> tbPrimaria.first ) then
                    sbCadena := ','||'
        '||sbCadena;
                ELSE
                    sbCadena := '
        '||sbCadena;
                end if;
                DBMS_LOB.APPEND(oclFile,sbCadena);
            end loop;
            
            DBMS_LOB.APPEND(oclFile,'
    )
    IS
        SELECT  *
        FROM    ' || lower( csbTABLA ));
            
            for i in tbPrimaria.first .. tbPrimaria.last loop
                if ( i = tbPrimaria.first ) then
                    sbCadena := '
        WHERE   ';
                else
                    sbCadena := '
        AND     ';
                end if;
            
                sbCadena := sbCadena || lower( tbPrimaria( i ).column_name ) || ' = ' || tbPrimaria( i ).alias;
                if ( i = tbPrimaria.last ) then
                    sbCadena := sbCadena || ';';
                end if;
                DBMS_LOB.APPEND(oclFile, sbCadena);
            end loop;
            
        end if;
        
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '
    --------------------------------------------
    -- Funciones y Procedimientos
    --------------------------------------------
    -- Insertar un registro
    PROCEDURE InsRecord
    (
        ircRecord in ' || lower( csbTABLA ) || '%rowtype
    );');
        
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '
    -- Insertar colección de registros
    PROCEDURE InsRecords
    (
        irctbRecord  IN OUT NOCOPY   tytb'||gsbTableCap||'
    );');
            
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '
    -- Insertar un record de tablas por columna
    PROCEDURE InsForEachColumn
    (');
        
        for i in tbCampos.first .. tbCampos.last loop     
            sbTipoDato := fsbTipoDato( tbCampos( i ).data_type );       
            if ( i <> tbCampos.last ) then                              
                DBMS_LOB.APPEND(oclFile, CHR(10)||          
        '       '||sbTipoDato||tbCampos( i ).column_name||' IN '||lower( csbTABLA )||'.'||tbCampos( i ).column_name||'%TYPE,');               
            else
                DBMS_LOB.APPEND(oclFile, CHR(10)||          
        '       '||sbTipoDato||tbCampos( i ).column_name||' IN '||lower( csbTABLA )||'.'||tbCampos( i ).column_name||'%TYPE');
            end if;    
        end loop;
        
        DBMS_LOB.APPEND(oclFile, CHR(10)||
'    );');
        
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '
    -- Insertar un record de tablas por columna masivamente
    PROCEDURE InsForEachColumnBulk
    (');
        
        for i in tbCampos.first .. tbCampos.last loop     
            sbTipoDato := fsbTipoDato( tbCampos( i ).data_type , TRUE);       
            if ( i <> tbCampos.last ) then                              
                DBMS_LOB.APPEND(oclFile, CHR(10)||          
        '       '||sbTipoDato||tbCampos( i ).column_name||' IN OUT NOCOPY ty'||tbCampos( i ).column_name||',');               
            else
                DBMS_LOB.APPEND(oclFile, CHR(10)||          
        '       '||sbTipoDato||tbCampos( i ).column_name||' IN OUT NOCOPY ty'||tbCampos( i ).column_name);
            end if;    
        end loop;
        
        DBMS_LOB.APPEND(oclFile, CHR(10)||
'    );');    
            
        if( tbPrimaria.first is not null ) then
        
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '
    -- Limpiar la memoria
    PROCEDURE ClearMemory;

    -- Eliminar un registro
    PROCEDURE DelRecord
    (');
        
            for i in tbPrimaria.first .. tbPrimaria.last loop
                sbCadena := tbPrimaria( i ).param_entrada;
                if ( i <> tbPrimaria.last ) then
                    sbCadena := sbCadena || ',';
                end if;
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || sbCadena);
            end loop;
    
            DBMS_LOB.APPEND(oclFile, CHR(10)||
'    );

    -- Actualizar un registro
    PROCEDURE UpRecord
    (
        ircRecord in ' || lower( csbTABLA ) || '%rowtype
    );
        
    -- Eliminar un grupo de registros
    PROCEDURE DelRecords
    (');
            for i in tbPrimaria.first .. tbPrimaria.last loop
                sbTipoDato := fsbTipoDato( tbPrimaria( i ).data_type, TRUE );
                if ( i <> tbPrimaria.last ) then
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || sbTipoDato||tbPrimaria( i ).column_name || ' IN OUT NOCOPY ty'||tbPrimaria( i ).column_name||',');
                else
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || sbTipoDato||tbPrimaria( i ).column_name || ' IN OUT NOCOPY ty'||tbPrimaria( i ).column_name);
                end if;
            end loop;
        DBMS_LOB.APPEND(oclFile, '     
    );
                
    -- Indica si el registro existe
    FUNCTION fblExist
    (');
        
            for i in tbPrimaria.first .. tbPrimaria.last loop
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || tbPrimaria( i ).param_entrada || ',');
            end loop;
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        inuCACHE IN NUMBER DEFAULT 1
    )
    RETURN BOOLEAN;
    
    -- Obtiene registro
    FUNCTION frcGetRecord
    (');
        
            for i in tbPrimaria.first .. tbPrimaria.last loop
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || tbPrimaria( i ).param_entrada || ',');
            end loop;
            
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        inuCACHE IN NUMBER DEFAULT 1
    )
    RETURN ' || lower( csbTABLA ) || '%ROWTYPE;
    
    -- Valida si existe un registro
    PROCEDURE AccKey
    (');
        
        for i in tbPrimaria.first .. tbPrimaria.last loop
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || tbPrimaria( i ).param_entrada || ',');
        end loop;
            
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '        inuCACHE IN NUMBER DEFAULT 1
    );
    
    -- Valida si está duplicad
    PROCEDURE ValidateDupValues
    (');
        
        for i in tbPrimaria.first .. tbPrimaria.last loop
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || tbPrimaria( i ).param_entrada || ',');
        end loop;
            
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '        inuCACHE IN NUMBER DEFAULT 1
    );
    ');        
    
        /* Recorre los campos de la PK para armar la lista de campos y de parámetros */
        FOR i IN tbPrimaria.first .. tbPrimaria.last LOOP

            IF (i = tbPrimaria.first ) THEN

                    IF (i = tbPrimaria.last) THEN
                        sbParamPK := tbPrimaria( i ).param_entrada||',';
                        sbWherePorPK := tbPrimaria(i).column_name||' = '||tbPrimaria( i ).alias;
                    ELSE
                        
                        sbWherePorPK := tbPrimaria(i).column_name||' = '||tbPrimaria( i ).alias||CHR(10)||'        ';
                    
                        sbParamPK := tbPrimaria( i ).param_entrada||',
        ';
                    END IF;
                    
                    sbVariablesPK := tbPrimaria( i ).alias||',
        ';        
                    sbCamposPKParaOutput := tbPrimaria( i ).alias;                    

            ELSIF (i = tbPrimaria.last) THEN
        
                    sbParamPK := sbParamPK ||tbPrimaria( i ).param_entrada||',';
                    
                    sbVariablesPK := sbVariablesPK ||tbPrimaria( i ).alias||',
        ';                
                    sbCamposPKParaOutput := sbCamposPKParaOutput||'||'' - ''||'||tbPrimaria( i ).alias;
                    
                    sbWherePorPK := tbPrimaria(i).column_name||' = '||tbPrimaria( i ).alias;
            
            ELSE
                    sbParamPK := sbParamPK ||tbPrimaria( i ).param_entrada||',
        ';                
                    sbVariablesPK := sbVariablesPK ||tbPrimaria( i ).alias||',
        ';                
                    sbCamposPKParaOutput := sbCamposPKParaOutput||'||'' - ''||'||tbPrimaria( i ).alias;
                    
                    sbWherePorPK := tbPrimaria(i).column_name||' = '||tbPrimaria( i ).alias||CHR(10)||'        ';
        
            END IF;
                                
        END LOOP;
        
        /* Prepara la variable para incrustarse en texto */
        sbCamposPKParaOutputCom := CHR(39)||'||'||sbCamposPKParaOutput||'||'||CHR(39);
        
        /* Recorre los campos creando las funciones para obtener campos puntuales */
        FOR i in tbCampos.FIRST .. tbCampos.LAST LOOP    

            -- No crea método para obtener o actualizar pk
            IF ( fboCampoEnPrimaryKey( tbPrimaria, tbCampos(i).column_name )) THEN
                CONTINUE;
            END IF;
                     
            -- Reemplaza el nombre de la tabla
            sbFunction := REPLACE(csbSpecTemplateFunction, '<Tabla>', Initcap(csbTABLA));                    
                            
            -- Reemplaza el nombre del campo
            sbFunction := REPLACE(sbFunction, '<Campo>', Initcap(tbCampos(i).column_name));
                            
            -- Reemplazan parámetros de entrada que son campos de la PK
            sbFunction := REPLACE(sbFunction, '<ParamPK>', sbParamPK);

            -- Reemplaza el tipo de función según el tipo de dato
            sbFunction := REPLACE(sbFunction, '<Type>', fsbTipoDato(tbCampos(i).data_type , FALSE, TRUE) );

            -- Escribe la función
            DBMS_LOB.APPEND(oclFile, sbFunction);

        END LOOP;
        
        /* Recorre los campos creando las funciones para obtener campos puntuales */
        FOR i in tbCampos.FIRST .. tbCampos.LAST LOOP    

            -- No crea método para obtener o actualizar pk
            IF ( fboCampoEnPrimaryKey( tbPrimaria, tbCampos(i).column_name )) THEN
                CONTINUE;
            END IF;
                       
            -- Reemplaza el nombre de la tabla
            sbFunction := REPLACE(csbSpecTemplateProc, '<Campo>', Initcap(tbCampos(i).column_name) );                    
                            
            -- Reemplazan parámetros de entrada que son campos de la PK
            sbFunction := REPLACE(sbFunction, '<ParamPK>', sbParamPK);

            -- Reemplaza el parámetro de entrada el campo
            sbFunction := REPLACE(sbFunction, '<ParamCampo>', tbCampos(i).param_entrada );

            -- Escribe la función
            DBMS_LOB.APPEND(oclFile, sbFunction);

        END LOOP;        
    
        end if; -- Si tiene PK
        
        DBMS_LOB.APPEND(oclFile, CHR(10)||CHR(10)||'END pktbl' || gsbTableCap || ';'||CHR(10)||'/'||CHR(10)||'CREATE OR REPLACE PACKAGE BODY pktbl' || gsbTableCap || '
IS    
    -------------------------
    --  PRIVATE VARIABLES
    -------------------------');
        
        if( tbPrimaria.first is not null ) then
            DBMS_LOB.APPEND(oclFile, CHR(10)||'
    -- Record Tabla ' || upper( csbTABLA ) || '
    rc' || gsbTableCap || ' cu' || gsbTableCap || '%ROWTYPE;
    
    -- Record nulo de la Tabla ' || upper( csbTABLA ) || '
    rcRecordNull ' || lower( csbTABLA ) || '%ROWTYPE;
        
    -------------------------
    --   PRIVATE METHODS   
    -------------------------
        
    PROCEDURE Load
    (');
        
        for i in tbPrimaria.first .. tbPrimaria.last loop
            sbCadena := tbPrimaria( i ).param_entrada;
            if ( i <> tbPrimaria.last ) then
                sbCadena := sbCadena || ',';
            end if;
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || sbCadena);
        end loop;
        DBMS_LOB.APPEND(oclFile, '
    );
        
    PROCEDURE LoadRecord
    (');
        
        for i in tbPrimaria.first .. tbPrimaria.last loop
            sbCadena := tbPrimaria( i ).param_entrada;
            if ( i <> tbPrimaria.last ) then
                sbCadena := sbCadena || ',';
            end if;
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || sbCadena);
        end loop;
        DBMS_LOB.APPEND(oclFile,'
    );
        
    FUNCTION fblInMemory
    (');
        
        for i in tbPrimaria.first .. tbPrimaria.last loop
            sbCadena := tbPrimaria( i ).param_entrada;
            if ( i <> tbPrimaria.last ) then
                sbCadena := sbCadena || ',';
            end if;
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || sbCadena);
        end loop;
        DBMS_LOB.APPEND(oclFile, '
    )
    RETURN BOOLEAN;');
        
        end if;
        
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '
    -----------------
    -- CONSTANTES
    -----------------
    CACHE                      CONSTANT NUMBER := 1;   -- Buscar en Cache
    
    -------------------------
    --  PRIVATE CONSTANTS
    -------------------------
    cnuRECORD_NO_EXISTE        CONSTANT NUMBER := ' || csbNO_EXISTE || '; -- Reg. no esta en BD
    cnuRECORD_YA_EXISTE        CONSTANT NUMBER := ' || csbEXISTE || '; -- Reg. ya esta en BD    
    -- Division
    csbDIVISION                CONSTANT VARCHAR2(20) := pkConstante.csbDIVISION;
    -- Modulo
    csbMODULE                  CONSTANT VARCHAR2(20) := pkConstante.csbMOD_CUZ;
    -- Texto adicionar para mensaje de error
    csbTABLA_PK                CONSTANT VARCHAR2(200):= ''(Tabla '||upper( csbTABLA )||') ( PK ['';
    csb_TABLA                  CONSTANT VARCHAR2(200):= ''(Tabla '||upper( csbTABLA )||')'';');

        
        if( tbPrimaria.first is not null ) then
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '
    -- Carga
    PROCEDURE Load
    (');
        
        for i in tbPrimaria.first .. tbPrimaria.last loop
            sbCadena := tbPrimaria( i ).param_entrada;
            if ( i <> tbPrimaria.last ) then
                sbCadena := sbCadena || ',';
            end if;
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || sbCadena);
        end loop;
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '    )
    IS
    BEGIN
        pkErrors.Push( ''pktbl' || gsbTableCap || '.Load'' );
        LoadRecord
        (');
            
        for i in tbPrimaria.first .. tbPrimaria.last loop
            sbCadena := tbPrimaria( i ).alias;
            if ( i <> tbPrimaria.last ) then
                sbCadena := sbCadena || ',';
            end if;
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '            ' || sbCadena);
        end loop;
            
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '        );
        -- Evalúa si se encontro el registro en la Base de datos
        IF ( rc' || gsbTableCap || '.' || lower( tbPrimaria( tbPrimaria.first ).column_name ) || ' IS NULL ) THEN
            pkErrors.Pop;
            RAISE NO_DATA_FOUND;
        END IF;
        pkErrors.Pop;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_NO_EXISTE );
            pkErrors.ADDSUFFIXTOMESSAGE ( csbTABLA_PK||'||sbCamposPKParaOutput||'||''] )'||CHR(39)||');
            pkErrors.Pop;
            RAISE LOGIN_DENIED;
    END Load;
    
    -- Carga    
    PROCEDURE LoadRecord
    (');
        
        for i in tbPrimaria.first .. tbPrimaria.last loop
            sbCadena := tbPrimaria( i ).param_entrada;
            if ( i <> tbPrimaria.last ) then
                sbCadena := sbCadena || ',';
            end if;
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || sbCadena);
        end loop;
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '    )
    IS
    BEGIN
        pkErrors.Push( ''pktbl' || gsbTableCap || '.LoadRecord'' );     
        IF ( cu' || gsbTableCap || '%ISOPEN ) THEN
            CLOSE cu' || gsbTableCap || ';
        END IF;
        -- Accesa ' || upper( csbTABLA ) || ' de la BD
        OPEN cu' || gsbTableCap || '
        (');
        for i in tbPrimaria.first .. tbPrimaria.last loop
            sbCadena := tbPrimaria( i ).alias;
            if ( i <> tbPrimaria.last ) then
                sbCadena := sbCadena || ',';
            end if;
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '            ' || sbCadena);
        end loop;
            
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '        );
        FETCH cu' || gsbTableCap || ' INTO rc' || gsbTableCap || ';
        IF ( cu' || gsbTableCap || '%NOTFOUND ) then
            rc' || gsbTableCap || ' := rcRecordNull;
        END IF;
        CLOSE cu' || gsbTableCap || ';
        pkErrors.Pop;
    
    END LoadRecord;    
    
    -- Indica si está en memoria  
    FUNCTION fblInMemory
    (');
        
        for i in tbPrimaria.first .. tbPrimaria.last loop
            sbCadena := tbPrimaria( i ).param_entrada;
            if ( i <> tbPrimaria.last ) then
                sbCadena := sbCadena || ',';
            end if;
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || sbCadena);
        end loop;
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '    )
    RETURN BOOLEAN
    IS
    BEGIN
        pkErrors.Push( ''pktbl' || gsbTableCap || '.fblInMemory'' );
        ');
            
        sbCadena := '
        IF ( ';
            
        for i in tbPrimaria.first .. tbPrimaria.last loop
            sbCadena := sbCadena || 'rc' || gsbTableCap || '.' || lower( tbPrimaria( i ).column_name ) || ' = ' || tbPrimaria( i ).alias;
            if ( i <> tbPrimaria.first) then
                sbCadena := '
            AND '||sbCadena;
            END if;
            DBMS_LOB.APPEND(oclFile, sbCadena);
            sbCadena := NULL;
        end loop;
        sbCadena := ' ) THEN';
        DBMS_LOB.APPEND(oclFile,sbCadena ||'
            pkErrors.Pop;
            RETURN( TRUE );
        END IF;
        pkErrors.Pop;
        RETURN( FALSE );
    
    END fblInMemory;
    
    -- Valida si existe registro
    PROCEDURE AccKey
    (');
        
        for i in tbPrimaria.first .. tbPrimaria.last loop
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || tbPrimaria( i ).param_entrada || ',');
        end loop;
            
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '        inuCACHE IN NUMBER DEFAULT 1
    )
    IS
    BEGIN
        pkErrors.Push( ''pktbl' || gsbTableCap || '.AccKey'' );
            
        --Valida si debe buscar primero en memoria Cache
        IF NOT (inuCACHE = CACHE AND fblInMemory(');
            
        for i in tbPrimaria.first .. tbPrimaria.last loop
            sbCadena := tbPrimaria( i ).alias;
            if ( i <> tbPrimaria.first ) then
                sbCadena := ', '||sbCadena;
            end if;
            DBMS_LOB.APPEND(oclFile, sbCadena);
        end loop;
            
        DBMS_LOB.APPEND(oclFile, ') ) THEN
        
            Load
            (');
                
            for i in tbPrimaria.first .. tbPrimaria.last loop
                sbCadena := tbPrimaria( i ).alias;
                if ( i <> tbPrimaria.last ) then
                    sbCadena := sbCadena || ',';
                end if;
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '                ' || sbCadena);
            end loop;
                
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '            );
        END IF;
        pkErrors.Pop;
    EXCEPTION
        WHEN LOGIN_DENIED THEN
            pkErrors.Pop;
            RAISE LOGIN_DENIED;
    END AccKey;
    
    -- Limpia memoria
    PROCEDURE ClearMemory 
    IS
    BEGIN
        pkErrors.Push( ''pktbl' || gsbTableCap || '.ClearMemory'' );
        rc' || gsbTableCap || ' := rcRecordNull;
        pkErrors.Pop;
    END ClearMemory;
    
    -- Elimina registro    
    PROCEDURE DelRecord
    (');
        
        for i in tbPrimaria.first .. tbPrimaria.last loop
            sbCadena := tbPrimaria( i ).param_entrada;
            if ( i <> tbPrimaria.last ) then
                sbCadena := sbCadena || ',';
            end if;
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || sbCadena);
        end loop;
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '    )
    IS
    BEGIN
        pkErrors.Push( ''pktbl' || gsbTableCap || '.DelRecord'' );
            
        --Elimina registro de la Tabla ' || upper( csbTABLA ) || '
        DELETE  ' || lower( csbTABLA ));
            
        for i in tbPrimaria.first .. tbPrimaria.last loop
            if ( i = tbPrimaria.first ) then
                sbCadena := 'WHERE   ';
            else
                sbCadena := 'AND     ';    
            end if;
            
            sbCadena := sbCadena || lower( tbPrimaria( i ).column_name ) || ' = ' || tbPrimaria( i ).alias;
            if ( i = tbPrimaria.last ) then
                sbCadena := sbCadena || ';';
            end if;
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || sbCadena);
        end loop;
            
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '
        IF ( sql%NOTFOUND ) THEN
            pkErrors.Pop;
            RAISE NO_DATA_FOUND;
        END IF;
        pkErrors.Pop;
            
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_NO_EXISTE );
                pkErrors.ADDSUFFIXTOMESSAGE ( csbTABLA_PK||'||sbCamposPKParaOutput||'||''] )'||CHR(39)||');
                pkErrors.Pop;
                RAISE LOGIN_DENIED;
    END DelRecord;');
        
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '    
    -- Elimina registros
    PROCEDURE DelRecords
    (');
        for i in tbPrimaria.first .. tbPrimaria.last loop
            sbTipoDato := fsbTipoDato( tbPrimaria( i ).data_type, TRUE );
            if ( i <> tbPrimaria.last ) then
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || sbTipoDato||tbPrimaria( i ).column_name || ' IN OUT NOCOPY ty'||tbPrimaria( i ).column_name||',');
            else
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || sbTipoDato||tbPrimaria( i ).column_name || ' IN OUT NOCOPY ty'||tbPrimaria( i ).column_name);
            end if;
        end loop;
    DBMS_LOB.APPEND(oclFile,'
    )
    IS
    BEGIN
        pkErrors.Push( ''pktbl' || gsbTableCap || '.DelRecords'' );
            
        -- Elimina registros de la Tabla '||lower( csbTABLA ));
            
        for i in tbPrimaria.first .. tbPrimaria.last loop
            sbTipoDato := fsbTipoDato( tbPrimaria( i ).data_type, TRUE );
            if ( i = tbPrimaria.first ) then
                DBMS_LOB.APPEND(oclFile, CHR(10)|| 
        '        FORALL indx IN '||sbTipoDato||tbPrimaria( i ).column_name||'.FIRST .. '||sbTipoDato||tbPrimaria( i ).column_name||'.LAST');
                DBMS_LOB.APPEND(oclFile, CHR(10)|| 
        '        DELETE '||tbPrimaria( i ).table_name||' WHERE '||tbPrimaria( i ).column_name||' = '||sbTipoDato||tbPrimaria( i ).column_name||'(indx)');
            elsif ( i <> tbPrimaria.last ) then
                DBMS_LOB.APPEND(oclFile, CHR(10)|| 
        '        AND '||tbPrimaria( i ).column_name||' = '||sbTipoDato||tbPrimaria( i ).column_name||'(indx)');
            else
                DBMS_LOB.APPEND(oclFile, CHR(10)|| 
        '        AND '||tbPrimaria( i ).column_name||' = '||sbTipoDato||tbPrimaria( i ).column_name||'(indx)');
            end if;            
        end loop;
        DBMS_LOB.APPEND(oclFile,';');
            
            DBMS_LOB.APPEND(oclFile, CHR(10)|| 
        '
        IF ( SQL%NOTFOUND ) THEN
            pkErrors.Pop;
            RAISE NO_DATA_FOUND;
        END IF;
            
        pkErrors.Pop;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_NO_EXISTE );   
            pkErrors.ADDSUFFIXTOMESSAGE ( csb_TABLA );
            pkErrors.Pop;
            RAISE LOGIN_DENIED;
    END DelRecords;');
        
        end if;
        
        
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '    
    -- Inserta registro por columna
    PROCEDURE InsForEachColumn
    (');
        
    for i in tbCampos.first .. tbCampos.last loop     
        sbTipoDato := fsbTipoDato( tbCampos( i ).data_type );       
        if ( i <> tbCampos.last ) then                              
            DBMS_LOB.APPEND(oclFile, CHR(10)||          
    '       '||sbTipoDato||tbCampos( i ).column_name||' IN '||lower( csbTABLA )||'.'||tbCampos( i ).column_name||'%TYPE,');               
        else
            DBMS_LOB.APPEND(oclFile, CHR(10)||          
    '       '||sbTipoDato||tbCampos( i ).column_name||' IN '||lower( csbTABLA )||'.'||tbCampos( i ).column_name||'%TYPE');
        end if;    
    end loop;
        
    DBMS_LOB.APPEND(oclFile, CHR(10)|| '
    )
    IS
      rcRecord ' || lower( csbTABLA ) || '%ROWTYPE;   -- Record de la Tabla '||lower( csbTABLA )||'
    BEGIN
       pkErrors.Push( ''pktbl'||lower( csbTABLA )||'.InsForEachColumn '');');
           
    for i in tbCampos.first .. tbCampos.last loop     
        sbTipoDato := fsbTipoDato( tbCampos( i ).data_type );  
        --rcRecord.crapcodi := inuCrapcodi;     
        if ( i <> tbCampos.last ) then                              
            DBMS_LOB.APPEND(oclFile, CHR(10)||          
    '       rcRecord.'||tbCampos( i ).column_name||' := '||sbTipoDato||tbCampos( i ).column_name||';');               
        else
            DBMS_LOB.APPEND(oclFile, CHR(10)||          
    '       rcRecord.'||tbCampos( i ).column_name||' := '||sbTipoDato||tbCampos( i ).column_name||';');
        end if;    
    end loop;
        
    DBMS_LOB.APPEND(oclFile, CHR(10)|| '
       InsRecord( rcRecord );
       pkErrors.Pop;
    EXCEPTION
        WHEN LOGIN_DENIED THEN
            pkErrors.Pop;
            RAISE LOGIN_DENIED;
    END InsForEachColumn;');
        
        
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '    
    -- Inserta registro de tablas por campo masivamente
    PROCEDURE InsForEachColumnBulk
    (');
        
    for i in tbCampos.first .. tbCampos.last loop     
        sbTipoDato := fsbTipoDato( tbCampos( i ).data_type, TRUE );       
        if ( i <> tbCampos.last ) then                              
            DBMS_LOB.APPEND(oclFile, CHR(10)||          
    '       '||sbTipoDato||tbCampos( i ).column_name||' IN OUT NOCOPY ty'||tbCampos( i ).column_name||',');               
        else
            DBMS_LOB.APPEND(oclFile, CHR(10)||          
    '       '||sbTipoDato||tbCampos( i ).column_name||' IN OUT NOCOPY ty'||tbCampos( i ).column_name);
        end if;    
    end loop;
        
    DBMS_LOB.APPEND(oclFile,'
    )
    IS      
    BEGIN
        pkErrors.Push('' pktbl'||lower( csbTABLA )||'.InsForEachColumnBulk '');');
           
       for i in tbCampos.first .. tbCampos.last loop
            sbTipoDato := fsbTipoDato( tbCampos( i ).data_type , TRUE);
            if ( i = tbCampos.first ) THEN
                
                DBMS_LOB.APPEND(oclFile, CHR(10)||'
        FORALL indx in '||sbTipoDato||tbCampos( i ).column_name||'.FIRST .. '||sbTipoDato||tbCampos( i ).column_name||'.LAST');
                DBMS_LOB.APPEND(oclFile,'
        INSERT INTO '||lower( csbTABLA )||'
        (');
                DBMS_LOB.APPEND(oclFile,' 
            '||tbCampos( i ).column_name||',');
        
            elsif ( i <> tbCampos.last ) then
                DBMS_LOB.APPEND(oclFile,' 
            '||tbCampos( i ).column_name||',');
        
            else
                DBMS_LOB.APPEND(oclFile,' 
            '||tbCampos( i ).column_name||'
        )
        VALUES 
        (');
        
            end if;            
        end loop;
            
        for i in tbCampos.first .. tbCampos.last loop
            sbTipoDato := fsbTipoDato( tbCampos( i ).data_type, TRUE );
            if ( i <> tbCampos.last ) THEN
                DBMS_LOB.APPEND(oclFile,'
            '||sbTipoDato||tbCampos( i ).column_name||'(indx),');
            ELSE
                DBMS_LOB.APPEND(oclFile,'
            '||sbTipoDato||tbCampos( i ).column_name||'(indx)');
            END if;
        END loop;
        
        DBMS_LOB.APPEND(oclFile,'
        );');
           
    DBMS_LOB.APPEND(oclFile, CHR(10)|| '
       pkErrors.Pop;
    EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
            pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_YA_EXISTE );
            pkErrors.ADDSUFFIXTOMESSAGE ( csb_TABLA );
            pkErrors.Pop;
            RAISE LOGIN_DENIED;
    END InsForEachColumnBulk;');
        
        
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '    
    -- Inserta un registro
    PROCEDURE InsRecord
    (
        ircRecord in ' || lower( csbTABLA ) || '%ROWTYPE
    )
    IS
    BEGIN
        pkErrors.Push( ''pktbl' || gsbTableCap || '.InsRecord'' );
            
        INSERT INTO ' || lower( csbTABLA ) || '
        (');
            
    for i in tbCampos.first .. tbCampos.last loop
            
        sbCadena := tbCampos( i ).alias;
            
        if ( i <> tbCampos.last ) then
            sbCadena := sbCadena || ',';
        end if;
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '            ' || sbCadena);
    end loop;
            
    DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ) 
        VALUES 
        (');
        
    for i in tbCampos.first .. tbCampos.last loop
            
        sbCadena := 'ircRecord.' || tbCampos( i ).alias;
            
        if ( i <> tbCampos.last ) then
            sbCadena := sbCadena || ',';
        end if;
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '            ' || sbCadena);
    end loop;
    DBMS_LOB.APPEND(oclFile, CHR(10)|| '        );
        
        pkErrors.Pop;
    
    EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
            pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_YA_EXISTE );
            pkErrors.ADDSUFFIXTOMESSAGE ( csb_TABLA );
            pkErrors.Pop;
            RAISE LOGIN_DENIED;
    END InsRecord;');
        
        
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '    
    -- Inserta tabla de registros
    PROCEDURE InsRecords
    (
        irctbRecord  IN OUT NOCOPY   tytb'||csbTABLA||
    '
    )
    IS      
    BEGIN
        pkErrors.Push('' pktbl'||lower( csbTABLA )||'.InsRecords'' );');
           
       for i in tbCampos.first .. tbCampos.last loop
            sbTipoDato := fsbTipoDato( tbCampos( i ).data_type );
            if ( i = tbCampos.first ) then
                DBMS_LOB.APPEND(oclFile,'
        FORALL indx IN irctbRecord.'||tbCampos( i ).column_name||'.FIRST .. irctbRecord.'||tbCampos( i ).column_name||'.LAST');
                DBMS_LOB.APPEND(oclFile, '
        INSERT INTO '||lower( csbTABLA )||'
        (');
                DBMS_LOB.APPEND(oclFile,'
            '||tbCampos( i ).column_name||',');
            elsif ( i <> tbCampos.last ) then
                DBMS_LOB.APPEND(oclFile,' 
            '||tbCampos( i ).column_name||',');
            else
                DBMS_LOB.APPEND(oclFile,' 
            '||tbCampos( i ).column_name||'
         )
         VALUES 
         (');
            end if;            
        end loop;
            
        for i in tbCampos.first .. tbCampos.last loop
            sbTipoDato := fsbTipoDato( tbCampos( i ).data_type );
            if ( i <> tbCampos.last ) then
                DBMS_LOB.APPEND(oclFile,'
            irctbRecord.'||tbCampos( i ).column_name||'(indx),');
            else
                DBMS_LOB.APPEND(oclFile,'
            irctbRecord.'||tbCampos( i ).column_name||'(indx)');
            end if;            
        end loop;
            
        DBMS_LOB.APPEND(oclFile,'
        );');
           
    DBMS_LOB.APPEND(oclFile, CHR(10)|| '
        pkErrors.Pop;
    EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
            pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_YA_EXISTE );
            pkErrors.ADDSUFFIXTOMESSAGE ( csb_TABLA );
            pkErrors.Pop;
            RAISE LOGIN_DENIED;
    END InsRecords;');
        
        
        if( tbPrimaria.first is not null ) then
        
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '    
    -- Actualiza registro
    PROCEDURE UpRecord
    (
        ircRecord IN ' || lower( csbTABLA ) || '%ROWTYPE
    )
    IS
    BEGIN
        pkErrors.Push( ''pktbl' || gsbTableCap || '.UpRecord'' );
            
        UPDATE  ' || lower( csbTABLA ));
            
        for i in tbCampos.first .. tbCampos.last loop
            
            if ( i = tbCampos.first ) then
                sbCadena := '        SET     ' || tbCampos( i ).alias || ' = ircRecord.' || tbCampos( i ).alias;
            else
                sbCadena := '                ' || tbCampos( i ).alias || ' = ircRecord.' || tbCampos( i ).alias;
            end if;
    
                   
            if ( i <> tbCampos.last ) then
                sbCadena := sbCadena || ',';
            end if;
            DBMS_LOB.APPEND(oclFile, CHR(10)|| sbCadena);
        end loop;
            
        for i in tbPrimaria.first .. tbPrimaria.last loop
            if ( i = tbPrimaria.first ) then
                sbCadena := 'WHERE   ';
            else
                sbCadena := 'AND     ';    
            end if;
            
            sbCadena := sbCadena || lower( tbPrimaria( i ).column_name ) || ' = ircRecord.' || lower( tbPrimaria( i ).column_name );
            if ( i = tbPrimaria.last ) then
                sbCadena := sbCadena || ';';
            end if;
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || sbCadena);
        end loop;
            
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '
        IF ( SQL%NOTFOUND ) THEN
            pkErrors.Pop;
            RAISE NO_DATA_FOUND;
        END IF;
            
        pkErrors.Pop;
    
    EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
            pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_YA_EXISTE );
            pkErrors.ADDSUFFIXTOMESSAGE ( csb_TABLA );
            pkErrors.Pop;
            RAISE LOGIN_DENIED;
        WHEN NO_DATA_FOUND THEN
            pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_NO_EXISTE );
            pkErrors.ADDSUFFIXTOMESSAGE ( csb_TABLA );
            pkErrors.Pop;
            RAISE LOGIN_DENIED;
    END UpRecord;
    
    -- Valida duplicados
    PROCEDURE ValidateDupValues
    (');
        
        for i in tbPrimaria.first .. tbPrimaria.last loop
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || tbPrimaria( i ).param_entrada || ',');
        end loop;
            
        DBMS_LOB.APPEND(oclFile,'
        inuCACHE    IN NUMBER DEFAULT 1
    )
    IS
    BEGIN
        pkErrors.Push( ''pktbl' || gsbTableCap || '.ValidateDupValues'' );
            
        --Valida si el registro ya existe
        IF ( fblExist( ');
            
        for i in tbPrimaria.first .. tbPrimaria.last loop
            DBMS_LOB.APPEND(oclFile, tbPrimaria( i ).alias|| ', ');
        end loop;
            
        DBMS_LOB.APPEND(oclFile, 'inuCACHE ) ) THEN
            pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_YA_EXISTE );
            pkErrors.ADDSUFFIXTOMESSAGE ( csbTABLA_PK||'||sbCamposPKParaOutput||'||''] )'||CHR(39)||');
            RAISE LOGIN_DENIED;
        END IF;
            
        pkErrors.Pop;
         
    EXCEPTION
        WHEN LOGIN_DENIED THEN
            pkErrors.Pop;
            RAISE LOGIN_DENIED;
    END ValidateDupValues;
    
    -- Valida si el registro existe
    FUNCTION fblExist
    (');
        
        for i in tbPrimaria.first .. tbPrimaria.last loop
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || tbPrimaria( i ).param_entrada || ',');
        end loop;
            
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '        inuCACHE    IN NUMBER DEFAULT 1
    )
    RETURN BOOLEAN
    IS
    BEGIN
        pkErrors.Push( ''pktbl' || gsbTableCap || '.fblExist'' );
            
        --Valida si debe buscar primero en memoria Caché
        IF (inuCACHE = CACHE AND fblInMemory( ');
            
        for i in tbPrimaria.first .. tbPrimaria.last loop
            sbCadena := tbPrimaria( i ).alias;
            if ( i <> tbPrimaria.last ) then
                sbCadena := sbCadena || ',';
            end if;
            DBMS_LOB.APPEND(oclFile, sbCadena);
        end loop;
            
        DBMS_LOB.APPEND(oclFile, ' ) ) THEN
                pkErrors.Pop;
                RETURN( TRUE );
        END IF;
        LoadRecord
        (');
            
        for i in tbPrimaria.first .. tbPrimaria.last loop
            sbCadena := tbPrimaria( i ).alias;
            if ( i <> tbPrimaria.last ) then
                sbCadena := sbCadena || ',';
            end if;
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '            ' || sbCadena);
        end loop;
            
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '        );
            
        -- Evalúa si se encontro el registro en la Base de datos
        IF ( rc' || gsbTableCap || '.' || lower( tbPrimaria( tbPrimaria.first ).column_name ) || ' IS NULL ) THEN
            pkErrors.Pop;
            RETURN( FALSE );
        END IF;
            
        pkErrors.Pop;
    
        RETURN( TRUE );
        
    END fblExist;
    
    -- Obtiene el registro
    FUNCTION frcGetRecord
    (');
        
        for i in tbPrimaria.first .. tbPrimaria.last loop
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || tbPrimaria( i ).param_entrada || ',');
        end loop;
            
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '        inuCACHE    IN NUMBER DEFAULT 1
    )
    RETURN ' || lower( csbTABLA ) || '%ROWTYPE
    IS
    BEGIN
        pkErrors.Push( ''pktbl' || gsbTableCap || '.frcGetRecord'' );
            
        --Valida si el registro ya existe
        AccKey
        (');
            
        for i in tbPrimaria.first .. tbPrimaria.last loop
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '            ' || tbPrimaria( i ).alias || ',');
        end loop;
            
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '            inuCACHE
        );
    
        pkErrors.Pop;
        RETURN ( rc' || gsbTableCap || ' );
         
    EXCEPTION
        WHEN LOGIN_DENIED THEN
            pkErrors.Pop;
            RAISE LOGIN_DENIED;
    END frcGetRecord;
    ');
            
    /* Recorre los campos creando las funciones para obtener campos puntuales */
    FOR i in tbCampos.FIRST .. tbCampos.LAST LOOP    
        
        -- No crea método para obtener o actualizar pk
        IF ( fboCampoEnPrimaryKey( tbPrimaria, tbCampos(i).column_name )) THEN
            CONTINUE;
        END IF;
                              
        -- Reemplaza el nombre de la tabla
        sbFunction := REPLACE(csbTemplateFunction, '<Tabla>', Initcap(csbTABLA));                    
                        
        -- Reemplaza el nombre del campo
        sbFunction := REPLACE(sbFunction, '<Campo>', Initcap(tbCampos(i).column_name));                                
                        
        -- Reemplazan parámetros de entrada que son campos de la PK
        sbFunction := REPLACE(sbFunction, '<ParamPK>', sbParamPK);

        -- Reemplaza las variables de la pk
        sbFunction := REPLACE(sbFunction, '<pk>', sbVariablesPK);

        -- Reemplaza el tipo de función según el tipo de dato
        sbFunction := REPLACE(sbFunction, '<Type>', fsbTipoDato(tbCampos(i).data_type , FALSE, TRUE) );

        -- Escribe la función
        DBMS_LOB.APPEND(oclFile, sbFunction);

    END LOOP;
    
    /* Recorre los campos creando los procedimientos para actualizar campos puntuales */
    FOR i in tbCampos.FIRST .. tbCampos.LAST LOOP    

        -- No crea método para obtener o actualizar pk
        IF ( fboCampoEnPrimaryKey( tbPrimaria, tbCampos(i).column_name )) THEN
            CONTINUE;
        END IF;

        -- Reemplaza el nombre de la tabla
        sbFunction := REPLACE(csbTemplateUpdate, '<Tabla>', Initcap(csbTABLA));                    
                        
        -- Reemplaza el nombre del campo
        sbFunction := REPLACE(sbFunction, '<Campo>', Initcap(tbCampos(i).column_name));
        
        -- Reemplaza el parámetro de entrada el campo
        sbFunction := REPLACE(sbFunction, '<ParamCampo>', tbCampos(i).param_entrada );
        
        -- Reemplaza el nombre del parámetro de entrada del campo
        sbFunction := REPLACE(sbFunction, '<AliasCampo>', tbCampos(i).param_alias );
                        
        -- Reemplazan parámetros de entrada que son campos de la PK
        sbFunction := REPLACE(sbFunction, '<ParamPK>', sbParamPK);

        -- Reemplaza las variables de la pk
        sbFunction := REPLACE(sbFunction, '<pk>', sbVariablesPK);

        -- Reemplaza el tipo de función según el tipo de dato
        sbFunction := REPLACE(sbFunction, '<Type>', fsbTipoDato(tbCampos(i).data_type , FALSE, TRUE) );

        -- Reemplaza los campos de la pk en el Where del update
        sbFunction := REPLACE(sbFunction, '<Condicion>', sbWherePorPK);

        -- Reemplaza los campos de la pk en el mensaje de error
        sbFunction := REPLACE(sbFunction, '<sbCamposPKParaOutput>', sbCamposPKParaOutputCom);

        -- Reemplaza los campos de la pk en el mensaje de error
        sbFunction := REPLACE(sbFunction, '<sbCamposPKParaOutputSinCom>', sbCamposPKParaOutput);

        -- Escribe la función
        DBMS_LOB.APPEND(oclFile, sbFunction);

    END LOOP;    
    
    END IF;
        
        DBMS_LOB.APPEND(oclFile, CHR(10)||'END pktbl' || gsbTableCap || ';'||CHR(10)||'/');
    
        --utl_file.fclose( flArchivo );
        
        dbms_output.put_line( 'Se creó el archivo PKTBL' || upper( csbTABLA ) || '.sql en la ruta ' || csbRUTA_ARCHIVO );
    EXCEPTION
        when OTHERS then
            onuErrorCode := 100;
            osbErrorMessage := sqlerrm;
            dbms_output.put_line( sqlerrm );
            /*if ( utl_file.is_open( flArchivo )) then
                utl_file.fclose( flArchivo );
            end if;*/
    END;    
END pkg_utilmark;
/
