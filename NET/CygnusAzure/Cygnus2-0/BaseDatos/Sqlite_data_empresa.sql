DROP table object_type;

CREATE TABLE "object_type" (
	"codigo"	INTEGER PRIMARY KEY AUTOINCREMENT,
	"object"	TEXT,
	"slash"	TEXT,
	"count_slash"	INTEGER,
	"priority"	TEXT,
	"grant"	BLOB
);

drop table paths;

CREATE TABLE object_path (
	codigo	INTEGER PRIMARY KEY AUTOINCREMENT,
	object_type	INTEGER,
	path	TEXT,
	user_default	TEXT,
	company	INTEGER,
	FOREIGN KEY(object_type) REFERENCES object_type(codigo)
);

insert into object_type (codigo,object,slash,count_slash,priority,grant) values (1,'Funcion','S',1,300,'E');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (2,'procedimiento','S',1,301,'E');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (3,'paquete','S',1,302,'E');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (4,'trigger','S',1,303,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (5,'tabla','N',0,100,'S|I|U|D');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (6,'secuencia','N',0,104,'S');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (7,'vista','N',0,103,'S');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (8,'indice','N',0,101,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (9,'sinonimo','N',0,108,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (10,'alter','N',0,105,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (11,'drop','N',0,1,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (12,'insert','N',0,201,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (13,'update','N',0,202,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (14,'delete','N',0,200,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (15,'script','S',1,400,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (16,'otros','N',0,500,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (17,'merge','N',0,203,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (18,'llave_primaria','',0,110,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (19,'llave_foranea','',0,111,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (20,'llave_unica','',0,113,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (21,'vista_mat','',0,114,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (22,'job','',0,401,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (23,'grant','',0,402,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (24,'EA','',0,501,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (25,'GI','',0,502,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (26,'GR','',0,503,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (27,'MD','',0,504,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (28,'OB','',0,505,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (29,'OP','',0,506,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (30,'PB','',0,507,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (31,'PI','',0,508,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (32,'RU','',0,509,'N');
insert into object_type (codigo,object,slash,count_slash,priority,grant) values (33,'TC','',0,510,'N');

insert into object_path (object_type,path,user_default,company) values (1,'server\\sql\\03fnc\\[nombre]','',99);
insert into object_path (object_type,path,user_default,company) values (2,'server\\sql\\04proc\\[nombre]','',99);
insert into object_path (object_type,path,user_default,company) values (3,'server\\sql\\05pkg\\[nombre]','',99);
insert into object_path (object_type,path,user_default,company) values (4,'server\\sql\\02tbls\\[nombre]\\05trg','',99);
insert into object_path (object_type,path,user_default,company) values (5,'server\\sql\\02tbls\\[nombre]\\00tbl','',99);
insert into object_path (object_type,path,user_default,company) values (6,'server\\sql\\01seq\\[nombre]','',99);
insert into object_path (object_type,path,user_default,company) values (7,'server\\sql\\06view\\[nombre]','',99);
insert into object_path (object_type,path,user_default,company) values (8,'server\\sql\\02tbls\\[nombre]\\02idx','',99);
insert into object_path (object_type,path,user_default,company) values (9,'server\\sql\\02tbls\\[nombre]\\11syn','',99);
insert into object_path (object_type,path,user_default,company) values (10,'server\\sql\\02tbls\\[nombre]\\00tbl','',99);
insert into object_path (object_type,path,user_default,company) values (11,'server\\sql\\02tbls\\[nombre]\\00tbl','',99);
insert into object_path (object_type,path,user_default,company) values (12,'server\\sql\\02tbls\\[nombre]\\06data','',99);
insert into object_path (object_type,path,user_default,company) values (13,'server\\sql\\02tbls\\[nombre]\\06data','',99);
insert into object_path (object_type,path,user_default,company) values (14,'server\\sql\\02tbls\\[nombre]\\06data','',99);
insert into object_path (object_type,path,user_default,company) values (15,'','',99);
insert into object_path (object_type,path,user_default,company) values (16,'','',99);
insert into object_path (object_type,path,user_default,company) values (17,'server\\sql\\02tbls\\[nombre]\\06data','',99);
insert into object_path (object_type,path,user_default,company) values (18,'server\\sql\\02tbls\\[nombre]\\01pkey','',99);
insert into object_path (object_type,path,user_default,company) values (19,'server\\sql\\02tbls\\[nombre]\\03fkey','',99);
insert into object_path (object_type,path,user_default,company) values (20,'server\\sql\\02tbls\\[nombre]\\04chk','',99);
insert into object_path (object_type,path,user_default,company) values (21,'server\\sql\\07vwmat\\[nombre]','',99);
insert into object_path (object_type,path,user_default,company) values (22,'server\\sql\\09job\\[nombre]','',99);
insert into object_path (object_type,path,user_default,company) values (23,'server\\sql\\10grt\\[nombre]','',99);
insert into object_path (object_type,path,user_default,company) values (24,'client\\framework\\EA\\[nombre]','FLEX',99);
insert into object_path (object_type,path,user_default,company) values (25,'client\\framework\\GI\\[nombre]','FLEX',99);
insert into object_path (object_type,path,user_default,company) values (26,'client\\framework\\GR\\[nombre]','FLEX',99);
insert into object_path (object_type,path,user_default,company) values (27,'client\\framework\\MD\\[nombre]','FLEX',99);
insert into object_path (object_type,path,user_default,company) values (28,'client\\framework\\OB\\[nombre]','FLEX',99);
insert into object_path (object_type,path,user_default,company) values (29,'client\\framework\\OP\\[nombre]','FLEX',99);
insert into object_path (object_type,path,user_default,company) values (30,'client\\framework\\PB\\[nombre]','FLEX',99);
insert into object_path (object_type,path,user_default,company) values (31,'client\\framework\\PI\\[nombre]','FLEX',99);
insert into object_path (object_type,path,user_default,company) values (32,'client\\framework\\RU\\[nombre]','FLEX',99);
insert into object_path (object_type,path,user_default,company) values (33,'client\\framework\\TC\\[nombre]','FLEX',99);

INSERT INTO configuration (key, value, company) VALUES ('SQLPLUS', 'RUTA', '');

INSERT INTO configuration (key,value) VALUES ('EMPRESA','-');

drop table company;

CREATE TABLE company (
	codigo	INTEGER PRIMARY KEY AUTOINCREMENT,
	descripcion	text,
	azure	TEXT,
	git	TEXT,
	sonar	TEXT
);

insert into company (codigo,descripcion,azure,git,sonar) values (99,'EPM','Y','Y','Y');