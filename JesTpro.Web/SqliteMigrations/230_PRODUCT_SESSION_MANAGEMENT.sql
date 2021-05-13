DROP TABLE IF EXISTS `product_session`;

CREATE TABLE `product_session` (
`Id` char(36) NOT NULL,
`IdProduct` char(36) NOT NULL,
`IdTeacher` char(36) NOT NULL,
`Description` TEXT COLLATE NOCASE,
`XCreateDate` datetime DEFAULT NULL,
`XUpdateDate` datetime DEFAULT NULL,
`XDeleteDate` datetime DEFAULT NULL,
`XLastEditUser` TEXT DEFAULT NULL,
`XCreationUser` TEXT DEFAULT NULL,
PRIMARY KEY (`Id`)
);

DROP TABLE IF EXISTS `product_session_attendance`;

CREATE TABLE `product_session_attendance` (
`Id` char(36) NOT NULL,
`IdSession` char(36) NOT NULL,
`IdCustomer` char(36) DEFAULT NULL,
`CustomerFullName` TEXT COLLATE NOCASE,
`Present` bit(1) NOT NULL DEFAULT 0,
`XCreateDate` datetime DEFAULT NULL,
`XUpdateDate` datetime DEFAULT NULL,
`XDeleteDate` datetime DEFAULT NULL,
`XLastEditUser` TEXT DEFAULT NULL,
`XCreationUser` TEXT DEFAULT NULL,
PRIMARY KEY (`Id`)
);
