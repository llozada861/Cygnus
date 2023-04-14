DECLARE
    PROCEDURE pObtFuentes
    (
        isbNombre   IN VARCHAR2,
        isbOwner    IN VARCHAR2,
        oclObjeto   OUT CLOB
    )
    IS
        sbText      VARCHAR2(4000);
        
        CURSOR cuDatos
        IS
            SELECT text
            FROM dba_source 
            WHERE NAME = isbNombre
            AND owner = isbOwner
            ORDER BY type,line;
    BEGIN
        dbms_lob.createtemporary(lob_loc => oclObjeto, cache => true, dur => dbms_lob.session);
        
        OPEN cuDatos;
        LOOP
            FETCH cuDatos INTO sbText;
            EXIT WHEN cuDatos%NOTFOUND;
            
            DBMS_LOB.APPEND(oclObjeto, sbText);
            
        END LOOP;
        CLOSE cuDatos;
    END;
BEGIN
    pObtFuentes(:isbNombre,:isbOwner,:oclObjeto);
END;
