/*
one aspnetusers can have many friendships, note_shares
One to Many Relationship 

aspnetusers is parent friendships, note_shares are children 
child tables get the foreign key that references primary key in parent

if an aspnetusers gets deleted frendships and note_shares cascade away however notes currently does not. either do that in delete-user logic in the app or add 
*/
CREATE TABLE `friendships` (
  `Id`          bigint unsigned NOT NULL AUTO_INCREMENT,
  `RequesterUserId` varchar(255) NOT NULL,   -- the user who sent the request
  `AddresseeUserId` varchar(255) NOT NULL,   -- the user who received it
  `Status`      tinyint NOT NULL DEFAULT 0,
                -- 0 = Pending, 1 = Accepted, 2 = Declined/Blocked
  `CreatedAt`   datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt`   datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQ_friendship` (`RequesterUserId`, `AddresseeUserId`), -- prevents duplicates -- also covers RequesterUserId lookups
  KEY `idx_addressee` (`AddresseeUserId`), -- needed, not covered by above
  CONSTRAINT `FK_friendships_requester` FOREIGN KEY (`RequesterUserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_friendships_addressee` FOREIGN KEY (`AddresseeUserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

CREATE TABLE `user_profiles` (
  `UserId`       varchar(255) NOT NULL,
  `DisplayName`  varchar(100) DEFAULT NULL,
  `AvatarPath`   varchar(500) DEFAULT NULL,  -- store file path or filename
  `CreatedAt`    datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt`    datetime NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`UserId`),
  CONSTRAINT `FK_user_profiles_user` FOREIGN KEY (`UserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- creates a profile row for every existing user in one shot will need to run this on Ubuntu server
INSERT INTO `user_profiles` (`UserId`, `DisplayName`, `AvatarPath`)
SELECT `Id`, `UserName`, NULL
FROM `aspnetusers`;

CREATE TABLE `note_shares` (
  `Id`     bigint unsigned NOT NULL AUTO_INCREMENT,
  `NoteId` bigint unsigned NOT NULL,
  `SharedWithUserId` varchar(255) NOT NULL,   -- the friend being shared TO
  PRIMARY KEY (`Id`),
  UNIQUE KEY `UQ_note_share` (`NoteId`, `SharedWithUserId`),
  CONSTRAINT `FK_note_shares_note` FOREIGN KEY (`NoteId`) REFERENCES `notes` (`Id`) ON DELETE CASCADE,
  CONSTRAINT `FK_note_shares_user` FOREIGN KEY (`SharedWithUserId`) REFERENCES `aspnetusers` (`Id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

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
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=41 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci

