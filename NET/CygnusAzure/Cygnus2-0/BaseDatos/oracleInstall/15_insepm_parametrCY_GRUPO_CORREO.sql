REM Archivo Generado Autom�ticamente
REM Nombre      : insepm_parametr_CY_GRUPO_CORREO.sql
REM Descripcion : script que registra el par�metro en epm_parametr
REM Autor       : @Llozada

BEGIN
    INSERT INTO epm_parametr (PARAMETER_ID, DESCRIPTION, VALUE, VAL_FUNCTION, MODULE_ID, DATA_TYPE, ALLOW_UPDATE)
     VALUES
     (
         'CY_GRUPO_CORREO',
         'Grupo para env�o de correos.',
         'personalepmopen',
         '',
         201,
         'VARCHAR2',
         'Y'
     );
    COMMIT;
END;
/