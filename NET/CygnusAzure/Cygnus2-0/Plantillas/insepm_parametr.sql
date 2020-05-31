REM Archivo Generado Automáticamente
REM Nombre      : insepm_parametr_<parametro>.sql
REM Descripcion : script que registra el parámetro en epm_parametr
REM Autor       : @Llozada

BEGIN
    INSERT INTO epm_parametr (PARAMETER_ID, DESCRIPTION, VALUE, VAL_FUNCTION, MODULE_ID, DATA_TYPE, ALLOW_UPDATE)
     VALUES
     (
         '<parametro>',
         '<descripcion>',
         '<valor>',
         '<funcion>',
         201,
         '<tipo>',
         'Y'
     );
    COMMIT;
END;
/