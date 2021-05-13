-- import to SQLite by running: sqlite3.exe db.sqlite3 -init sqlite.sql

PRAGMA journal_mode = MEMORY;
PRAGMA synchronous = OFF;
PRAGMA foreign_keys = OFF;
PRAGMA ignore_check_constraints = OFF;
PRAGMA auto_vacuum = NONE;
PRAGMA secure_delete = OFF;

DROP TABLE IF EXISTS `attachment`;

CREATE TABLE `attachment` (
`Id` char(36) NOT NULL,
`IdResource` char(36) NOT NULL,
`Name` TEXT COLLATE NOCASE NOT NULL,
`Size` bigINTEGER NOT NULL,
`FullPath` mediumtext,
`XCreateDate` datetime DEFAULT NULL,
`XUpdateDate` datetime DEFAULT NULL,
`XDeleteDate` datetime DEFAULT NULL,
`XLastEditUser` TEXT DEFAULT NULL,
`XCreationUser` TEXT DEFAULT NULL,
PRIMARY KEY (`Id`)
);

DROP TABLE IF EXISTS `audit`;

CREATE TABLE `audit` (
`Id` INTEGER NOT NULL ,
`User` TEXT DEFAULT NULL,
`TableName` TEXT DEFAULT NULL,
`DateTime` datetime DEFAULT NULL,
`KeyValues` TEXT,
`OldValues` TEXT,
`NewValues` TEXT,
PRIMARY KEY (`Id`)
);

DROP TABLE IF EXISTS `credit_note`;

CREATE TABLE `credit_note` (
`Id` char(36) NOT NULL,
`IdReceipt` char(36) NOT NULL,
`Description` text COLLATE NOCASE NOT NULL,
`IssueDate` datetime NOT NULL,
`CreditNoteNumber` TEXT DEFAULT NULL,
`CreditNotePath` TEXT DEFAULT NULL,
`InvoiceNumber` TEXT DEFAULT NULL,
`InvoiceDate` datetime NOT NULL,
`IssuedBy` char(36) DEFAULT NULL,
`XCreateDate` datetime DEFAULT NULL,
`XUpdateDate` datetime DEFAULT NULL,
`XDeleteDate` datetime DEFAULT NULL,
`XLastEditUser` TEXT DEFAULT NULL,
`XCreationUser` TEXT DEFAULT NULL,
PRIMARY KEY (`Id`)
);

DROP TABLE IF EXISTS `creditnote_number`;

CREATE TABLE `creditnote_number` (
`IdCreditNote` char(36) NOT NULL,
`number` INTEGER DEFAULT NULL,
`year` INTEGER DEFAULT NULL,
PRIMARY KEY (`IdCreditNote`)
);

DROP TABLE IF EXISTS `customer`;

