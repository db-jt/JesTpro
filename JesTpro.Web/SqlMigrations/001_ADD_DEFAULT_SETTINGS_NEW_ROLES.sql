DELETE FROM `role`;
INSERT INTO `role` 
	VALUES ('SuperAdmin','This user can do everything in jestpro',0),
		   ('Watcher','Read only admin',1),
		   ('User','Standard company user',2),
		   ('PowerUser','Company manager',3),
		   ('Teacher','Company teacher',4);