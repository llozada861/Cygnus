/*****************************************************************
Archivo Generado Automáticamente
@Autor Llozada
******************************************************************/
DECLARE
    iclTemplateSource ge_xsl_template.template_source%type;
    isbGenerationType ge_xsl_template.generation_type%type;
    isbValidate       VARCHAR2(1);
    oclTemplateXsl    ge_xsl_template.template_xsl%type;
    nuType      NUMBER;
    nuErrorCode NUMBER;
    sbErrMsg    VARCHAR2(2000);
    nuCurrentJVMHeapSize NUMBER;
    
    CURSOR cuXslTemplate IS
    SELECT  *
    FROM    ge_xsl_template
	WHERE generation_type != 'N';    
BEGIN
    
    for i in cuXslTemplate loop
    
    	ut_trace.trace('Inicia template: '||i.xsl_template_id,2);

        iclTemplateSource := i.template_source;
        isbGenerationType := i.generation_type;
        isbValidate       := 'Y';
        oclTemplateXsl    := null;

		BEGIN

		    GE_BOXsl_Template.generateXslTemplate
		        (
		        iclTemplateSource,
		        isbGenerationType,
		        isbValidate,
		        oclTemplateXsl
		        );
		        
	        ge_boxsl_template.updxsltemplatesource(i.xsl_template_id,iclTemplateSource,oclTemplateXsl);
		        
    	EXCEPTION
      		WHEN OTHERS THEN
				dbms_output.put_line('Error CONTROLADO, template: '||i.xsl_template_id);
				dbms_output.put_line(''||sqlcode);
				dbms_output.put_line(''||sqlerrm);
		END;

    END loop;
    
    commit;

EXCEPTION
    when ex.CONTROLLED_ERROR then
        raise ex.CONTROLLED_ERROR;
    when others then
        Errors.setError;
        raise ex.CONTROLLED_ERROR;
END;
/