CREATE TABLE `customer` (
`Id` char(36) NOT NULL,
`IdType` char(36) NOT NULL,
`FirstName` TEXT COLLATE NOCASE NOT NULL,
`LastName` TEXT COLLATE NOCASE NOT NULL,
`FullName` TEXT COLLATE NOCASE DEFAULT NULL,
`Gender` char(1) DEFAULT NULL,
`FiscalCode` char(16) COLLATE NOCASE DEFAULT NULL,
`Address` TEXT COLLATE NOCASE DEFAULT NULL,
`HouseNumber` TEXT COLLATE NOCASE DEFAULT NULL,
`City` TEXT COLLATE NOCASE DEFAULT NULL,
`PostalCode` TEXT COLLATE NOCASE DEFAULT NULL,
`State` TEXT COLLATE NOCASE DEFAULT NULL,
`Country` TEXT COLLATE NOCASE DEFAULT NULL,
`BirthDate` datetime NOT NULL,
`TutorType` TEXT COLLATE NOCASE DEFAULT NULL,
`TutorFirstName` TEXT COLLATE NOCASE DEFAULT NULL,
`TutorLastName` TEXT COLLATE NOCASE DEFAULT NULL,
`TutorFiscalCode` char(16) COLLATE NOCASE DEFAULT NULL,
`TutorBirthDate` datetime DEFAULT NULL,
`TutorPhoneNumber` TEXT DEFAULT NULL,
`TutorEmail` TEXT COLLATE NOCASE DEFAULT NULL,
`Note` text COLLATE NOCASE,
`MembershipFee` decimal(15,2) DEFAULT NULL,
`MembershipLastPayDate` datetime DEFAULT NULL,
`MembershipFeeExpiryDate` datetime DEFAULT NULL,
`PhoneNumber` TEXT DEFAULT NULL,
`PhoneNumberAlternative` TEXT DEFAULT NULL,
`Email` TEXT COLLATE NOCASE DEFAULT NULL,
`MedicalCertificateExpiration` datetime DEFAULT NULL,
`Photo` TEXT DEFAULT NULL,
`PhotoFullPath` TEXT DEFAULT NULL,
`XCreateDate` datetime DEFAULT NULL,
`XUpdateDate` datetime DEFAULT NULL,
`XDeleteDate` datetime DEFAULT NULL,
`XLastEditUser` TEXT DEFAULT NULL,
`XCreationUser` TEXT DEFAULT NULL,
PRIMARY KEY (`Id`)
);

DROP TABLE IF EXISTS `customer_productinstance`;

CREATE TABLE `customer_productinstance` (
`Id` char(36) NOT NULL,
`IdCustomer` char(36) NOT NULL,
`IdProductInstance` char(36) NOT NULL,
`IdReceipt` char(36) DEFAULT NULL,
`Name` TEXT COLLATE NOCASE DEFAULT NULL,
`Description` TEXT COLLATE NOCASE,
`XCreationUser` TEXT DEFAULT NULL,
`Price` decimal(15,2) NOT NULL,
`Discount` decimal(15,2) DEFAULT NULL,
`CostAmount` decimal(15,2) NOT NULL,
`DiscountDescription` text COLLATE NOCASE,
`DiscountType` INTEGER DEFAULT NULL,
`IdReversal` char(36) DEFAULT NULL,
`ReversalDate` datetime DEFAULT NULL,
`ReversalCredit` decimal(15,2) DEFAULT NULL,
`ReversalDescription` TEXT COLLATE NOCASE,
`ExpirationDate` datetime DEFAULT NULL,
`PaymentStatus` INTEGER DEFAULT NULL,
`XCreateDate` datetime DEFAULT NULL,
`XUpdateDate` datetime DEFAULT NULL,
`XDeleteDate` datetime DEFAULT NULL,
`XLastEditUser` TEXT DEFAULT NULL,
PRIMARY KEY (`Id`)
);

DROP TABLE IF EXISTS `customer_type`;

CREATE TABLE `customer_type` (
`Id` char(36) NOT NULL,
`Name` TEXT COLLATE NOCASE NOT NULL,
`Description` text COLLATE NOCASE,
`CostAmount` decimal(15,2) NOT NULL,
`Duration` TEXT COLLATE NOCASE DEFAULT NULL,
`XCreateDate` datetime DEFAULT NULL,
`XUpdateDate` datetime DEFAULT NULL,
`XDeleteDate` datetime DEFAULT NULL,
`XLastEditUser` TEXT DEFAULT NULL,
`XCreationUser` TEXT DEFAULT NULL,
PRIMARY KEY (`Id`)
);

DROP TABLE IF EXISTS `image`;

CREATE TABLE `image` (
`Id` char(36) NOT NULL,
`Path` TEXT NOT NULL,
`XCreateDate` datetime DEFAULT NULL,
`XUpdateDate` datetime DEFAULT NULL,
`XDeleteDate` datetime DEFAULT NULL,
`XLastEditUser` TEXT DEFAULT NULL,
`XCreationUser` TEXT DEFAULT NULL,
`IsDefault` bit(1) NOT NULL DEFAULT 0,
PRIMARY KEY (`Id`)
);

