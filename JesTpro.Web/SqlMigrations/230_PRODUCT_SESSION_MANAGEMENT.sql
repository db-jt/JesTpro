DROP TABLE IF EXISTS `product_session`;

CREATE TABLE `product_session` (
  `Id` char(36) NOT NULL,
  `IdProduct` char(36) NOT NULL,
  `IdTeacher` char(36) NOT NULL,
  `Description` longtext,
  `XCreateDate` datetime DEFAULT NULL,
  `XUpdateDate` datetime DEFAULT NULL,
  `XDeleteDate` datetime DEFAULT NULL,
  `XLastEditUser` varchar(45) DEFAULT NULL,
  `XCreationUser` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

DROP TABLE IF EXISTS `product_session_attendance`;

CREATE TABLE `product_session_attendance` (
  `Id` char(36) NOT NULL,
  `IdSession` char(36) NOT NULL,
  `IdCustomer` char(36) NULL,
  `CustomerFullName` text, 
  `Present` bit(1) NOT NULL DEFAULT 0,
  `XCreateDate` datetime DEFAULT NULL,
  `XUpdateDate` datetime DEFAULT NULL,
  `XDeleteDate` datetime DEFAULT NULL,
  `XLastEditUser` varchar(45) DEFAULT NULL,
  `XCreationUser` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;