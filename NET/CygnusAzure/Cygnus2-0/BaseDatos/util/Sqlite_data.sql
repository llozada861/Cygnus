CREATE TABLE azure (
	codigo	INTEGER PRIMARY KEY AUTOINCREMENT,
	usuario	TEXT,
	correo	TEXT,
	dias	INTEGER,
	url	TEXT,
	empresa	INTEGER,
	defecto	TEXT,
	token	TEXT,
	proyecto	TEXT,
	FOREIGN KEY(empresa) REFERENCES company(codigo)
);

CREATE TABLE repositories (
	codigo	INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT,
	descripcion	TEXT,
	url	TEXT,
	ruta	TEXT,
	empresa	INTEGER,
	FOREIGN KEY(empresa) REFERENCES company(codigo)
);

CREATE TABLE repository_branch (
	repositorio_id	INTEGER,
	rama	TEXT,
	PRIMARY KEY(repositorio_id,rama)
);

CREATE TABLE story_user (
	codigo	INTEGER,
	descripcion	TEXT,
	usuario	TEXT,
	empresa	INTEGER,
	FOREIGN KEY(empresa) REFERENCES company(codigo),
	PRIMARY KEY(codigo)
);

CREATE TABLE task_user (
	codigo	INTEGER PRIMARY KEY AUTOINCREMENT,
	descripcion	TEXT,
	estado	TEXT,
	fecha_actualiza	TEXT,
	usuario	TEXT,
	completado	NUMERIC,
	fecha_display	TEXT,
	fecha_registro	TEXT,
	hist_usuario	INTEGER,
	fecha_inicio	TEXT,
	empresa	INTEGER,
	FOREIGN KEY(empresa) REFERENCES company(codigo),
	FOREIGN KEY(hist_usuario) REFERENCES story_user(codigo)
);

CREATE TABLE week (
	codigo	INTEGER PRIMARY KEY AUTOINCREMENT,
	fecha_ini	TEXT,
	fecha_fin	TEXT,
	descripcion	TEXT
);

CREATE TABLE timexweek (
	codigo	INTEGER PRIMARY KEY AUTOINCREMENT,
	id_hoja	INTEGER,
	fecha_registro	TEXT,
	usuario	BLOB,
	lunes	NUMERIC,
	martes	NUMERIC,
	miercoles	NUMERIC,
	jueves	NUMERIC,
	viernes	NUMERIC,
	sabado	NUMERIC,
	domingo	NUMERIC,
	fecha_actualiza	TEXT,
	observacion	TEXT,
	requerimiento	INTEGER,
	FOREIGN KEY(id_hoja) REFERENCES week(codigo),
	FOREIGN KEY(requerimiento) REFERENCES task_user(codigo)
);

CREATE TABLE sequence (
	codigo	INTEGER PRIMARY KEY AUTOINCREMENT
);

insert into sequence (codigo) values (null);

CREATE TABLE task_pred (
	codigo	INTEGER,
	descripcion	TEXT,
	PRIMARY KEY(codigo)
);

insert into task_pred (CODIGO, DESCRIPCION) values (-1, 'Vacaciones');
insert into task_pred (CODIGO, DESCRIPCION) values (-2, 'Beneficio');
insert into task_pred (CODIGO, DESCRIPCION) values (-3, 'Calamidad');
insert into task_pred (CODIGO, DESCRIPCION) values (-4, 'Incapacidad');
insert into task_pred (CODIGO, DESCRIPCION) values (-5, 'Permiso');
insert into task_pred (CODIGO, DESCRIPCION) values (-6, 'Compensatorio');
insert into task_pred (CODIGO, DESCRIPCION) values (-7, 'Día Familia');

INSERT INTO azure (usuario, correo, dias, url, empresa, defecto, token, proyecto) VALUES ('', '', '10', 'https://grupoepm.visualstudio.com', '99', 'S', 'o7ue2wxhuejoo4dshlr5kxpypjvmrfi6udami4fe4psmzgbe62ka', 'OPEN');

