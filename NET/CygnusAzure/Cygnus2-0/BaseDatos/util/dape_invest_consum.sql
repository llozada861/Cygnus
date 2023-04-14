CREATE OR REPLACE PACKAGE DAPE_invest_consum
is 
	/* Cursor general para acceso por Llave Primaria */
	CURSOR cuRecord 
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type
	)
	IS
		SELECT PE_invest_consum.*,PE_invest_consum.rowid
		FROM PE_invest_consum
		WHERE  Invest_Consum_Id = inuInvest_Consum_Id;


	/* Cursor general para acceso por RowId */
	CURSOR cuRecordByRowId
	(
		irirowid in varchar2
	)
	IS
        SELECT PE_invest_consum.*,PE_invest_consum.rowid
		FROM PE_invest_consum
		WHERE 
			rowId = irirowid;


	/* Subtipos */
	subtype styPE_invest_consum  is  cuRecord%rowtype;
	type    tyRefCursor is  REF CURSOR;

	/*Tipos*/
	type tytbPE_invest_consum is table of styPE_invest_consum index by binary_integer;
	type tyrfRecords is ref cursor return styPE_invest_consum;

	/* Tipos referenciando al registro */
	type tytbInvest_Consum_Id is table of PE_invest_consum.Invest_Consum_Id%type index by binary_integer;
	type tytbProduct_Id is table of PE_invest_consum.Product_Id%type index by binary_integer;
	type tytbMeasurement_Element is table of PE_invest_consum.Measurement_Element%type index by binary_integer;
	type tytbConsumption_Type is table of PE_invest_consum.Consumption_Type%type index by binary_integer;
	type tytbConsumption_Period is table of PE_invest_consum.Consumption_Period%type index by binary_integer;
	type tytbMeasure_Point is table of PE_invest_consum.Measure_Point%type index by binary_integer;
	type tytbInvestigate_Consum is table of PE_invest_consum.Investigate_Consum%type index by binary_integer;
	type tytbBilled_Consum is table of PE_invest_consum.Billed_Consum%type index by binary_integer;
	type tytbInvestigate_Request is table of PE_invest_consum.Investigate_Request%type index by binary_integer;
	type tytbInvest_Cons_State_Id is table of PE_invest_consum.Invest_Cons_State_Id%type index by binary_integer;
	type tytbObservation is table of PE_invest_consum.Observation%type index by binary_integer;
	type tytbRegister_Date is table of PE_invest_consum.Register_Date%type index by binary_integer;
	type tytbrowid is table of rowid index by binary_integer;

	type tyrcPE_invest_consum is record
	(
		Invest_Consum_Id   tytbInvest_Consum_Id,
		Product_Id   tytbProduct_Id,
		Measurement_Element   tytbMeasurement_Element,
		Consumption_Type   tytbConsumption_Type,
		Consumption_Period   tytbConsumption_Period,
		Measure_Point   tytbMeasure_Point,
		Investigate_Consum   tytbInvestigate_Consum,
		Billed_Consum   tytbBilled_Consum,
		Investigate_Request   tytbInvestigate_Request,
		Invest_Cons_State_Id   tytbInvest_Cons_State_Id,
		Observation   tytbObservation,
		Register_Date   tytbRegister_Date,
		row_id tytbrowid
	);


	/***** Metodos Publicos ****/

    FUNCTION fsbVersion
    RETURN varchar2;

	FUNCTION fsbGetMessageDescription
	return varchar2;

	PROCEDURE ClearMemory;

	FUNCTION fblExist
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type
	)
	RETURN boolean;

	PROCEDURE AccKey
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type
	);

	PROCEDURE AccKeyByRowId
	(
		iriRowID    in rowid
	);

	PROCEDURE ValDuplicate
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type
	);

	PROCEDURE getRecord
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		orcRecord out nocopy styPE_invest_consum
	);

	FUNCTION frcGetRcData
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type
	)
	RETURN styPE_invest_consum;

	FUNCTION frcGetRcData
	RETURN styPE_invest_consum;

	FUNCTION frcGetRecord
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type
	)
	RETURN styPE_invest_consum;

	PROCEDURE getRecords 
	(
		isbQuery in varchar2,
		otbResult out nocopy tytbPE_invest_consum
	);

	FUNCTION frfGetRecords
	(
		isbCriteria in varchar2 default null,
		iblLock in boolean default false
	)
	RETURN tyRefCursor;

	PROCEDURE insRecord
	(
		ircPE_invest_consum in styPE_invest_consum
	);

	PROCEDURE insRecord
	(
		ircPE_invest_consum in styPE_invest_consum,
        orirowid   out varchar2
	);

	PROCEDURE insRecords
	(
		iotbPE_invest_consum in out nocopy tytbPE_invest_consum
	);

	PROCEDURE delRecord
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuLock in number default 1
	);

	PROCEDURE delByRowID
	(
		iriRowID    in rowid,
		inuLock in number default 1
	);

	PROCEDURE delRecords
	(
		iotbPE_invest_consum in out nocopy tytbPE_invest_consum,
		inuLock in number default 1
	);

	PROCEDURE updRecord
	(
		ircPE_invest_consum in styPE_invest_consum,
		inuLock in number default 0
	);

	PROCEDURE updRecords
	(
		iotbPE_invest_consum in out nocopy tytbPE_invest_consum,
		inuLock in number default 1
	);

	PROCEDURE updProduct_Id
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuProduct_Id$ in PE_invest_consum.Product_Id%type,
		inuLock in number default 0
	);

	PROCEDURE updMeasurement_Element
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuMeasurement_Element$ in PE_invest_consum.Measurement_Element%type,
		inuLock in number default 0
	);

	PROCEDURE updConsumption_Type
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuConsumption_Type$ in PE_invest_consum.Consumption_Type%type,
		inuLock in number default 0
	);

	PROCEDURE updConsumption_Period
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuConsumption_Period$ in PE_invest_consum.Consumption_Period%type,
		inuLock in number default 0
	);

	PROCEDURE updMeasure_Point
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuMeasure_Point$ in PE_invest_consum.Measure_Point%type,
		inuLock in number default 0
	);

	PROCEDURE updInvestigate_Consum
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuInvestigate_Consum$ in PE_invest_consum.Investigate_Consum%type,
		inuLock in number default 0
	);

	PROCEDURE updBilled_Consum
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuBilled_Consum$ in PE_invest_consum.Billed_Consum%type,
		inuLock in number default 0
	);

	PROCEDURE updInvestigate_Request
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuInvestigate_Request$ in PE_invest_consum.Investigate_Request%type,
		inuLock in number default 0
	);

	PROCEDURE updInvest_Cons_State_Id
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuInvest_Cons_State_Id$ in PE_invest_consum.Invest_Cons_State_Id%type,
		inuLock in number default 0
	);

	PROCEDURE updObservation
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		isbObservation$ in PE_invest_consum.Observation%type,
		inuLock in number default 0
	);

	PROCEDURE updRegister_Date
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		idtRegister_Date$ in PE_invest_consum.Register_Date%type,
		inuLock in number default 0
	);

	FUNCTION fnuGetInvest_Consum_Id
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuRaiseError in number default 1
	)
	RETURN PE_invest_consum.Invest_Consum_Id%type;

	FUNCTION fnuGetProduct_Id
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuRaiseError in number default 1
	)
	RETURN PE_invest_consum.Product_Id%type;

	FUNCTION fnuGetMeasurement_Element
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuRaiseError in number default 1
	)
	RETURN PE_invest_consum.Measurement_Element%type;

	FUNCTION fnuGetConsumption_Type
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuRaiseError in number default 1
	)
	RETURN PE_invest_consum.Consumption_Type%type;

	FUNCTION fnuGetConsumption_Period
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuRaiseError in number default 1
	)
	RETURN PE_invest_consum.Consumption_Period%type;

	FUNCTION fnuGetMeasure_Point
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuRaiseError in number default 1
	)
	RETURN PE_invest_consum.Measure_Point%type;

	FUNCTION fnuGetInvestigate_Consum
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuRaiseError in number default 1
	)
	RETURN PE_invest_consum.Investigate_Consum%type;

	FUNCTION fnuGetBilled_Consum
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuRaiseError in number default 1
	)
	RETURN PE_invest_consum.Billed_Consum%type;

	FUNCTION fnuGetInvestigate_Request
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuRaiseError in number default 1
	)
	RETURN PE_invest_consum.Investigate_Request%type;

	FUNCTION fnuGetInvest_Cons_State_Id
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuRaiseError in number default 1
	)
	RETURN PE_invest_consum.Invest_Cons_State_Id%type;

	FUNCTION fsbGetObservation
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuRaiseError in number default 1
	)
	RETURN PE_invest_consum.Observation%type;

	FUNCTION fdtGetRegister_Date
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuRaiseError in number default 1
	)
	RETURN PE_invest_consum.Register_Date%type;

	PROCEDURE LockByPk
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		orcPE_invest_consum  out styPE_invest_consum
	);

	PROCEDURE LockByRowID
	(
		irirowid    in  varchar2,
		orcPE_invest_consum  out styPE_invest_consum
	);

	PROCEDURE SetUseCache
	(
		iblUseCache    in  boolean
	);
END DAPE_invest_consum;
/
CREATE OR REPLACE PACKAGE BODY FLEX.DAPE_invest_consum
IS

    /*constantes locales al paquete*/
    cnuRECORD_NOT_EXIST constant number(1) := 1;
    cnuRECORD_ALREADY_EXIST constant number(1) := 2;
    cnuAPPTABLEBUSSY constant number(4) := 6951;
    cnuINS_PK_NULL constant number(4):= 1682;
    cnuRECORD_HAVE_CHILDREN constant number(4):= -2292;
    csbVersion   CONSTANT VARCHAR2(20) := 'SAO399230';
    csbTABLEPARAMETER   CONSTANT VARCHAR2(30) := 'PE_INVEST_CONSUM';

	/* Cursor para bloqueo de un registro por llave primaria */
	CURSOR cuLockRcByPk 
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type
	)
	IS
		SELECT PE_invest_consum.*,PE_invest_consum.rowid 
		FROM PE_invest_consum
		WHERE  Invest_Consum_Id = inuInvest_Consum_Id
		FOR UPDATE NOWAIT;

	/* Cursor para bloqueo de un registro por rowid */
	CURSOR cuLockRcbyRowId 
	(
		irirowid in varchar2
	)
	IS
		SELECT PE_invest_consum.*,PE_invest_consum.rowid 
		FROM PE_invest_consum
		WHERE 
			rowId = irirowid
		FOR UPDATE NOWAIT;


	/*Tipos*/
	type tyrfPE_invest_consum is ref cursor;

	/*Variables Globales*/
	rcRecOfTab tyrcPE_invest_consum;

	rcData cuRecord%rowtype;

    blDAO_USE_CACHE    boolean := null;


	/* Metodos privados */
    FUNCTION fsbGetMessageDescription
    return varchar2
    is
        CURSOR cuGetMessageDescription
        IS
        SELECT  ge_entity.display_name
        FROM    ge_entity
        WHERE   ge_entity.name_ = csbTABLEPARAMETER;

        sbTableDescription varchar2(32000);

    BEGIN
        OPEN    cuGetMessageDescription;
        FETCH   cuGetMessageDescription INTO sbTableDescription;
        CLOSE   cuGetMessageDescription;

        if (sbTableDescription IS NULL)  then
            sbTableDescription:= csbTABLEPARAMETER;
        end if;

        return sbTableDescription;
    END fsbGetMessageDescription;
	
	PROCEDURE GetDAO_USE_CACHE
	IS
	BEGIN
	    if ( blDAO_USE_CACHE is null ) then
	        blDAO_USE_CACHE :=  ge_boparameter.fsbget('DAO_USE_CACHE') = 'Y';
	    end if;
	END;
	FUNCTION fsbPrimaryKey( rcI in styPE_invest_consum default rcData )
	return varchar2
	IS
		sbPk varchar2(500);
	BEGIN
		sbPk:='[';
		sbPk:=sbPk||ut_convert.fsbToChar(rcI.Invest_Consum_Id);
		sbPk:=sbPk||']';
		return sbPk;
	END;
	PROCEDURE LockByPk
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		orcPE_invest_consum  out styPE_invest_consum
	)
	IS
		rcError styPE_invest_consum;
	BEGIN
		rcError.Invest_Consum_Id:=inuInvest_Consum_Id;

		Open cuLockRcByPk(inuInvest_Consum_Id);

		fetch cuLockRcByPk into orcPE_invest_consum;
		if cuLockRcByPk%notfound  then
			close cuLockRcByPk;
			raise no_data_found;
		end if;
		close cuLockRcByPk ;
	EXCEPTION
		when no_data_found then
			if cuLockRcByPk%isopen then
				close cuLockRcByPk;
			end if;
			Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '|| fsbPrimaryKey(rcError));
			raise ex.CONTROLLED_ERROR;
		when ex.RESOURCE_BUSY THEN
			if cuLockRcByPk%isopen then
				close cuLockRcByPk;
			end if;
			errors.setError(cnuAPPTABLEBUSSY,fsbPrimaryKey(rcError)||'|'|| fsbGetMessageDescription );
			raise ex.controlled_error;
		when others then
			if cuLockRcByPk%isopen then
				close cuLockRcByPk;
			end if;
			raise;
	END;
	PROCEDURE LockByRowID
	(
		irirowid    in  varchar2,
		orcPE_invest_consum  out styPE_invest_consum
	)
	IS
	BEGIN
		Open cuLockRcbyRowId
		(
			irirowid
		);

		fetch cuLockRcbyRowId into orcPE_invest_consum;
		if cuLockRcbyRowId%notfound  then
			close cuLockRcbyRowId;
			raise no_data_found;
		end if;
		close cuLockRcbyRowId;
	EXCEPTION
		when no_data_found then
			if cuLockRcbyRowId%isopen then
				close cuLockRcbyRowId;
			end if;
			Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' rowid=['||iriRowID||']');
			raise ex.CONTROLLED_ERROR;
		when ex.RESOURCE_BUSY THEN
			if cuLockRcbyRowId%isopen then
				close cuLockRcbyRowId;
			end if;
			errors.setError(cnuAPPTABLEBUSSY,'rowid=['||irirowid||']|'||fsbGetMessageDescription );
			raise ex.controlled_error;
		when others then
			if cuLockRcbyRowId%isopen then
				close cuLockRcbyRowId;
			end if;
			raise;
	END;
	PROCEDURE DelRecordOfTables
	(
		itbPE_invest_consum  in out nocopy tytbPE_invest_consum
	)
	IS
	BEGIN
			rcRecOfTab.Invest_Consum_Id.delete;
			rcRecOfTab.Product_Id.delete;
			rcRecOfTab.Measurement_Element.delete;
			rcRecOfTab.Consumption_Type.delete;
			rcRecOfTab.Consumption_Period.delete;
			rcRecOfTab.Measure_Point.delete;
			rcRecOfTab.Investigate_Consum.delete;
			rcRecOfTab.Billed_Consum.delete;
			rcRecOfTab.Investigate_Request.delete;
			rcRecOfTab.Invest_Cons_State_Id.delete;
			rcRecOfTab.Observation.delete;
			rcRecOfTab.Register_Date.delete;
			rcRecOfTab.row_id.delete;
	END;
	PROCEDURE FillRecordOfTables
	(
		itbPE_invest_consum  in out nocopy tytbPE_invest_consum,
		oblUseRowId out boolean
	)
	IS
	BEGIN
		DelRecordOfTables(itbPE_invest_consum);

		for n in itbPE_invest_consum.first .. itbPE_invest_consum.last loop
			rcRecOfTab.Invest_Consum_Id(n) := itbPE_invest_consum(n).Invest_Consum_Id;
			rcRecOfTab.Product_Id(n) := itbPE_invest_consum(n).Product_Id;
			rcRecOfTab.Measurement_Element(n) := itbPE_invest_consum(n).Measurement_Element;
			rcRecOfTab.Consumption_Type(n) := itbPE_invest_consum(n).Consumption_Type;
			rcRecOfTab.Consumption_Period(n) := itbPE_invest_consum(n).Consumption_Period;
			rcRecOfTab.Measure_Point(n) := itbPE_invest_consum(n).Measure_Point;
			rcRecOfTab.Investigate_Consum(n) := itbPE_invest_consum(n).Investigate_Consum;
			rcRecOfTab.Billed_Consum(n) := itbPE_invest_consum(n).Billed_Consum;
			rcRecOfTab.Investigate_Request(n) := itbPE_invest_consum(n).Investigate_Request;
			rcRecOfTab.Invest_Cons_State_Id(n) := itbPE_invest_consum(n).Invest_Cons_State_Id;
			rcRecOfTab.Observation(n) := itbPE_invest_consum(n).Observation;
			rcRecOfTab.Register_Date(n) := itbPE_invest_consum(n).Register_Date;
			rcRecOfTab.row_id(n) := itbPE_invest_consum(n).rowid;

			-- Indica si el rowid es Nulo
			oblUseRowId:=rcRecOfTab.row_id(n) is NOT NULL;
		end loop;
	END;

	PROCEDURE Load
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type
	)
	IS
		rcRecordNull cuRecord%rowtype;
	BEGIN
		if cuRecord%isopen then
			close cuRecord;
		end if;
		open cuRecord(inuInvest_Consum_Id);

		fetch cuRecord into rcData;
		if cuRecord%notfound  then
			close cuRecord;
			rcData := rcRecordNull;
			raise no_data_found;
		end if;
		close cuRecord;
	END;
	PROCEDURE LoadByRowId
	(
		irirowid in varchar2
	)
	IS
		rcRecordNull cuRecordByRowId%rowtype;
	BEGIN
		if cuRecordByRowId%isopen then
			close cuRecordByRowId;
		end if;
		open cuRecordByRowId(irirowid);

		fetch cuRecordByRowId into rcData;
		if cuRecordByRowId%notfound  then
			close cuRecordByRowId;
			rcData := rcRecordNull;
			raise no_data_found;
		end if;
		close cuRecordByRowId;
	END;
	FUNCTION fblAlreadyLoaded
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type
	)
	RETURN boolean
	IS
	BEGIN
		if (
			inuInvest_Consum_Id = rcData.Invest_Consum_Id
		   ) then
			return ( true );
		end if;
		return (false);
	END;

	/***** Fin metodos privados *****/

	/***** Metodos publicos ******/
    FUNCTION fsbVersion
    RETURN varchar2
	IS
	BEGIN
		return csbVersion;
	END;

	PROCEDURE ClearMemory
	IS
		rcRecordNull cuRecord%rowtype;
	BEGIN
		rcData := rcRecordNull;
	END;
	FUNCTION fblExist
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type
	)
	RETURN boolean
	IS
	BEGIN
		Load(inuInvest_Consum_Id);
		return(TRUE);
	EXCEPTION
		when no_data_found then
			return(FALSE);
	END;
	PROCEDURE AccKey
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type
	)
	IS
		rcError styPE_invest_consum;
	BEGIN
		rcError.Invest_Consum_Id:=inuInvest_Consum_Id;

		Load(inuInvest_Consum_Id);
	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
			raise ex.CONTROLLED_ERROR;
	END;
	PROCEDURE AccKeyByRowId
	(
		iriRowID    in rowid
	)
	IS
	BEGIN
		LoadByRowId(iriRowID);
	EXCEPTION
		when no_data_found then
            Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' rowid=['||iriRowID||']');
            raise ex.CONTROLLED_ERROR;
	END;
	PROCEDURE ValDuplicate
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type
	)
	IS
	BEGIN
		Load(inuInvest_Consum_Id);
		Errors.setError(cnuRECORD_ALREADY_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey);
		raise ex.CONTROLLED_ERROR;

	EXCEPTION
		when no_data_found then
			null;
	END;
	PROCEDURE getRecord
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		orcRecord out nocopy styPE_invest_consum
	)
	IS
		rcError styPE_invest_consum;
	BEGIN
		rcError.Invest_Consum_Id:=inuInvest_Consum_Id;

		Load(inuInvest_Consum_Id);
		orcRecord := rcData;
	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '|| fsbPrimaryKey(rcError));
			raise ex.CONTROLLED_ERROR;
	END;
	FUNCTION frcGetRecord
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type
	)
	RETURN styPE_invest_consum
	IS
		rcError styPE_invest_consum;
	BEGIN
		rcError.Invest_Consum_Id:=inuInvest_Consum_Id;

		Load(inuInvest_Consum_Id);
		return(rcData);
	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '|| fsbPrimaryKey(rcError));
			raise ex.CONTROLLED_ERROR;
	END;
	FUNCTION frcGetRcData
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type
	)
	RETURN styPE_invest_consum
	IS
		rcError styPE_invest_consum;
	BEGIN
		rcError.Invest_Consum_Id:=inuInvest_Consum_Id;
        -- si usa cache y esta cargado retorna
		if  blDAO_USE_CACHE AND fblAlreadyLoaded(inuInvest_Consum_Id) then
			 return(rcData);
		end if;
		Load(inuInvest_Consum_Id);
		return(rcData);
	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '|| fsbPrimaryKey(rcError));
			raise ex.CONTROLLED_ERROR;
	END;
	FUNCTION frcGetRcData
	RETURN styPE_invest_consum
	IS
	BEGIN
		return(rcData);
	END;
	PROCEDURE getRecords
	(
		isbQuery in varchar2,
		otbResult out nocopy tytbPE_invest_consum
	)
	IS
		rfPE_invest_consum tyrfPE_invest_consum;
		n number(4) := 1;
		sbFullQuery VARCHAR2 (32000) := 'SELECT PE_invest_consum.*, PE_invest_consum.rowid FROM PE_invest_consum';
		nuMaxTbRecords number(5):=ge_boparameter.fnuget('MAXREGSQUERY');
	BEGIN
		otbResult.delete;
		if isbQuery is not NULL and length(isbQuery) > 0 then
			sbFullQuery := sbFullQuery||' WHERE '||isbQuery;
		end if;

		open rfPE_invest_consum for sbFullQuery;

		fetch rfPE_invest_consum bulk collect INTO otbResult;

		close rfPE_invest_consum;
		if otbResult.count = 0  then
			raise no_data_found;
		end if;

	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription);
			raise ex.CONTROLLED_ERROR;
	END;
	FUNCTION frfGetRecords
	(
		isbCriteria in varchar2 default null,
		iblLock in boolean default false
	)
	RETURN tyRefCursor
	IS
		rfQuery tyRefCursor;
		sbSQL VARCHAR2 (32000) := 'select PE_invest_consum.*, PE_invest_consum.rowid FROM PE_invest_consum';
	BEGIN
		if isbCriteria is not null then
			sbSQL := sbSQL||' where '||isbCriteria;
		end if;
		if iblLock then
			sbSQL := sbSQL||' for update nowait';
		end if;
		open rfQuery for sbSQL;
		return(rfQuery);
	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription);
			raise ex.CONTROLLED_ERROR;
	END;
	PROCEDURE insRecord
	(
		ircPE_invest_consum in styPE_invest_consum
	)
	IS
		rirowid varchar2(200);
	BEGIN
		insRecord(ircPE_invest_consum,rirowid);
	END;
	PROCEDURE insRecord
	(
		ircPE_invest_consum in styPE_invest_consum,
        orirowid   out varchar2
	)
	IS
	BEGIN
		if ircPE_invest_consum.Invest_Consum_Id is NULL then
			Errors.SetError(cnuINS_PK_NULL,
			                fsbGetMessageDescription||'|Invest_Consum_Id');
			raise ex.controlled_error;
		end if;

		insert into PE_invest_consum
		(
			Invest_Consum_Id,
			Product_Id,
			Measurement_Element,
			Consumption_Type,
			Consumption_Period,
			Measure_Point,
			Investigate_Consum,
			Billed_Consum,
			Investigate_Request,
			Invest_Cons_State_Id,
			Observation,
			Register_Date
		)
		values
		(
			ircPE_invest_consum.Invest_Consum_Id,
			ircPE_invest_consum.Product_Id,
			ircPE_invest_consum.Measurement_Element,
			ircPE_invest_consum.Consumption_Type,
			ircPE_invest_consum.Consumption_Period,
			ircPE_invest_consum.Measure_Point,
			ircPE_invest_consum.Investigate_Consum,
			ircPE_invest_consum.Billed_Consum,
			ircPE_invest_consum.Investigate_Request,
			ircPE_invest_consum.Invest_Cons_State_Id,
			ircPE_invest_consum.Observation,
			ircPE_invest_consum.Register_Date
		)
            returning
			rowid
		into
			orirowid;
		ClearMemory;
	EXCEPTION
		when dup_val_on_index then
			Errors.setError(cnuRECORD_ALREADY_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(ircPE_invest_consum));
			raise ex.CONTROLLED_ERROR;
	END;
	PROCEDURE insRecords
	(
		iotbPE_invest_consum in out nocopy tytbPE_invest_consum
	)
	IS
		blUseRowID boolean;
	BEGIN
		FillRecordOfTables(iotbPE_invest_consum,blUseRowID);
		forall n in iotbPE_invest_consum.first..iotbPE_invest_consum.last
			insert into PE_invest_consum
			(
				Invest_Consum_Id,
				Product_Id,
				Measurement_Element,
				Consumption_Type,
				Consumption_Period,
				Measure_Point,
				Investigate_Consum,
				Billed_Consum,
				Investigate_Request,
				Invest_Cons_State_Id,
				Observation,
				Register_Date
			)
			values
			(
				rcRecOfTab.Invest_Consum_Id(n),
				rcRecOfTab.Product_Id(n),
				rcRecOfTab.Measurement_Element(n),
				rcRecOfTab.Consumption_Type(n),
				rcRecOfTab.Consumption_Period(n),
				rcRecOfTab.Measure_Point(n),
				rcRecOfTab.Investigate_Consum(n),
				rcRecOfTab.Billed_Consum(n),
				rcRecOfTab.Investigate_Request(n),
				rcRecOfTab.Invest_Cons_State_Id(n),
				rcRecOfTab.Observation(n),
				rcRecOfTab.Register_Date(n)
			);
		ClearMemory;
	EXCEPTION
		when dup_val_on_index then
			Errors.setError(cnuRECORD_ALREADY_EXIST,fsbGetMessageDescription);
			raise ex.CONTROLLED_ERROR;
	END;
	PROCEDURE delRecord
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuLock in number default 1
	)
	IS
		rcError styPE_invest_consum;
	BEGIN
		rcError.Invest_Consum_Id := inuInvest_Consum_Id;

		if inuLock=1 then
			LockByPk
			(
				inuInvest_Consum_Id,
				rcData
			);
		end if;


		delete
		from PE_invest_consum
		where
       		Invest_Consum_Id=inuInvest_Consum_Id;
            if sql%notfound then
                raise no_data_found;
            end if;
		ClearMemory;
	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
         raise ex.CONTROLLED_ERROR;
		when ex.RECORD_HAVE_CHILDREN then
			Errors.setError(cnuRECORD_HAVE_CHILDREN,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
			raise ex.CONTROLLED_ERROR;
	END;
	PROCEDURE delByRowID
	(
		iriRowID    in rowid,
		inuLock in number default 1
	)
	IS
		rcRecordNull cuRecord%rowtype;
		rcError  styPE_invest_consum;
	BEGIN
		if inuLock=1 then
			LockByRowId(iriRowID,rcData);
		end if;


		delete
		from PE_invest_consum
		where
			rowid = iriRowID
		returning
			Invest_Consum_Id
		into
			rcError.Invest_Consum_Id;
            if sql%notfound then
			 raise no_data_found;
		    end if;
            if rcData.rowID=iriRowID then
			 rcData := rcRecordNull;
		    end if;
		ClearMemory;
	EXCEPTION
		when no_data_found then
            Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' rowid=['||iriRowID||']');
            raise ex.CONTROLLED_ERROR;
		when ex.RECORD_HAVE_CHILDREN then
            Errors.setError(cnuRECORD_HAVE_CHILDREN,fsbGetMessageDescription||' '||' rowid=['||iriRowID||']');
            raise ex.CONTROLLED_ERROR;
	END;
	PROCEDURE delRecords
	(
		iotbPE_invest_consum in out nocopy tytbPE_invest_consum,
		inuLock in number default 1
	)
	IS
		blUseRowID boolean;
		rcAux styPE_invest_consum;
	BEGIN
		FillRecordOfTables(iotbPE_invest_consum, blUseRowID);
        if ( blUseRowId ) then
			if inuLock = 1 then
				for n in iotbPE_invest_consum.first .. iotbPE_invest_consum.last loop
					LockByRowId
					(
						rcRecOfTab.row_id(n),
						rcAux
					);
				end loop;
			end if;

			forall n in iotbPE_invest_consum.first .. iotbPE_invest_consum.last
				delete
				from PE_invest_consum
				where
					rowid = rcRecOfTab.row_id(n);
		else
			if inuLock = 1 then
				for n in iotbPE_invest_consum.first .. iotbPE_invest_consum.last loop
					LockByPk
					(
						rcRecOfTab.Invest_Consum_Id(n),
						rcAux
					);
				end loop;
			end if;

			forall n in iotbPE_invest_consum.first .. iotbPE_invest_consum.last
				delete
				from PE_invest_consum
				where
		         	Invest_Consum_Id = rcRecOfTab.Invest_Consum_Id(n);
		end if;
		ClearMemory;
	EXCEPTION
            when ex.RECORD_HAVE_CHILDREN then
                  Errors.setError(cnuRECORD_HAVE_CHILDREN,fsbGetMessageDescription);
                  raise ex.CONTROLLED_ERROR;
	END;
	PROCEDURE updRecord
	(
		ircPE_invest_consum in styPE_invest_consum,
		inuLock in number default 0
	)
	IS
		nuInvest_Consum_Id	PE_invest_consum.Invest_Consum_Id%type;
	BEGIN
		if ircPE_invest_consum.rowid is not null then
			if inuLock=1 then
				LockByRowId(ircPE_invest_consum.rowid,rcData);
			end if;
			update PE_invest_consum
			set
				Product_Id = ircPE_invest_consum.Product_Id,
				Measurement_Element = ircPE_invest_consum.Measurement_Element,
				Consumption_Type = ircPE_invest_consum.Consumption_Type,
				Consumption_Period = ircPE_invest_consum.Consumption_Period,
				Measure_Point = ircPE_invest_consum.Measure_Point,
				Investigate_Consum = ircPE_invest_consum.Investigate_Consum,
				Billed_Consum = ircPE_invest_consum.Billed_Consum,
				Investigate_Request = ircPE_invest_consum.Investigate_Request,
				Invest_Cons_State_Id = ircPE_invest_consum.Invest_Cons_State_Id,
				Observation = ircPE_invest_consum.Observation,
				Register_Date = ircPE_invest_consum.Register_Date
			where
				rowid = ircPE_invest_consum.rowid
			returning
				Invest_Consum_Id
			into
				nuInvest_Consum_Id;
		else
			if inuLock=1 then
				LockByPk
				(
					ircPE_invest_consum.Invest_Consum_Id,
					rcData
				);
			end if;

			update PE_invest_consum
			set
				Product_Id = ircPE_invest_consum.Product_Id,
				Measurement_Element = ircPE_invest_consum.Measurement_Element,
				Consumption_Type = ircPE_invest_consum.Consumption_Type,
				Consumption_Period = ircPE_invest_consum.Consumption_Period,
				Measure_Point = ircPE_invest_consum.Measure_Point,
				Investigate_Consum = ircPE_invest_consum.Investigate_Consum,
				Billed_Consum = ircPE_invest_consum.Billed_Consum,
				Investigate_Request = ircPE_invest_consum.Investigate_Request,
				Invest_Cons_State_Id = ircPE_invest_consum.Invest_Cons_State_Id,
				Observation = ircPE_invest_consum.Observation,
				Register_Date = ircPE_invest_consum.Register_Date
			where
				Invest_Consum_Id = ircPE_invest_consum.Invest_Consum_Id
			returning
				Invest_Consum_Id
			into
				nuInvest_Consum_Id;
		end if;
		if
			nuInvest_Consum_Id is NULL
		then
			raise no_data_found;
		end if;
		ClearMemory;
	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||fsbPrimaryKey(ircPE_invest_consum));
			raise ex.CONTROLLED_ERROR;
	END;
	PROCEDURE updRecords
	(
		iotbPE_invest_consum in out nocopy tytbPE_invest_consum,
		inuLock in number default 1
	)
	IS
		blUseRowID boolean;    
		rcAux styPE_invest_consum;
	BEGIN
		FillRecordOfTables(iotbPE_invest_consum,blUseRowID);
		if blUseRowID then
			if inuLock = 1 then
				for n in iotbPE_invest_consum.first .. iotbPE_invest_consum.last loop
					LockByRowId
					(
						rcRecOfTab.row_id(n),
						rcAux
					);
				end loop;
			end if;

			forall n in iotbPE_invest_consum.first .. iotbPE_invest_consum.last
				update PE_invest_consum
				set
					Product_Id = rcRecOfTab.Product_Id(n),
					Measurement_Element = rcRecOfTab.Measurement_Element(n),
					Consumption_Type = rcRecOfTab.Consumption_Type(n),
					Consumption_Period = rcRecOfTab.Consumption_Period(n),
					Measure_Point = rcRecOfTab.Measure_Point(n),
					Investigate_Consum = rcRecOfTab.Investigate_Consum(n),
					Billed_Consum = rcRecOfTab.Billed_Consum(n),
					Investigate_Request = rcRecOfTab.Investigate_Request(n),
					Invest_Cons_State_Id = rcRecOfTab.Invest_Cons_State_Id(n),
					Observation = rcRecOfTab.Observation(n),
					Register_Date = rcRecOfTab.Register_Date(n)
				where
					rowid =  rcRecOfTab.row_id(n);
		else
			if inuLock = 1 then
				for n in iotbPE_invest_consum.first .. iotbPE_invest_consum.last loop
					LockByPk
					(
						rcRecOfTab.Invest_Consum_Id(n),
						rcAux
					);
				end loop;
			end if;

			forall n in iotbPE_invest_consum.first .. iotbPE_invest_consum.last
				update PE_invest_consum
				SET
					Product_Id = rcRecOfTab.Product_Id(n),
					Measurement_Element = rcRecOfTab.Measurement_Element(n),
					Consumption_Type = rcRecOfTab.Consumption_Type(n),
					Consumption_Period = rcRecOfTab.Consumption_Period(n),
					Measure_Point = rcRecOfTab.Measure_Point(n),
					Investigate_Consum = rcRecOfTab.Investigate_Consum(n),
					Billed_Consum = rcRecOfTab.Billed_Consum(n),
					Investigate_Request = rcRecOfTab.Investigate_Request(n),
					Invest_Cons_State_Id = rcRecOfTab.Invest_Cons_State_Id(n),
					Observation = rcRecOfTab.Observation(n),
					Register_Date = rcRecOfTab.Register_Date(n)
				where
					Invest_Consum_Id = rcRecOfTab.Invest_Consum_Id(n)
;
		end if;
		ClearMemory;
	END;
	PROCEDURE updProduct_Id
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuProduct_Id$ in PE_invest_consum.Product_Id%type,
		inuLock in number default 0
	)
	IS
		rcError styPE_invest_consum;
	BEGIN
		rcError.Invest_Consum_Id := inuInvest_Consum_Id;
		if inuLock=1 then
			LockByPk
			(
				inuInvest_Consum_Id,
				rcData
			);
		end if;

		update PE_invest_consum
		set
			Product_Id = inuProduct_Id$
		where
			Invest_Consum_Id = inuInvest_Consum_Id;

		if sql%notfound then
			raise no_data_found;
		end if;

		rcData.Product_Id:= inuProduct_Id$;
		ClearMemory;

	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
			raise ex.CONTROLLED_ERROR;
	END;
	PROCEDURE updMeasurement_Element
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuMeasurement_Element$ in PE_invest_consum.Measurement_Element%type,
		inuLock in number default 0
	)
	IS
		rcError styPE_invest_consum;
	BEGIN
		rcError.Invest_Consum_Id := inuInvest_Consum_Id;
		if inuLock=1 then
			LockByPk
			(
				inuInvest_Consum_Id,
				rcData
			);
		end if;

		update PE_invest_consum
		set
			Measurement_Element = inuMeasurement_Element$
		where
			Invest_Consum_Id = inuInvest_Consum_Id;

		if sql%notfound then
			raise no_data_found;
		end if;

		rcData.Measurement_Element:= inuMeasurement_Element$;
		ClearMemory;

	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
			raise ex.CONTROLLED_ERROR;
	END;
	PROCEDURE updConsumption_Type
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuConsumption_Type$ in PE_invest_consum.Consumption_Type%type,
		inuLock in number default 0
	)
	IS
		rcError styPE_invest_consum;
	BEGIN
		rcError.Invest_Consum_Id := inuInvest_Consum_Id;
		if inuLock=1 then
			LockByPk
			(
				inuInvest_Consum_Id,
				rcData
			);
		end if;

		update PE_invest_consum
		set
			Consumption_Type = inuConsumption_Type$
		where
			Invest_Consum_Id = inuInvest_Consum_Id;

		if sql%notfound then
			raise no_data_found;
		end if;

		rcData.Consumption_Type:= inuConsumption_Type$;
		ClearMemory;

	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
			raise ex.CONTROLLED_ERROR;
	END;
	PROCEDURE updConsumption_Period
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuConsumption_Period$ in PE_invest_consum.Consumption_Period%type,
		inuLock in number default 0
	)
	IS
		rcError styPE_invest_consum;
	BEGIN
		rcError.Invest_Consum_Id := inuInvest_Consum_Id;
		if inuLock=1 then
			LockByPk
			(
				inuInvest_Consum_Id,
				rcData
			);
		end if;

		update PE_invest_consum
		set
			Consumption_Period = inuConsumption_Period$
		where
			Invest_Consum_Id = inuInvest_Consum_Id;

		if sql%notfound then
			raise no_data_found;
		end if;

		rcData.Consumption_Period:= inuConsumption_Period$;
		ClearMemory;

	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
			raise ex.CONTROLLED_ERROR;
	END;
	PROCEDURE updMeasure_Point
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuMeasure_Point$ in PE_invest_consum.Measure_Point%type,
		inuLock in number default 0
	)
	IS
		rcError styPE_invest_consum;
	BEGIN
		rcError.Invest_Consum_Id := inuInvest_Consum_Id;
		if inuLock=1 then
			LockByPk
			(
				inuInvest_Consum_Id,
				rcData
			);
		end if;

		update PE_invest_consum
		set
			Measure_Point = inuMeasure_Point$
		where
			Invest_Consum_Id = inuInvest_Consum_Id;

		if sql%notfound then
			raise no_data_found;
		end if;

		rcData.Measure_Point:= inuMeasure_Point$;
		ClearMemory;

	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
			raise ex.CONTROLLED_ERROR;
	END;
	PROCEDURE updInvestigate_Consum
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuInvestigate_Consum$ in PE_invest_consum.Investigate_Consum%type,
		inuLock in number default 0
	)
	IS
		rcError styPE_invest_consum;
	BEGIN
		rcError.Invest_Consum_Id := inuInvest_Consum_Id;
		if inuLock=1 then
			LockByPk
			(
				inuInvest_Consum_Id,
				rcData
			);
		end if;

		update PE_invest_consum
		set
			Investigate_Consum = inuInvestigate_Consum$
		where
			Invest_Consum_Id = inuInvest_Consum_Id;

		if sql%notfound then
			raise no_data_found;
		end if;

		rcData.Investigate_Consum:= inuInvestigate_Consum$;
		ClearMemory;

	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
			raise ex.CONTROLLED_ERROR;
	END;
	PROCEDURE updBilled_Consum
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuBilled_Consum$ in PE_invest_consum.Billed_Consum%type,
		inuLock in number default 0
	)
	IS
		rcError styPE_invest_consum;
	BEGIN
		rcError.Invest_Consum_Id := inuInvest_Consum_Id;
		if inuLock=1 then
			LockByPk
			(
				inuInvest_Consum_Id,
				rcData
			);
		end if;

		update PE_invest_consum
		set
			Billed_Consum = inuBilled_Consum$
		where
			Invest_Consum_Id = inuInvest_Consum_Id;

		if sql%notfound then
			raise no_data_found;
		end if;

		rcData.Billed_Consum:= inuBilled_Consum$;
		ClearMemory;

	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
			raise ex.CONTROLLED_ERROR;
	END;
	PROCEDURE updInvestigate_Request
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuInvestigate_Request$ in PE_invest_consum.Investigate_Request%type,
		inuLock in number default 0
	)
	IS
		rcError styPE_invest_consum;
	BEGIN
		rcError.Invest_Consum_Id := inuInvest_Consum_Id;
		if inuLock=1 then
			LockByPk
			(
				inuInvest_Consum_Id,
				rcData
			);
		end if;

		update PE_invest_consum
		set
			Investigate_Request = inuInvestigate_Request$
		where
			Invest_Consum_Id = inuInvest_Consum_Id;

		if sql%notfound then
			raise no_data_found;
		end if;

		rcData.Investigate_Request:= inuInvestigate_Request$;
		ClearMemory;

	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
			raise ex.CONTROLLED_ERROR;
	END;
	PROCEDURE updInvest_Cons_State_Id
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuInvest_Cons_State_Id$ in PE_invest_consum.Invest_Cons_State_Id%type,
		inuLock in number default 0
	)
	IS
		rcError styPE_invest_consum;
	BEGIN
		rcError.Invest_Consum_Id := inuInvest_Consum_Id;
		if inuLock=1 then
			LockByPk
			(
				inuInvest_Consum_Id,
				rcData
			);
		end if;

		update PE_invest_consum
		set
			Invest_Cons_State_Id = inuInvest_Cons_State_Id$
		where
			Invest_Consum_Id = inuInvest_Consum_Id;

		if sql%notfound then
			raise no_data_found;
		end if;

		rcData.Invest_Cons_State_Id:= inuInvest_Cons_State_Id$;
		ClearMemory;

	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
			raise ex.CONTROLLED_ERROR;
	END;
	PROCEDURE updObservation
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		isbObservation$ in PE_invest_consum.Observation%type,
		inuLock in number default 0
	)
	IS
		rcError styPE_invest_consum;
	BEGIN
		rcError.Invest_Consum_Id := inuInvest_Consum_Id;
		if inuLock=1 then
			LockByPk
			(
				inuInvest_Consum_Id,
				rcData
			);
		end if;

		update PE_invest_consum
		set
			Observation = isbObservation$
		where
			Invest_Consum_Id = inuInvest_Consum_Id;

		if sql%notfound then
			raise no_data_found;
		end if;

		rcData.Observation:= isbObservation$;
		ClearMemory;

	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
			raise ex.CONTROLLED_ERROR;
	END;
	PROCEDURE updRegister_Date
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		idtRegister_Date$ in PE_invest_consum.Register_Date%type,
		inuLock in number default 0
	)
	IS
		rcError styPE_invest_consum;
	BEGIN
		rcError.Invest_Consum_Id := inuInvest_Consum_Id;
		if inuLock=1 then
			LockByPk
			(
				inuInvest_Consum_Id,
				rcData
			);
		end if;

		update PE_invest_consum
		set
			Register_Date = idtRegister_Date$
		where
			Invest_Consum_Id = inuInvest_Consum_Id;

		if sql%notfound then
			raise no_data_found;
		end if;

		rcData.Register_Date:= idtRegister_Date$;
		ClearMemory;

	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
			raise ex.CONTROLLED_ERROR;
	END;
	FUNCTION fnuGetInvest_Consum_Id
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuRaiseError in number default 1
	)
	RETURN PE_invest_consum.Invest_Consum_Id%type
	IS
		rcError styPE_invest_consum;
	BEGIN

		rcError.Invest_Consum_Id := inuInvest_Consum_Id;

        -- si usa cache y esta cargado retorna
		if  blDAO_USE_CACHE AND fblAlreadyLoaded(inuInvest_Consum_Id) then
			 return(rcData.Invest_Consum_Id);
		end if;
		Load(inuInvest_Consum_Id);
		return(rcData.Invest_Consum_Id);
	EXCEPTION
		when no_data_found then
			if inuRaiseError = 1 then
				Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
				raise ex.CONTROLLED_ERROR;
			else
				return null;
			end if;
	END;
	FUNCTION fnuGetProduct_Id
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuRaiseError in number default 1
	)
	RETURN PE_invest_consum.Product_Id%type
	IS
		rcError styPE_invest_consum;
	BEGIN

		rcError.Invest_Consum_Id := inuInvest_Consum_Id;

        -- si usa cache y esta cargado retorna
		if  blDAO_USE_CACHE AND fblAlreadyLoaded(inuInvest_Consum_Id) then
			 return(rcData.Product_Id);
		end if;
		Load(inuInvest_Consum_Id);
		return(rcData.Product_Id);
	EXCEPTION
		when no_data_found then
			if inuRaiseError = 1 then
				Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
				raise ex.CONTROLLED_ERROR;
			else
				return null;
			end if;
	END;
	FUNCTION fnuGetMeasurement_Element
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuRaiseError in number default 1
	)
	RETURN PE_invest_consum.Measurement_Element%type
	IS
		rcError styPE_invest_consum;
	BEGIN

		rcError.Invest_Consum_Id := inuInvest_Consum_Id;

        -- si usa cache y esta cargado retorna
		if  blDAO_USE_CACHE AND fblAlreadyLoaded(inuInvest_Consum_Id) then
			 return(rcData.Measurement_Element);
		end if;
		Load(inuInvest_Consum_Id);
		return(rcData.Measurement_Element);
	EXCEPTION
		when no_data_found then
			if inuRaiseError = 1 then
				Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
				raise ex.CONTROLLED_ERROR;
			else
				return null;
			end if;
	END;
	FUNCTION fnuGetConsumption_Type
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuRaiseError in number default 1
	)
	RETURN PE_invest_consum.Consumption_Type%type
	IS
		rcError styPE_invest_consum;
	BEGIN

		rcError.Invest_Consum_Id := inuInvest_Consum_Id;

        -- si usa cache y esta cargado retorna
		if  blDAO_USE_CACHE AND fblAlreadyLoaded(inuInvest_Consum_Id) then
			 return(rcData.Consumption_Type);
		end if;
		Load(inuInvest_Consum_Id);
		return(rcData.Consumption_Type);
	EXCEPTION
		when no_data_found then
			if inuRaiseError = 1 then
				Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
				raise ex.CONTROLLED_ERROR;
			else
				return null;
			end if;
	END;
	FUNCTION fnuGetConsumption_Period
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuRaiseError in number default 1
	)
	RETURN PE_invest_consum.Consumption_Period%type
	IS
		rcError styPE_invest_consum;
	BEGIN

		rcError.Invest_Consum_Id := inuInvest_Consum_Id;

        -- si usa cache y esta cargado retorna
		if  blDAO_USE_CACHE AND fblAlreadyLoaded(inuInvest_Consum_Id) then
			 return(rcData.Consumption_Period);
		end if;
		Load(inuInvest_Consum_Id);
		return(rcData.Consumption_Period);
	EXCEPTION
		when no_data_found then
			if inuRaiseError = 1 then
				Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
				raise ex.CONTROLLED_ERROR;
			else
				return null;
			end if;
	END;
	FUNCTION fnuGetMeasure_Point
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuRaiseError in number default 1
	)
	RETURN PE_invest_consum.Measure_Point%type
	IS
		rcError styPE_invest_consum;
	BEGIN

		rcError.Invest_Consum_Id := inuInvest_Consum_Id;

        -- si usa cache y esta cargado retorna
		if  blDAO_USE_CACHE AND fblAlreadyLoaded(inuInvest_Consum_Id) then
			 return(rcData.Measure_Point);
		end if;
		Load(inuInvest_Consum_Id);
		return(rcData.Measure_Point);
	EXCEPTION
		when no_data_found then
			if inuRaiseError = 1 then
				Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
				raise ex.CONTROLLED_ERROR;
			else
				return null;
			end if;
	END;
	FUNCTION fnuGetInvestigate_Consum
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuRaiseError in number default 1
	)
	RETURN PE_invest_consum.Investigate_Consum%type
	IS
		rcError styPE_invest_consum;
	BEGIN

		rcError.Invest_Consum_Id := inuInvest_Consum_Id;

        -- si usa cache y esta cargado retorna
		if  blDAO_USE_CACHE AND fblAlreadyLoaded(inuInvest_Consum_Id) then
			 return(rcData.Investigate_Consum);
		end if;
		Load(inuInvest_Consum_Id);
		return(rcData.Investigate_Consum);
	EXCEPTION
		when no_data_found then
			if inuRaiseError = 1 then
				Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
				raise ex.CONTROLLED_ERROR;
			else
				return null;
			end if;
	END;
	FUNCTION fnuGetBilled_Consum
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuRaiseError in number default 1
	)
	RETURN PE_invest_consum.Billed_Consum%type
	IS
		rcError styPE_invest_consum;
	BEGIN

		rcError.Invest_Consum_Id := inuInvest_Consum_Id;

        -- si usa cache y esta cargado retorna
		if  blDAO_USE_CACHE AND fblAlreadyLoaded(inuInvest_Consum_Id) then
			 return(rcData.Billed_Consum);
		end if;
		Load(inuInvest_Consum_Id);
		return(rcData.Billed_Consum);
	EXCEPTION
		when no_data_found then
			if inuRaiseError = 1 then
				Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
				raise ex.CONTROLLED_ERROR;
			else
				return null;
			end if;
	END;
	FUNCTION fnuGetInvestigate_Request
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuRaiseError in number default 1
	)
	RETURN PE_invest_consum.Investigate_Request%type
	IS
		rcError styPE_invest_consum;
	BEGIN

		rcError.Invest_Consum_Id := inuInvest_Consum_Id;

        -- si usa cache y esta cargado retorna
		if  blDAO_USE_CACHE AND fblAlreadyLoaded(inuInvest_Consum_Id) then
			 return(rcData.Investigate_Request);
		end if;
		Load(inuInvest_Consum_Id);
		return(rcData.Investigate_Request);
	EXCEPTION
		when no_data_found then
			if inuRaiseError = 1 then
				Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
				raise ex.CONTROLLED_ERROR;
			else
				return null;
			end if;
	END;
	FUNCTION fnuGetInvest_Cons_State_Id
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuRaiseError in number default 1
	)
	RETURN PE_invest_consum.Invest_Cons_State_Id%type
	IS
		rcError styPE_invest_consum;
	BEGIN

		rcError.Invest_Consum_Id := inuInvest_Consum_Id;

        -- si usa cache y esta cargado retorna
		if  blDAO_USE_CACHE AND fblAlreadyLoaded(inuInvest_Consum_Id) then
			 return(rcData.Invest_Cons_State_Id);
		end if;
		Load(inuInvest_Consum_Id);
		return(rcData.Invest_Cons_State_Id);
	EXCEPTION
		when no_data_found then
			if inuRaiseError = 1 then
				Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
				raise ex.CONTROLLED_ERROR;
			else
				return null;
			end if;
	END;
	FUNCTION fsbGetObservation
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuRaiseError in number default 1
	)
	RETURN PE_invest_consum.Observation%type
	IS
		rcError styPE_invest_consum;
	BEGIN

		rcError.Invest_Consum_Id := inuInvest_Consum_Id;

        -- si usa cache y esta cargado retorna
		if  blDAO_USE_CACHE AND fblAlreadyLoaded(inuInvest_Consum_Id) then
			 return(rcData.Observation);
		end if;
		Load(inuInvest_Consum_Id);
		return(rcData.Observation);
	EXCEPTION
		when no_data_found then
			if inuRaiseError = 1 then
				Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
				raise ex.CONTROLLED_ERROR;
			else
				return null;
			end if;
	END;
	FUNCTION fdtGetRegister_Date
	(
		inuInvest_Consum_Id in PE_invest_consum.Invest_Consum_Id%type,
		inuRaiseError in number default 1
	)
	RETURN PE_invest_consum.Register_Date%type
	IS
		rcError styPE_invest_consum;
	BEGIN

		rcError.Invest_Consum_Id := inuInvest_Consum_Id;

        -- si usa cache y esta cargado retorna
		if  blDAO_USE_CACHE AND fblAlreadyLoaded(inuInvest_Consum_Id) then
			 return(rcData.Register_Date);
		end if;
		Load(inuInvest_Consum_Id);
		return(rcData.Register_Date);
	EXCEPTION
		when no_data_found then
			if inuRaiseError = 1 then
				Errors.setError(cnuRECORD_NOT_EXIST,fsbGetMessageDescription||' '||fsbPrimaryKey(rcError));
				raise ex.CONTROLLED_ERROR;
			else
				return null;
			end if;
	END;
	PROCEDURE SetUseCache
	(
		iblUseCache    in  boolean
	) IS
	Begin
	    blDAO_USE_CACHE := iblUseCache;
	END;

begin
    GetDAO_USE_CACHE;
end DAPE_invest_consum;
/