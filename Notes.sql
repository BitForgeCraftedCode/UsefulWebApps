DROP TABLE notes;

CREATE TABLE `notes` (
  `Id` bigint UNSIGNED NOT NULL AUTO_INCREMENT,
  `Note` varchar(5000) NOT NULL,
  `UserId` varchar(255) NOT NULL,
  `NoteTitle` varchar(100) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;





INSERT INTO notes (Note, UserId, NoteTitle) VALUES ("This can be a 5,000 char note about things i am thinking of.", "251d80ae-93a3-401c-9be9-1ef83e30d541", "Whats on my mind");
INSERT INTO notes (Note, UserId, NoteTitle) VALUES ("Note reminding me to change the Honda's oil at 190,000 miles", "251d80ae-93a3-401c-9be9-1ef83e30d541", "Change Oil At");

INSERT INTO notes (Note, UserId, NoteTitle) VALUES ("This can be a 5,000 char note about things i am thinking of.", "538fed59-838e-4041-b957-7566a480f11e", "Whats on my ducks mind");
INSERT INTO notes (Note, UserId, NoteTitle) VALUES ("clean the ferrit cage and alos need to clean bathroom and mow the lawn", "538fed59-838e-4041-b957-7566a480f11e", "annoying shit i have to do");


SELECT * FROM notes WHERE UserId = '251d80ae-93a3-401c-9be9-1ef83e30d541';

DELETE FROM usefulwebapps.to_do_list WHERE ID>=1;

ALTER TABLE usefulwebapps.to_do_list AUTO_INCREMENT = 1;