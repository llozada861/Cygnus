CREATE OR REPLACE PACKAGE DA<nombre_tabla>
is 
	/* Cursor general para acceso por Llave Primaria */
	CURSOR cuRecord 
	(
		<campos_primarios>
	)
	IS
		SELECT <nombre_tabla>.*,<nombre_tabla>.rowid
		FROM <nombre_tabla>
		<where_campos_primarios>;


	/* Cursor general para acceso por RowId */
	CURSOR cuRecordByRowId
	(
		irirowid in varchar2
	)
	IS
        SELECT <nombre_tabla>.*,<nombre_tabla>.rowid
		FROM <nombre_tabla>
		WHERE 
			rowId = irirowid;


	/* Subtipos */
	subtype sty<nombre_tabla>  is  cuRecord%rowtype;
	type    tyRefCursor is  REF CURSOR;

	/*Tipos*/
	type tytb<nombre_tabla> is table of sty<nombre_tabla> index by binary_integer;
	type tyrfRecords is ref cursor return sty<nombre_tabla>;

	/* Tipos referenciando al registro */
	type tytb<field> is table of <nombre_tabla>.<field>%type index by binary_integer;
	type tytbrowid is table of rowid index by binary_integer;

	type tyrc<nombre_tabla> is record
	(
		<field>   tytb<field>,
		row_id tytbrowid
	);

	/***** Metodos Publicos ****/

    FUNCTION fsbVersion
    RETURN varchar2;

	FUNCTION <nombre_tabla>
	return varchar2;

	PROCEDURE ClearMemory;

	FUNCTION fblExist
	(
		<campos_primarios>
	)
	RETURN boolean;

	PROCEDURE AccKey
	(
		<campos_primarios>
	);

	PROCEDURE AccKeyByRowId
	(
		iriRowID    in rowid
	);

	PROCEDURE ValDuplicate
	(
		<campos_primarios>
	);

	PROCEDURE getRecord
	(
		<campos_primarios>,
		orcRecord out nocopy sty<nombre_tabla>
	);

	FUNCTION frcGetRcData
	(
		<campos_primarios>
	)
	RETURN sty<nombre_tabla>;

	FUNCTION frcGetRcData
	RETURN sty<nombre_tabla>;

	FUNCTION frcGetRecord
	(
		<campos_primarios>
	)
	RETURN sty<nombre_tabla>;

	PROCEDURE getRecords 
	(
		isbQuery in varchar2,
		otbResult out nocopy tytb<nombre_tabla>
	);

	FUNCTION frfGetRecords
	(
		isbCriteria in varchar2 default null,
		iblLock in boolean default false
	)
	RETURN tyRefCursor;

	PROCEDURE insRecord
	(
		irc<nombre_tabla> in sty<nombre_tabla>
	);

	PROCEDURE insRecord
	(
		irc<nombre_tabla> in sty<nombre_tabla>,
        orirowid   out varchar2
	);

	PROCEDURE insRecords
	(
		iotb<nombre_tabla> in out nocopy tytb<nombre_tabla>
	);

	PROCEDURE delRecord
	(
		<campos_primarios>,
		inuLock in number default 1
	);

	PROCEDURE delByRowID
	(
		iriRowID    in rowid,
		inuLock in number default 1
	);

	PROCEDURE delRecords
	(
		iotb<nombre_tabla> in out nocopy tytb<nombre_tabla>,
		inuLock in number default 1
	);

	PROCEDURE updRecord
	(
		irc<nombre_tabla> in sty<nombre_tabla>,
		inuLock in number default 0
	);

	PROCEDURE updRecords
	(
		iotb<nombre_tabla> in out nocopy tytb<nombre_tabla>,
		inuLock in number default 1
	);

	--Por cada campo se hace este procedimiento
	PROCEDURE upd<field>
	(
		<campos_primarios>,
		inu<field>$ in <nombre_tabla>.<field>%type,
		inuLock in number default 0
	);

	--Por cada campo se hace este procedimiento
	FUNCTION fnuGet<field>
	(
		<campos_primarios>,
		inuRaiseError in number default 1
	)
	RETURN <nombre_tabla>.<field>%type;

	PROCEDURE LockByPk
	(
		<campos_primarios>,
		orc<nombre_tabla>  out sty<nombre_tabla>
	);

	PROCEDURE LockByRowID
	(
		irirowid    in  varchar2,
		orc<nombre_tabla>  out sty<nombre_tabla>
	);

	PROCEDURE SetUseCache
	(
		iblUseCache    in  boolean
	);
