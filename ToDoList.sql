/*
parent of to_do_items

One to_do_lists row can have many to_do_items 

One to_do_lists row can have many to_do_list_shares


*/

CREATE TABLE `to_do_lists` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `UserId` varchar(255) NOT NULL,
  `ListTitle` varchar(100) NOT NULL,
  `Version` int unsigned NOT NULL DEFAULT '0',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `FK_to_do_lists_user` (`UserId`),
  CONSTRAINT `FK_to_do_lists_user` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `to_do_items` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ListId` bigint unsigned NOT NULL,
  `ToDoItem` varchar(100) NOT NULL,
  `Complete` tinyint(1) NOT NULL DEFAULT '0',
  `SortOrder` int unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `IX_items_listid` (`ListId`),
  CONSTRAINT `FK_items_list` FOREIGN KEY (`ListId`) REFERENCES `to_do_lists` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `to_do_list_shares` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ListId` bigint unsigned NOT NULL,
  `SharedWithUserId` varchar(255) NOT NULL, -- the friend being shared TO
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQ_todolist_share` (`ListId`,`SharedWithUserId`),
  KEY `IX_shares_listid` (`ListId`),
  KEY `IX_shares_user` (`SharedWithUserId`),
  CONSTRAINT `FK_todolist_shares_list` FOREIGN KEY (`ListId`) REFERENCES `to_do_lists` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_todolist_shares_user` FOREIGN KEY (`SharedWithUserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

