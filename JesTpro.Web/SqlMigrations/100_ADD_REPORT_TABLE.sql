CREATE TABLE `report` (
  `Id` char(36) NOT NULL,
  `Family` int(11) NOT NULL,
  `Name` varchar(256) NOT NULL,
  `Description` longtext NOT NULL,
  `ColumnMap` mediumtext NULL,
  `ParameterMap` mediumtext NULL,
  `Value` longtext NULL,
  `Enabled` bit(1) NOT NULL,
  `XCreateDate` datetime DEFAULT NULL,
  `XUpdateDate` datetime DEFAULT NULL,
  `XDeleteDate` datetime DEFAULT NULL,
  `XLastEditUser` varchar(45) DEFAULT NULL,
  `XCreationUser` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `Id_UNIQUE` (`Id`)
);