END DA<nombre_tabla>;
/
CREATE OR REPLACE PACKAGE BODY DA<nombre_tabla>
IS
    /*constantes locales al paquete*/
    cnuRECORD_NOT_EXIST constant number(1) := 1;
    cnuRECORD_ALREADY_EXIST constant number(1) := 2;
    cnuAPPTABLEBUSSY constant number(4) := 6951;
    cnuINS_PK_NULL constant number(4):= 1682;
    cnuRECORD_HAVE_CHILDREN constant number(4):= -2292;
    csbVersion   CONSTANT VARCHAR2(20) := '<numero_oc>';
    csbTABLEPARAMETER   CONSTANT VARCHAR2(30) := '<nombre_tabla>';

	/* Cursor para bloqueo de un registro por llave primaria */
	CURSOR cuLockRcByPk 
	(
		<campos_primarios>
	)
	IS
		SELECT <nombre_tabla>.*,<nombre_tabla>.rowid 
		FROM <nombre_tabla>
		<where_campos_primarios>
		FOR UPDATE NOWAIT;

	/* Cursor para bloqueo de un registro por rowid */
	CURSOR cuLockRcbyRowId 
	(
		irirowid in varchar2
	)
	IS
		SELECT <nombre_tabla>.*,<nombre_tabla>.rowid 
		FROM <nombre_tabla>
		WHERE 
			rowId = irirowid
		FOR UPDATE NOWAIT;

	/*Tipos*/
	type tyrf<nombre_tabla> is ref cursor;

	/*Variables Globales*/
	rcRecOfTab tyrc<nombre_tabla>;

	rcData cuRecord%rowtype;

    blDAO_USE_CACHE    boolean := null;


	/* Metodos privados */	
	PROCEDURE GetDAO_USE_CACHE
	IS
	BEGIN
	    if ( blDAO_USE_CACHE is null ) then
	        blDAO_USE_CACHE :=  ge_boparameter.fsbget('DAO_USE_CACHE') = 'Y';
	    end if;
	END;
	FUNCTION fsbPrimaryKey( rcI in sty<nombre_tabla> default rcData )
	return varchar2
	IS
		sbPk varchar2(500);
	BEGIN
		sbPk:='[';
		sbPk:=sbPk||ut_convert.fsbToChar(rcI.<llave_primaria>);
		sbPk:=sbPk||']';
		return sbPk;
	END;
	
	PROCEDURE LockByPk
	(
		<campos_primarios>,
		orc<nombre_tabla>  out sty<nombre_tabla>
	)
	IS
		rcError sty<nombre_tabla>;
	BEGIN
		rcError.<llave_primaria>:=<primaria_con_tipo>;

		Open cuLockRcByPk(<primaria_con_tipo>);

		fetch cuLockRcByPk into orc<nombre_tabla>;
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
			Errors.setError(cnuRECORD_NOT_EXIST,<nombre_tabla>||' '|| fsbPrimaryKey(rcError));
			raise ex.CONTROLLED_ERROR;
		when ex.RESOURCE_BUSY THEN
			if cuLockRcByPk%isopen then
				close cuLockRcByPk;
			end if;
			errors.setError(cnuAPPTABLEBUSSY,fsbPrimaryKey(rcError)||'|'|| <nombre_tabla> );
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
		orc<nombre_tabla>  out sty<nombre_tabla>
	)
	IS
	BEGIN
		Open cuLockRcbyRowId
		(
			irirowid
		);

		fetch cuLockRcbyRowId into orc<nombre_tabla>;
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
			Errors.setError(cnuRECORD_NOT_EXIST,<nombre_tabla>||' rowid=['||iriRowID||']');
			raise ex.CONTROLLED_ERROR;
		when ex.RESOURCE_BUSY THEN
			if cuLockRcbyRowId%isopen then
				close cuLockRcbyRowId;
			end if;
			errors.setError(cnuAPPTABLEBUSSY,'rowid=['||irirowid||']|'||<nombre_tabla> );
			raise ex.controlled_error;
		when others then
			if cuLockRcbyRowId%isopen then
				close cuLockRcbyRowId;
			end if;
			raise;
	END;
	
	PROCEDURE DelRecordOfTables
	(
		itb<nombre_tabla>  in out nocopy tytb<nombre_tabla>
	)
	IS
	BEGIN
			rcRecOfTab.<field>.delete;
			rcRecOfTab.row_id.delete;
	END;
	
	PROCEDURE FillRecordOfTables
	(
		itb<nombre_tabla>  in out nocopy tytb<nombre_tabla>,
		oblUseRowId out boolean
	)
	IS
	BEGIN
		DelRecordOfTables(itb<nombre_tabla>);

		for n in itb<nombre_tabla>.first .. itb<nombre_tabla>.last loop
			rcRecOfTab.<field>(n) := itb<nombre_tabla>(n).<field>;
			rcRecOfTab.row_id(n) := itb<nombre_tabla>(n).rowid;

			-- Indica si el rowid es Nulo
			oblUseRowId:=rcRecOfTab.row_id(n) is NOT NULL;
		end loop;
	END;

	PROCEDURE Load
	(
		<campos_primarios>
	)
	IS
		rcRecordNull cuRecord%rowtype;
	BEGIN
		if cuRecord%isopen then
			close cuRecord;
		end if;
		open cuRecord(<primaria_con_tipo>);

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
		<campos_primarios>
	)
	RETURN boolean
	IS
	BEGIN
		if (
			<primaria_con_tipo> = rcData.<llave_primaria>
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
		<campos_primarios>
	)
	RETURN boolean
	IS
	BEGIN
		Load(<primaria_con_tipo>);
		return(TRUE);
	EXCEPTION
		when no_data_found then
			return(FALSE);
	END;
	
	PROCEDURE AccKey
	(
		<campos_primarios>
	)
	IS
		rcError sty<nombre_tabla>;
	BEGIN
		rcError.<llave_primaria>:=<primaria_con_tipo>;

		Load(<primaria_con_tipo>);
	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,<nombre_tabla>||' '||fsbPrimaryKey(rcError));
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
            Errors.setError(cnuRECORD_NOT_EXIST,<nombre_tabla>||' rowid=['||iriRowID||']');
            raise ex.CONTROLLED_ERROR;
	END;
	
	PROCEDURE ValDuplicate
	(
		<campos_primarios>
	)
	IS
	BEGIN
		Load(<primaria_con_tipo>);
		Errors.setError(cnuRECORD_ALREADY_EXIST,<nombre_tabla>||' '||fsbPrimaryKey);
		raise ex.CONTROLLED_ERROR;

	EXCEPTION
		when no_data_found then
			null;
	END;
	
	PROCEDURE getRecord
	(
		<campos_primarios>,
		orcRecord out nocopy sty<nombre_tabla>
	)
	IS
		rcError sty<nombre_tabla>;
	BEGIN
		rcError.<llave_primaria>:=<primaria_con_tipo>;

		Load(<primaria_con_tipo>);
		orcRecord := rcData;
	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,<nombre_tabla>||' '|| fsbPrimaryKey(rcError));
			raise ex.CONTROLLED_ERROR;
	END;
	
	FUNCTION frcGetRecord
	(
		<campos_primarios>
	)
	RETURN sty<nombre_tabla>
	IS
		rcError sty<nombre_tabla>;
	BEGIN
		rcError.<llave_primaria>:=<primaria_con_tipo>;

		Load(<primaria_con_tipo>);
		return(rcData);
	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,<nombre_tabla>||' '|| fsbPrimaryKey(rcError));
			raise ex.CONTROLLED_ERROR;
	END;
	
	FUNCTION frcGetRcData
	(
		<campos_primarios>
	)
	RETURN sty<nombre_tabla>
	IS
		rcError sty<nombre_tabla>;
	BEGIN
		rcError.<llave_primaria>:=<primaria_con_tipo>;
        -- si usa cache y esta cargado retorna
		if  blDAO_USE_CACHE AND fblAlreadyLoaded(<primaria_con_tipo>) then
			 return(rcData);
		end if;
		Load(<primaria_con_tipo>);
		return(rcData);
	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,<nombre_tabla>||' '|| fsbPrimaryKey(rcError));
			raise ex.CONTROLLED_ERROR;
	END;
	
	FUNCTION frcGetRcData
	RETURN sty<nombre_tabla>
	IS
	BEGIN
		return(rcData);
	END;
	
	PROCEDURE getRecords
	(
		isbQuery in varchar2,
		otbResult out nocopy tytb<nombre_tabla>
	)
	IS
		rf<nombre_tabla> tyrf<nombre_tabla>;
		n number(4) := 1;
		sbFullQuery VARCHAR2 (32000) := 'SELECT <nombre_tabla>.*, <nombre_tabla>.rowid FROM <nombre_tabla>';
		nuMaxTbRecords number(5):=ge_boparameter.fnuget('MAXREGSQUERY');
	BEGIN
		otbResult.delete;
		if isbQuery is not NULL and length(isbQuery) > 0 then
			sbFullQuery := sbFullQuery||' WHERE '||isbQuery;
		end if;

		open rf<nombre_tabla> for sbFullQuery;

		fetch rf<nombre_tabla> bulk collect INTO otbResult;

		close rf<nombre_tabla>;
		if otbResult.count = 0  then
			raise no_data_found;
		end if;

	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,<nombre_tabla>);
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
		sbSQL VARCHAR2 (32000) := 'select <nombre_tabla>.*, <nombre_tabla>.rowid FROM <nombre_tabla>';
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
			Errors.setError(cnuRECORD_NOT_EXIST,<nombre_tabla>);
			raise ex.CONTROLLED_ERROR;
	END;
	
	PROCEDURE insRecord
	(
		irc<nombre_tabla> in sty<nombre_tabla>
	)
	IS
		rirowid varchar2(200);
	BEGIN
		insRecord(irc<nombre_tabla>,rirowid);
	END;
	
	PROCEDURE insRecord
	(
		irc<nombre_tabla> in sty<nombre_tabla>,
        orirowid   out varchar2
	)
	IS
	BEGIN
		if irc<nombre_tabla>.<llave_primaria> is NULL then
			Errors.SetError(cnuINS_PK_NULL,
			                <nombre_tabla>||'|<llave_primaria>');
			raise ex.controlled_error;
		end if;

		insert into <nombre_tabla>
		(
			<field>,
		)
		values
		(
			irc<nombre_tabla>.<field>,
		)
            returning
			rowid
		into
			orirowid;
		ClearMemory;
	EXCEPTION
		when dup_val_on_index then
			Errors.setError(cnuRECORD_ALREADY_EXIST,<nombre_tabla>||' '||fsbPrimaryKey(irc<nombre_tabla>));
			raise ex.CONTROLLED_ERROR;
	END;
	
	PROCEDURE insRecords
	(
		iotb<nombre_tabla> in out nocopy tytb<nombre_tabla>
	)
	IS
		blUseRowID boolean;
	BEGIN
		FillRecordOfTables(iotb<nombre_tabla>,blUseRowID);
		forall n in iotb<nombre_tabla>.first..iotb<nombre_tabla>.last
			insert into <nombre_tabla>
			(
				<field>,
			)
			values
			(
				rcRecOfTab.<field>(n),
			);
		ClearMemory;
	EXCEPTION
		when dup_val_on_index then
			Errors.setError(cnuRECORD_ALREADY_EXIST,<nombre_tabla>);
			raise ex.CONTROLLED_ERROR;
	END;
	
	PROCEDURE delRecord
	(
		<campos_primarios>,
		inuLock in number default 1
	)
	IS
		rcError sty<nombre_tabla>;
	BEGIN
		rcError.<llave_primaria> := <primaria_con_tipo>;

		if inuLock=1 then
			LockByPk
			(
				<primaria_con_tipo>,
				rcData
			);
		end if;


		delete
		from <nombre_tabla>
		where
       		<llave_primaria>=<primaria_con_tipo>;
            if sql%notfound then
                raise no_data_found;
            end if;
		ClearMemory;
	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,<nombre_tabla>||' '||fsbPrimaryKey(rcError));
         raise ex.CONTROLLED_ERROR;
		when ex.RECORD_HAVE_CHILDREN then
			Errors.setError(cnuRECORD_HAVE_CHILDREN,<nombre_tabla>||' '||fsbPrimaryKey(rcError));
			raise ex.CONTROLLED_ERROR;
	END;
	
	PROCEDURE delByRowID
	(
		iriRowID    in rowid,
		inuLock in number default 1
	)
	IS
		rcRecordNull cuRecord%rowtype;
		rcError  sty<nombre_tabla>;
	BEGIN
		if inuLock=1 then
			LockByRowId(iriRowID,rcData);
		end if;


		delete
		from <nombre_tabla>
		where
			rowid = iriRowID
		returning
			<llave_primaria>
		into
			rcError.<llave_primaria>;
            if sql%notfound then
			 raise no_data_found;
		    end if;
            if rcData.rowID=iriRowID then
			 rcData := rcRecordNull;
		    end if;
		ClearMemory;
	EXCEPTION
		when no_data_found then
            Errors.setError(cnuRECORD_NOT_EXIST,<nombre_tabla>||' rowid=['||iriRowID||']');
            raise ex.CONTROLLED_ERROR;
		when ex.RECORD_HAVE_CHILDREN then
            Errors.setError(cnuRECORD_HAVE_CHILDREN,<nombre_tabla>||' '||' rowid=['||iriRowID||']');
            raise ex.CONTROLLED_ERROR;
	END;
	
	PROCEDURE delRecords
	(
		iotb<nombre_tabla> in out nocopy tytb<nombre_tabla>,
		inuLock in number default 1
	)
	IS
		blUseRowID boolean;
		rcAux sty<nombre_tabla>;
	BEGIN
		FillRecordOfTables(iotb<nombre_tabla>, blUseRowID);
        if ( blUseRowId ) then
			if inuLock = 1 then
				for n in iotb<nombre_tabla>.first .. iotb<nombre_tabla>.last loop
					LockByRowId
					(
						rcRecOfTab.row_id(n),
						rcAux
					);
				end loop;
			end if;

			forall n in iotb<nombre_tabla>.first .. iotb<nombre_tabla>.last
				delete
				from <nombre_tabla>
				where
					rowid = rcRecOfTab.row_id(n);
		else
			if inuLock = 1 then
				for n in iotb<nombre_tabla>.first .. iotb<nombre_tabla>.last loop
					LockByPk
					(
						rcRecOfTab.<llave_primaria>(n),
						rcAux
					);
				end loop;
			end if;

			forall n in iotb<nombre_tabla>.first .. iotb<nombre_tabla>.last
				delete
				from <nombre_tabla>
				where
		         	<llave_primaria> = rcRecOfTab.<llave_primaria>(n);
		end if;
		ClearMemory;
	EXCEPTION
            when ex.RECORD_HAVE_CHILDREN then
                  Errors.setError(cnuRECORD_HAVE_CHILDREN,<nombre_tabla>);
                  raise ex.CONTROLLED_ERROR;
	END;
	
	PROCEDURE updRecord
	(
		irc<nombre_tabla> in sty<nombre_tabla>,
		inuLock in number default 0
	)
	IS
		nu<llave_primaria>	<nombre_tabla>.<llave_primaria>%type;
	BEGIN
		if irc<nombre_tabla>.rowid is not null then
			if inuLock=1 then
				LockByRowId(irc<nombre_tabla>.rowid,rcData);
			end if;
			update <nombre_tabla>
			set
				<field> = irc<nombre_tabla>.<field>,
			where
				rowid = irc<nombre_tabla>.rowid
			returning
				<llave_primaria>
			into
				nu<llave_primaria>;
		else
			if inuLock=1 then
				LockByPk
				(
					irc<nombre_tabla>.<llave_primaria>,
					rcData
				);
			end if;

			update <nombre_tabla>
			set
				<field> = irc<nombre_tabla>.<field>,
			where
				<llave_primaria> = irc<nombre_tabla>.<llave_primaria>
			returning
				<llave_primaria>
			into
				nu<llave_primaria>;
		end if;
		if
			nu<llave_primaria> is NULL
		then
			raise no_data_found;
		end if;
		ClearMemory;
	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,<nombre_tabla>||fsbPrimaryKey(irc<nombre_tabla>));
			raise ex.CONTROLLED_ERROR;
	END;
	
	PROCEDURE updRecords
	(
		iotb<nombre_tabla> in out nocopy tytb<nombre_tabla>,
		inuLock in number default 1
	)
	IS
		blUseRowID boolean;    
		rcAux sty<nombre_tabla>;
	BEGIN
		FillRecordOfTables(iotb<nombre_tabla>,blUseRowID);
		if blUseRowID then
			if inuLock = 1 then
				for n in iotb<nombre_tabla>.first .. iotb<nombre_tabla>.last loop
					LockByRowId
					(
						rcRecOfTab.row_id(n),
						rcAux
					);
				end loop;
			end if;

			forall n in iotb<nombre_tabla>.first .. iotb<nombre_tabla>.last
				update <nombre_tabla>
				set
					<field> = rcRecOfTab.<field>(n),
				where
					rowid =  rcRecOfTab.row_id(n);
		else
			if inuLock = 1 then
				for n in iotb<nombre_tabla>.first .. iotb<nombre_tabla>.last loop
					LockByPk
					(
						rcRecOfTab.<llave_primaria>(n),
						rcAux
					);
				end loop;
			end if;

			forall n in iotb<nombre_tabla>.first .. iotb<nombre_tabla>.last
				update <nombre_tabla>
				SET
					<field> = rcRecOfTab.<field>(n),
				where
					<llave_primaria> = rcRecOfTab.<llave_primaria>(n)
