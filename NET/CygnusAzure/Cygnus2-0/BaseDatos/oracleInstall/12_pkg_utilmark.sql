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
    
    PROCEDURE pObtTareasBD
    (
        inuHoja          IN NUMBER,
        isbUsuario       IN VARCHAR2,
        onuRefCursor    OUT tyRefCursor,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2 
    );
    
    PROCEDURE pInsertaTareaAzure
    (
        isbDescripcion  IN VARCHAR2,
        isbIdAzure      IN NUMBER,
        isbEstado       IN VARCHAR2,
        isbUsuario      IN VARCHAR2,
        inuCompletado   IN NUMBER,
        isbFechaCreacion IN VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2,
        isbIdHU         IN NUMBER DEFAULT 0
    );
    
    PROCEDURE pinshorashoja
    (
        inuid_hoja IN NUMBER,
        idtfecha   IN DATE,
        inuMon     IN NUMBER,
        inuTue     IN NUMBER,
        inuWed     IN NUMBER,
        inuThu     IN NUMBER,
        inuFri     IN NUMBER,
        inuSat     IN NUMBER,
        inuSun     IN NUMBER,
        isbusuario IN VARCHAR2,
        isbobservaciones IN VARCHAR2,
        inurequerimiento IN NUMBER,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2  
    );
    
    PROCEDURE pActualizaTarea
    (
        inuHoja         IN NUMBER,
        inuHorasHoja    IN NUMBER,
        inuRq           IN NUMBER,
        isbDescripcion  IN VARCHAR2,
        isbIdAzure      IN NUMBER,
        isbEstado       IN VARCHAR2,
        isbUsuario      IN VARCHAR2,
        inuMon     IN NUMBER,
        inuTue     IN NUMBER,
        inuWed     IN NUMBER,
        inuThu     IN NUMBER,
        inuFri     IN NUMBER,
        inuSat     IN NUMBER,
        inuSun     IN NUMBER,
        inuCompletado IN NUMBER,
        onuSeqNeg       OUT NUMBER,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2,
        isbIdHU         IN NUMBER DEFAULT 0,
        isbFechaIni     IN VARCHAR2 DEFAULT NULL 
    );
    
    PROCEDURE pEliminaTarea
    (
        inuHorasHoja    IN NUMBER,
        inuRequerimiento    IN NUMBER,
        inuIdHoja    IN NUMBER,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2 
    );
    
    PROCEDURE pObtHojasBD
    (
        isbUsuario       IN VARCHAR2,
        onuRefCursor    OUT tyRefCursor,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2 
    );
    
    PROCEDURE pObtDetalleRq
    (
        inuRq           IN NUMBER,
        onuTotal        OUT NUMBER,
        onuTotalAzure   OUT NUMBER,
        onuHU           OUT NUMBER,
        onuTotalHU      OUT NUMBER,
        onuRefCursor    OUT tyrefcursor,
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
    
    PROCEDURE pEliminarAreaAzure
    (        
        isbArea          IN  VARCHAR2,
        isbUsuario       IN  VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    );
    
    PROCEDURE pCombinarTareas
    (
        inuOrigen       IN NUMBER,
        inuDestino      IN NUMBER,
        isbUsuario      IN VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2 
    );
        
    PROCEDURE pCargarTareasPred
    (
        onuRefCursor    OUT tyRefCursor,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2 
    );
END pkg_utilmark;
/
CREATE OR REPLACE PACKAGE BODY pkg_utilmark
AS
    CURSOR cuHojas
    IS
       SELECT codigo,fecha_fin
       FROM flex.ll_hoja
       WHERE trunc(SYSDATE) BETWEEN fecha_ini AND fecha_fin;
           
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
             201,
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
        AND instr(','||sbValoresExcluir||',',','||menscodi||',') = 0;
        
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
            OR DOMINGO > 0;    
        
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
            SET rol = isbCodigo,
                email = isbEmail
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
        sbOrden        VARCHAR2(100);
        sbUsuarioCon   VARCHAR2(100);
        nuCantidad     NUMBER;
        nuExiste       NUMBER;
        
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
    
    PROCEDURE pInsertaTareaAzure
    (
        isbDescripcion  IN VARCHAR2,
        isbIdAzure      IN NUMBER,
        isbEstado       IN VARCHAR2,
        isbUsuario      IN VARCHAR2,
        inuCompletado   IN NUMBER,
        isbFechaCreacion IN VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2,
        isbIdHU         IN NUMBER DEFAULT 0
    )
    IS
       nucodigo NUMBER(10);
	   nuCodHojaSigSem NUMBER;
       nucodigoRq NUMBER;
       nuCodHoja NUMBER;
       dtFechaFin DATE;
       dtFechaCreaAzure DATE;
       nuTotalRq NUMBER;
       dia_semana NUMBER;
       lunes NUMBER;
       martes NUMBER;
       miercoles NUMBER;
       jueves NUMBER;
       viernes NUMBER;
       sabado NUMBER;
       domingo NUMBER;
       dtFechaInCygnus DATE;
        
       CURSOR cuRq
       IS
            SELECT codigo,fecha_inicio
            FROM flex.ll_requerimiento
            WHERE id_azure = isbIdAzure
            AND usuario = isbUsuario;
			
	   CURSOR cuSigSemana
       (
            idtFecha DATE
        )
       IS
            SELECT codigo
            FROM flex.ll_hoja
            WHERE trunc(idtFecha+7) BETWEEN fecha_ini AND fecha_fin;
            
        CURSOR cuHojasAzure
        (
            idtFecha DATE
        )
        IS
           SELECT codigo,fecha_fin
           FROM flex.ll_hoja
           WHERE trunc(idtFecha) BETWEEN fecha_ini AND fecha_fin;
           
       CURSOR cuTotalRq
       (
            inuReq NUMBER
       )
        IS
            SELECT (sum(lunes) + sum(martes) + sum(miercoles) + sum(jueves) + sum(viernes) + sum(sabado) + sum(domingo)) horas
            FROM flex.ll_horashoja hh
            WHERE requerimiento = inuReq;
    BEGIN
       pInicializaError(onuErrorCode,osbErrorMessage);
       
       dtFechaCreaAzure := to_date(isbFechaCreacion,'dd/mm/yyyy');
       dia_semana := to_char(dtFechaCreaAzure,'D');
       
       OPEN cuRq;
       FETCH cuRq INTO nucodigoRq,dtFechaInCygnus;
       CLOSE cuRq;
       
       OPEN cuHojasAzure(dtFechaCreaAzure);
       FETCH cuHojasAzure INTO nuCodHoja,dtFechaFin;
       CLOSE cuHojasAzure;
       
       OPEN cuSigSemana(dtFechaCreaAzure);
       FETCH cuSigSemana INTO nuCodHojaSigSem;
       CLOSE cuSigSemana;
       
       IF(nucodigoRq IS NULL)THEN
           nucodigo := flex.seq_ll_requerimiento.nextval;
            
           INSERT INTO flex.ll_requerimiento
           (codigo,descripcion,id_azure,estado,usuario,fecha_actualiza,fecha_display,completado,fecha_registro,hist_usuario,fecha_inicio)       
           VALUES 
           (nucodigo,isbDescripcion,isbIdAzure,isbEstado,isbUsuario,NULL,(dtFechaFin+7),inuCompletado,SYSDATE,isbIdHU,dtFechaCreaAzure);
           
           COMMIT;
           
           lunes := 0;
           martes := 0;
           miercoles := 0;
           jueves := 0;
           viernes := 0;
           sabado := 0;
           domingo := 0;
           
           CASE dia_semana
               WHEN 1 THEN domingo := inuCompletado;
               WHEN 2 THEN lunes := inuCompletado;
               WHEN 3 THEN martes := inuCompletado;
               WHEN 4 THEN miercoles := inuCompletado;
               WHEN 5 THEN jueves := inuCompletado;
               WHEN 6 THEN viernes := inuCompletado;
               WHEN 7 THEN sabado := inuCompletado;
           END CASE;
           
           pinshorashoja
           (
               nuCodHoja,
               SYSDATE,
               lunes,
               martes,
               miercoles,
               jueves,
               viernes,
               sabado,
               domingo,
               isbUsuario,
               NULL,
               nucodigo,
               onuErrorCode,
               osbErrorMessage
           );
       ELSE
           nucodigo := nucodigoRq;
           
           UPDATE flex.ll_requerimiento
           SET descripcion = isbDescripcion,
               completado = inuCompletado,
               estado = isbEstado,
               fecha_actualiza = SYSDATE,
               hist_usuario = isbIdHU,
               fecha_inicio = nvl(dtFechaCreaAzure,fecha_inicio)
           WHERE codigo = nucodigo;
           
           COMMIT;
       END IF;
       
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;   
    END pInsertaTareaAzure; 
    
    PROCEDURE pActualizaTarea
    (
        inuHoja         IN NUMBER,
        inuHorasHoja    IN NUMBER,
        inuRq           IN NUMBER,
        isbDescripcion  IN VARCHAR2,
        isbIdAzure      IN NUMBER,
        isbEstado       IN VARCHAR2,
        isbUsuario      IN VARCHAR2,
        inuMon     IN NUMBER,
        inuTue     IN NUMBER,
        inuWed     IN NUMBER,
        inuThu     IN NUMBER,
        inuFri     IN NUMBER,
        inuSat     IN NUMBER,
        inuSun     IN NUMBER,
        inuCompletado IN NUMBER,
        onuSeqNeg       OUT NUMBER,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2,
        isbIdHU         IN NUMBER DEFAULT 0,
        isbFechaIni     IN VARCHAR2 DEFAULT NULL
    )
    IS
       nucodigo NUMBER(10);
       nuCodHoja NUMBER;
       nuIdAzure    NUMBER;
       dtFechaFin DATE;
       dtFechaCreaAzure DATE;
       dtFechaInCygnus  DATE;
       
       CURSOR cuHorasHojas
       IS
           SELECT codigo
           FROM flex.ll_horashoja
           WHERE codigo = inuHorasHoja
           AND   id_hoja = inuHoja;
           
       CURSOR cuDatosHoja
       IS
           SELECT fecha_fin
           FROM flex.ll_hoja
           WHERE codigo = inuHoja;
           
       CURSOR cuRq
       IS
            SELECT codigo,fecha_inicio
            FROM flex.ll_requerimiento
            WHERE id_azure = isbIdAzure
            AND usuario = isbUsuario;
               
    BEGIN
       pInicializaError(onuErrorCode,osbErrorMessage);
       onuSeqNeg := 0;
       dtFechaCreaAzure := to_date(isbFechaIni,'dd/mm/yyyy');
       
       nuCodHoja := NULL;
       
       IF(isbIdAzure = 0)THEN
            onuSeqNeg := flex.seq_ll_negativa.nextval*-1;
            nuIdAzure := onuSeqNeg;
       ELSE
            nuIdAzure := isbIdAzure;
       END IF;
       
       IF(inuRq = 0)THEN
           OPEN cuRq;
           FETCH cuRq INTO nucodigo,dtFechaInCygnus;
           CLOSE cuRq;
       ELSE
            nucodigo := inuRq;
       END IF;
        
       IF(nvl(nucodigo,0) = 0)THEN
           nucodigo := flex.seq_ll_requerimiento.nextval;
               
           OPEN cuDatosHoja;
           FETCH cuDatosHoja INTO dtFechaFin;
           CLOSE cuDatosHoja;
               
           INSERT INTO flex.ll_requerimiento
           (codigo,descripcion,id_azure,estado,usuario,fecha_actualiza,fecha_display,completado,fecha_registro,hist_usuario,fecha_inicio)       
           VALUES 
           (nucodigo,isbDescripcion,nuIdAzure,isbEstado,isbUsuario,NULL, (dtFechaFin+7),inuCompletado,SYSDATE,isbIdHU,dtFechaCreaAzure);
       ELSE
               
           UPDATE flex.ll_requerimiento
           SET id_azure = nuIdAzure,
               descripcion = isbDescripcion,
               completado = inuCompletado,
               estado = isbEstado,
               fecha_actualiza = SYSDATE,
               hist_usuario = isbIdHU,
               fecha_inicio = nvl(dtFechaCreaAzure,fecha_inicio)
           WHERE codigo = nucodigo
           AND usuario = isbUsuario;
       END IF;
       
       OPEN cuHorasHojas;
       FETCH cuHorasHojas INTO nuCodHoja;
       CLOSE cuHorasHojas;
       
       IF(nuCodHoja > 0) THEN
            UPDATE flex.ll_horashoja
            SET    lunes = inuMon,
                   martes = inuTue,
                   miercoles = inuWed,
                   jueves = inuThu,
                   viernes = inuFri,
                   sabado = inuSat,
                   domingo = inuSun,
                   fecha_actualiza = SYSDATE
             WHERE codigo = inuHorasHoja;             
       ELSE                      
           pinshorashoja
           (
               inuHoja,
               SYSDATE,
               inuMon,
               inuTue,
               inuWed,
               inuThu,
               inuFri,
               inuSat,
               inuSun,
               isbUsuario,
               NULL,
               nucodigo,
               onuErrorCode,
               osbErrorMessage
           );
       END IF; 
       
       COMMIT;  
       
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;   
    END pActualizaTarea; 
    
    PROCEDURE pinshorashoja
    (
        inuid_hoja IN NUMBER,
        idtfecha   IN DATE,
        inuMon     IN NUMBER,
        inuTue     IN NUMBER,
        inuWed     IN NUMBER,
        inuThu     IN NUMBER,
        inuFri     IN NUMBER,
        inuSat     IN NUMBER,
        inuSun     IN NUMBER,
        isbusuario IN VARCHAR2,
        isbobservaciones IN VARCHAR2,
        inurequerimiento IN NUMBER,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2  
    )       
    IS
        nucodigo NUMBER(10);
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        nucodigo := flex.seq_ll_horashoja.nextval;        
                
        INSERT INTO flex.ll_horashoja
        (codigo, id_hoja, fecha_registro, usuario, requerimiento, lunes, martes, miercoles, jueves, viernes, sabado, domingo)        
        VALUES
        (nucodigo,inuid_hoja,idtfecha,isbusuario,inurequerimiento,inuMon,inuTue,inuWed,inuThu,inuFri,inuSat,inuSun);
        
        COMMIT;
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;
    END pinshorashoja; 
    
    PROCEDURE pEliminaTarea
    (
        inuHorasHoja    IN NUMBER,
        inuRequerimiento    IN NUMBER,
        inuIdHoja    IN NUMBER,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2 
    )
    IS
       dtFechaIni   DATE;
       sbUsuario   VARCHAR2(100);
       
       CURSOR cuDatosHoja
       IS
           SELECT fecha_ini
           FROM flex.ll_hoja
           WHERE codigo = inuIdHoja;
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        
        SELECT user INTO sbUsuario
        FROM DUAL;
        
        DELETE FROM flex.ll_horashoja
        WHERE codigo =  inuHorasHoja;
        
        COMMIT;
        
        BEGIN
            DELETE FROM flex.ll_requerimiento 
            WHERE codigo =  inuRequerimiento
            AND usuario = sbUsuario;
            
            COMMIT; 
        EXCEPTION
            WHEN OTHERS THEN
                OPEN cuDatosHoja;
                FETCH cuDatosHoja INTO dtFechaIni;
                CLOSE cuDatosHoja;
            
                UPDATE flex.ll_requerimiento
                SET fecha_display = (dtFechaIni - 1),
                    fecha_actualiza = SYSDATE
                WHERE codigo = inuRequerimiento;
                
                COMMIT;
        END; 
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM; 
    END pEliminaTarea;
    
    PROCEDURE pObtTareasBD
    (
        inuHoja          IN NUMBER,
        isbUsuario       IN VARCHAR2,
        onuRefCursor    OUT tyRefCursor,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2 
    )
    IS
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        
        OPEN onuRefCursor FOR
        WITH qHojaActual AS
            (
                SELECT fecha_fin
                FROM flex.ll_hoja
                WHERE trunc(SYSDATE) BETWEEN fecha_ini AND fecha_fin
            )
            SELECT 
                   id_azure idAzure, 
                   rq.descripcion,
                   estado,
                   rq.codigo id_rq,
                   hh.codigo id,
                   hh.id_hoja,
                   hh.lunes,
                   hh.martes,
                   hh.miercoles,
                   hh.jueves,
                   hh.viernes,
                   hh.sabado,
                   hh.domingo,
                   rq.fecha_display,
                   qHojaActual.fecha_fin,
                   rq.completado,
                   nvl(rq.hist_usuario,0) hu,
                   rq.fecha_inicio,
                   (SELECT (sum(lunes) + sum(martes) + sum(miercoles) + sum(jueves) + sum(viernes) + sum(sabado) + sum(domingo)) horas
                    FROM flex.ll_horashoja hh
                    WHERE requerimiento = rq.codigo) total_rq
            FROM flex.ll_horashoja hh,flex.ll_requerimiento rq,qHojaActual
            WHERE hh.requerimiento = rq.codigo
            AND   hh.usuario = isbUsuario
            AND   hh.id_hoja = inuHoja
            ORDER BY id_azure DESC;
            
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;   
    END pObtTareasBD;  
    
    PROCEDURE pObtHojasBD
    (
        isbUsuario       IN VARCHAR2,
        onuRefCursor    OUT tyRefCursor,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2 
    )
    IS
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        
        OPEN onuRefCursor FOR
            SELECT * FROM (
            SELECT codigo,
                   fecha_ini,
                   fecha_fin,
                   descripcion,
                   NVL(
                   (SELECT sum(lunes) + sum(martes) + sum(miercoles) + sum(jueves) + sum(viernes) + sum(sabado) + sum(domingo)
                    FROM flex.ll_horashoja hh
                    WHERE hh.id_hoja = h.codigo
                    AND hh.usuario = isbUsuario )
                   ,0) horas
            FROM flex.ll_hoja h
            WHERE trunc(SYSDATE+8) BETWEEN fecha_ini AND fecha_fin
            UNION
            SELECT codigo,
                   fecha_ini,
                   fecha_fin,
                   descripcion,
                   NVL(
                   (SELECT sum(lunes) + sum(martes) + sum(miercoles) + sum(jueves) + sum(viernes) + sum(sabado) + sum(domingo)
                    FROM flex.ll_horashoja hh
                    WHERE hh.id_hoja = h.codigo
                    AND hh.usuario = isbUsuario )
                   ,0) horas
            FROM flex.ll_hoja h
            WHERE fecha_fin < SYSDATE
            OR   trunc(SYSDATE) BETWEEN fecha_ini AND fecha_fin
            ORDER BY fecha_fin DESC)
            WHERE rownum < 13;
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;   
    END pObtHojasBD;  
    
    PROCEDURE pCargarTareasPred
    (
        onuRefCursor    OUT tyRefCursor,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2 
    )
    IS
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        
        OPEN onuRefCursor FOR
            SELECT * 
            FROM flex.ll_requerimiento
            WHERE usuario = 'GENERAL';
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;   
    END pCargarTareasPred; 
    
    PROCEDURE pObtDetalleRq
    (
        inuRq           IN NUMBER,
        onuTotal        OUT NUMBER,
        onuTotalAzure   OUT NUMBER,
        onuHU           OUT NUMBER,
        onuTotalHU      OUT NUMBER,
        onuRefCursor    OUT tyrefcursor,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2 
    )
    IS
        sbUsuario   VARCHAR2(100);
        
        CURSOR cuTotalRq
        IS
            SELECT (sum(lunes) + sum(martes) + sum(miercoles) + sum(jueves) + sum(viernes) + sum(sabado) + sum(domingo)) horas
            FROM flex.ll_horashoja hh
            WHERE requerimiento = inuRq;
            
        CURSOR cuTotalAzure
        IS
            SELECT nvl(completado,0),hist_usuario
            FROM flex.ll_requerimiento
            WHERE codigo = inuRq;
            
        CURSOR cuTotalHU
        (
            inuHU   NUMBER,
            isbUsuario  VARCHAR2
        )
        IS
            SELECT sum(lunes) + sum(martes) + sum(miercoles) + sum(jueves) + sum(viernes) + sum(sabado) + sum(domingo)
            FROM flex.ll_horashoja hh
            WHERE hh.usuario = isbUsuario
            AND EXISTS (SELECT 1
                        FROM flex.ll_requerimiento rq
                        WHERE rq.hist_usuario = inuHU
                        AND rq.usuario = isbUsuario
                        AND rq.codigo = hh.requerimiento);
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        
        onuTotal := 0;
        onuTotalAzure := 0;
        onuHU := 0;
        onuTotalHU := 0;
        
        OPEN cuTotalRq;
        FETCH cuTotalRq INTO onuTotal;
        CLOSE cuTotalRq;
        
        OPEN cuTotalAzure;
        FETCH cuTotalAzure INTO onuTotalAzure,onuHU;
        CLOSE cuTotalAzure;
        
        SELECT user INTO sbUsuario
        FROM DUAL;
        
        onuHU := nvl(onuHU,0);
        
        IF(onuHU > 0 )THEN
            OPEN cuTotalHU(onuHU,sbUsuario);
            FETCH cuTotalHU INTO onuTotalHU;
            CLOSE cuTotalHU;
        END IF;
        
        OPEN onuRefCursor FOR
            SELECT fecha_ini fecha, lunes hora, rq.descripcion
            FROM flex.ll_horashoja hh,flex.ll_hoja ho, flex.ll_requerimiento rq
            WHERE requerimiento = inuRq
            AND hh.id_hoja = ho.codigo
            AND hh.requerimiento = rq.codigo
            AND lunes > 0
            UNION
            SELECT fecha_ini+1,martes, rq.descripcion
            FROM flex.ll_horashoja hh,flex.ll_hoja ho, flex.ll_requerimiento rq
            WHERE requerimiento = inuRq
            AND hh.id_hoja = ho.codigo
            AND hh.requerimiento = rq.codigo
            AND martes > 0
            UNION
            SELECT fecha_ini+2,miercoles, rq.descripcion
            FROM flex.ll_horashoja hh,flex.ll_hoja ho, flex.ll_requerimiento rq
            WHERE requerimiento = inuRq
            AND hh.id_hoja = ho.codigo
            AND hh.requerimiento = rq.codigo
            AND miercoles > 0
            UNION
            SELECT fecha_ini+3,jueves, rq.descripcion
            FROM flex.ll_horashoja hh,flex.ll_hoja ho, flex.ll_requerimiento rq
            WHERE requerimiento = inuRq
            AND hh.id_hoja = ho.codigo
            AND hh.requerimiento = rq.codigo
            AND jueves > 0
            UNION
            SELECT fecha_ini+4,viernes, rq.descripcion
            FROM flex.ll_horashoja hh,flex.ll_hoja ho, flex.ll_requerimiento rq
            WHERE requerimiento = inuRq
            AND hh.id_hoja = ho.codigo
            AND hh.requerimiento = rq.codigo
            AND viernes > 0
            UNION
            SELECT fecha_ini+5,sabado, rq.descripcion
            FROM flex.ll_horashoja hh,flex.ll_hoja ho, flex.ll_requerimiento rq
            WHERE requerimiento = inuRq
            AND hh.id_hoja = ho.codigo
            AND hh.requerimiento = rq.codigo
            AND sabado > 0
            UNION
            SELECT fecha_ini+6,domingo, rq.descripcion
            FROM flex.ll_horashoja hh,flex.ll_hoja ho, flex.ll_requerimiento rq
            WHERE requerimiento = inuRq
            AND hh.id_hoja = ho.codigo
            AND hh.requerimiento = rq.codigo
            AND domingo > 0;
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;   
    END pObtDetalleRq;

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
    
    PROCEDURE pEliminarAreaAzure
    (        
        isbArea          IN  VARCHAR2,
        isbUsuario       IN  VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2
    )
    IS
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        
        DELETE flex.ll_azure
        WHERE usuario = isbUsuario
        AND area = isbArea;
                
        COMMIT;
        
    EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN 
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM; 
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;     
    END pEliminarAreaAzure; 
    
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
    
    PROCEDURE pCombinarTareas
    (
        inuOrigen       IN NUMBER,
        inuDestino      IN NUMBER,
        isbUsuario      IN VARCHAR2,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2 
    )
    IS
        rcOrigen     flex.ll_horashoja%ROWTYPE;
        rcDestino    flex.ll_horashoja%ROWTYPE;    
    
        CURSOR cuOrigen
        IS
            SELECT hh.*
            FROM flex.ll_horashoja hh,flex.ll_requerimiento rq
            WHERE hh.usuario = isbUsuario
            AND rq.id_azure = inuOrigen
            AND hh.requerimiento = rq.codigo;
            
        CURSOR cuDestino
        (
            inuHoja NUMBER
        )
        IS
            SELECT hh.*
            FROM flex.ll_horashoja hh,flex.ll_requerimiento rq
            WHERE hh.usuario = isbUsuario
            AND rq.id_azure = inuDestino
            AND id_hoja = inuHoja
            AND hh.requerimiento = rq.codigo;
            
        TYPE tytbEliminar IS TABLE OF cuDestino%ROWTYPE INDEX BY BINARY_INTEGER;
        tbEliminar tytbEliminar;
    BEGIN
        pInicializaError(onuErrorCode,osbErrorMessage);
        
        tbEliminar.delete;
        
        OPEN cuOrigen;
        LOOP
            FETCH cuOrigen INTO rcOrigen;
            EXIT WHEN cuOrigen%NOTFOUND;
            
            OPEN cuDestino(rcOrigen.id_hoja);
            FETCH cuDestino INTO rcDestino;
            CLOSE cuDestino;
            
            IF(rcDestino.codigo IS NOT NULL)THEN
                 UPDATE flex.ll_horashoja
                 SET    lunes = rcDestino.lunes + rcOrigen.lunes,
                        martes = rcDestino.martes + rcOrigen.martes,
                        miercoles = rcDestino.miercoles + rcOrigen.miercoles,
                        jueves = rcDestino.jueves + rcOrigen.jueves,
                        viernes = rcDestino.viernes + rcOrigen.viernes,
                        sabado = rcDestino.sabado + rcOrigen.sabado,
                        domingo = rcDestino.domingo + rcOrigen.domingo,
                        fecha_actualiza = SYSDATE,
                        observacion = 'Hoja combinada [origen:'||inuOrigen||' - destino: '||inuDestino||']'                          
                  WHERE codigo = rcDestino.codigo;             
            ELSE                      
                pinshorashoja
                (
                    rcOrigen.id_hoja,
                    SYSDATE,
                    rcOrigen.lunes,
                    rcOrigen.martes,
                    rcOrigen.miercoles,
                    rcOrigen.jueves,
                    rcOrigen.viernes,
                    rcOrigen.sabado,
                    rcOrigen.domingo,
                    isbUsuario,
                    'Hoja combinada [origen:'||inuOrigen||' - destino: '||inuDestino||']',
                    rcOrigen.requerimiento,
                    onuErrorCode,
                    osbErrorMessage
                );
            END IF; 
            
            --Se elimina el origen
            DELETE FROM flex.ll_horashoja
            WHERE codigo = rcOrigen.codigo;
            
            tbEliminar(tbEliminar.count) := rcOrigen;
            
            COMMIT;
            
        END LOOP;
        CLOSE cuOrigen;
        
        IF(tbEliminar.count > 0)THEN
            FOR i IN tbEliminar.first .. tbEliminar.last LOOP
                --Se elimina el origen
                DELETE FROM flex.ll_requerimiento
                WHERE codigo = tbEliminar(i).requerimiento;
                
                COMMIT;
            END LOOP;
        END IF;
    
    EXCEPTION
        WHEN OTHERS THEN
            onuErrorCode := SQLCODE;
            osbErrorMessage := SQLERRM;
    END pCombinarTareas;
    
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
        
        gsbTableCap     varchar2( 100 );  
    
        CURSOR cuPrimaria IS
            select  --+ ordered
                    tcol.table_name, tcol.column_name, tcol.data_type, tcol.data_length, tcol.data_precision, tcol.data_scale,
                    'i' || decode( tcol.data_type, 'NUMBER', 'nu', 'VARCHAR2', 'sb', 'DATE', 'dt', 'BOOLEAN', 'bl' ) || substr( tcol.column_name, 1, 1 ) || lower( substr( tcol.column_name, 2 ) ) alias,
                    'i' || decode( tcol.data_type, 'NUMBER', 'nu', 'VARCHAR2', 'sb', 'DATE', 'dt', 'BOOLEAN', 'bl' ) || substr( tcol.column_name, 1, 1 ) || lower( substr( tcol.column_name, 2 ) ) || ' ' || 'in' || ' ' || lower( tcol.table_name || '.' || tcol.column_name || '%type' ) param_entrada
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
                    tcol.table_name, tcol.column_name, lower( tcol.column_name ) alias, data_type
            from    all_tab_columns tcol
            where   tcol.table_name = upper( csbTABLA )
            and     tcol.owner = upper( csbOWNER );
                    
        TYPE tytbPrimaria IS TABLE OF cuPrimaria%rowtype INDEX BY binary_integer;
        TYPE tytbCampos IS TABLE OF cuCampos%rowtype INDEX BY binary_integer;
        
        tbPrimaria      tytbPrimaria;
        tbCampos        tytbCampos;
        sbExiste        varchar2( 1 );    
        sbCadena        varchar2( 32767 );
        sbUser          varchar2( 20 );
        sbTipoDato      varchar2( 20 );    
        
        FUNCTION fsbTipoDato ( 
            isbTipoDato IN  VARCHAR2
        )
        RETURN VARCHAR2
        IS
            sbToken VARCHAR2(20) := NULL;
        BEGIN
            CASE UPPER(isbTipoDato) 
                WHEN 'NUMBER' THEN
                    sbToken := 'inu';
                WHEN 'INTEGER' THEN
                    sbToken := 'inu';
                WHEN 'VARCHAR2' THEN
                    sbToken := 'isb';
                WHEN 'CHAR' THEN
                    sbToken := 'isb';
                WHEN 'NCHAR' THEN
                    sbToken := 'isb';
                WHEN 'DATE' THEN
                    sbToken := 'idt';
                WHEN 'CLOB' THEN
                    sbToken := 'icl';
                WHEN 'BLOB' THEN
                    sbToken := 'ibl';
            END CASE;
            RETURN sbToken;   
        END;
        
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
        
        DBMS_LOB.APPEND(oclFile, 'CREATE OR REPLACE PACKAGE pktbl' || gsbTableCap || '
            IS
            /**************************************************************************
            Copyright (c) 2018 EPM - Empresas Públicas de Medellín
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
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '
        -- Define colecciones de cada columna de la tabla ' || gsbTableCap);
        
            for i in tbCampos.first .. tbCampos.last loop            
                if ( i <> tbCampos.last ) then 
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| 
        '   TYPE ty'||tbCampos( i ).column_name||' IS TABLE OF '||gsbTableCap||'.'||tbCampos( i ).column_name||'%TYPE INDEX BY BINARY_INTEGER;');            
                else
                DBMS_LOB.APPEND(oclFile, CHR(10)||
        '   TYPE ty'||tbCampos( i ).column_name||' IS TABLE OF '||gsbTableCap||'.'||tbCampos( i ).column_name||'%TYPE INDEX BY BINARY_INTEGER;');        
                end if;    
            end loop;    
        
            DBMS_LOB.APPEND(oclFile, CHR(10)||'
        -- Define registro de colecciones' || sbCadena);
            
            DBMS_LOB.APPEND(oclFile, CHR(10)|| 
        '   TYPE tytb'||gsbTableCap||' IS RECORD
        (' || sbCadena);
        
            for i in tbCampos.first .. tbCampos.last loop            
                if ( i <> tbCampos.last ) then
                    sbCadena := sbCadena ||                 
        '       '||tbCampos( i ).column_name||'  ty'||tbCampos( i ).column_name||','||CHR(10);
                else
                    sbCadena := sbCadena ||
        '       '||tbCampos( i ).column_name||'  ty'||tbCampos( i ).column_name;            
                end if;                
            end loop;        
            DBMS_LOB.APPEND(oclFile, CHR(10)|| sbCadena);        
            DBMS_LOB.APPEND(oclFile, CHR(10)|| 
        '    );');    
        end if;
    
        --------------------------------------------
        -- Constants
        --------------------------------------------
    
        --------------------------------------------
        -- Variables
        --------------------------------------------');
        
        
        if( tbPrimaria.first is not null ) then
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '
        -- Cursor para accesar ' || gsbTableCap || '
        ' || 'CURSOR cu' || gsbTableCap || ' 
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
            select  *
            from    ' || lower( csbTABLA ));
            
            for i in tbPrimaria.first .. tbPrimaria.last loop
                if ( i = tbPrimaria.first ) then
                    sbCadena := 'where   ';
                else
                    sbCadena := 'and     ';    
                end if;
            
                sbCadena := sbCadena || lower( tbPrimaria( i ).column_name ) || ' = ' || tbPrimaria( i ).alias;
                if ( i = tbPrimaria.last ) then
                    sbCadena := sbCadena || ';';
                end if;
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || sbCadena);
            end loop;
            
        end if;
        
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '
        --------------------------------------------
        -- Funciones y Procedimientos
        --------------------------------------------
    
        PROCEDURE InsRecord
        (
            ircRecord in ' || lower( csbTABLA ) || '%rowtype
        );');
        
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '
        PROCEDURE InsRecords
        (
            irctbRecord  IN OUT NOCOPY   tytb'||gsbTableCap||'
        );');
            
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '
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
        PROCEDURE InsForEachColumnBulk
        (');
        
        for i in tbCampos.first .. tbCampos.last loop     
            sbTipoDato := fsbTipoDato( tbCampos( i ).data_type );       
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
        PROCEDURE ClearMemory;
        
        PROCEDURE DelRecord
        (');
        
            for i in tbPrimaria.first .. tbPrimaria.last loop
                sbCadena := tbPrimaria( i ).param_entrada;
                if ( i <> tbPrimaria.last ) then
                    sbCadena := sbCadena || ',';
                end if;
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || sbCadena);
            end loop;
    
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '    );
        
        PROCEDURE UpRecord
        (
            ircRecord in ' || lower( csbTABLA ) || '%rowtype
        );
        
        PROCEDURE DelRecords
        (');
            for i in tbPrimaria.first .. tbPrimaria.last loop
                sbTipoDato := fsbTipoDato( tbPrimaria( i ).data_type );
                if ( i <> tbPrimaria.last ) then
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || sbTipoDato||tbPrimaria( i ).column_name || ' IN OUT NOCOPY ty'||tbPrimaria( i ).column_name||',');
                else
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || sbTipoDato||tbPrimaria( i ).column_name || ' IN OUT NOCOPY ty'||tbPrimaria( i ).column_name);
                end if;
            end loop;
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '     
        );
                
        FUNCTION fblExist
        (');
        
            for i in tbPrimaria.first .. tbPrimaria.last loop
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || tbPrimaria( i ).param_entrada || ',');
            end loop;
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        inuCACHE in number default 1
        )
        RETURN boolean;
    
        FUNCTION frcGetRecord
        (');
        
            for i in tbPrimaria.first .. tbPrimaria.last loop
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || tbPrimaria( i ).param_entrada || ',');
            end loop;
            
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        inuCACHE in number default 1
        )
        RETURN ' || lower( csbTABLA ) || '%rowtype;
    
        PROCEDURE AccKey
        (');
        
            for i in tbPrimaria.first .. tbPrimaria.last loop
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || tbPrimaria( i ).param_entrada || ',');
            end loop;
            
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        inuCACHE in number default 1
        );
    
        PROCEDURE ValidateDupValues
        (');
        
            for i in tbPrimaria.first .. tbPrimaria.last loop
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || tbPrimaria( i ).param_entrada || ',');
            end loop;
            
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        inuCACHE in number default 1
        );');
        
        end if;
    
        DBMS_LOB.APPEND(oclFile, CHR(10)|| 'END pktbl' || gsbTableCap || ';
    /
        CREATE OR REPLACE PACKAGE BODY pktbl' || gsbTableCap || '    
        IS    
            -------------------------
            --  PRIVATE VARIABLES
            -------------------------');
        
        if( tbPrimaria.first is not null ) then
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '    -- Record Tabla ' || upper( csbTABLA ) || '
        rc' || gsbTableCap || ' cu' || gsbTableCap || '%rowtype;
        -- Record nulo de la Tabla ' || upper( csbTABLA ) || '
        rcRecordNull ' || lower( csbTABLA ) || '%rowtype;
        
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
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '    );
        
        PROCEDURE LoadRecord
        (');
        
            for i in tbPrimaria.first .. tbPrimaria.last loop
                sbCadena := tbPrimaria( i ).param_entrada;
                if ( i <> tbPrimaria.last ) then
                    sbCadena := sbCadena || ',';
                end if;
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || sbCadena);
            end loop;
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '    );
        
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
        RETURN boolean;');
        
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
        csbMODULE                  CONSTANT VARCHAR2(20) := pkConstante.csbMOD_CUZ;');
        
        if( tbPrimaria.first is not null ) then
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '    
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
            if ( rc' || gsbTableCap || '.' || lower( tbPrimaria( tbPrimaria.first ).column_name ) || ' is null ) then
                pkErrors.Pop;
                raise NO_DATA_FOUND;
            end if;
            pkErrors.Pop;
        EXCEPTION
            when NO_DATA_FOUND then
                pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_NO_EXISTE );
                pkErrors.ADDSUFFIXTOMESSAGE ('||CHR(39)||'(Tabla '||upper( csbTABLA )||')'||CHR(39)||');
                pkErrors.Pop;
                raise LOGIN_DENIED;
        END Load;
        
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
            if ( cu' || gsbTableCap || '%isopen ) then
                close cu' || gsbTableCap || ';
            end if;
            -- Accesa ' || upper( csbTABLA ) || ' de la BD
            open cu' || gsbTableCap || '
            (');
            for i in tbPrimaria.first .. tbPrimaria.last loop
                sbCadena := tbPrimaria( i ).alias;
                if ( i <> tbPrimaria.last ) then
                    sbCadena := sbCadena || ',';
                end if;
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '            ' || sbCadena);
            end loop;
            
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        );
            fetch cu' || gsbTableCap || ' into rc' || gsbTableCap || ';
            if ( cu' || gsbTableCap || '%notfound ) then
                close cu' || gsbTableCap || ';
                pkErrors.Pop;
                rc' || gsbTableCap || ' := rcRecordNull;
                RETURN;
            end if;
            close cu' || gsbTableCap || ';
            pkErrors.Pop;
    
        END LoadRecord;    
        
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
        RETURN boolean
        IS
        BEGIN
            pkErrors.Push( ''pktbl' || gsbTableCap || '.fblInMemory'' );
            ');
            
            sbCadena := 'if ( ';
            
            for i in tbPrimaria.first .. tbPrimaria.last loop
                sbCadena := sbCadena || 'rc' || gsbTableCap || '.' || lower( tbPrimaria( i ).column_name ) || ' = ' || tbPrimaria( i ).alias;
                if ( i <> tbPrimaria.last ) then
                    sbCadena := sbCadena || ' and ';
                end if;
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '            ' || sbCadena);
                sbCadena := null;
            end loop;
            sbCadena := sbCadena || ' ) then';
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || sbCadena || '
                pkErrors.Pop;
                RETURN( true );
            end if;
            pkErrors.Pop;
            RETURN( false );
    
        END fblInMemory;
    
        PROCEDURE AccKey
        (');
        
            for i in tbPrimaria.first .. tbPrimaria.last loop
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || tbPrimaria( i ).param_entrada || ',');
            end loop;
            
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        inuCACHE in number default 1
        )
        IS
        BEGIN
            pkErrors.Push( ''pktbl' || gsbTableCap || '.AccKey'' );
            
            --Valida si debe buscar primero en memoria Cache
            if ( inuCACHE = CACHE ) then
                if ( fblInMemory(');
            
            for i in tbPrimaria.first .. tbPrimaria.last loop
                sbCadena := tbPrimaria( i ).alias;
                if ( i <> tbPrimaria.last ) then
                    sbCadena := sbCadena || ',';
                end if;
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '                ' || sbCadena);
            end loop;
            
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '            ) ) then
                    pkErrors.Pop;
                    RETURN;
                end if;
            end if;
            Load
            (');
            
            for i in tbPrimaria.first .. tbPrimaria.last loop
                sbCadena := tbPrimaria( i ).alias;
                if ( i <> tbPrimaria.last ) then
                    sbCadena := sbCadena || ',';
                end if;
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '            ' || sbCadena);
            end loop;
            
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        );
            pkErrors.Pop;
        EXCEPTION
            when LOGIN_DENIED then
                pkErrors.Pop;
                raise LOGIN_DENIED;
        END AccKey;
    
        PROCEDURE ClearMemory 
        IS
        BEGIN
            pkErrors.Push( ''pktbl' || gsbTableCap || '.ClearMemory'' );
            rc' || gsbTableCap || ' := rcRecordNull;
            pkErrors.Pop;
        END ClearMemory;    
        
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
            delete  ' || lower( csbTABLA ));
            
            for i in tbPrimaria.first .. tbPrimaria.last loop
                if ( i = tbPrimaria.first ) then
                    sbCadena := 'where   ';
                else
                    sbCadena := 'and     ';    
                end if;
            
                sbCadena := sbCadena || lower( tbPrimaria( i ).column_name ) || ' = ' || tbPrimaria( i ).alias;
                if ( i = tbPrimaria.last ) then
                    sbCadena := sbCadena || ';';
                end if;
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || sbCadena);
            end loop;
            
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '
            if ( sql%notfound ) then
                pkErrors.Pop;
                raise NO_DATA_FOUND;
            end if;
            pkErrors.Pop;
            
            EXCEPTION
                when NO_DATA_FOUND then
                    pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_NO_EXISTE );
                    pkErrors.ADDSUFFIXTOMESSAGE ('||CHR(39)||'(Tabla '||upper( csbTABLA )||')'||CHR(39)||');
                    pkErrors.Pop;
                    RAISE login_denied;
        END DelRecord;');
        
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '    
        PROCEDURE DelRecords
        (');
            for i in tbPrimaria.first .. tbPrimaria.last loop
                sbTipoDato := fsbTipoDato( tbPrimaria( i ).data_type );
                if ( i <> tbPrimaria.last ) then
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || sbTipoDato||tbPrimaria( i ).column_name || ' IN OUT NOCOPY ty'||tbPrimaria( i ).column_name||',');
                else
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || sbTipoDato||tbPrimaria( i ).column_name || ' IN OUT NOCOPY ty'||tbPrimaria( i ).column_name);
                end if;
            end loop;
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '
        )
        IS
        BEGIN
            pkErrors.Push( ''pktbl' || gsbTableCap || '.DelRecords'' );
            
            -- Elimina registros de la Tabla '||lower( csbTABLA ));
            
            for i in tbPrimaria.first .. tbPrimaria.last loop
                sbTipoDato := fsbTipoDato( tbPrimaria( i ).data_type );
                if ( i = tbPrimaria.first ) then
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| 
            '        FORALL indx in '||sbTipoDato||tbPrimaria( i ).column_name||'.FIRST .. '||sbTipoDato||tbPrimaria( i ).column_name||'.LAST');
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| 
            '         DELETE '||tbPrimaria( i ).table_name||' WHERE '||tbPrimaria( i ).column_name||' = '||sbTipoDato||tbPrimaria( i ).column_name||'(indx)');
                elsif ( i <> tbPrimaria.last ) then
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| 
            '         AND '||tbPrimaria( i ).column_name||' = '||sbTipoDato||tbPrimaria( i ).column_name||'(indx)');
                else
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| 
            '         AND '||tbPrimaria( i ).column_name||' = '||sbTipoDato||tbPrimaria( i ).column_name||'(indx)');
                end if;            
            end loop;
            DBMS_LOB.APPEND(oclFile, CHR(10)|| 
            '        ;');
            
                DBMS_LOB.APPEND(oclFile, CHR(10)|| 
            '
            IF ( SQL%NOTFOUND ) THEN
                pkErrors.Pop;
                RAISE NO_DATA_FOUND;
            END IF;
            
            pkErrors.Pop;
        EXCEPTION
            when NO_DATA_FOUND then
                pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_NO_EXISTE );   
                pkErrors.ADDSUFFIXTOMESSAGE ('||CHR(39)||'(Tabla '||upper( csbTABLA )||')'||CHR(39)||');
                pkErrors.Pop;         
                raise LOGIN_DENIED;
        END DelRecords;');
        
        end if;
        
        
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '    
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
          rcRecord ' || lower( csbTABLA ) || '%ROWTYPE;   /* Record de la Tabla '||lower( csbTABLA )||' */
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
        PROCEDURE InsForEachColumnBulk
        (');
        
        for i in tbCampos.first .. tbCampos.last loop     
            sbTipoDato := fsbTipoDato( tbCampos( i ).data_type );       
            if ( i <> tbCampos.last ) then                              
                DBMS_LOB.APPEND(oclFile, CHR(10)||          
        '       '||sbTipoDato||tbCampos( i ).column_name||' IN OUT NOCOPY ty'||tbCampos( i ).column_name||',');               
            else
                DBMS_LOB.APPEND(oclFile, CHR(10)||          
        '       '||sbTipoDato||tbCampos( i ).column_name||' IN OUT NOCOPY ty'||tbCampos( i ).column_name);
            end if;    
        end loop;
        
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '
        )
        IS      
        BEGIN
           pkErrors.Push('' pktbl'||lower( csbTABLA )||'.InsForEachColumnBulk '');');
           
           for i in tbCampos.first .. tbCampos.last loop
                sbTipoDato := fsbTipoDato( tbCampos( i ).data_type );
                if ( i = tbCampos.first ) then
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| 
            '        FORALL indx in '||sbTipoDato||tbCampos( i ).column_name||'.FIRST .. '||sbTipoDato||tbCampos( i ).column_name||'.LAST');
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| 
            '         INSERT INTO '||lower( csbTABLA )||' (');
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| 
            '           '||tbCampos( i ).column_name||',');
                elsif ( i <> tbCampos.last ) then
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| 
            '           '||tbCampos( i ).column_name||',');
                else
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| 
            '           '||tbCampos( i ).column_name||' ) VALUES (
            ');
                end if;            
            end loop;
            
            for i in tbCampos.first .. tbCampos.last loop
                sbTipoDato := fsbTipoDato( tbCampos( i ).data_type );
                if ( i <> tbCampos.last ) then
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| 
            '           '||sbTipoDato||tbCampos( i ).column_name||'(indx),');
                else
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| 
            '           '||sbTipoDato||tbCampos( i ).column_name||'(indx) )
            ');
                end if;            
            end loop;
            
            DBMS_LOB.APPEND(oclFile, CHR(10)|| 
            '        ;');
           
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '
           pkErrors.Pop;
        EXCEPTION
            WHEN DUP_VAL_ON_INDEX THEN
                pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_YA_EXISTE );
                 pkErrors.ADDSUFFIXTOMESSAGE (''(Tabla '||csbTABLA||')'');
                 pkErrors.Pop;
                 RAISE LOGIN_DENIED;
        END InsForEachColumnBulk;');
        
        
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '    
        PROCEDURE InsRecord
        (
            ircRecord in ' || lower( csbTABLA ) || '%rowtype
        )
        IS
        BEGIN
            pkErrors.Push( ''pktbl' || gsbTableCap || '.InsRecord'' );
            
            insert into ' || lower( csbTABLA ) || '
            (');
            
        for i in tbCampos.first .. tbCampos.last loop
            
            sbCadena := tbCampos( i ).alias;
            
            if ( i <> tbCampos.last ) then
                sbCadena := sbCadena || ',';
            end if;
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '            ' || sbCadena);
        end loop;
            
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ) 
            values 
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
            when DUP_VAL_ON_INDEX then
                pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_YA_EXISTE );
                pkErrors.ADDSUFFIXTOMESSAGE ('||CHR(39)||'(Tabla '||upper( csbTABLA )||')'||CHR(39)||');
                pkErrors.Pop;
                raise LOGIN_DENIED;
        END InsRecord;');
        
        
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '    
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
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| 
            '        FORALL indx in irctbRecord.'||tbCampos( i ).column_name||'.FIRST .. irctbRecord.'||tbCampos( i ).column_name||'.LAST');
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| 
            '         INSERT INTO '||lower( csbTABLA )||' (');
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| 
            '           '||tbCampos( i ).column_name||',');
                elsif ( i <> tbCampos.last ) then
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| 
            '           '||tbCampos( i ).column_name||',');
                else
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| 
            '           '||tbCampos( i ).column_name||' ) VALUES (
            ');
                end if;            
            end loop;
            
            for i in tbCampos.first .. tbCampos.last loop
                sbTipoDato := fsbTipoDato( tbCampos( i ).data_type );
                if ( i <> tbCampos.last ) then
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| 
            '           irctbRecord.'||tbCampos( i ).column_name||'(indx),');
                else
                    DBMS_LOB.APPEND(oclFile, CHR(10)|| 
            '           irctbRecord.'||tbCampos( i ).column_name||'(indx) )
            ');
                end if;            
            end loop;
            
            DBMS_LOB.APPEND(oclFile, CHR(10)|| 
            '        ;');
           
        DBMS_LOB.APPEND(oclFile, CHR(10)|| '
           pkErrors.Pop;
        EXCEPTION
            WHEN DUP_VAL_ON_INDEX THEN
                pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_YA_EXISTE );
                 pkErrors.ADDSUFFIXTOMESSAGE (''(Tabla '||csbTABLA||')'');
                 pkErrors.Pop;
                 RAISE LOGIN_DENIED;
        END InsRecords;');
        
        
        if( tbPrimaria.first is not null ) then
        
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '    
        PROCEDURE UpRecord
        (
            ircRecord in ' || lower( csbTABLA ) || '%rowtype
        )
        IS
        BEGIN
            pkErrors.Push( ''pktbl' || gsbTableCap || '.UpRecord'' );
            
            update  ' || lower( csbTABLA ));
            
            for i in tbCampos.first .. tbCampos.last loop
            
                if ( i = tbCampos.first ) then
                    sbCadena := '        set     ' || tbCampos( i ).alias || ' = ircRecord.' || tbCampos( i ).alias;
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
                    sbCadena := 'where   ';
                else
                    sbCadena := 'and     ';    
                end if;
            
                sbCadena := sbCadena || lower( tbPrimaria( i ).column_name ) || ' = ircRecord.' || lower( tbPrimaria( i ).column_name );
                if ( i = tbPrimaria.last ) then
                    sbCadena := sbCadena || ';';
                end if;
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || sbCadena);
            end loop;
            
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '
            if ( sql%notfound ) then
                pkErrors.Pop;
                raise NO_DATA_FOUND;
            end if;
            
            pkErrors.Pop;
    
        EXCEPTION
            WHEN DUP_VAL_ON_INDEX THEN
                pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_YA_EXISTE );
                pkErrors.ADDSUFFIXTOMESSAGE ('||CHR(39)||'(Tabla '||upper( csbTABLA )||')'||CHR(39)||');
                pkErrors.Pop;
                RAISE LOGIN_DENIED;
            when NO_DATA_FOUND then
                pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_NO_EXISTE );
                pkErrors.ADDSUFFIXTOMESSAGE ('||CHR(39)||'(Tabla '||upper( csbTABLA )||')'||CHR(39)||');
                pkErrors.Pop;
                raise LOGIN_DENIED;
        END UpRecord;
    
        PROCEDURE ValidateDupValues
        (');
        
            for i in tbPrimaria.first .. tbPrimaria.last loop
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || tbPrimaria( i ).param_entrada || ',');
            end loop;
            
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        inuCACHE in number default 1
        )
        IS
        BEGIN
            pkErrors.Push( ''pktbl' || gsbTableCap || '.ValidateDupValues'' );
            
            --Valida si el registro ya existe
            if ( fblExist(');
            
            for i in tbPrimaria.first .. tbPrimaria.last loop
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '                ' || tbPrimaria( i ).alias || ',');
            end loop;
            
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '                inuCACHE
            ) ) then
                pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_YA_EXISTE );
                pkErrors.ADDSUFFIXTOMESSAGE ('||CHR(39)||'(Tabla '||upper( csbTABLA )||')'||CHR(39)||');
                raise LOGIN_DENIED;
            end if;
            
            pkErrors.Pop;
         
        EXCEPTION
            when LOGIN_DENIED then
                pkErrors.Pop;
                raise LOGIN_DENIED;
        END ValidateDupValues;
    
        FUNCTION fblExist
        (');
        
            for i in tbPrimaria.first .. tbPrimaria.last loop
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || tbPrimaria( i ).param_entrada || ',');
            end loop;
            
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        inuCACHE in number default 1
        )
        RETURN boolean
        IS
        BEGIN
            pkErrors.Push( ''pktbl' || gsbTableCap || '.fblExist'' );
            
            --Valida si debe buscar primero en memoria Caché
            if ( inuCACHE = CACHE ) then
                if ( fblInMemory(');
            
            for i in tbPrimaria.first .. tbPrimaria.last loop
                sbCadena := tbPrimaria( i ).alias;
                if ( i <> tbPrimaria.last ) then
                    sbCadena := sbCadena || ',';
                end if;
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '                ' || sbCadena);
            end loop;
            
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '            ) ) then
                    pkErrors.Pop;
                    RETURN( true );
                end if;
            end if;
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
            if ( rc' || gsbTableCap || '.' || lower( tbPrimaria( tbPrimaria.first ).column_name ) || ' is null ) then
                pkErrors.Pop;
                RETURN( false );
            end if;
            
            pkErrors.Pop;
    
            RETURN( true );
        
        END fblExist;
    
        FUNCTION frcGetRecord
        (');
        
            for i in tbPrimaria.first .. tbPrimaria.last loop
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '        ' || tbPrimaria( i ).param_entrada || ',');
            end loop;
            
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '        inuCACHE in number default 1
        )
        RETURN ' || lower( csbTABLA ) || '%rowtype
        IS
        BEGIN
            pkErrors.Push( ''pktbl' || gsbTableCap || '.frcGetRecord'' );
            
            --Valida si el registro ya existe
            AccKey
            (');
            
            for i in tbPrimaria.first .. tbPrimaria.last loop
                DBMS_LOB.APPEND(oclFile, CHR(10)|| '                ' || tbPrimaria( i ).alias || ',');
            end loop;
            
            DBMS_LOB.APPEND(oclFile, CHR(10)|| '                inuCACHE
            );
    
            pkErrors.Pop;
            RETURN ( rc' || gsbTableCap || ' );
         
        EXCEPTION
            when LOGIN_DENIED then
                pkErrors.Pop;
                raise LOGIN_DENIED;
        END frcGetRecord;');
        
        end if;
        
        DBMS_LOB.APPEND(oclFile, CHR(10)|| 'END pktbl' || gsbTableCap || ';'||CHR(10)||
    '/');
    
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
