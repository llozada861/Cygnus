--@Autor: Llozada
DECLARE
    CURSOR cuUsuarios
    IS
        SELECT user_id 
        FROM sa_user
        WHERE mask IN (
        'ABETANB',
        'CVILLARR',
        'ARIVERIV',
        'ETACHEJI',
        'EHENAOHE',
        'IOSORIOF',
        'JRINCONG',
        'JMORALEM',
        'JMORENRI',
        'JHERNASI',
        'JSUAREZM',
        'JACOSTAC',
        'JTABATAB',
        'JVEGAPER',
        'JMARMARI',
        'JARISTZ',
        'JCATUCHE',
        'JJARAMVA',
        'JRADAG',
        'LANGELES',
        'LBLANQUI',
        'LOCAMPRE',
        'LMACIAS',
        'MCASTANC',
        'MALVARAS',
        'OSUAREZO',
        'ROLARTEP',
        'TRUIZDIA',
        'VJIMENEC',
        'ZMORENOR',
        'JVELAV',
        'JROBAYO1',
        'LLOZADA',
        'CRAMIREH'
        );  
        
     rc cuUsuarios%ROWTYPE; 
BEGIN
    dbms_output.put_line('INICIA');
    OPEN cuUsuarios;
    LOOP  
        rc := null;
        FETCH cuUsuarios INTO rc;
        EXIT WHEN rc.user_id IS null;
        
        dbms_output.put_line('Ini User_id :'||rc.user_id);
		
		begin
        INSERT INTO sa_user_roles (role_id, user_id)
        VALUES(1, rc.user_id);
        EXCEPTION
            WHEN OTHERS THEN
                dbms_output.put_line(SQLERRM);
        END;
        
        begin
        --172,173,189,2,-99,2,170
        INSERT INTO sa_user_roles (role_id, user_id)
        VALUES(172, rc.user_id);
        EXCEPTION
            WHEN OTHERS THEN
                dbms_output.put_line(SQLERRM);
        END;
        
        begin
        INSERT INTO sa_user_roles (role_id, user_id)
        VALUES(173, rc.user_id);
        EXCEPTION
            WHEN OTHERS THEN
                dbms_output.put_line(SQLERRM);
        END;
        
        begin        
        INSERT INTO sa_user_roles (role_id, user_id)
        VALUES(189, rc.user_id);
        EXCEPTION
            WHEN OTHERS THEN
                dbms_output.put_line(SQLERRM);
        END;
        
        begin
        INSERT INTO sa_user_roles (role_id, user_id)
        VALUES(2, rc.user_id);
        EXCEPTION
            WHEN OTHERS THEN
                dbms_output.put_line(SQLERRM);
        END;
        
        --INSERT INTO sa_user_roles (role_id, user_id)
        --VALUES(-99, rc.user_id);
        
        begin
        INSERT INTO sa_user_roles (role_id, user_id)
        VALUES(170, rc.user_id);
        EXCEPTION
            WHEN OTHERS THEN
                dbms_output.put_line(SQLERRM);
        END;
        
        dbms_output.put_line('User_id :'||rc.user_id);
    END LOOP;
    CLOSE cuUsuarios;
END;
/