SELECT codigo,
       fecha_ini,
       fecha_fin,
       descripcion,
       (SELECT sum(lunes) + sum(martes) + sum(miercoles) + sum(jueves) + sum(viernes) + sum(sabado) + sum(domingo)
        FROM horashoja hh
        WHERE hh.id_hoja = h.codigo
        AND hh.usuario = 'SQL_LLOZADA' ) horas
FROM hoja h
WHERE fecha_fin < SYSDATE
OR   SYSDATE BETWEEN fecha_ini AND fecha_fin
/
SELECT codigo,
       fecha_ini,
       fecha_fin,
       descripcion,
       NVL(
       (SELECT sum(lunes) + sum(martes) + sum(miercoles) + sum(jueves) + sum(viernes) + sum(sabado) + sum(domingo)
        FROM flex.ll_horashoja hh
        WHERE hh.id_hoja = h.codigo
        AND hh.usuario = isbUsuario )
       ,0) horas
FROM flex.ll_hoja h
WHERE fecha_fin > SYSDATE
AND fecha_fin < SYSDATE + 7
/
SELECT * --codigo
       FROM flex.ll_hoja
       WHERE trunc(SYSDATE) BETWEEN fecha_ini AND fecha_fin
/
SELECT (sum(lunes) + sum(martes) + sum(miercoles) + sum(jueves) + sum(viernes) + sum(sabado) + sum(domingo)) horas
FROM horashoja 
WHERE id_hoja = 43
AND usuario = 'SQL_LLOZADA'
/
SELECT * --sum(lunes) + sum(martes) + sum(miercoles) + sum(jueves) + sum(viernes) + sum(sabado) + sum(domingo)
FROM horashoja 
WHERE id_hoja = 43
AND usuario = 'SQL_LLOZADA'

/
SELECT * FROM hoja
/
            /*WITH qHojaActual AS
            (
                SELECT fecha_fin
                FROM flex.ll_hoja
                WHERE trunc(SYSDATE) BETWEEN fecha_ini AND fecha_fin
            )*/
            SELECT 
                   id_azure idAzure, 
                   rq.descripcion,
                   estado,
                   rq.codigo id_rq,
                   hh.codigo id,
                   hh.id_hoja,
                   hh.lunes,
                   hh.martes,
                   hh.miercoles,
                   hh.jueves,
                   hh.viernes,
                   hh.sabado,
                   hh.domingo,
                   rq.fecha_display,
                   ho.fecha_fin
                   --qHojaActual.fecha_fin
            FROM flex.ll_horashoja hh,flex.ll_requerimiento rq, flex.ll_hoja ho --qHojaActual
            WHERE hh.requerimiento = rq.codigo
            AND   hh.usuario = &isbUsuario
            AND   hh.id_hoja = &inuHoja
            AND   hh.id_hoja = ho.codigo
            AND   ho.fecha_fin <= rq.fecha_display
            --AND   rq.fecha_display  <= (ho.fecha_fin + 7)  --(qHojaActual.fecha_fin + 7) 
/
SELECT * FROM flex.ll_requerimiento WHERE usuario = 'SQL_LLOZADA' AND fecha_registro IS NOT NULL ORDER BY fecha_registro DESC
 FOR UPDATE
/  
SELECT * FROM flex.ll_horashoja WHERE usuario = 'SQL_LLOZADA'
ORDER BY fecha_registro DESC
 FOR UPDATE
