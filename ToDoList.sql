DROP TABLE to_do_list;

CREATE TABLE `to_do_list` (
  `Id` bigint UNSIGNED NOT NULL AUTO_INCREMENT,
  `ToDoItem` varchar(100) NOT NULL,
  `Complete` BOOLEAN NOT NULL,
  `UserId` varchar(255) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=10 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;



ALTER TABLE to_do_list ADD UserId varchar(255) NOT NULL;

INSERT INTO to_do_list (ToDoItem, Complete, UserId) VALUES ("Buy Beer", False, "818fd1e7-05ab-44f5-9276-68f20ec3c70d");
INSERT INTO to_do_list (ToDoItem, Complete, UserId) VALUES ("Clean Pets", True, "818fd1e7-05ab-44f5-9276-68f20ec3c70d");
INSERT INTO to_do_list (ToDoItem, Complete, UserId) VALUES ("Do Homework", False, "818fd1e7-05ab-44f5-9276-68f20ec3c70d");
INSERT INTO to_do_list (ToDoItem, Complete, UserId) VALUES ("Build Apps", False, "818fd1e7-05ab-44f5-9276-68f20ec3c70d");

INSERT INTO to_do_list (ToDoItem, Complete, UserId) VALUES ("mow lawn", False, "19e5b54f-e998-4494-90a2-797cfcfc9fc3");
INSERT INTO to_do_list (ToDoItem, Complete, UserId) VALUES ("fix sink", True, "19e5b54f-e998-4494-90a2-797cfcfc9fc3");
INSERT INTO to_do_list (ToDoItem, Complete, UserId) VALUES ("change oil", False, "19e5b54f-e998-4494-90a2-797cfcfc9fc3");
INSERT INTO to_do_list (ToDoItem, Complete, UserId) VALUES ("drink beer", False, "19e5b54f-e998-4494-90a2-797cfcfc9fc3");

SELECT * FROM to_do_list WHERE UserId = '818fd1e7-05ab-44f5-9276-68f20ec3c70d';

DELETE FROM usefulwebapps.to_do_list WHERE ID>=1;

ALTER TABLE usefulwebapps.to_do_list AUTO_INCREMENT = 1;

/*adding multiple to do lists for users*/
/*1st delete all current to do lists -- sorry*/
DELETE FROM to_do_list;
/*2nd add a title col -- each list will have a title (only way i can think to do this. Not perfect but will do)*/
ALTER TABLE to_do_list ADD ListTitle varchar(100) NOT NULL;

INSERT INTO to_do_list (ToDoItem, Complete, UserId, ListTitle) VALUES ("Buy Beer", False, "251d80ae-93a3-401c-9be9-1ef83e30d541", "Shopping");
INSERT INTO to_do_list (ToDoItem, Complete, UserId, ListTitle) VALUES ("Buy whiskey", False, "251d80ae-93a3-401c-9be9-1ef83e30d541", "Shopping");
INSERT INTO to_do_list (ToDoItem, Complete, UserId, ListTitle) VALUES ("gift for work party", False, "251d80ae-93a3-401c-9be9-1ef83e30d541", "Shopping");

INSERT INTO to_do_list (ToDoItem, Complete, UserId, ListTitle) VALUES ("change generator oil", False, "251d80ae-93a3-401c-9be9-1ef83e30d541", "House Work");
INSERT INTO to_do_list (ToDoItem, Complete, UserId, ListTitle) VALUES ("paint bathroom", False, "251d80ae-93a3-401c-9be9-1ef83e30d541", "House Work");
INSERT INTO to_do_list (ToDoItem, Complete, UserId, ListTitle) VALUES ("build shoe rack", False, "251d80ae-93a3-401c-9be9-1ef83e30d541", "House Work");

INSERT INTO to_do_list (ToDoItem, Complete, UserId, ListTitle) VALUES ("Laundry", False, "538fed59-838e-4041-b957-7566a480f11e", "Chores");
INSERT INTO to_do_list (ToDoItem, Complete, UserId, ListTitle) VALUES ("Cook Dinner", False, "538fed59-838e-4041-b957-7566a480f11e", "Chores");
INSERT INTO to_do_list (ToDoItem, Complete, UserId, ListTitle) VALUES ("Take Audrey to Dr", False, "538fed59-838e-4041-b957-7566a480f11e", "Chores");