DROP TABLE IF EXISTS `invoice_number`;

CREATE TABLE `invoice_number` (
`IdReceipt` char(36) NOT NULL,
`number` INTEGER DEFAULT NULL,
`year` INTEGER DEFAULT NULL,
PRIMARY KEY (`IdReceipt`)
);

DROP TABLE IF EXISTS `payment_receipt`;

CREATE TABLE `payment_receipt` (
`Id` char(36) NOT NULL,
`IdCustomer` char(36) DEFAULT NULL,
`Name` TEXT COLLATE NOCASE NOT NULL,
`Description` TEXT COLLATE NOCASE,
`IssueDate` datetime NOT NULL,
`PaymentDate` datetime DEFAULT NULL,
`CostAmount` decimal(15,2) NOT NULL,
`ReceiptPath` TEXT DEFAULT NULL,
`InvoiceNumber` TEXT  DEFAULT NULL,
`CustomerName` TEXT COLLATE NOCASE DEFAULT NULL,
`CustomerFiscalCode` TEXT COLLATE NOCASE DEFAULT NULL,
`CustomerAddress` TEXT COLLATE NOCASE DEFAULT NULL,
`IssuedBy` char(36) DEFAULT NULL,
`PaymentType` TEXT COLLATE NOCASE DEFAULT NULL,
`XCreateDate` datetime DEFAULT NULL,
`XUpdateDate` datetime DEFAULT NULL,
`XDeleteDate` datetime DEFAULT NULL,
`XLastEditUser` TEXT DEFAULT NULL,
`XCreationUser` TEXT DEFAULT NULL,
PRIMARY KEY (`Id`)
);

DROP TABLE IF EXISTS `payment_receipt_detail`;

CREATE TABLE `payment_receipt_detail` (
`Id` char(36) NOT NULL,
`IdResource` char(36) NOT NULL,
`IdReceipt` char(36) NOT NULL,
`ReceiptDetailType` INTEGER NOT NULL,
`Name` TEXT COLLATE NOCASE NOT NULL,
`Description` TEXT COLLATE NOCASE,
`CostAmount` decimal(15,2) NOT NULL,
`ProductAmount` INTEGER NOT NULL DEFAULT '1',
`XCreateDate` datetime DEFAULT NULL,
`XUpdateDate` datetime DEFAULT NULL,
`XDeleteDate` datetime DEFAULT NULL,
`XLastEditUser` TEXT DEFAULT NULL,
`XCreationUser` TEXT DEFAULT NULL,
PRIMARY KEY (`Id`)
);

DROP TABLE IF EXISTS `product`;

CREATE TABLE `product` (
`Id` char(36) NOT NULL,
`Name` TEXT COLLATE NOCASE NOT NULL,
`Description` TEXT COLLATE NOCASE,
`CategoryName` TEXT COLLATE NOCASE DEFAULT NULL,
`TeacherName` TEXT COLLATE NOCASE DEFAULT NULL,
`XCreationUser` TEXT DEFAULT NULL,
`XCreateDate` datetime DEFAULT NULL,
`XUpdateDate` datetime DEFAULT NULL,
`XDeleteDate` datetime DEFAULT NULL,
`XLastEditUser` TEXT DEFAULT NULL,
`StartDate` datetime NOT NULL,
`EndDate` datetime DEFAULT NULL,
PRIMARY KEY (`Id`)
);

DROP TABLE IF EXISTS `product_instance`;

