    /***************************************************************************
    <Procedure Fuente="Propiedad Intelectual de Empresas Publicas de Medellín">
    <Unidad> pProcesoMasivo[NOMBRE] </Unidad>
    <Descripcion>
        Api principal para el proceso [NOMBRE]
    </Descripcion>
    <Autor> [desarrollador] - MVM </Autor>
    <Fecha>[fecha]</Fecha>
    <Parametros>
        <param nombre="inuHilo" tipo="NUMBER" Direccion="IN">
            Hilo a procesar
        </param>
        <param nombre="inuHilos" tipo="NUMBER" Direccion="IN">
            Cantidad de hilos
        </param>
        <param nombre="inuTotal" tipo="NUMBER" Direccion="IN">
            Cantidad de registros a procesar
        </param>
        <param nombre="isbPrograma" tipo="estaprog.esprprog%type" Direccion="IN">
            Programa
        </param>
        [PARAMETROS_HTML]
        <param nombre="isbUsuario" tipo="VARCHAR2" Direccion="IN">
            Usuario que ejecuta
        </param>
        <param nombre="isbTerminal" tipo="VARCHAR2" Direccion="IN">
            Terminal
        </param>
        <param nombre="onuCodigoError" tipo="Pkg_Epm_Utilidades.tyCodigoError" Direccion="OUT">
            Código de error
        </param>
        <param nombre="osbMensajeError" tipo="Pkg_Epm_Utilidades.tyMensajeError" Direccion="OUT">
            Mensaje de error
        </param>
    </Parametros>
    <Historial>
    <Modificacion Autor="[desarrollador]" Fecha="[fecha]" Inc="WOXXXX">
        Método creado por Cygnus.
    </Modificacion>
    </Historial>
    </Procedure>
    ***************************************************************************/
    PROCEDURE pProcesoMasivo[NOMBRE]
    (
        inuHilo         IN  NUMBER,
        inuHilos        IN  NUMBER,
        inuTotal        IN  NUMBER,
        isbPrograma     IN  estaprog.esprprog%type,
        [PARAMETROS]
        isbUsuario      IN  VARCHAR2,
        isbTerminal     IN  VARCHAR2,
        onuCodigoError  OUT Pkg_Epm_Utilidades.tyCodigoError,
        osbMensajeError OUT Pkg_Epm_Utilidades.tyMensajeError
    )
    IS
        csbMT_NAME              VARCHAR2(50)   := 'pProcesoMasivo[NOMBRE]';
        sbProgramaHilo          estaprog.esprprog%TYPE;
        nuPorcentaje            estaprog.esprporc%TYPE := 1;     
    BEGIN
        pkg_epm_utilidades.trace_setmsg (csbSP_NAME||csbMT_NAME, cnuNVLTRC, csbPUSH);

        --Programa del hilo a procesar
        sbProgramaHilo := isbPrograma||'-'||to_char(inuHilo);

        -- Inserta registro en estados de programa - ESTAPROG
        pkStatusExeProgramMgr.AddRecord (sbProgramaHilo,'Procesando ...',inuTotal);
        pkGeneralServices.CommitTransaction;

        /******************* START USER CODE ***********************/        
            --Se actualiza el proceso en estaprog
            nuPorcentaje := nuPorcentaje + 1;
            Pkstatusexeprogrammgr.Upstatusexeprogramat(sbProgramaHilo, 'Proceso en ejecucion...', inuTotal, nuPorcentaje);
            pkGeneralServices.committransaction;
       /******************** END USER CODE ************************/

        --Finaliza proceso
        Pkstatusexeprogrammgr.Upstatusexeprogramat(sbProgramaHilo, 'Proceso en ejecucion...', inuTotal, inuTotal);
        Pkstatusexeprogrammgr.Processfinishok(sbProgramaHilo, 'Proceso Finalizado');
        pkGeneralServices.CommitTransaction;

        pkg_epm_utilidades.trace_setmsg (csbSP_NAME||csbMT_NAME, cnuNVLTRC, csbPOP);
    EXCEPTION
        WHEN LOGIN_DENIED OR ex.CONTROLLED_ERROR OR pkConstante.exERROR_LEVEL2 THEN
            Pkg_Epm_Utilidades.Trace_SetMsg(csbSP_NAME||csbMT_NAME, cnuNVLTRC, csbPOP_ERC);
            Pkstatusexeprogrammgr.Processfinishnok(sbProgramaHilo, SQLERRM);
            Pkgeneralservices.Committransaction;
            RAISE;
        WHEN OTHERS THEN
            Pkg_Epm_Utilidades.Trace_SetMsg(csbSP_NAME||csbMT_NAME, cnuNVLTRC, csbPOP_ERR);
            Errors.SetError;
            Pkstatusexeprogrammgr.Processfinishnok(sbProgramaHilo, SQLERRM);
            Pkgeneralservices.Committransaction;
            RAISE ex.CONTROLLED_ERROR;
    END pProcesoMasivo[NOMBRE];

/*
<param nombre="isbParam" tipo="VARCHAR2" Direccion="IN">
            Parámetros adicionales
        </param>
*/    
