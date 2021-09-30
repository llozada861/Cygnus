CREATE OR REPLACE PROCEDURE p_DC_CreaTriggerAudit
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
    <Procedure Fuente="Propiedad Intelectual de Empresas Públicas de Medellín">
    <Unidad>p_DC_CreaTriggerAudit</Unidad>
    <Descripcion>
        Crea el fuente para un trigger de auditoría
    </Descripcion>
    <Autor> Diego Fernando Coba - MVM Ingeniería de Software </Autor>
    <Fecha> 14-Ene-2021 </Fecha>
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
    <Historial>
        <Modificacion Autor="dcoba" Fecha="14-Ene-2021" Inc="NNNNNN">
            Creación del método.
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

    -- Nombre del método
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
    <Procedure Fuente="Propiedad Intelectual de Empresas Publicas de Medellín">
    <Unidad> TRG_AUD_[{TABLE}] <'||csbSLASH||'Unidad>
    <Descripcion>
    Registra auditoría de la tabla [{TABLE_DESC}]
    <'||csbSLASH||'Descripcion>
    <Autor> [{AUTOR}] - MVM <'||csbSLASH||'Autor>
    <Fecha>[{DATE}]<'||csbSLASH||'Fecha>
    <Historial>
    <Modificacion Autor="[{LOGIN}]" Fecha="[{DATE}]" Inc="[{TICKET}]">
     Creación del trigger.
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

    /* Recorre la colección de registros */
    nuIdxCampos := tbCampos.FIRST;
    LOOP
        EXIT WHEN nuIdxCampos IS NULL;

        sbFieldCond := REPLACE(sbFieldCondTemplate, '[{FIELD}]', tbCampos(nuIdxCampos).COLUMN_NAME);
        sbFieldCond := REPLACE(sbFieldCond, '[{DEFAULT_VAL}]', tbCampos(nuIdxCampos).DEFAULT_VALUE);

        -- Adiciona OR y Enter si no es el último
        IF (nuIdxCampos <> tbCampos.LAST) THEN
            sbFieldCond := sbFieldCond || ' OR'||csbENTER;
        END IF;

        -- Adiciona la condición a la lista de condiciones
        sbFielConditions := sbFielConditions || sbFieldCond;

        -- Llenar auditoría para el campo
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
/