/
SELECT * FROM flex.ll_usuarios FOR UPDATE
/
SELECT * FROM flex.ll_azure FOR UPDATE
/
SELECT * FROM flex.ll_hoja FOR UPDATE
/
DELETE FROM flex.ll_horashoja WHERE usuario = 'SQL_LLOZADA'
/
DELETE FROM flex.ll_requerimiento WHERE usuario = 'SQL_LLOZADA'
/
SELECT * FROM flex.ll_requerimiento rq
WHERE usuario = 'SQL_LLOZADA'
AND NOT EXISTS(SELECT 1 FROM flex.ll_horashoja WHERE requerimiento = rq.codigo)
FOR UPDATE
/
--origen -8
SELECT hh.*
FROM flex.ll_horashoja hh,flex.ll_requerimiento rq
WHERE hh.usuario = 'SQL_LLOZADA'
AND rq.id_azure = -5
AND hh.requerimiento = rq.codigo
/
--destino -5
SELECT hh.*
FROM flex.ll_horashoja hh,flex.ll_requerimiento rq
WHERE hh.usuario = 'SQL_LLOZADA'
AND rq.id_azure = -8
AND id_hoja = 17
AND hh.requerimiento = rq.codigo
/
--Total
SELECT (sum(lunes) + sum(martes) + sum(miercoles) + sum(jueves) + sum(viernes) + sum(sabado) + sum(domingo)) horas
FROM horashoja hh
WHERE requerimiento = 73
/
--Detalle
SELECT fecha_ini fecha, lunes hora, rq.descripcion
FROM horashoja hh,hoja ho, requerimiento rq
WHERE requerimiento = 73
AND hh.id_hoja = ho.codigo
AND hh.requerimiento = rq.codigo
AND lunes > 0
UNION
SELECT fecha_ini+1,martes, rq.descripcion
FROM horashoja hh,hoja ho, requerimiento rq
WHERE requerimiento = 73
AND hh.id_hoja = ho.codigo
AND hh.requerimiento = rq.codigo
AND martes > 0
UNION
SELECT fecha_ini+2,miercoles, rq.descripcion
FROM horashoja hh,hoja ho, requerimiento rq
WHERE requerimiento = 73
AND hh.id_hoja = ho.codigo
AND hh.requerimiento = rq.codigo
AND miercoles > 0
UNION
SELECT fecha_ini+3,jueves, rq.descripcion
FROM horashoja hh,hoja ho, requerimiento rq
WHERE requerimiento = 73
AND hh.id_hoja = ho.codigo
AND hh.requerimiento = rq.codigo
AND jueves > 0
UNION
SELECT fecha_ini+4,viernes, rq.descripcion
FROM horashoja hh,hoja ho, requerimiento rq
WHERE requerimiento = 73
AND hh.id_hoja = ho.codigo
AND hh.requerimiento = rq.codigo
AND viernes > 0
UNION
SELECT fecha_ini+5,sabado, rq.descripcion
FROM horashoja hh,hoja ho, requerimiento rq
WHERE requerimiento = 73
AND hh.id_hoja = ho.codigo
AND hh.requerimiento = rq.codigo
AND sabado > 0
UNION
SELECT fecha_ini+6,domingo, rq.descripcion
FROM horashoja hh,hoja ho, requerimiento rq
WHERE requerimiento = 73
AND hh.id_hoja = ho.codigo
AND hh.requerimiento = rq.codigo
AND domingo > 0;

/
SELECT fecha_ini, lunes,
       fecha_ini+1,martes,
       fecha_ini+2,miercoles,
       fecha_ini+3,jueves,
       fecha_ini+4,viernes,
       fecha_ini+5,sabado,
       fecha_ini+6,domingo
FROM horashoja hh,hoja ho
WHERE requerimiento = 73
AND hh.id_hoja = ho.codigo
/
SELECT * FROM (
            SELECT codigo,
                   fecha_ini,
                   fecha_fin,
                   descripcion,
                   NVL(
                   (SELECT sum(lunes) + sum(martes) + sum(miercoles) + sum(jueves) + sum(viernes) + sum(sabado) + sum(domingo)
                    FROM flex.ll_horashoja hh
                    WHERE hh.id_hoja = h.codigo
                    AND hh.usuario = &isbUsuario )
                   ,0) horas
            FROM flex.ll_hoja h
            WHERE trunc(SYSDATE+8) BETWEEN fecha_ini AND fecha_fin
            UNION
            SELECT codigo,
                   fecha_ini,
                   fecha_fin,
                   descripcion,
                   NVL(
                   (SELECT sum(lunes) + sum(martes) + sum(miercoles) + sum(jueves) + sum(viernes) + sum(sabado) + sum(domingo)
                    FROM flex.ll_horashoja hh
                    WHERE hh.id_hoja = h.codigo
                    AND hh.usuario = &isbUsuario )
                   ,0) horas
            FROM flex.ll_hoja h
            WHERE fecha_fin < SYSDATE
            OR   trunc(SYSDATE) BETWEEN fecha_ini AND fecha_fin
            ORDER BY fecha_fin DESC)
            WHERE rownum < 13;
