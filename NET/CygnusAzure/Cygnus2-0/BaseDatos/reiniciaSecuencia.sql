declare
    nuval number;
begin
	--1. Reiniciar la secuencia seq_ll_objetosbl.

    SELECT seq_ll_objetosbl.nextval
    INTO nuval
    FROM dual;

    /*el valor máximo menos el current*/
    execute immediate 'alter sequence seq_ll_objetosbl increment BY '||(<nuObjetosBl> - nuval);

    SELECT seq_ll_objetosbl.nextval
    INTO nuval
    FROM dual;

    execute immediate 'alter sequence seq_ll_objetosbl increment BY 1';
	
	--2. Reiniciar la secuencia seq_ll_logapli.
	SELECT seq_ll_logapli.nextval
    INTO nuval
    FROM dual;

    /*el valor máximo menos el current*/
    execute immediate 'alter sequence seq_ll_logapli increment BY '||(<nuObjetosLog> - nuval);

    SELECT seq_ll_logapli.nextval
    INTO nuval
    FROM dual;

    execute immediate 'alter sequence seq_ll_logapli increment BY 1';
	
	--3. Reiniciar la secuencia seq_ll_requerimiento.
	SELECT seq_ll_requerimiento.nextval
    INTO nuval
    FROM dual;

    /*el valor máximo menos el current*/
    execute immediate 'alter sequence seq_ll_requerimiento increment BY '||(<nuRq> - nuval);

    SELECT seq_ll_requerimiento.nextval
    INTO nuval
    FROM dual;

    execute immediate 'alter sequence seq_ll_requerimiento increment BY 1';
	
	--4. Reiniciar la secuencia seq_ll_horashoja.
	SELECT seq_ll_horashoja.nextval
    INTO nuval
    FROM dual;

    /*el valor máximo menos el current*/
    execute immediate 'alter sequence seq_ll_horashoja increment BY '||(<nuHorasHoja> - nuval);

    SELECT seq_ll_horashoja.nextval
    INTO nuval
    FROM dual;

    execute immediate 'alter sequence seq_ll_horashoja increment BY 1';
	
	--5. Reiniciar la secuencia seq_ll_negativa.
	SELECT seq_ll_negativa.nextval
    INTO nuval
    FROM dual;

    /*el valor máximo menos el current*/
    execute immediate 'alter sequence seq_ll_negativa increment BY '||(<nuSeqNeg> - nuval);

    SELECT seq_ll_negativa.nextval
    INTO nuval
    FROM dual;

    execute immediate 'alter sequence seq_ll_negativa increment BY 1';
	
End;
/