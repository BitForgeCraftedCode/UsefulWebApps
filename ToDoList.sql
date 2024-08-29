DROP TABLE to_do_list;

CREATE TABLE to_do_list (
	Id int NOT NULL AUTO_INCREMENT,
    ToDoItem varchar(100) NOT NULL,
    Complete BOOLEAN NOT NULL,
    PRIMARY KEY (Id)
);

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