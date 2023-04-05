declare
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
begin
    pGenerapktbl(:isbTabla,:isbOwner,:isOrder,:oclFile,:onuErrorCode,:osbErrorMessage);
end;
/