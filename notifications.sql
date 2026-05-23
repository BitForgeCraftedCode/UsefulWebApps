CREATE TABLE `notifications` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `UserId` varchar(255) NOT NULL,
  `SenderUserId` varchar(255) DEFAULT NULL,
  `Message` text NOT NULL,
  `NotificationType` varchar(100) DEFAULT NULL,
  `RelatedEntityId` bigint unsigned DEFAULT NULL,
  `IsRead` tinyint(1) NOT NULL DEFAULT '0',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `idx_notifications_user_read` (`UserId`,`IsRead`),
  KEY `idx_notifications_created` (`CreatedAt`),
  KEY `fk_notifications_sender` (`SenderUserId`),
  CONSTRAINT `fk_notifications_sender` FOREIGN KEY (`SenderUserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE SET NULL,
  CONSTRAINT `fk_notifications_user` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci