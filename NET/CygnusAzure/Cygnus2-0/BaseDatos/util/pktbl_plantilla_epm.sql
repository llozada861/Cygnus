CREATE OR REPLACE PACKAGE [nombre]
IS
/**************************************************************************
    Copyright (c) 2020 EPM - Empresas Públicas de Medellín
    Archivo generado automaticamente.
    [usuario] - [empresa]
            
    Nombre      :   [nombre]
    Descripción :   Paquete de primer nivel, para la tabla [tabla]
    Autor       :   Generador automatico paquetes de primer nivel.
    Fecha       :   [fecha]
    WO          :   [caso]
            
    Historial de Modificaciones
    ---------------------------------------------------------------------------
    Fecha         Autor         Descripcion
    =====         =======       ===============================================
***************************************************************************/
                
    --------------------------------------------
    --  Type and Subtypes
    --------------------------------------------

    -- Define colecciones de cada columna de la tabla [tabla]
    [campos_definicion]

    -- Define registro de colecciones
    TYPE tytb[tabla] IS RECORD
    (
        [campos_coleccion]
    );
    --------------------------------------------
    -- Constants
    --------------------------------------------
        
    --------------------------------------------
    -- Variables
    --------------------------------------------

    -- Cursor para accesar [tabla]
    [cursor_tabla]

    --------------------------------------------
    -- Funciones y Procedimientos
    --------------------------------------------
    -- Insertar un registro
    PROCEDURE InsRecord
    (
        ircRecord in [tabla]%rowtype
    );

    -- Insertar colección de registros
    PROCEDURE InsRecords
    (
        irctbRecord  IN OUT NOCOPY   tytb[tabla]
    );

    -- Insertar un record de tablas por columna
    PROCEDURE InsForEachColumn
    (
       [campos_tipo]
    );

    -- Insertar un record de tablas por columna masivamente
    PROCEDURE InsForEachColumnBulk
    (
       [campos_tipo_tb]
    );

    -- Limpiar la memoria
    PROCEDURE ClearMemory;

    -- Eliminar un registro
    PROCEDURE DelRecord
    (
        [llave_primaria]
    );

    -- Actualizar un registro
    PROCEDURE UpRecord
    (
        ircRecord in [tabla]%rowtype
    );
        
    -- Eliminar un grupo de registros
    PROCEDURE DelRecords
    (
        [llave_primaria_tb]    
    );
                
    -- Indica si el registro existe
    FUNCTION fblExist
    (
        [llave_primaria],
        inuCACHE IN NUMBER DEFAULT 1
    )
    RETURN BOOLEAN;
    
    -- Obtiene registro
    FUNCTION frcGetRecord
    (
        [llave_primaria],
        inuCACHE IN NUMBER DEFAULT 1
    )
    RETURN [tabla]%ROWTYPE;
    
    -- Valida si existe un registro
    PROCEDURE AccKey
    (
        [llave_primaria],
        inuCACHE IN NUMBER DEFAULT 1
    );
    
    -- Valida si está duplicad
    PROCEDURE ValidateDupValues
    (
        [llave_primaria],
        inuCACHE IN NUMBER DEFAULT 1
    );
    
    [metodos_campos]