;
		end if;
		ClearMemory;
	END;
	
	--Se repite por cada campo
	PROCEDURE upd<field>
	(
		<campos_primarios>,
		inu<field>$ in <nombre_tabla>.<field>%type,
		inuLock in number default 0
	)
	IS
		rcError sty<nombre_tabla>;
	BEGIN
		rcError.<llave_primaria> := <primaria_con_tipo>;
		if inuLock=1 then
			LockByPk
			(
				<primaria_con_tipo>,
				rcData
			);
		end if;

		update <nombre_tabla>
		set
			<field> = inu<field>$
		where
			<llave_primaria> = <primaria_con_tipo>;

		if sql%notfound then
			raise no_data_found;
		end if;

		rcData.<field>:= inu<field>$;
		ClearMemory;

	EXCEPTION
		when no_data_found then
			Errors.setError(cnuRECORD_NOT_EXIST,<nombre_tabla>||' '||fsbPrimaryKey(rcError));
			raise ex.CONTROLLED_ERROR;
	END;
	
	--Se repite por cada campo	
	FUNCTION fnuGet<field>
	(
		<campos_primarios>,
		inuRaiseError in number default 1
	)
	RETURN <nombre_tabla>.<field>%type
	IS
		rcError sty<nombre_tabla>;
	BEGIN

		rcError.<llave_primaria> := <primaria_con_tipo>;

        -- si usa cache y esta cargado retorna
		if  blDAO_USE_CACHE AND fblAlreadyLoaded(<primaria_con_tipo>) then
			 return(rcData.<field>);
		end if;
		Load(<primaria_con_tipo>);
		return(rcData.<field>);
	EXCEPTION
		when no_data_found then
			if inuRaiseError = 1 then
				Errors.setError(cnuRECORD_NOT_EXIST,<nombre_tabla>||' '||fsbPrimaryKey(rcError));
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
end DA<nombre_tabla>;
/