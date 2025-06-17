DECLARE       
    PROCEDURE pReplicaRegla
    (
        id_regla    in number,
        oclSalida   out clob,
        onuErrorCode    OUT NUMBER,
        osbErrorMessage OUT VARCHAR2 
    )
    is
        sql_ins VARCHAR2(32000):=NULL;
        sbVariable varchar2(15);
        nuConfExpreId gr_config_expression.config_expression_id%type;
        sbObjectName varchar2(100);
        
        csbCARACTER_SEPA    VARCHAR2(1) := ';';
        
        CURSOR cuGetConfExpre
        (
            inuConfExpreID IN gr_config_expression.config_expression_id%type
        )
        IS
            SELECT rowid,a.* FROM gr_config_expression a
            WHERE config_expression_id in (inuConfExpreID);

        rcGetConfExpre  cuGetConfExpre%rowtype;
        
        sql_ins2 VARCHAR2(32000);
        sql_ins3  VARCHAR2(32000):=NULL;
        sbentrega_1 VARCHAR2(8000);
        sbentrega_2 VARCHAR2(8000);
        
        TYPE tyTabla IS TABLE OF VARCHAR2(2000) INDEX BY BINARY_INTEGER;
        tbstring tyTabla;
        
        PROCEDURE ParseString
        (
            ivaCadena               IN  VARCHAR2,
            ivaToken                IN  VARCHAR2,
            otbSalida               OUT tyTabla
        )
        IS
            csbMETHODNAME           CONSTANT VARCHAR2(30) := 'ParseString';

            nuIniBusqueda           NUMBER := 1;
            nuFinBusqueda           NUMBER := 1;
            sbArgumento             VARCHAR2( 2000 );
            nuIndArgumentos         NUMBER := 1;
            nuLongitudArg           NUMBER;
        BEGIN

            -- Recorre la lista de argumentos y los guarda en un tabla pl-sql
            WHILE( ivaCadena IS NOT NULL ) LOOP
                -- Busca el separador en la cadena y almacena su posicion
                nuFinBusqueda := INSTR( ivaCadena, ivaToken, nuIniBusqueda );

                -- Si no exite el pipe, debe haber un argumento
                IF ( nuFinBusqueda = 0 ) THEN
                    -- Obtiene el argumento
                    sbArgumento := SUBSTR( ivaCadena, nuIniBusqueda );
                    otbSalida( nuIndArgumentos ) := sbArgumento;

                    -- Termina el ciclo
                    EXIT;
                END IF;

                -- Obtiene el argumento hasta el separador
                nuLongitudArg := nuFinBusqueda - nuIniBusqueda;
                sbArgumento := SUBSTR( ivaCadena, nuIniBusqueda, nuLongitudArg );
                -- Lo adiciona a la tabla de argumentos, quitando espacios y ENTER a los lados
                otbSalida( nuIndArgumentos ) := TRIM( REPLACE( sbArgumento, CHR( 13 ), '' ));
                -- Inicializa la posicion inicial con la posicion del caracterer
                -- despues del pipe
                nuIniBusqueda := nuFinBusqueda + 1;
                -- Incrementa el indice de la tabla de argumentos
                nuIndArgumentos := nuIndArgumentos + 1;
            END LOOP;
        EXCEPTION
            WHEN OTHERS THEN
                dbms_output.put_line('ERROR OTHERS '||SQLERRM);
        END ParseString;
    BEGIN
    
        onuErrorCode := 0;
        osbErrorMessage := null;

        nuConfExpreId := id_regla;

        sbVariable := 'IdConfExpre';
        
        dbms_lob.createtemporary(lob_loc => oclSalida, cache => true, dur => dbms_lob.session);
        
        OPEN cuGetConfExpre(nuConfExpreId);
        FETCH cuGetConfExpre INTO rcGetConfExpre;
        CLOSE cuGetConfExpre;
        
        sql_ins := 'EXPRESSION = ''';

        sql_ins2:=rcGetConfExpre.EXPRESSION||''','||chr(10)||'           LAST_MODIFI_DATE = ';

        sql_ins3:='to_date ('''||
        rcGetConfExpre.LAST_MODIFI_DATE||''',''DD/MM/YYYY HH24:MI:SS''),'||chr(10)||'           MODIFICATION_TYPE ='''||
        rcGetConfExpre.MODIFICATION_TYPE||''', '||chr(10)||'           EXECUTION_TYPE = '''||
        rcGetConfExpre.EXECUTION_TYPE||''', '||chr(10)||'           DESCRIPTION = '''||
        rcGetConfExpre.DESCRIPTION||''','||chr(10)||'           OBJECT_TYPE = '''||rcGetConfExpre.OBJECT_TYPE||''','||chr(10)||
        '           expression_notes = null,'||chr(10)||'           code = null';

        ParseString(sql_ins2,csbCARACTER_SEPA,tbstring);

        
        sbentrega_1 := '/******************************************************************
Propiedad Intelectual de Empresas Publicas de Medellín
Archivo     up_<DDMMYYYY>_gr_config_expression.sql
Autor       <Nombre autor>
Fecha       <AAAAMMDD>

Descripción
Observaciones

Historia de Modificaciones
Fecha         Autor               Modificación
<AAAAMMDD>  <Nombre Autor>              Creación
******************************************************************/
declare
    tbConfigs   dagr_config_expression.tytbConfig_Expression_Id;
    onuExprId gr_config_expression.config_expression_id%type := NULL;
    nuCountErr number := 0;
    nuProc number := 0;
    nuErrorCode          NUMBER(15);
    sbErrorMsg           VARCHAR2(2000);

    IdConfExpre GR_CONFIG_EXPRESSION.config_expression_id%type;
BEGIN

    dbms_output.put_line(''Inicia Proceso ''||sysdate);
    IdConfExpre := '||nuConfExpreId||';

    dbms_output.put_line(''Actualizando la Regla: ''||IdConfExpre);

    UPDATE GR_CONFIG_EXPRESSION
       SET ';

        sbentrega_2 := '
    where config_expression_id = IdConfExpre;

    dbms_output.put_line(''Regenerando Regla: ''||IdConfExpre);

    BEGIN
        GR_BOINTERFACE_BODY.CreateStprByConfExpreId(IdConfExpre);
        dbms_output.put_line(''Expresion Generada = ''||IdConfExpre);

        EXCEPTION
            when ex.CONTROLLED_ERROR then
                 Errors.getError(nuErrorCode,sbErrorMsg);
                 dbms_output.put_line(substr(''ExprId = ''||IdConfExpre||'', Err : ''||nuErrorCode||'', ''||sbErrorMsg,1,250));

            when others then
                 Errors.setError;
                        Errors.getError(nuErrorCode,sbErrorMsg);
                 dbms_output.put_line(substr(''ExprId = ''||IdConfExpre||'', Err : ''||nuErrorCode||'', ''||sbErrorMsg,1,250));

    END;
        
    dbms_output.put_line(''Termina regenerar Regla: ''||IdConfExpre);

    --<INSERT_TABLA> o <UPDATE_TABLA>

    commit;

    dbms_output.put_line(''Fin Proceso ''||sysdate);
EXCEPTION
when ex.CONTROLLED_ERROR  then
    rollback;
    Errors.getError(nuErrorCode, sbErrorMsg);
    dbms_output.put_line(''ERROR CONTROLLED '');
    dbms_output.put_line(''error onuErrorCode: ''||nuErrorCode);
    dbms_output.put_line(''error osbErrorMess: ''||sbErrorMsg);

when OTHERS then
    rollback;
    Errors.setError;
    Errors.getError(nuErrorCode, sbErrorMsg);
    dbms_output.put_line(''ERROR OTHERS '');
    dbms_output.put_line(''error onuErrorCode: ''||nuErrorCode);
    dbms_output.put_line(''error osbErrorMess: ''||sbErrorMsg);
END;';
        
        DBMS_LOB.APPEND(oclSalida,sbentrega_1);
        DBMS_LOB.APPEND(oclSalida,sql_ins);

        FOR i IN 1..tbstring.count
        LOOP
              if i<> tbstring.count then
                if (i= tbstring.first) then
                  DBMS_LOB.APPEND(oclSalida,tbstring(i)||csbCARACTER_SEPA);
                else
                  if i<> (tbstring.count-1) then
                    DBMS_LOB.APPEND(oclSalida,tbstring(i)||csbCARACTER_SEPA||'''||'||chr(10)||'                        ''');

                  else
                    DBMS_LOB.APPEND(oclSalida,tbstring(i)||csbCARACTER_SEPA);
                  END if;
                END if;
              else
                DBMS_LOB.APPEND(oclSalida,tbstring(i));
              END if;
        END LOOP;

        DBMS_LOB.APPEND(oclSalida,sql_ins3);
        DBMS_LOB.APPEND(oclSalida,sbentrega_2);
        DBMS_LOB.APPEND(oclSalida,chr(10)||'/');

    EXCEPTION
        when OTHERS then
            dbms_output.put_line('ERROR OTHERS ');
            dbms_output.put_line('error onuErrorCode: '||onuErrorCode);
            dbms_output.put_line('error osbErrorMess: '||osbErrorMessage);
    END;
begin
    pReplicaRegla
    (
        :id_regla,
        :oclFile,
        :onuErrorCode,
        :osbErrorMessage
     );
end;
