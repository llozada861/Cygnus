    /***************************************************************************
    <Procedure Fuente="Propiedad Intelectual de Empresas Publicas de Medellín">
    <Unidad> pProcesoPost[NOMBRE] </Unidad>
    <Descripcion>
        Api POST para el proceso [NOMBRE]
    </Descripcion>
    <Autor> [desarrollador] - MVM </Autor>
    <Fecha>[fecha]</Fecha>
    <Parametros>
        <param nombre="isbPrograma" tipo="estaprog.esprprog%type" Direccion="IN">
            Programa
        </param>
        <param nombre="inuHilos" tipo="NUMBER" Direccion="IN">
            Cantidad de hilos
        </param>
        [PARAMETROS_HTML]
    </Parametros>
    <Historial>
    <Modificacion Autor="[desarrollador]" Fecha="[fecha]" Inc="WOXXXX">
        Método creado por Cygnus.
    </Modificacion>
    </Historial>
    </Procedure>
    ***************************************************************************/
    PROCEDURE pProcesoPost[NOMBRE]
    (
        isbPrograma IN estaprog.esprprog%type,
        inuHilos    IN NUMBER,
        [PARAMETROS]
    )
    IS
        csbMT_NAME      VARCHAR2(50)   := 'pProcesoPost[NOMBRE]';
    BEGIN
        pkg_epm_utilidades.trace_setmsg (csbSP_NAME||csbMT_NAME, cnuNVLTRC, csbPUSH);

        /******************* START USER CODE ***********************/        
            
       /******************** END USER CODE ************************/

        pkg_epm_utilidades.trace_setmsg (csbSP_NAME||csbMT_NAME, cnuNVLTRC, csbPOP);
    EXCEPTION
        WHEN LOGIN_DENIED OR ex.CONTROLLED_ERROR OR pkConstante.exERROR_LEVEL2 THEN
            Pkg_Epm_Utilidades.Trace_SetMsg(csbSP_NAME||csbMT_NAME, cnuNVLTRC, csbPOP_ERC);
            RAISE;
        WHEN OTHERS THEN
            Pkg_Epm_Utilidades.Trace_SetMsg(csbSP_NAME||csbMT_NAME, cnuNVLTRC, csbPOP_ERR);
            Errors.SetError;
            RAISE ex.CONTROLLED_ERROR;
    END pProcesoPost[NOMBRE];