CREATE TABLE `product_instance` (
`Id` char(36) NOT NULL,
`IdProduct` char(36) NOT NULL,
`Name` TEXT COLLATE NOCASE NOT NULL,
`Description` TEXT COLLATE NOCASE,
`Days` INTEGER NOT NULL DEFAULT '0',
`Weeks` INTEGER NOT NULL DEFAULT '0',
`Months` INTEGER NOT NULL DEFAULT '0',
`Years` INTEGER NOT NULL DEFAULT '0',
`Price` decimal(15,2) NOT NULL DEFAULT '0.00',
`XCreateDate` datetime DEFAULT NULL,
`XUpdateDate` datetime DEFAULT NULL,
`XDeleteDate` datetime DEFAULT NULL,
`XLastEditUser` TEXT DEFAULT NULL,
`XCreationUser` TEXT DEFAULT NULL,
PRIMARY KEY (`Id`)
);

DROP TABLE IF EXISTS `report`;

CREATE TABLE `report` (
`Id` char(36) NOT NULL,
`Family` INTEGER NOT NULL,
`Name` TEXT COLLATE NOCASE NOT NULL,
`Description` TEXT COLLATE NOCASE NOT NULL,
`ColumnMap` mediumtext,
`ParameterMap` mediumtext,
`Value` TEXT,
`Enabled` bit(1) NOT NULL,
`XCreateDate` datetime DEFAULT NULL,
`XUpdateDate` datetime DEFAULT NULL,
`XDeleteDate` datetime DEFAULT NULL,
`XLastEditUser` TEXT DEFAULT NULL,
`XCreationUser` TEXT DEFAULT NULL,
PRIMARY KEY (`Id`)
);

DROP TABLE IF EXISTS `role`;

CREATE TABLE `role` (
`Name` TEXT NOT NULL,
`Description` TEXT DEFAULT NULL,
`Id` INTEGER NOT NULL,
PRIMARY KEY (`Id`)
);

INSERT INTO `role` VALUES 
	('SuperAdmin','This user can do everything in jestpro',0),
	('Watcher','Read only admin',1),
	('User','Standard company user',2),
	('PowerUser','Company manager',3),
	('Teacher','Company teacher',4);

DROP TABLE IF EXISTS `settings`;

CREATE TABLE `settings` (
`Id` INTEGER NOT NULL ,
`Key` TEXT COLLATE NOCASE DEFAULT NULL,
`Value` TEXT COLLATE NOCASE DEFAULT NULL,
`Type` TEXT COLLATE NOCASE DEFAULT NULL,
`Required` bit(1) NOT NULL DEFAULT 0,
PRIMARY KEY (`Id`)
);

INSERT INTO `settings` (key,value,type,required) VALUES 
('email.SmtpServer',NULL,'string',0),
('email.SmtpPort',NULL,'int',0),
('email.SmtpUsername',NULL,'string',0),
('email.SmtpPassword',NULL,'string',0),
('email.EnableSsl',NULL,'bool',0),
('email.MailFrom',NULL,'string',0),
('email.NameFrom',NULL,'string',0),
('company.name',NULL,'string',0),
('company.address',NULL,'string',0),
('company.cap',NULL,'string',0),
('company.city',NULL,'string',0),
('company.state',NULL,'string',0),
('company.country',NULL,'string',0),
('company.vat',NULL,'string',0),
('company.fiscalcode',NULL,'string',0),
('company.pec',NULL,'string',0),
('company.email',NULL,'string',0),
('company.phone',NULL,'string',0),
('company.fax',NULL,'string',0),
('company.invoice-type',NULL,'string',1),
('company.invoice-logo',NULL,'image',0),
('product.categories','[]','select',0),
('product.teachers','[]','select',0),
('company.expiryMonthLimit','2','int',1),
('company.receipt-header',NULL,'string-multi',0),
('company.receipt-footer',NULL,'string-multi',0);

DROP TABLE IF EXISTS `sql_migrations`;

CREATE TABLE `sql_migrations` (
`Id` INTEGER NOT NULL ,
`FileName` TEXT NOT NULL,
`XCreateDate` datetime DEFAULT CURRENT_TIMESTAMP,
`Status` TEXT DEFAULT NULL,
`Error` mediumtext,
PRIMARY KEY (`Id`)
);

DROP TABLE IF EXISTS `translate`;

