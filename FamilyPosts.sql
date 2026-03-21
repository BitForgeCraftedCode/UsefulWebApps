/*
one post can have many images, comments, and reactions
One to Many Relationship 

Post is parent images, comments, and reactions are children 
child tables get the foreign key that references primary key in parent
*/
CREATE TABLE `family_posts` (
  `PostId` bigint unsigned NOT NULL AUTO_INCREMENT,

  `UserId` varchar(255) NOT NULL,
  `UserName` varchar(256) NOT NULL,

  `Content` mediumtext,

  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT NULL,
  `LastActivityAt` datetime DEFAULT NULL,

  PRIMARY KEY (`PostId`),
  KEY `IX_family_posts_UserId` (`UserId`),
  KEY `IX_family_posts_CreatedAt` (`CreatedAt`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `family_post_images` (
  `PostImageId` bigint unsigned NOT NULL AUTO_INCREMENT,

  `PostId` bigint unsigned NOT NULL,

  `ImagePath` varchar(500) NOT NULL,
  `SortOrder` int NOT NULL DEFAULT 0,

  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,

  PRIMARY KEY (`PostImageId`),
  KEY `IX_family_post_images_PostId` (`PostId`), 

  CONSTRAINT `FK_family_post_images_Post`
    FOREIGN KEY (`PostId`)
    REFERENCES `family_posts` (`PostId`)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `family_post_comments` (
  `CommentId` bigint unsigned NOT NULL AUTO_INCREMENT,

  `PostId` bigint unsigned NOT NULL,

  `UserId` varchar(255) NOT NULL,
  `UserName` varchar(256) NOT NULL,

  `Comment` varchar(3000) NOT NULL,

  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` datetime DEFAULT NULL,

  PRIMARY KEY (`CommentId`),
  KEY `IX_family_post_comments_PostId` (`PostId`), /*add index to PostId Fk for performant lookup*/

  CONSTRAINT `FK_family_post_comments_Post`
    FOREIGN KEY (`PostId`)
    REFERENCES `family_posts` (`PostId`)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE `family_post_reactions` (
  `ReactionId` bigint unsigned NOT NULL AUTO_INCREMENT,

  `PostId` bigint unsigned NOT NULL,

  `UserId` varchar(255) NOT NULL,
  `UserName` varchar(256) NOT NULL,

  `ReactionType` int NOT NULL DEFAULT 1,

  `CreatedAt` datetime NOT NULL DEFAULT CURRENT_TIMESTAMP,

  PRIMARY KEY (`ReactionId`),
  /*UNIQUE KEY to enforce one reaction per user per post*/
  UNIQUE KEY `UQ_Post_User` (`PostId`, `UserId`, `ReactionType`),
  KEY `IX_family_post_reactions_PostId` (`PostId`),

  CONSTRAINT `FK_family_post_reactions_Post`
    FOREIGN KEY (`PostId`)
    REFERENCES `family_posts` (`PostId`)
    ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;