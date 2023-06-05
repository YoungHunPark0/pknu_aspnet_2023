CREATE TABLE `todoitems` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Title` varchar(100) NOT NULL,
  `TodoDate` varchar(30) NOT NULL,
  `IsComplete` tinyint(1) DEFAULT NULL,
  PRIMARY KEY (`Id`)
);
