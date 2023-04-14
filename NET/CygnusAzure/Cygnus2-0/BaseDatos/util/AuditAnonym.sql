declare
    PROCEDURE p_DC_CreaTriggerAudit
    (
        isbTableName    IN  VARCHAR2,
        isbAutor        IN  VARCHAR2,
        isbLogin        IN  VARCHAR2,
        isbTicket       IN  VARCHAR2,
        isbPK           IN  VARCHAR2,
        osbScript       OUT CLOB
    )
    IS
    /*******************************************************************************
        <Procedure Fuente="Propiedad Intelectual de Empresas P�blicas de Medell�n">
        <Unidad>p_DC_CreaTriggerAudit</Unidad>
        <Descripcion>
            Crea el fuente para un trigger de auditor�a
        </Descripcion>
        <Autor> Diego Fernando Coba - MVM Ingenier�a de Software </Autor>
        <Fecha> 14-Ene-2021 </Fecha>
            <param nombre="isbTableName" tipo="TYPE" Direccion="In" >
                Tabla a la que se le va a crear la auditor�a
            </param>
            <param nombre="isbAutor" tipo="TYPE" Direccion="In" >
                Autor
            </param>
            <param nombre="isbLogin" tipo="TYPE" Direccion="In" >
                Usuario
            </param>
            <param nombre="isbTicket" tipo="TYPE" Direccion="In" >
                WO o HU
            </param>
            <param nombre="isbPK" tipo="TYPE" Direccion="In" >
                Llame primaria
            </param>
            <param nombre="osbScript" tipo="TYPE" Direccion="Out" >
                Contenido del Script
            </param>
        <Historial>
            <Modificacion Autor="dcoba" Fecha="14-Ene-2021" Inc="NNNNNN">
                Creaci�n del m�todo.
            </Modificacion>
        </Historial>
        </Procedure>
    *******************************************************************************/

        csbSP_NAME                  CONSTANT VARCHAR2(32)                   := $$PLSQL_UNIT||'.';
        csbPUSH                     CONSTANT VARCHAR2( 4)                   := pkg_EPM_Constante.fsbPUSH     ;
        csbPOP                      CONSTANT VARCHAR2( 4)                   := pkg_EPM_Constante.fsbPOP      ;
        csbPOP_ERC                  CONSTANT VARCHAR2( 4)                   := pkg_EPM_Constante.fsbPOP_ERC  ;
        csbPOP_ERR                  CONSTANT VARCHAR2( 4)                   := pkg_EPM_Constante.fsbPOP_ERR  ;
        cnuNVLTRC                   CONSTANT NUMBER  ( 2)                   := pkg_EPM_Constante.fnuNVL_BO;

        -- Nombre del m�todo
        sbNameMethod        VARCHAR2(30) := 'p_DC_CreaTriggerAudit';

        CURSOR  cuTableDesc
        (
            isbTabla    IN  VARCHAR2
        )
        IS
            SELECT  comments
            FROM    dba_tab_comments
            WHERE   TABLE_NAME = UPPER(isbTabla);

        sbTableDesc dba_tab_comments.comments%TYPE;

        CURSOR cuCampos
        (
            isbTabla    IN  VARCHAR2
        )
        IS
            SELECT  COLUMN_NAME,
                    DECODE(DATA_TYPE, 'VARCHAR2', '''-''', 'NUMBER', -1, 'DATE', 'TO_DATE('''||TO_CHAR(UT_Date.fdtMinDate, 'YYYY/MM/DD HH24:MI:SS')||''', ''YYYY/MM/DD HH24:MI:SS'')') DEFAULT_VALUE
            FROM
            (
                SELECT  tc.COLUMN_NAME COLUMN_NAME,
                        tc.DATA_TYPE,
                        tc.DATA_PRECISION,
                        tc.DATA_SCALE,
                        cc.COMMENTS COMMENTS,
                        tc.COLUMN_ID
                FROM    dba_tab_columns tc, dba_col_comments cc
                WHERE   tc.TABLE_NAME = UPPER(isbTabla)
                AND     tc.TABLE_NAME = cc.TABLE_NAME
                AND     tc.COLUMN_NAME = cc.COLUMN_NAME
                AND     tc.COLUMN_NAME <> UPPER(isbPK)
            )
            ORDER BY COLUMN_ID, COLUMN_NAME DESC;

        TYPE tytbCampos IS TABLE OF cuCampos%ROWTYPE INDEX BY BINARY_INTEGER;
        tbCampos tytbCampos;
        nuIdxCampos BINARY_INTEGER;

        sbFieldCondTemplate VARCHAR2(500)   := '    ( NVL( new.[{FIELD}],[{DEFAULT_VAL}] ) <> NVL( old.[{FIELD}],[{DEFAULT_VAL}]) )';
        sbFieldCond         VARCHAR2(32000);
        sbFielConditions    VARCHAR2(32000);
        csbENTER            VARCHAR2(2)     := CHR(13);
        csbSLASH            VARCHAR2(2)     := CHR(47);
        sbFieldAuditTemplate    VARCHAR2(500) :=
    '
        IF (NVL( :new.[{FIELD}], [{DEFAULT_VAL}]) <> NVL( :old.[{FIELD}], [{DEFAULT_VAL}])) THEN
            rcAudit.O_[{FIELD}]  := :old.[{FIELD}];
            rcAudit.N_[{FIELD}]  := :new.[{FIELD}];
        END IF;

    ';
        sbFieldAudit        VARCHAR2(32000);
        sbFieldsAudits      VARCHAR2(32000);

        sbScript            CLOB :=
    'CREATE OR REPLACE TRIGGER TRG_AUD_[{TABLE}]
    AFTER UPDATE OR DELETE ON [{TABLE}]
    REFERENCING OLD AS OLD NEW AS NEW
    FOR EACH ROW
    WHEN
    (
    [{CONDITIONS}]
    )
    DECLARE

    '||csbSLASH||'****************************************************************************
        <Procedure Fuente="Propiedad Intelectual de Empresas Publicas de Medell�n">
        <Unidad> TRG_AUD_[{TABLE}] <'||csbSLASH||'Unidad>
        <Descripcion>
        Registra auditor�a de la tabla [{TABLE_DESC}]
        <'||csbSLASH||'Descripcion>
        <Autor> [{AUTOR}] - MVM <'||csbSLASH||'Autor>
        <Fecha>[{DATE}]<'||csbSLASH||'Fecha>
        <Historial>
        <Modificacion Autor="[{LOGIN}]" Fecha="[{DATE}]" Inc="[{TICKET}]">
         Creaci�n del trigger.
        <'||csbSLASH||'Modificacion>
        <'||csbSLASH||'Historial>
        <'||csbSLASH||'Procedure>
    *****************************************************************************'||csbSLASH||'

        rcAudit                      audit_[{TABLE}]%ROWTYPE;

        ----------------------------------------------------------------------------
        -- Constantes:
        ----------------------------------------------------------------------------

        -- Para el control de traza:
        csbSP_NAME                  CONSTANT VARCHAR2(32)                   := $$PLSQL_UNIT||''.''             ;
        csbPUSH                     CONSTANT VARCHAR2( 4)                   := pkg_EPM_Constante.fsbPUSH     ;
        csbPOP                      CONSTANT VARCHAR2( 4)                   := pkg_EPM_Constante.fsbPOP      ;
        csbPOP_ERC                  CONSTANT VARCHAR2( 4)                   := pkg_EPM_Constante.fsbPOP_ERC  ;
        csbPOP_ERR                  CONSTANT VARCHAR2( 4)                   := pkg_EPM_Constante.fsbPOP_ERR  ;
        cnuNVLTRC                   CONSTANT NUMBER  ( 2)                   := pkg_EPM_Constante.fnuNVL_TR   ;

    BEGIN

        pkg_epm_utilidades.trace_setmsg(csbSP_NAME, cnuNVLTRC, csbPUSH);

        rcAudit.CURRENT_USER_MASK           :=   Pkg_Epm_Utilidades.fsbUsuario;
        rcAudit.CURRENT_TERMINAL            :=   pkg_epm_utilidades.fsbTerminal;
        rcAudit.CURRENT_DATE_               :=   SYSDATE;
        rcAudit.CURRENT_EXE_NAME            :=   AU_BOSystem.getSystemProcessName;
        rcAudit.CURRENT_PROGRAM_NAME        :=   Pkg_Epm_Utilidades.fsbPrograma;
        rcAudit.CURRENT_TERM_IP_ADDR        :=   AU_BOSystem.getSystemUserIPAddress;
        rcAudit.CURRENT_USER_SO             :=   SYS_CONTEXT(''USERENV'', ''OS_USER'');
        rcAudit.[{PK}]                      :=   :old.[{PK}];

        IF DELETING THEN
            rcAudit.Current_Event := ''DELETE'';
        ELSIF UPDATING THEN
            rcAudit.Current_Event := ''UPDATE'';
        END IF;
        [{AUDIT_FIELDS_BLOCKS}]
        INSERT INTO audit_[{TABLE}] VALUES rcAudit;

        pkg_epm_utilidades.trace_setmsg(csbSP_NAME, cnuNVLTRC, csbPOP);

    EXCEPTION
        WHEN EX.CONTROLLED_ERROR THEN
            pkg_epm_utilidades.trace_setmsg(csbSP_NAME, cnuNVLTRC, csbPOP_ERC);
            RAISE EX.CONTROLLED_ERROR;
        WHEN OTHERS THEN
            pkg_epm_utilidades.trace_setmsg(csbSP_NAME, cnuNVLTRC, csbPOP_ERR);
            errors.seterror;
            RAISE EX.CONTROLLED_ERROR;
    END  TRG_AUD_[{TABLE}];
    '||csbSLASH||'
    ';

    BEGIN

        pkg_EPM_Utilidades.trace_SetMsg(csbSP_NAME||sbNameMethod, cnuNVLTRC, csbPUSH);

        IF (cuTableDesc%ISOPEN) THEN
            CLOSE cuTableDesc;
        END IF;
        OPEN    cuTableDesc(isbTableName);
        FETCH   cuTableDesc INTO sbTableDesc;
        CLOSE   cuTableDesc;

        sbScript := REPLACE(sbScript, '[{TABLE}]', upper(isbTableName));
        sbScript := REPLACE(sbScript, '[{AUTOR}]', isbAutor);
        sbScript := REPLACE(sbScript, '[{DATE}]', TO_CHAR(SYSDATE, 'YYYY/MM/DD'));
        sbScript := REPLACE(sbScript, '[{LOGIN}]', isbLogin);
        sbScript := REPLACE(sbScript, '[{TICKET}]', isbTicket);
        sbScript := REPLACE(sbScript, '[{PK}]', isbPK);
        sbScript := REPLACE(sbScript, '[{TABLE_DESC}]', sbTableDesc);

        IF (cuCampos%ISOPEN) THEN
            CLOSE cuCampos;
        END IF;
        OPEN    cuCampos(isbTableName);
        FETCH   cuCampos BULK COLLECT INTO tbCampos;
        CLOSE   cuCampos;

        /* Recorre la colecci�n de registros */
        nuIdxCampos := tbCampos.FIRST;
        LOOP
            EXIT WHEN nuIdxCampos IS NULL;

            sbFieldCond := REPLACE(sbFieldCondTemplate, '[{FIELD}]', tbCampos(nuIdxCampos).COLUMN_NAME);
            sbFieldCond := REPLACE(sbFieldCond, '[{DEFAULT_VAL}]', tbCampos(nuIdxCampos).DEFAULT_VALUE);

            -- Adiciona OR y Enter si no es el �ltimo
            IF (nuIdxCampos <> tbCampos.LAST) THEN
                sbFieldCond := sbFieldCond || ' OR'||csbENTER;
            END IF;

            -- Adiciona la condici�n a la lista de condiciones
            sbFielConditions := sbFielConditions || sbFieldCond;

            -- Llenar auditor�a para el campo
            sbFieldAudit := REPLACE(sbFieldAuditTemplate, '[{FIELD}]', tbCampos(nuIdxCampos).COLUMN_NAME);
            sbFieldAudit := REPLACE(sbFieldAudit, '[{DEFAULT_VAL}]', tbCampos(nuIdxCampos).DEFAULT_VALUE);

            sbFieldsAudits := sbFieldsAudits || sbFieldAudit;

            /* Avanza al siguiente registro */
            nuIdxCampos := tbCampos.NEXT(nuIdxCampos);
        END LOOP;

        -- Reemplaza las condiciones y el llenado
        sbScript := REPLACE(sbScript, '[{CONDITIONS}]', sbFielConditions);
        sbScript := REPLACE(sbScript, '[{AUDIT_FIELDS_BLOCKS}]', sbFieldsAudits);

        osbScript := sbScript;

        pkg_EPM_Utilidades.trace_SetMsg(csbSP_NAME||sbNameMethod, cnuNVLTRC, csbPOP);

    EXCEPTION
        WHEN Epm_Errors.EX_CTRLERROR THEN
            pkg_EPM_Utilidades.trace_SetMsg(csbSP_NAME||sbNameMethod, cnuNVLTRC, csbPOP_ERC);
            RAISE;
        WHEN OTHERS THEN
            Epm_Errors.SetError;
            pkg_EPM_Utilidades.trace_SetMsg(csbSP_NAME||sbNameMethod, cnuNVLTRC, csbPOP_ERR);
            RAISE Epm_Errors.EX_CTRLERROR;
    END p_DC_CreaTriggerAudit;
    
    PROCEDURE p_DC_GeneraAudit
    (
        isbTableName    IN  VARCHAR2,
        isbAutor        IN  VARCHAR2,
        isbLogin        IN  VARCHAR2,
        isbTicket       IN  VARCHAR2,
        isbPK           IN  VARCHAR2,
        osbScript       OUT CLOB,
        osbTrgScript    OUT CLOB
    )
    /*******************************************************************************
        <Procedure Fuente="Propiedad Intelectual de Empresas P�blicas de Medell�n">
        <Unidad>p_DC_GeneraAudit</Unidad>
        <Descripcion>
            Genera una tabla de auditor�a
        </Descripcion>
        <Autor> Diego Fernando Coba - MVM Ingenier�a de Software </Autor>
        <Fecha> 14-Ene-2021 </Fecha>
        <Parametros>
            <param nombre="isbTableName" tipo="TYPE" Direccion="In" >
                Tabla a la que se le va a crear la auditor�a
            </param>
            <param nombre="isbAutor" tipo="TYPE" Direccion="In" >
                Autor
            </param>
            <param nombre="isbLogin" tipo="TYPE" Direccion="In" >
                Usuario
            </param>
            <param nombre="isbTicket" tipo="TYPE" Direccion="In" >
                WO o HU
            </param>
            <param nombre="isbPK" tipo="TYPE" Direccion="In" >
                Llame primaria
            </param>
            <param nombre="osbScript" tipo="TYPE" Direccion="Out" >
                Contenido del Script
            </param>
        </Parametros>
        <Historial>
            <Modificacion Autor="dcoba" Fecha="14-Ene-2021" Inc="NNNNNN">
                Creaci�n del m�todo.
            </Modificacion>
        </Historial>
        </Procedure>
    *******************************************************************************/
    IS
        -- Nombre del m�todo
        sbNameMethod        VARCHAR2(30) := 'p_DC_GeneraAudit';

        csbSP_NAME                  CONSTANT VARCHAR2(32)                   := $$PLSQL_UNIT||'.';
        csbPUSH                     CONSTANT VARCHAR2( 4)                   := pkg_EPM_Constante.fsbPUSH     ;
        csbPOP                      CONSTANT VARCHAR2( 4)                   := pkg_EPM_Constante.fsbPOP      ;
        csbPOP_ERC                  CONSTANT VARCHAR2( 4)                   := pkg_EPM_Constante.fsbPOP_ERC  ;
        csbPOP_ERR                  CONSTANT VARCHAR2( 4)                   := pkg_EPM_Constante.fsbPOP_ERR  ;
        cnuNVLTRC                   CONSTANT NUMBER  ( 2)                   := pkg_EPM_Constante.fnuNVL_BO;

        CURSOR  cuTableDesc
        (
            isbTabla    IN  VARCHAR2
        )
        IS
            SELECT  comments
            FROM    dba_tab_comments
            WHERE   TABLE_NAME = UPPER(isbTabla);

        sbTableDesc dba_tab_comments.comments%TYPE;

        CURSOR cuCampos
        (
            isbTabla    IN  VARCHAR2
        )
        IS
            SELECT  COLUMN_NAME,
                    DATA_TYPE,
                    DATA_PRECISION,
                    DATA_SCALE,
                    COMMENTS
            FROM
            (
                WITH vwOLD_NEW AS
                (
                    SELECT 'O_' MODO, ' antes del cambio' SUFIJO FROM dual
                    UNION
                    SELECT 'N_' MODO, ' despu�s del cambio' SUFIJO FROM dual
                )
                SELECT  MODO||tc.COLUMN_NAME COLUMN_NAME,
                        tc.DATA_TYPE,
                        DECODE(tc.DATA_TYPE, 'VARCHAR2', DATA_LENGTH,tc.DATA_PRECISION) DATA_PRECISION,
                        tc.DATA_SCALE,
                        cc.COMMENTS||SUFIJO COMMENTS,
                        tc.COLUMN_ID,
                        MODO
                FROM    dba_tab_columns tc, dba_col_comments cc, vwOLD_NEW
                WHERE   tc.TABLE_NAME = UPPER(isbTabla)
                AND     tc.TABLE_NAME = cc.TABLE_NAME
                AND     tc.COLUMN_NAME = cc.COLUMN_NAME
                AND     tc.COLUMN_NAME <> UPPER(isbPK)

                UNION

                -- Primary Key (No debe llevar O_ ni N_)
                SELECT  tc.COLUMN_NAME COLUMN_NAME,
                        tc.DATA_TYPE,
                        DECODE(tc.DATA_TYPE, 'VARCHAR2', DATA_LENGTH, tc.DATA_PRECISION) DATA_PRECISION,
                        tc.DATA_SCALE,
                        cc.COMMENTS COMMENTS,
                        tc.COLUMN_ID,
                        'O' MODO
                FROM    dba_tab_columns tc, dba_col_comments cc
                WHERE   tc.TABLE_NAME = UPPER(isbTabla)
                AND     tc.TABLE_NAME = cc.TABLE_NAME
                AND     tc.COLUMN_NAME = cc.COLUMN_NAME
                AND     tc.COLUMN_NAME = UPPER(isbPK)
            )
            ORDER BY COLUMN_ID, COLUMN_NAME DESC;

        TYPE tytbCampos IS TABLE OF cuCampos%ROWTYPE INDEX BY BINARY_INTEGER;
        tbCampos tytbCampos;

        csbENTER            VARCHAR2(2) := CHR(13);
        csbSLASH            VARCHAR2(2) := CHR(47);
        nuIdxCampos         BINARY_INTEGER;

        sbTemplate_Campo    VARCHAR2(2000) := '[{FIELD}] [{TYPE}]([{PRECISION}],[{SCALE}])';
        sbTemplate_No_Pre   VARCHAR2(2000) := '[{FIELD}] [{TYPE}]';
        sbCampo             VARCHAR2(2000);
        sbCampos            VARCHAR2(32000);
        sbTemplate_Comment  VARCHAR2(1000) := 'COMMENT ON COLUMN audit_[{TABLE}].[{FIELD}] IS ''[{COMMENT}]'''||csbENTER||'/'||csbENTER;
        sbComment           VARCHAR2(32000);
        sbComments          VARCHAR2(32767);
        sbScript            CLOB :=
    '-- ************************************************************************
    -- Propiedad intelectual de Empresas P�blicas de Medell�n Copyright 2021
    --
    -- Archivo            craudit_[{TABLE}].sql
    -- Descripci�n        Creaci�n de auditor�a para la tabla [{TABLE_DESC}]
    -- Observaciones
    --
    -- Autor              [{AUTOR}]
    -- Fecha              [{DATE}]
    --
    -- Historia de Modificaciones
    -- Fecha        Autor      Modificaci�n
    -- [{DATE}]  [{LOGIN}]   [{TICKET}] - Creaci�n
    -- ************************************************************************

    PROMPT - Script    craudit_[{TABLE}].sql
    PROMPT - Autor     [{AUTOR}]
    PROMPT

    PROMPT - Creando tabla audit_[{TABLE}]

    CREATE TABLE audit_[{TABLE}]
    (
        CURRENT_EXE_NAME	    VARCHAR2(50) NOT NULL,
        CURRENT_USER_MASK       VARCHAR2(30) NOT NULL,
        CURRENT_TERMINAL        VARCHAR2(50) NOT NULL,
        CURRENT_USER_SO         VARCHAR2(30),
        CURRENT_DATE_           TIMESTAMP(6) NOT NULL,
        CURRENT_PROGRAM_NAME    VARCHAR2(64) NOT NULL,
        CURRENT_TERM_IP_ADDR    VARCHAR2(30),
        CURRENT_EVENT           VARCHAR2(30) NOT NULL,
    [FIELDS]
    )
    '||csbSLASH||'
    COMMENT ON TABLE audit_[{TABLE}] IS ''Auditor�a para la tabla [{TABLE_DESC}]''
    '||csbSLASH||'
    COMMENT ON COLUMN audit_[{TABLE}].CURRENT_EXE_NAME IS ''Nombre del ejecutable''
    '||csbSLASH||'
    COMMENT ON COLUMN audit_[{TABLE}].CURRENT_USER_MASK IS ''Usuario de base de datos que realiza el cambio''
    '||csbSLASH||'
    COMMENT ON COLUMN audit_[{TABLE}].CURRENT_TERMINAL IS ''Terminal desde donde se aplica el cambio''
    '||csbSLASH||'
    COMMENT ON COLUMN audit_[{TABLE}].CURRENT_USER_SO IS ''Usuario de sistema operativo que realiza el cambio''
    '||csbSLASH||'
    COMMENT ON COLUMN audit_[{TABLE}].CURRENT_DATE_ IS ''Momento en el que se aplica el cambio''
    '||csbSLASH||'
    COMMENT ON COLUMN audit_[{TABLE}].CURRENT_PROGRAM_NAME IS ''Nombre del programa''
    '||csbSLASH||'
    COMMENT ON COLUMN audit_[{TABLE}].CURRENT_TERM_IP_ADDR IS ''Direcci�n IP desde la cual se realiza el cambio''
    '||csbSLASH||'
    COMMENT ON COLUMN audit_[{TABLE}].CURRENT_EVENT IS ''Evento (Actualizaci�n o Borrado)''
    '||csbSLASH||'
    [{COLUMN_COMMENTS}]
    ';

    BEGIN

        pkg_EPM_Utilidades.trace_SetMsg(csbSP_NAME||sbNameMethod, cnuNVLTRC, csbPUSH);

        /* Consulta informaci�n de los campos */
        IF (cuCampos%ISOPEN) THEN
            CLOSE cuCampos;
        END IF;
        OPEN    cuCampos(isbTableName);
        FETCH   cuCampos BULK COLLECT INTO tbCampos;
        CLOSE   cuCampos;

        IF (cuTableDesc%ISOPEN) THEN
            CLOSE cuTableDesc;
        END IF;
        OPEN    cuTableDesc(isbTableName);
        FETCH   cuTableDesc INTO sbTableDesc;
        CLOSE   cuTableDesc;

        /* Adiciona campos */
        sbScript := REPLACE(sbScript, '[{AUTOR}]', isbAutor);
        sbScript := REPLACE(sbScript, '[{LOGIN}]', isbLogin);
        sbScript := REPLACE(sbScript, '[{DATE}]', TO_CHAR(SYSDATE, 'YYYY/MM/DD'));
        sbScript := REPLACE(sbScript, '[{TICKET}]', isbTicket);
        sbScript := REPLACE(sbScript, '[{TABLE}]', UPPER(isbTableName));
        sbScript := REPLACE(sbScript, '[{TABLE_DESC}]', sbTableDesc);

        /* Recorre la colecci�n de registros */
        nuIdxCampos := tbCampos.FIRST;
        LOOP
            EXIT WHEN nuIdxCampos IS NULL;

            -- Antepone coma y enter a la lista de campos
            IF (nuIdxCampos <> tbCampos.FIRST) THEN
                sbCampos := sbCampos||','||csbENTER;
            END IF;

            -- Define si usa template con precisi�n o sin precisi�n
            IF ( tbCampos(nuIdxCampos).DATA_TYPE IN ('DATE', 'CLOB', 'BLOB') ) THEN
                sbCampo := REPLACE(sbTemplate_No_Pre, '[{FIELD}]', tbCampos(nuIdxCampos).COLUMN_NAME);
            ELSE
                sbCampo := REPLACE(sbTemplate_Campo, '[{FIELD}]', tbCampos(nuIdxCampos).COLUMN_NAME);
            END IF;

            sbCampo := REPLACE(sbCampo, '[{TYPE}]', tbCampos(nuIdxCampos).DATA_TYPE);
            sbCampo := REPLACE(sbCampo, '[{PRECISION}]', tbCampos(nuIdxCampos).DATA_PRECISION);
            sbCampo := REPLACE(sbCampo, '[{SCALE}]', tbCampos(nuIdxCampos).DATA_SCALE);

            -- Ajusta la indentaci�n
            sbcampo := '    '||sbCampo;

            -- Si el campo es varchar, elimina la coma porque no se usa escala
            IF ( tbCampos(nuIdxCampos).DATA_TYPE = 'VARCHAR2') THEN
                sbcampo := REPLACE(sbcampo, ',', '');
            END IF;

            -- Adiciona a la lista de campos
            sbCampos := sbCampos||sbCampo;

            -- Arma el comentario
            sbComment := REPLACE(sbTemplate_Comment, '[{TABLE}]', upper(isbTableName));
            sbComment := REPLACE(sbComment, '[{FIELD}]', tbCampos(nuIdxCampos).COLUMN_NAME);
            sbComment := REPLACE(sbComment, '[{COMMENT}]', tbCampos(nuIdxCampos).COMMENTS);

            -- Adiciona el comentario
            sbComments := sbComments||sbComment;

            /* Avanza al siguiente registro */
            nuIdxCampos := tbCampos.NEXT(nuIdxCampos);
        END LOOP;

        -- Reemplaza la lista de campos
        sbScript := REPLACE(sbScript, '[FIELDS]', sbCampos);

        -- Reemplaza la lista de comentarios
        sbScript := REPLACE(sbScript, '[{COLUMN_COMMENTS}]', sbComments);

        -- Retorna el script creado
        osbScript := sbScript;

        -- Crea el script para el trigger que realizar� la auditor�a
        p_DC_CreaTriggerAudit
        (
            isbTableName,
            isbAutor,
            isbLogin,
            isbTicket,
            isbPK,
            osbTrgScript
        );

        pkg_EPM_Utilidades.trace_SetMsg(csbSP_NAME||sbNameMethod, cnuNVLTRC, csbPOP);

    EXCEPTION
        WHEN Epm_Errors.EX_CTRLERROR THEN
            pkg_EPM_Utilidades.trace_SetMsg(csbSP_NAME||sbNameMethod, cnuNVLTRC, csbPOP_ERC);
            RAISE;
        WHEN OTHERS THEN
            Epm_Errors.SetError;
            pkg_EPM_Utilidades.trace_SetMsg(csbSP_NAME||sbNameMethod, cnuNVLTRC, csbPOP_ERR);
            RAISE Epm_Errors.EX_CTRLERROR;
    END p_DC_GeneraAudit;
begin
    p_DC_GeneraAudit(:isbTableName,:isbAutor,:isbLogin,:isbTicket,:isbPK,:osbScript,:osbTrgScript);
end;