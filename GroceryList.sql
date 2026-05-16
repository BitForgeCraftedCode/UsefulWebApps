-- New grocery lists parent table
CREATE TABLE `grocery_lists` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `UserId` varchar(255) NOT NULL,
  `ListTitle` varchar(100) NOT NULL,
  `Version` int unsigned NOT NULL DEFAULT '0',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `FK_grocery_lists_user` (`UserId`),
  CONSTRAINT `FK_grocery_lists_user` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Grocery list items 
CREATE TABLE `grocery_list_items` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ListId` bigint unsigned NOT NULL,
  `GroceryItem` varchar(100) NOT NULL,
  `Category` varchar(50) NOT NULL,
  `Complete` tinyint(1) NOT NULL DEFAULT '0',
  `SortOrder` int unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `IX_grocery_items_listid` (`ListId`),
  CONSTRAINT `FK_grocery_items_list` FOREIGN KEY (`ListId`) REFERENCES `grocery_lists` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Sharing table 
CREATE TABLE `grocery_list_shares` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `ListId` bigint unsigned NOT NULL,
  `SharedWithUserId` varchar(255) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQ_grocerylist_share` (`ListId`,`SharedWithUserId`),
  KEY `IX_grocery_shares_listid` (`ListId`),
  KEY `IX_grocery_shares_user` (`SharedWithUserId`),
  CONSTRAINT `FK_grocery_shares_list` FOREIGN KEY (`ListId`) REFERENCES `grocery_lists` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_grocery_shares_user` FOREIGN KEY (`SharedWithUserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- Template table
CREATE TABLE `grocery_list_templates` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `UserId` varchar(255) NOT NULL,
  `GroceryItem` varchar(100) NOT NULL,
  `Category` varchar(50) NOT NULL,
  `Complete` tinyint(1) NOT NULL DEFAULT '0',
  `SortOrder` int unsigned NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`),
  KEY `IX_grocery_templates_user` (`UserId`),
  CONSTRAINT `FK_grocery_templates_user` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `grocery_categories` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Category` varchar(50) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