/
SELECT to_number('1,5') FROM dual
/
SELECT to_date('16/04/2020','dd/mm/yyyy') FROM dual
/
SELECT 'INSERT INTO flex.ll_hojas (codigo,fecha_ini,fecha_fin,descripcion) '||
                   'VALUES ('||codigo||','''||to_char(fecha_ini,'dd/mm/yyyy')||''','''||to_char(fecha_fin,'dd/mm/yyyy')||''','''||descripcion||''');' valor
            FROM flex.ll_hoja; 
/
SELECT 'INSERT INTO flex.ll_requerimiento (CODIGO, DESCRIPCION, ID_AZURE, ESTADO, FECHA_ACTUALIZA, USUARIO, FECHA_DISPLAY, COMPLETADO, FECHA_REGISTRO, HIST_USUARIO) '||
       'values ('||codigo||','''||descripcion||''','||id_azure||','''||estado||''','''||to_char(nvl(fecha_actualiza,SYSDATE),'dd/mm/yyyy')||''','''||usuario||''','''||to_char(nvl(fecha_display,SYSDATE),'dd/mm/yyyy')||''','||nvl(completado,0)||','''||to_char(nvl(fecha_registro,SYSDATE),'dd/mm/yyyy')||''','||hist_usuario||');' valor
            FROM flex.ll_requerimiento;  
/
SELECT 'INSERT INTO flex.ll_horashoja (CODIGO, ID_HOJA, FECHA_REGISTRO, USUARIO, REQUERIMIENTO, LUNES, MARTES, MIERCOLES, JUEVES, VIERNES, SABADO, DOMINGO, FECHA_ACTUALIZA, OBSERVACION) '||
       'VALUES ('||CODIGO||','||ID_HOJA||','''||to_char(FECHA_REGISTRO,'dd/mm/yyyy')||''','''||USUARIO||''','||REQUERIMIENTO||','||LUNES||','||MARTES||','||MIERCOLES||','||JUEVES||','||VIERNES||','||SABADO||','||DOMINGO||','''||to_char(FECHA_ACTUALIZA,'dd/mm/yyyy')||''','''||OBSERVACION||''');'
            FROM flex.ll_horashoja 
            WHERE LUNES > 0
            OR MARTES > 0
            OR MIERCOLES > 0
            OR JUEVES > 0
            OR VIERNES > 0
            OR SABADO > 0
            OR DOMINGO > 0;
/
SELECT 'INSERT INTO flex.ll_estandar (token,descripcion,valor) VALUES('''||token||''','''||descripcion||''','''||dbms_lob.(valor,)||''');'
FROM flex.ll_estandar            
/
SELECT *
            FROM flex.ll_horashoja 
/                        
SELECT *
            FROM flex.ll_requerimiento;  
/
--Reporte total
SELECT * FROM (
            SELECT fecha_ini fecha, 
                   rq.hist_usuario,
                   rq.id_azure,
                   rq.descripcion,
                   lunes horaCygnus,
                   rq.completado horaAzure
            FROM flex.ll_horashoja hh,flex.ll_hoja ho, flex.ll_requerimiento rq
            WHERE hh.usuario = 'SQL_LLOZADA'
            AND   ho.fecha_ini >= '01/07/2020'
            AND   ho.fecha_fin <= '31/07/2020'
            AND hh.id_hoja = ho.codigo
            AND hh.requerimiento = rq.codigo
            AND lunes > 0                        
            UNION
            SELECT fecha_ini+1 fecha,
                   rq.hist_usuario,
                   rq.id_azure,
                   rq.descripcion,
                   martes horaCygnus,
                   rq.completado horaAzure
            FROM flex.ll_horashoja hh,flex.ll_hoja ho, flex.ll_requerimiento rq
            WHERE hh.usuario = 'SQL_LLOZADA'
            AND   ho.fecha_ini >= '01/07/2020'
            AND   ho.fecha_fin <= '31/07/2020'
            AND   hh.id_hoja = ho.codigo
            AND hh.requerimiento = rq.codigo
            AND martes > 0
            UNION
            SELECT fecha_ini+2 fecha,
                   rq.hist_usuario,
                   rq.id_azure,
                   rq.descripcion,
                   miercoles horaCygnus,
                   rq.completado horaAzure
            FROM flex.ll_horashoja hh,flex.ll_hoja ho, flex.ll_requerimiento rq
            WHERE hh.usuario = 'SQL_LLOZADA'
            AND   ho.fecha_ini >= '01/07/2020'
            AND   ho.fecha_fin <= '31/07/2020'
            AND hh.id_hoja = ho.codigo
            AND hh.requerimiento = rq.codigo
            AND miercoles > 0
            UNION
            SELECT fecha_ini+3 fecha,
                   rq.hist_usuario,
                   rq.id_azure,
                   rq.descripcion,
                   jueves horaCygnus,
                   rq.completado horaAzure
            FROM flex.ll_horashoja hh,flex.ll_hoja ho, flex.ll_requerimiento rq
            WHERE hh.usuario = 'SQL_LLOZADA'
            AND   ho.fecha_ini >= '01/07/2020'
            AND   ho.fecha_fin <= '31/07/2020'
            AND hh.id_hoja = ho.codigo
            AND hh.requerimiento = rq.codigo
            AND jueves > 0
            UNION
            SELECT fecha_ini+4 fecha,
                   rq.hist_usuario,
                   rq.id_azure,
                   rq.descripcion,
                   viernes horaCygnus,
                   rq.completado horaAzure
            FROM flex.ll_horashoja hh,flex.ll_hoja ho, flex.ll_requerimiento rq
            WHERE hh.usuario = 'SQL_LLOZADA'
            AND   ho.fecha_ini >= '01/07/2020'
            AND   ho.fecha_fin <= '31/07/2020'
            AND hh.id_hoja = ho.codigo
            AND hh.requerimiento = rq.codigo
            AND viernes > 0
            UNION
            SELECT fecha_ini+5 fecha,
                   rq.hist_usuario,
                   rq.id_azure,
                   rq.descripcion,
                   sabado horaCygnus,
                   rq.completado horaAzure
            FROM flex.ll_horashoja hh,flex.ll_hoja ho, flex.ll_requerimiento rq
            WHERE hh.usuario = 'SQL_LLOZADA'
            AND   ho.fecha_ini >= '01/07/2020'
            AND   ho.fecha_fin <= '31/07/2020'
            AND hh.id_hoja = ho.codigo
            AND hh.requerimiento = rq.codigo
            AND sabado > 0
            UNION
            SELECT fecha_ini+6 fecha,
                   rq.hist_usuario,
                   rq.id_azure,
                   rq.descripcion,
                   domingo horaCygnus,
                   rq.completado horaAzure
            FROM flex.ll_horashoja hh,flex.ll_hoja ho, flex.ll_requerimiento rq
            WHERE hh.usuario = 'SQL_LLOZADA'
            AND   ho.fecha_ini >= '01/07/2020'
            AND   ho.fecha_fin <= '31/07/2020'
            AND hh.id_hoja = ho.codigo
            AND hh.requerimiento = rq.codigo
            AND domingo > 0 
            )
            ORDER BY fecha;  
/
SELECT * FROM ll_horashoja WHERE usuario = 'SQL_LLOZADA'   
/
SELECT fecha_inicio,
       hist_usuario,
       id_azure,
       estado,
       completado
FROM flex.ll_requerimiento
WHERE usuario = 'SQL_LLOZADA'
AND fecha_inicio >= '01/06/2020'
AND fecha_inicio <= '30/06/2020'
ORDER BY fecha_inicio DESC
/
--SELECT DISTINCT hist_usuario FROM (
SELECT *
FROM flex.ll_requerimiento
WHERE usuario = 'SQL_LLOZADA'
ORDER BY fecha_registro DESC --)
/
--C#
"SELECT * FROM ("+
"            SELECT fecha_ini fecha, "+
"                   rq.hist_usuario,"+
"                   rq.id_azure,"+
"                   rq.descripcion,"+
"                   lunes horaCygnus,"+
"                   rq.completado horaAzure"+
"            FROM flex.ll_horashoja hh,flex.ll_hoja ho, flex.ll_requerimiento rq"+
"            WHERE hh.usuario = :usuario"+
"            AND   ho.fecha_ini >= :fecha_i"+
"            AND   ho.fecha_fin <= :fecha_f"+
"            AND hh.id_hoja = ho.codigo"+
"            AND hh.requerimiento = rq.codigo"+
"            AND lunes > 0                        "+
"            UNION"+
"            SELECT fecha_ini+1 fecha,"+
"                   rq.hist_usuario,"+
"                   rq.id_azure,"+
"                   rq.descripcion,"+
"                   martes horaCygnus,"+
"                   rq.completado horaAzure"+
"            FROM flex.ll_horashoja hh,flex.ll_hoja ho, flex.ll_requerimiento rq"+
"            WHERE hh.usuario = :usuario"+
"            AND   ho.fecha_ini >= :fecha_i"+
"            AND   ho.fecha_fin <= :fecha_f"+
"            AND   hh.id_hoja = ho.codigo"+
"            AND hh.requerimiento = rq.codigo"+
"            AND martes > 0"+
"            UNION"+
"            SELECT fecha_ini+2 fecha,"+
"                   rq.hist_usuario,"+
"                   rq.id_azure,"+
"                   rq.descripcion,"+
"                   miercoles horaCygnus,"+
"                   rq.completado horaAzure"+
"            FROM flex.ll_horashoja hh,flex.ll_hoja ho, flex.ll_requerimiento rq"+
"            WHERE hh.usuario = :usuario"+
"            AND   ho.fecha_ini >= :fecha_i"+
"            AND   ho.fecha_fin <= :fecha_f"+
"            AND hh.id_hoja = ho.codigo"+
"            AND hh.requerimiento = rq.codigo"+
"            AND miercoles > 0"+
"            UNION"+
"            SELECT fecha_ini+3 fecha,"+
"                   rq.hist_usuario,"+
"                   rq.id_azure,"+
"                   rq.descripcion,"+
"                   jueves horaCygnus,"+
"                   rq.completado horaAzure"+
"            FROM flex.ll_horashoja hh,flex.ll_hoja ho, flex.ll_requerimiento rq"+
"            WHERE hh.usuario = :usuario"+
"            AND   ho.fecha_ini >= :fecha_i"+
"            AND   ho.fecha_fin <= :fecha_f"+
"            AND hh.id_hoja = ho.codigo"+
"            AND hh.requerimiento = rq.codigo"+
"            AND jueves > 0"+
"            UNION"+
"            SELECT fecha_ini+4 fecha,"+
"                   rq.hist_usuario,"+
"                   rq.id_azure,"+
"                   rq.descripcion,"+
"                   viernes horaCygnus,"+
"                   rq.completado horaAzure"+
"            FROM flex.ll_horashoja hh,flex.ll_hoja ho, flex.ll_requerimiento rq"+
"            WHERE hh.usuario = :usuario"+
"            AND   ho.fecha_ini >= :fecha_i"+
"            AND   ho.fecha_fin <= :fecha_f"+
"            AND hh.id_hoja = ho.codigo"+
"            AND hh.requerimiento = rq.codigo"+
"            AND viernes > 0"+
"            UNION"+
"            SELECT fecha_ini+5 fecha,"+
"                   rq.hist_usuario,"+
"                   rq.id_azure,"+
"                   rq.descripcion,"+
"                   sabado horaCygnus,"+
"                   rq.completado horaAzure"+
"            FROM flex.ll_horashoja hh,flex.ll_hoja ho, flex.ll_requerimiento rq"+
"            WHERE hh.usuario = :usuario"+
"            AND   ho.fecha_ini >= :fecha_i"+
"            AND   ho.fecha_fin <= :fecha_f"+
"            AND hh.id_hoja = ho.codigo"+
"            AND hh.requerimiento = rq.codigo"+
"            AND sabado > 0"+
"            UNION"+
"            SELECT fecha_ini+6 fecha,"+
"                   rq.hist_usuario,"+
"                   rq.id_azure,"+
"                   rq.descripcion,"+
"                   domingo horaCygnus,"+
"                   rq.completado horaAzure"+
"            FROM flex.ll_horashoja hh,flex.ll_hoja ho, flex.ll_requerimiento rq"+
"            WHERE hh.usuario = :usuario"+
"            AND   ho.fecha_ini >= :fecha_i"+
"            AND   ho.fecha_fin <= :fecha_f"+
"            AND hh.id_hoja = ho.codigo"+
"            AND hh.requerimiento = rq.codigo"+
"            AND domingo > 0 "+
"            )"+
"            ORDER BY fecha"
/
"SELECT fecha_inicio,"+
"       hist_usuario,"+
"       id_azure,"+
"       estado,"+
"       completado "+
"FROM flex.ll_requerimiento "+
"WHERE usuario = :usuario "+
"AND fecha_inicio >= :fecha_i "+
"AND fecha_inicio <= :fecha_f "
/
SELECT * FROM flex.ll_usuarios
/
SELECT * FROM dba_users WHERE username = 'SQL_LLOZA6'
/
SELECT * 
FROM flex.ll_requerimiento 
WHERE usuario = 'SQL_LLOZADA' 
--AND descripcion_hu IS NOT  NULL
ORDER BY fecha_registro DESC
/
SELECT hist_usuario,descripcion_hu FROM 
(
    SELECT * FROM 
    (
        SELECT DISTINCT hist_usuario,descripcion_hu
        FROM flex.ll_requerimiento 
        WHERE usuario = 'SQL_LLOZADA' 
        AND descripcion_hu IS NOT  NULL
    )
    ORDER BY hist_usuario DESC
)
WHERE ROWNUM < 30
/
"SELECT hist_usuario,descripcion_hu FROM "+
"("+
"    SELECT * FROM "+
"    ("+
"        SELECT DISTINCT hist_usuario,descripcion_hu"+
"        FROM flex.ll_requerimiento "+
"        WHERE usuario = 'SQL_LLOZADA' "+
"        AND descripcion_hu IS NOT  NULL"+
"    )"+
"    ORDER BY hist_usuario DESC"+
")"+
"WHERE ROWNUM < 30"+

