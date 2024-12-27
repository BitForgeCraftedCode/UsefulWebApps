DROP TABLE grocery_list;

CREATE TABLE `grocery_list_usersaved` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `GroceryItem` varchar(100) NOT NULL,
  `Category` varchar(50) NOT NULL,
  `Complete` BOOLEAN NOT NULL,
  `UserId` varchar(255) NOT NULL,
  `SortOrder` int unsigned NOT NULL DEFAULT '1',
  PRIMARY KEY (`Id`)
);

CREATE TABLE `grocery_list` (
  `Id` bigint unsigned NOT NULL AUTO_INCREMENT,
  `GroceryItem` varchar(100) NOT NULL,
  `Category` varchar(50) NOT NULL,
  `Complete` BOOLEAN NOT NULL,
  `UserId` varchar(255) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

ALTER TABLE grocery_list ADD UserId varchar(255) NOT NULL;

ALTER TABLE grocery_list MODIFY Id bigint UNSIGNED NOT NULL AUTO_INCREMENT; 

ALTER TABLE grocery_list ADD SortOrder int unsigned NOT NULL DEFAULT 1;

ALTER TABLE grocery_list ADD ShareUserId varchar(255);

CREATE TABLE grocery_categories (
	Id int NOT NULL AUTO_INCREMENT,
    Category varchar(50) NOT NULL,
    PRIMARY KEY (Id)
);

INSERT INTO grocery_categories (Category) VALUES ("Produce");
INSERT INTO grocery_categories (Category) VALUES ("Meat");
INSERT INTO grocery_categories (Category) VALUES ("Dairy");
INSERT INTO grocery_categories (Category) VALUES ("Deli");
INSERT INTO grocery_categories (Category) VALUES ("Canned");
INSERT INTO grocery_categories (Category) VALUES ("Pantry");
INSERT INTO grocery_categories (Category) VALUES ("Snacks");
INSERT INTO grocery_categories (Category) VALUES ("Bakery");
INSERT INTO grocery_categories (Category) VALUES ("Beverages");
INSERT INTO grocery_categories (Category) VALUES ("Paper Goods");
INSERT INTO grocery_categories (Category) VALUES ("Personal Care");
INSERT INTO grocery_categories (Category) VALUES ("Cleaners");
INSERT INTO grocery_categories (Category) VALUES ("Household");
INSERT INTO grocery_categories (Category) VALUES ("Toiletry");
INSERT INTO grocery_categories (Category) VALUES ("Frozen");
INSERT INTO grocery_categories (Category) VALUES ("Dry/Baking");
INSERT INTO grocery_categories (Category) VALUES ("Other");

INSERT INTO grocery_list (GroceryItem, Category, Complete, UserId) VALUES ("Buy Beer", "Beverages", False, "818fd1e7-05ab-44f5-9276-68f20ec3c70d");
INSERT INTO grocery_list (GroceryItem, Category, Complete, UserId) VALUES ("Chicken Nuggets", "Frozen", False, "818fd1e7-05ab-44f5-9276-68f20ec3c70d");
INSERT INTO grocery_list (GroceryItem, Category, Complete, UserId) VALUES ("Dish soap", "Household", False, "818fd1e7-05ab-44f5-9276-68f20ec3c70d");
INSERT INTO grocery_list (GroceryItem, Category, Complete, UserId) VALUES ("Black beans 2 15oz cans", "Canned", False, "818fd1e7-05ab-44f5-9276-68f20ec3c70d");
INSERT INTO grocery_list (GroceryItem, Category, Complete, UserId) VALUES ("Pizza", "Frozen", False, "818fd1e7-05ab-44f5-9276-68f20ec3c70d");
INSERT INTO grocery_list (GroceryItem, Category, Complete, UserId) VALUES ("Sponges", "Household", False, "818fd1e7-05ab-44f5-9276-68f20ec3c70d");

INSERT INTO grocery_list (GroceryItem, Category, Complete, UserId) VALUES ("Soda", "Beverages", False, "19e5b54f-e998-4494-90a2-797cfcfc9fc3");
INSERT INTO grocery_list (GroceryItem, Category, Complete, UserId) VALUES ("Pizza", "Frozen", False, "19e5b54f-e998-4494-90a2-797cfcfc9fc3");
INSERT INTO grocery_list (GroceryItem, Category, Complete, UserId) VALUES ("Pet Food", "Household", False, "19e5b54f-e998-4494-90a2-797cfcfc9fc3");
INSERT INTO grocery_list (GroceryItem, Category, Complete, UserId) VALUES ("Diced Tomato", "Canned", False, "19e5b54f-e998-4494-90a2-797cfcfc9fc3");
INSERT INTO grocery_list (GroceryItem, Category, Complete, UserId) VALUES ("TV Dinners", "Frozen", False, "19e5b54f-e998-4494-90a2-797cfcfc9fc3");
INSERT INTO grocery_list (GroceryItem, Category, Complete, UserId) VALUES ("Paper Plates", "Household", False, "19e5b54f-e998-4494-90a2-797cfcfc9fc3");


DELETE FROM usefulwebapps.grocery_list WHERE Id >= 1;

ALTER TABLE usefulwebapps.grocery_list AUTO_INCREMENT = 1;

UPDATE grocery_list SET SortOrder = 2 WHERE Category = 'Produce' AND UserId = "251d80ae-93a3-401c-9be9-1ef83e30d541";

UPDATE grocery_list SET SortOrder = 3 WHERE Category = 'Deli';

UPDATE grocery_list SET SortOrder = 1 WHERE Category = 'Meat';

SELECT * FROM grocery_list WHERE UserId = "251d80ae-93a3-401c-9be9-1ef83e30d541" ORDER BY SortOrder ASC, Category ASC, GroceryItem ASC;

SELECT DISTINCT Category, SortOrder FROM grocery_list WHERE UserId = "251d80ae-93a3-401c-9be9-1ef83e30d541" ORDER BY SortOrder ASC, Category ASC;