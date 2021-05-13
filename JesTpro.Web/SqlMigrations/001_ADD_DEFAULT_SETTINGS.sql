
DELETE FROM `translate`;
INSERT INTO `translate` 
(
	`Key`,
	`It`,
	`En`,
	`Fr`,
	`Es`,
	`De`
)
VALUES (
	'[email.invoice]',
	'Nuova ricevuta di pagamento',
	'New payment invoice',
	'New payment invoice',
	'New payment invoice',
	'New payment invoice'
);

DELETE FROM `role`;
INSERT INTO `role` 
	VALUES ('SuperAdmin','This user can do everything in jestpro',0),
		   ('Watcher','Read only admin',1),
		   ('User','Standard company user',2),
		   ('PowerUser','Company manager',3),
		   ('Teacher','Company teacher',4);

DELETE FROM `settings`;
INSERT INTO `settings` (`Key`, `Type`) VALUES ('email.SmtpServer', 'string');
INSERT INTO `settings` (`Key`, `Type`) VALUES ('email.SmtpPort', 'int');
INSERT INTO `settings` (`Key`, `Type`) VALUES ('email.SmtpUsername', 'string');
INSERT INTO `settings` (`Key`, `Type`) VALUES ('email.SmtpPassword', 'string');
INSERT INTO `settings` (`Key`, `Type`) VALUES ('email.EnableSsl', 'bool');
INSERT INTO `settings` (`Key`, `Type`) VALUES ('email.MailFrom', 'string');
INSERT INTO `settings` (`Key`, `Type`) VALUES ('email.NameFrom', 'string');