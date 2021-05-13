CREATE TABLE `attachment` (
  `Id` char(36) NOT NULL,
  `IdResource` char(36) NOT NULL,
  `Name` varchar(256) NOT NULL,
  `Size` bigint NOT NULL,
  `FullPath` mediumtext,
  `XCreateDate` datetime DEFAULT NULL,
  `XUpdateDate` datetime DEFAULT NULL,
  `XDeleteDate` datetime DEFAULT NULL,
  `XLastEditUser` varchar(45) DEFAULT NULL,
  `XCreationUser` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