END [nombre];
/
CREATE OR REPLACE PACKAGE BODY [nombre]
IS    
    -------------------------
    --  PRIVATE VARIABLES
    -------------------------

    -- Record Tabla [tabla]
    rc[tabla] cu[tabla]%ROWTYPE;
    
    -- Record nulo de la Tabla [tabla]
    rcRecordNull [tabla]%ROWTYPE;
        
    -------------------------
    --   PRIVATE METHODS   
    -------------------------
        
    PROCEDURE Load
    (
        [llave_primaria]
    );
        
    PROCEDURE LoadRecord
    (
        [llave_primaria]
    );
        
    FUNCTION fblInMemory
    (
        [llave_primaria]
    )
    RETURN BOOLEAN;

    -----------------
    -- CONSTANTES
    -----------------
    CACHE                      CONSTANT NUMBER := 1;   -- Buscar en Cache
    
    -------------------------
    --  PRIVATE CONSTANTS
    -------------------------
    cnuRECORD_NO_EXISTE        CONSTANT NUMBER := 12201; -- Reg. no esta en BD
    cnuRECORD_YA_EXISTE        CONSTANT NUMBER := 12202; -- Reg. ya esta en BD    
    -- Division
    csbDIVISION                CONSTANT VARCHAR2(20) := pkConstante.csbDIVISION;
    -- Modulo
    csbMODULE                  CONSTANT VARCHAR2(20) := pkConstante.csbMOD_CUZ;
    -- Texto adicionar para mensaje de error
    csbTABLA_PK                CONSTANT VARCHAR2(200):= '(Tabla [tabla]) ( PK [';
    csb_TABLA                  CONSTANT VARCHAR2(200):= '(Tabla [tabla])';

    -- Carga
    PROCEDURE Load
    (
        [llave_primaria]
    )
    IS
    BEGIN
        pkErrors.Push( '[nombre].Load' );
        LoadRecord
        (
            [llave_primaria_alias]
        );
        -- Evalúa si se encontro el registro en la Base de datos
        IF ( rc[tabla].[llave_primaria_sinTipo] IS NULL ) THEN
            pkErrors.Pop;
            RAISE NO_DATA_FOUND;
        END IF;
        pkErrors.Pop;
    EXCEPTION
        WHEN NO_DATA_FOUND THEN
            pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_NO_EXISTE );
            pkErrors.ADDSUFFIXTOMESSAGE ( csbTABLA_PK||[llave_primaria_alias]||'] )');
            pkErrors.Pop;
            RAISE LOGIN_DENIED;
    END Load;
    
    -- Carga    
    PROCEDURE LoadRecord
    (
        [llave_primaria]
    )
    IS
    BEGIN
        pkErrors.Push( '[nombre].LoadRecord' );     
        IF ( cu[tabla]%ISOPEN ) THEN
            CLOSE cu[tabla];
        END IF;
        -- Accesa [tabla] de la BD
        OPEN cu[tabla]
        (
            [llave_primaria_alias]
        );
        FETCH cu[tabla] INTO rc[tabla];
        IF ( cu[tabla]%NOTFOUND ) then
            rc[tabla] := rcRecordNull;
        END IF;
        CLOSE cu[tabla];
        pkErrors.Pop;
    
    END LoadRecord;    
    
    -- Indica si está en memoria  
    FUNCTION fblInMemory
    (
        [llave_primaria]
    )
    RETURN BOOLEAN
    IS
    BEGIN
        pkErrors.Push( '[nombre].fblInMemory' );
        
        IF ( [compara_pks] ) THEN
            pkErrors.Pop;
            RETURN( TRUE );
        END IF;
        pkErrors.Pop;
        RETURN( FALSE );
    
    END fblInMemory;
    
    -- Valida si existe registro
    PROCEDURE AccKey
    (
        [llave_primaria],
        inuCACHE IN NUMBER DEFAULT 1
    )
    IS
    BEGIN
        pkErrors.Push( '[nombre].AccKey' );
            
        --Valida si debe buscar primero en memoria Cache
        IF NOT (inuCACHE = CACHE AND fblInMemory([llave_primaria_alias]) ) THEN
        
            Load
            (
                [llave_primaria_alias]
            );
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
        pkErrors.Push( '[nombre].ClearMemory' );
        rc[tabla] := rcRecordNull;
        pkErrors.Pop;
    END ClearMemory;
    
    -- Elimina registro    
    PROCEDURE DelRecord
    (
        [llave_primaria]
    )
    IS
    BEGIN
        pkErrors.Push( '[nombre].DelRecord' );
            
        --Elimina registro de la Tabla [tabla]
        DELETE  [tabla]
        WHERE   [compara_pks];

        IF ( sql%NOTFOUND ) THEN
            pkErrors.Pop;
            RAISE NO_DATA_FOUND;
        END IF;
        pkErrors.Pop;
            
        EXCEPTION
            WHEN NO_DATA_FOUND THEN
                pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_NO_EXISTE );
                pkErrors.ADDSUFFIXTOMESSAGE ( csbTABLA_PK||[llave_primaria_alias]||'] )');
                pkErrors.Pop;
                RAISE LOGIN_DENIED;
    END DelRecord;
    
    -- Elimina registros
    PROCEDURE DelRecords
    (
        [llave_primaria_tb]
    )
    IS
    BEGIN
        pkErrors.Push( '[nombre].DelRecords' );
            
        -- Elimina registros de la Tabla [tabla]
        [borrado_forall]

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
    END DelRecords;
    
    -- Inserta registro por columna
    PROCEDURE InsForEachColumn
    (
       InsForEachColumn

    )
    IS
      rcRecord [tabla]%ROWTYPE;   -- Record de la Tabla [tabla]
    BEGIN
       pkErrors.Push( '[nombre].InsForEachColumn ');
       
       [record_campos]

       InsRecord( rcRecord );
       pkErrors.Pop;
    EXCEPTION
        WHEN LOGIN_DENIED THEN
            pkErrors.Pop;
            RAISE LOGIN_DENIED;
    END InsForEachColumn;
    
    -- Inserta registro de tablas por campo masivamente
    PROCEDURE InsForEachColumnBulk
    (
       [campos_tipo_tb]
    )
    IS      
    BEGIN
        pkErrors.Push(' [nombre].InsForEachColumnBulk ');

        FORALL indx in [llave_primaria_sinTipo].FIRST .. [llave_primaria_sinTipo].LAST
        INSERT INTO [tabla]
        ( 
            [lista_campos_sintipo]
        )
        VALUES 
        (
            [lista_campos_tbidx]
        );

       pkErrors.Pop;
    EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
            pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_YA_EXISTE );
            pkErrors.ADDSUFFIXTOMESSAGE ( csb_TABLA );
            pkErrors.Pop;
            RAISE LOGIN_DENIED;
    END InsForEachColumnBulk;
    
    -- Inserta un registro
    PROCEDURE InsRecord
    (
        ircRecord in [tabla]%ROWTYPE
    )
    IS
    BEGIN
        pkErrors.Push( '[nombre].InsRecord' );
            
        INSERT INTO [tabla]
        (
            [lista_campos_sintipo]
        ) 
        VALUES 
        (
            [lista_campos_record]
        );
        
        pkErrors.Pop;
    
    EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
            pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_YA_EXISTE );
            pkErrors.ADDSUFFIXTOMESSAGE ( csb_TABLA );
            pkErrors.Pop;
            RAISE LOGIN_DENIED;
    END InsRecord;
    
    -- Inserta tabla de registros
    PROCEDURE InsRecords
    (
        irctbRecord  IN OUT NOCOPY   tytb[tabla]
    )
    IS      
    BEGIN
        pkErrors.Push(' [nombre].InsRecords' );
        FORALL indx IN irctbRecord.[llave_primaria_sinTipo].FIRST .. irctbRecord.[llave_primaria_sinTipo].LAST
        INSERT INTO [tabla]
        (
            [lista_campos_sintipo]
         )
         VALUES 
         (
            [lista_campos_tbRecord]
        );

        pkErrors.Pop;
    EXCEPTION
        WHEN DUP_VAL_ON_INDEX THEN
            pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_YA_EXISTE );
            pkErrors.ADDSUFFIXTOMESSAGE ( csb_TABLA );
            pkErrors.Pop;
            RAISE LOGIN_DENIED;
    END InsRecords;
    
    -- Actualiza registro
    PROCEDURE UpRecord
    (
        ircRecord IN [tabla]%ROWTYPE
    )
    IS
    BEGIN
        pkErrors.Push( '[nombre].UpRecord' );
            
        UPDATE  [tabla]
        SET     [campos_set_update]
        WHERE   [compara_pks];

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
    (
        [llave_primaria],
        inuCACHE    IN NUMBER DEFAULT 1
    )
    IS
    BEGIN
        pkErrors.Push( '[nombre].ValidateDupValues' );
            
        --Valida si el registro ya existe
        IF ( fblExist( [llave_primaria_alias], inuCACHE ) ) THEN
            pkErrors.SetErrorCode( csbDIVISION, csbMODULE, cnuRECORD_YA_EXISTE );
            pkErrors.ADDSUFFIXTOMESSAGE ( csbTABLA_PK||[llave_primaria_alias]||'] )');
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
    (
        [llave_primaria],
        inuCACHE    IN NUMBER DEFAULT 1
    )
    RETURN BOOLEAN
    IS
    BEGIN
        pkErrors.Push( '[nombre].fblExist' );
            
        --Valida si debe buscar primero en memoria Caché
        IF (inuCACHE = CACHE AND fblInMemory( [llave_primaria_alias] ) ) THEN
                pkErrors.Pop;
                RETURN( TRUE );
        END IF;
        LoadRecord
        (
            [llave_primaria_alias]
        );
            
        -- Evalúa si se encontro el registro en la Base de datos
        IF ( rc[tabla].[llave_primaria_sinTipo] IS NULL ) THEN
            pkErrors.Pop;
            RETURN( FALSE );
        END IF;
            
        pkErrors.Pop;
    
        RETURN( TRUE );
        
    END fblExist;
    
    -- Obtiene el registro
    FUNCTION frcGetRecord
    (
        [llave_primaria],
        inuCACHE    IN NUMBER DEFAULT 1
    )
    RETURN [tabla]%ROWTYPE
    IS
    BEGIN
        pkErrors.Push( '[nombre].frcGetRecord' );
            
        --Valida si el registro ya existe
        AccKey
        (
            [llave_primaria_alias],
            inuCACHE
        );
    
        pkErrors.Pop;
        RETURN ( rc[tabla] );
         
    EXCEPTION
        WHEN LOGIN_DENIED THEN
            pkErrors.Pop;
            RAISE LOGIN_DENIED;
    END frcGetRecord;
    
    [metodos_campos_cuerpo]
END [nombre];
/