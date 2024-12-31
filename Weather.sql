DROP TABLE locations;

CREATE TABLE `locations` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `City` varchar(500) NOT NULL,
  `Latitude` double,
  `Longitude` double,
  `Country` varchar(500) NOT NULL,
  `State` varchar(500),
  `Zip` varchar(50),
  `UserId` varchar(255) NOT NULL,
  `IsDefault` BOOLEAN NOT NULL,
  PRIMARY KEY (`Id`)
);