CREATE TABLE `translate` (
`Id` INTEGER NOT NULL ,
`Key` TEXT COLLATE NOCASE NOT NULL,
`It` TEXT COLLATE NOCASE DEFAULT NULL,
`En` TEXT COLLATE NOCASE DEFAULT NULL,
`Fr` TEXT COLLATE NOCASE DEFAULT NULL,
`Es` TEXT COLLATE NOCASE DEFAULT NULL,
`De` TEXT COLLATE NOCASE DEFAULT NULL,
PRIMARY KEY (`Id`)
);

INSERT INTO `translate` VALUES (3,'[email.invoice]','Nuova ricevuta di pagamento','New payment invoice','New payment invoice','New payment invoice','New payment invoice');

DROP TABLE IF EXISTS `user`;

CREATE TABLE `user` (
`Id` char(36) NOT NULL,
`FirstName` TEXT COLLATE NOCASE DEFAULT NULL,
`LastName` TEXT COLLATE NOCASE DEFAULT NULL,
`UserName` TEXT COLLATE NOCASE NOT NULL,
`PasswordHash` TEXT NOT NULL,
`Email` TEXT COLLATE NOCASE DEFAULT NULL,
`Disabled` bit(1) NOT NULL DEFAULT 0,
`PasswordSalt` TEXT NOT NULL,
`IdRole` INTEGER DEFAULT NULL,
`DashboardData` TEXT,
`Lang` char(2) DEFAULT NULL,
`XCreateDate` datetime DEFAULT NULL,
`XUpdateDate` datetime DEFAULT NULL,
`XDeleteDate` datetime DEFAULT NULL,
`XLastEditUser` TEXT DEFAULT NULL,
`XCreationUser` TEXT DEFAULT NULL,
PRIMARY KEY (`Id`)
);

CREATE UNIQUE INDEX `attachment_Id_UNIQUE` ON `attachment` (`Id`);
CREATE UNIQUE INDEX `audit_Id_UNIQUE` ON `audit` (`Id`);
CREATE UNIQUE INDEX `creditnote_number_IdCreditNote_UNIQUE` ON `creditnote_number` (`IdCreditNote`);
CREATE UNIQUE INDEX `creditnote_number_UniqueNumber` ON `creditnote_number` (`number`);
CREATE UNIQUE INDEX `customer_Id_UNIQUE` ON `customer` (`Id`);
CREATE UNIQUE INDEX `customer_productinstance_Id_UNIQUE` ON `customer_productinstance` (`Id`);
CREATE UNIQUE INDEX `customer_type_Id_UNIQUE` ON `customer_type` (`Id`);
CREATE UNIQUE INDEX `image_Id_UNIQUE` ON `image` (`Id`);
CREATE UNIQUE INDEX `invoice_number_IdReceipt_UNIQUE` ON `invoice_number` (`IdReceipt`);
CREATE UNIQUE INDEX `invoice_number_UniqueNumber` ON `invoice_number` (`number`);
CREATE UNIQUE INDEX `product_Id_UNIQUE` ON `product` (`Id`);
CREATE UNIQUE INDEX `product_instance_Id_UNIQUE` ON `product_instance` (`Id`);
CREATE UNIQUE INDEX `report_Id_UNIQUE` ON `report` (`Id`);
CREATE UNIQUE INDEX `role_Name_UNIQUE` ON `role` (`Name`);
CREATE UNIQUE INDEX `role_IdInt_UNIQUE` ON `role` (`Id`);
CREATE UNIQUE INDEX `translate_ID_UNIQUE_KEY` ON `translate` (`Key`);
CREATE UNIQUE INDEX `user_Id_UNIQUE` ON `user` (`Id`);
CREATE UNIQUE INDEX `user_UserName_UNIQUE` ON `user` (`UserName`);

COMMIT;
PRAGMA ignore_check_constraints = ON;
PRAGMA foreign_keys = ON;
PRAGMA journal_mode = WAL;
PRAGMA synchronous = NORMAL;
