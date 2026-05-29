DROP TABLE notes;
/*
one notes can have many note_shares
One to many relationship

notes is the parent of note_shares
*/
CREATE TABLE `notes` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Note` mediumtext NOT NULL,
  `UserId` varchar(255) NOT NULL,
  `NoteTitle` varchar(100) NOT NULL,
  `Version` int unsigned NOT NULL DEFAULT '0',
  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  KEY `IX_notes_userid` (`UserId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `note_shares` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `NoteId` bigint unsigned NOT NULL,
  `SharedWithUserId` varchar(255) NOT NULL,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQ_note_share` (`NoteId`,`SharedWithUserId`),
  KEY `IX_note_shares_user` (`SharedWithUserId`),
  KEY `IX_note_shares_noteid` (`NoteId`),
  CONSTRAINT `FK_note_shares_note` FOREIGN KEY (`NoteId`) REFERENCES `notes` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_note_shares_user` FOREIGN KEY (`SharedWithUserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
