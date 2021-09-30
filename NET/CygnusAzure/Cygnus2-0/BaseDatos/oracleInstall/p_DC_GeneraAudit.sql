CREATE OR REPLACE PROCEDURE p_DC_GeneraAudit
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
    <Procedure Fuente="Propiedad Intelectual de Empresas Públicas de Medellín">
    <Unidad>p_DC_GeneraAudit</Unidad>
    <Descripcion>
        Genera una tabla de auditoría
    </Descripcion>
    <Autor> Diego Fernando Coba - MVM Ingeniería de Software </Autor>
    <Fecha> 14-Ene-2021 </Fecha>
    <Parametros>
        <param nombre="isbTableName" tipo="TYPE" Direccion="In" >
            Tabla a la que se le va a crear la auditoría
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
            Creación del método.
        </Modificacion>
    </Historial>
    </Procedure>
*******************************************************************************/
IS
    -- Nombre del método
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
                SELECT 'N_' MODO, ' después del cambio' SUFIJO FROM dual
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
-- Propiedad intelectual de Empresas Públicas de Medellín Copyright 2021
--
-- Archivo            craudit_[{TABLE}].sql
-- Descripción        Creación de auditoría para la tabla [{TABLE_DESC}]
-- Observaciones
--
-- Autor              [{AUTOR}]
-- Fecha              [{DATE}]
--
-- Historia de Modificaciones
-- Fecha        Autor      Modificación
-- [{DATE}]  [{LOGIN}]   [{TICKET}] - Creación
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
COMMENT ON TABLE audit_[{TABLE}] IS ''Auditoría para la tabla [{TABLE_DESC}]''
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
COMMENT ON COLUMN audit_[{TABLE}].CURRENT_TERM_IP_ADDR IS ''Dirección IP desde la cual se realiza el cambio''
'||csbSLASH||'
COMMENT ON COLUMN audit_[{TABLE}].CURRENT_EVENT IS ''Evento (Actualización o Borrado)''
'||csbSLASH||'
[{COLUMN_COMMENTS}]
';

BEGIN

    pkg_EPM_Utilidades.trace_SetMsg(csbSP_NAME||sbNameMethod, cnuNVLTRC, csbPUSH);

    /* Consulta información de los campos */
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

    /* Recorre la colección de registros */
    nuIdxCampos := tbCampos.FIRST;
    LOOP
        EXIT WHEN nuIdxCampos IS NULL;

        -- Antepone coma y enter a la lista de campos
        IF (nuIdxCampos <> tbCampos.FIRST) THEN
            sbCampos := sbCampos||','||csbENTER;
        END IF;

        -- Define si usa template con precisión o sin precisión
        IF ( tbCampos(nuIdxCampos).DATA_TYPE IN ('DATE', 'CLOB', 'BLOB') ) THEN
            sbCampo := REPLACE(sbTemplate_No_Pre, '[{FIELD}]', tbCampos(nuIdxCampos).COLUMN_NAME);
        ELSE
            sbCampo := REPLACE(sbTemplate_Campo, '[{FIELD}]', tbCampos(nuIdxCampos).COLUMN_NAME);
        END IF;

        sbCampo := REPLACE(sbCampo, '[{TYPE}]', tbCampos(nuIdxCampos).DATA_TYPE);
        sbCampo := REPLACE(sbCampo, '[{PRECISION}]', tbCampos(nuIdxCampos).DATA_PRECISION);
        sbCampo := REPLACE(sbCampo, '[{SCALE}]', tbCampos(nuIdxCampos).DATA_SCALE);

        -- Ajusta la indentación
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

    -- Crea el script para el trigger que realizará la auditoría
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
/
