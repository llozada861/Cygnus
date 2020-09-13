alter table object_type ADD column path TEXT;

update object_type set path = '\server\sql\03fnc\' where object = 'Funcion';
update object_type set path = '\server\sql\04proc\' where object = 'procedimiento';
update object_type set path = '\server\sql\05pkg\' where object = 'paquete';