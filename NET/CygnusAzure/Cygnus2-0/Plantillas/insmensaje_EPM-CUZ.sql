REM Archivo Generado Automáticamente
REM Nombre      = insmensaje_EPM-CUZ-:CODIGO.sql
REM Descripcion = script que registra en la tabla mensaje

BEGIN
    --realiza la insercción del mensaje EPM - CUZ - <codigo>
    INSERT INTO mensaje (menscodi,mensdesc,mensdivi,mensmodu,menscaus,mensposo ) 
    VALUES
    (
        :CODIGO,
        :DESCRIPCION,
        'EPM',
        'CUZ',
        :CAUSA,
        :SOLUCION
    );
    COMMIT;    
END;
/
MERGE INTO MENSAJE A USING
 (SELECT
  :CODIGO as MENSCODI,
  :DESCRIPCION as MENSDESC,
  'EPM' as MENSDIVI,
  'CUZ' as MENSMODU,
  :CAUSA as MENSCAUS,
  :SOLUCION  as MENSPOSO
  FROM DUAL) B
ON (A.MENSDIVI = B.MENSDIVI and A.MENSMODU = B.MENSMODU and A.MENSCODI = B.MENSCODI)
WHEN NOT MATCHED THEN 
INSERT (
  MENSCODI, MENSDESC, MENSDIVI, MENSMODU, MENSCAUS, 
  MENSPOSO)
VALUES (
  B.MENSCODI, B.MENSDESC, B.MENSDIVI, B.MENSMODU, B.MENSCAUS, 
  B.MENSPOSO)
WHEN MATCHED THEN
UPDATE SET 
  A.MENSDESC = B.MENSDESC,
  A.MENSCAUS = B.MENSCAUS,
  A.MENSPOSO = B.MENSPOSO;

COMMIT;
/
update html 
set documentation = 'REM Archivo Generado Automáticamente
REM Nombre      = insmensaje_EPM-CUZ-:CODIGO.sql
REM Descripcion = script que registra en la tabla mensaje

BEGIN
    --realiza la insercción del mensaje EPM - CUZ - <codigo>
    INSERT INTO mensaje (menscodi,mensdesc,mensdivi,mensmodu,menscaus,mensposo ) 
    VALUES
    (
        :CODIGO,
        :DESCRIPCION,
        ''EPM'',
        ''CUZ'',
        :CAUSA,
        :SOLUCION
    );
    COMMIT;    
END;
/'
where name = 'PLANTILLA_MENSAJE';

