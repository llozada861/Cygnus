REM Archivo Generado Automáticamente
REM Nombre      : insepm_parametr_EXC_MENSAJE_MARK.sql
REM Descripcion : script que registra el parámetro en epm_parametr
REM Autor       : @Llozada

BEGIN
    INSERT INTO epm_parametr (PARAMETER_ID, DESCRIPTION, VALUE, VAL_FUNCTION, MODULE_ID, DATA_TYPE, ALLOW_UPDATE)
     VALUES
     (
         'EXC_MENSAJE_MARK',
         'Valores a excluir',
         '900196',
         '',
         201,
         'VARCHAR2',
         'Y'
     );
    COMMIT;
END;
/