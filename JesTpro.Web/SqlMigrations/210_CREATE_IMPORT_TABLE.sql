CREATE TABLE `massive_request` (
  `Id` char(36) NOT NULL,
  `Description` longtext,
  `ImportStatus` int(11) NOT NULL,
  `ImportType` int(11) NOT NULL,
  `FileToImport` varchar(200) DEFAULT NULL,
  `Error` longtext,
  `LastExecution` datetime(6) DEFAULT NULL,
  `XCreateDate` datetime DEFAULT NULL,
  `XUpdateDate` datetime DEFAULT NULL,
  `XDeleteDate` datetime DEFAULT NULL,
  `XLastEditUser` varchar(45) DEFAULT NULL,
  `XCreationUser` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;
