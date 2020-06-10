    /******************************************************************************************************
    <Procedure Fuente="Propiedad Intelectual de Empresas Publicas de Medellín">
    <Unidad> fnuObtCantidad[NOMBRE]</Unidad>
    <Descripcion>
         Obtiene la cantidad de registros a procesar para el hilo
    </Descripcion>
    <Autor>[desarrollador] - MVM </Autor>
    <Fecha>[fecha]</Fecha>
    <Parametros>
        <param nombre="inuHilo" tipo="NUMBER" Direccion="In" >
             Hilo a procesar
        </param>
        <param nombre="isbPrograma" tipo="estaprog.esprprog%type" Direccion="In" >
             Programa estaprog
        </param>
        <param nombre="inuHilo" tipo="NUMBER" Direccion="In" >
             Hilo a procesar
        </param>
        <param nombre="inuHilos" tipo="NUMBER" Direccion="In" >
             Cantidad de hilos
        </param>
        [PARAMETROS_HTML]
    </Parametros>
    <Retorno Nombre = "Numérico" Tipo = "NUMBER">
        Descripcion
    </Retorno>
    <Historial>
    <Modificacion Autor="[desarrollador]" Fecha="[fecha]" Inc="WOXXXXX">
          Creacion.
    </Modificacion>
    </Historial>
    </Procedure>
    ******************************************************************************************************/
    FUNCTION fnuObtCantidad[NOMBRE]
    (
        inuHilo     IN NUMBER,
        isbPrograma IN estaprog.esprprog%type,
        inuHilos    IN NUMBER,
        [PARAMETROS]
    )
    RETURN NUMBER
    IS
        csbMT_NAME      VARCHAR2(50)   := 'fnuObtCantidad[NOMBRE]';
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
    END fnuObtCantidad[NOMBRE];
