REM Archivo Generado Automáticamente
REM Nombre      : insmensaje_EPM-CUZ-<codigo>.sql
REM Descripcion : script que registra en la tabla mensaje
REM Autor       : @Llozada

BEGIN
    --realiza la insercción del mensaje EPM - CUZ - <codigo>
    INSERT INTO mensaje (menscodi,mensdesc,mensdivi,mensmodu,menscaus,mensposo ) 
	VALUES
	(
		<codigo>,
		'<descripcion>',
		'EPM',
		'CUZ',
		'<causa>',
		'<solucion>'
	);
    COMMIT;    
END;
/