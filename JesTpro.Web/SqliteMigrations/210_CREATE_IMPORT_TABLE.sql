CREATE TABLE `massive_request` (
`Id` char(36) NOT NULL,
`Description` text COLLATE NOCASE DEFAULT NULL,
`ImportStatus` INTEGER NOT NULL,
`ImportType` INTEGER NOT NULL,
`FileToImport` text COLLATE NOCASE DEFAULT NULL,
`Error` text COLLATE NOCASE DEFAULT NULL,
`LastExecution` datetime DEFAULT NULL,
`XCreateDate` datetime DEFAULT NULL,
`XUpdateDate` datetime DEFAULT NULL,
`XDeleteDate` datetime DEFAULT NULL,
`XLastEditUser` TEXT DEFAULT NULL,
`XCreationUser` TEXT DEFAULT NULL,
PRIMARY KEY (`Id`)
);
