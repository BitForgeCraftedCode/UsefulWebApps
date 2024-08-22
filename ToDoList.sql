DROP TABLE to_do_list;

CREATE TABLE to_do_list (
	Id int NOT NULL AUTO_INCREMENT,
    ToDoItem varchar(100) NOT NULL,
    Complete BOOLEAN NOT NULL,
    PRIMARY KEY (Id)
);

INSERT INTO to_do_list (ToDoItem, Complete) VALUES ("Buy Beer", False);
INSERT INTO to_do_list (ToDoItem, Complete) VALUES ("Clean Pets", True);
INSERT INTO to_do_list (ToDoItem, Complete) VALUES ("Do Homework", False);
INSERT INTO to_do_list (ToDoItem, Complete) VALUES ("Build Apps", False);

DELETE FROM usefulwebapps.to_do_list WHERE ID>=1;

ALTER TABLE usefulwebapps.to_do_list AUTO_INCREMENT = 1;