Insert into FLEX.EPM_REGPROC
   (PROCESO, API_DESC, API_PROCESO, HILOS, GENERA_ARCHIVO [api_pre] [api_post] [api_cant])
 Values
   ('[NOMBRE]', '[descripcion]','[api_prin]', [hilos], 'N' [api_preval] [api_postval] [api_cantval]);
COMMIT;
/
SELECT * FROM epm_regproc
/
PKG_EPM_BOCORTRECONSIRIUS.FNUGETQUANTITY
