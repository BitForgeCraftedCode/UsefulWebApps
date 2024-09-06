DROP TABLE recipe_categories_join;
DROP TABLE recipes;
/*
recipes is the child table of recipe_courses, recipe_cuisines, and recipe_difficulties

the app will not allow the deletion of recipe_courses, recipe_cuisines, recipe_difficulties, and recipe_categories

i plan to allow for editing and adding of cuisines, and categories 

No edit or add of difficulties or courses

Future note: primary keys best to be unsigned bigint-- why waste half the points
RecipeId bigint unsigned NOT NULL AUTO_INCREMENT

to back-up/restore use command prompt not power shell

to back-up:

mysqldump -u root -p usefulwebapps > C:\MySQLBackup\usefulwebapps_2024_09_03.sql

to restore:

mysql -u root -p usefulwebapps < C:\MySQLBackup\usefulwebapps_2024_09_03.sql

*/
CREATE TABLE `recipes` (
  `RecipeId` bigint unsigned NOT NULL AUTO_INCREMENT,
  `RecipeTitle` varchar(100) NOT NULL,
  `RecipeDescription` varchar(200) DEFAULT NULL,
  `CourseId` bigint unsigned DEFAULT NULL,
  `CuisineId` bigint unsigned DEFAULT NULL,
  `DifficultyId` bigint unsigned DEFAULT NULL,
  `PrepTime` smallint unsigned NOT NULL,
  `CookTime` smallint unsigned NOT NULL,
  `Rating` tinyint unsigned NOT NULL,
  `Servings` tinyint unsigned NOT NULL,
  `Nutrition` varchar(2000) DEFAULT NULL,
  `Ingredients` varchar(3000) NOT NULL,
  `Instructions` varchar(3000) NOT NULL,
  `Notes` varchar(2000) DEFAULT NULL,
  `UserId` varchar(255) NOT NULL,
  PRIMARY KEY (`RecipeId`),
  KEY `CourseId` (`CourseId`),
  KEY `CuisineId` (`CuisineId`),
  KEY `DifficultyId` (`DifficultyId`),
  FULLTEXT KEY `fulltext` (`RecipeTitle`,`Ingredients`),
  CONSTRAINT `recipes_ibfk_1` FOREIGN KEY (`CourseId`) REFERENCES `recipe_courses` (`CourseId`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `recipes_ibfk_2` FOREIGN KEY (`CuisineId`) REFERENCES `recipe_cuisines` (`CuisineId`) ON DELETE RESTRICT ON UPDATE CASCADE,
  CONSTRAINT `recipes_ibfk_3` FOREIGN KEY (`DifficultyId`) REFERENCES `recipe_difficulties` (`DifficultyId`) ON DELETE RESTRICT ON UPDATE CASCADE
);

ALTER TABLE recipes add fulltext index `fulltext`(RecipeTitle, Ingredients);

ALTER TABLE recipes ADD UserId varchar(255) NOT NULL;

ALTER TABLE recipes ADD UserName varchar(256) NOT NULL;

show create table recipes;

UPDATE recipes SET UserId = '19e5b54f-e998-4494-90a2-797cfcfc9fc3', UserName = 'BeefCakeTheMightyStandardUser' WHERE RecipeId = 1;
UPDATE recipes SET UserId = '19e5b54f-e998-4494-90a2-797cfcfc9fc3', UserName = 'BeefCakeTheMightyStandardUser' WHERE RecipeId = 2;
UPDATE recipes SET UserId = '19e5b54f-e998-4494-90a2-797cfcfc9fc3', UserName = 'BeefCakeTheMightyStandardUser' WHERE RecipeId = 3;
UPDATE recipes SET UserId = '19e5b54f-e998-4494-90a2-797cfcfc9fc3', UserName = 'BeefCakeTheMightyStandardUser' WHERE RecipeId = 4;
UPDATE recipes SET UserId = '19e5b54f-e998-4494-90a2-797cfcfc9fc3', UserName = 'BeefCakeTheMightyStandardUser' WHERE RecipeId = 15;
UPDATE recipes SET UserId = '19e5b54f-e998-4494-90a2-797cfcfc9fc3', UserName = 'BeefCakeTheMightyStandardUser' WHERE RecipeId = 16;
UPDATE recipes SET UserId = '19e5b54f-e998-4494-90a2-797cfcfc9fc3', UserName = 'BeefCakeTheMightyStandardUser' WHERE RecipeId = 17;

UPDATE recipes SET UserId = '818fd1e7-05ab-44f5-9276-68f20ec3c70d', UserName = 'BeefCakeTheMightyAdmin' WHERE RecipeId = 18;
UPDATE recipes SET UserId = '818fd1e7-05ab-44f5-9276-68f20ec3c70d', UserName = 'BeefCakeTheMightyAdmin' WHERE RecipeId = 19;
UPDATE recipes SET UserId = '818fd1e7-05ab-44f5-9276-68f20ec3c70d', UserName = 'BeefCakeTheMightyAdmin' WHERE RecipeId = 20;
UPDATE recipes SET UserId = '818fd1e7-05ab-44f5-9276-68f20ec3c70d', UserName = 'BeefCakeTheMightyAdmin' WHERE RecipeId = 21;
UPDATE recipes SET UserId = '818fd1e7-05ab-44f5-9276-68f20ec3c70d', UserName = 'BeefCakeTheMightyAdmin' WHERE RecipeId = 22;

/*
MANY TO MANY
One recipe can have many categories
One category can have many recipies

*/
DROP TABLE recipe_categories;
CREATE TABLE `recipe_categories` (
  `CategoryId` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Category` varchar(50) NOT NULL,
  PRIMARY KEY (`CategoryId`)
);

INSERT INTO recipe_categories (Category) 
VALUES ("Beverages"),("Sides"),("Breakfast"),
("Lunch"),("Brunch"),("Dinner"),
("Breads"),("Appetizers"),("Main Dish"),
("Dessert"),("Soups"),("Stews & Chili"),
("Pasta, Sauces, & Noodles"),("Salad & Dressings"),("Grilling"),
("Smoked"),("Burgers"),("Sandwiches"),
("Pizza"),("Slow & Pressure Cooker"),("Skillet & Stir-Fries"),
("Oven Baked & Broiled"),("Beans, Grains, & Rice"),("Tofu"),("Casseroles"),
("Diet"),("Meatless & Vegan"),("Eggs"),
("Poultry"),("Chicken"),("Duck"),("Turkey"),
("Beef"),("Veal"),("Pork"),("Lamb"),("Sausages"),("Other Meat - wild game etc."),
("Fish and Seafood"),("Nuts and Seeds"),
("Fruit"),("Vegetables"),
("Brownies"),("Cookies & Biscuits"),("Cakes & Cupcakes"),
("Custards & Puddings"),("Pies, Tarts, Cobblers, & Crisp"),("Chocolates & Candie"),
("Pastries"),("Frozen");

DROP TABLE recipe_categories_join;
CREATE TABLE `recipe_categories_join` (
  `RecipeId` bigint unsigned NOT NULL,
  `CategoryId` bigint unsigned NOT NULL,
  PRIMARY KEY (`RecipeId`,`CategoryId`),
  KEY `CategoryId` (`CategoryId`),
  CONSTRAINT `recipe_categories_join_ibfk_1` FOREIGN KEY (`RecipeId`) REFERENCES `recipes` (`RecipeId`),
  CONSTRAINT `recipe_categories_join_ibfk_2` FOREIGN KEY (`CategoryId`) REFERENCES `recipe_categories` (`CategoryId`)
);

/*
ONE TO MANY -- once recipe can have only one course but one course can have many recipes

Parent tables to recipes
*/
DROP TABLE recipe_courses;
CREATE TABLE `recipe_courses` (
  `CourseId` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Course` varchar(50) NOT NULL,
  PRIMARY KEY (`CourseId`)
);

INSERT INTO recipe_courses (Course) 
VALUES ("Hors D'Oeuvre"),("Amuse-Bouche"),("Soup"),("Appetizer"),("Salad"),("Palate Cleanser"),("Main Course"),("Dessert"),("Mignardise");

DROP TABLE recipe_cuisines;
CREATE TABLE `recipe_cuisines` (
  `CuisineId` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Cuisine` varchar(50) NOT NULL,
  PRIMARY KEY (`CuisineId`)
);

INSERT INTO recipe_cuisines (Cuisine)
VALUES ("Italian"),("Indian"),("Mexican"),
("Japanese"),("French"),("Chinese"),
("Middle Eastern"),("Thai"),("Greek"),
("Brazilian"),("Spanish"),("Vietnamese"),
("Korean"),("African"),("Caribbean"),("American"),("Russian");

INSERT INTO recipe_cuisines (cuisine) VALUES ("SOME OTHER VALUE");

DROP TABLE recipe_difficulties;
CREATE TABLE `recipe_difficulties` (
  `DifficultyId` bigint unsigned NOT NULL AUTO_INCREMENT,
  `Difficulty` varchar(25) NOT NULL,
  PRIMARY KEY (`DifficultyId`)
);

INSERT INTO recipe_difficulties (Difficulty)
VALUES ("Easy"),("Medium"),("Hard"),("Pro Chef");
/*
https://stackoverflow.com/questions/1481476/when-to-use-on-update-cascade

comments is the child of recipes

one comment can have only 1 recipe but 1 recipe can have many comments ONE TO MANY

ON DELETE CASCADE to delete all comments if a recipe is deleted 
*/
CREATE TABLE `recipe_comments`(
	`CommentId` bigint unsigned NOT NULL AUTO_INCREMENT,
    `Comment` varchar(1000) NOT NULL,
    `UserId` varchar(255) NOT NULL,
    `UserName` varchar(256) NOT NULL,
    `RecipeId` bigint unsigned DEFAULT NULL,
    PRIMARY KEY (`CommentId`),
    KEY `RecipeId` (`RecipeId`),
    CONSTRAINT `recipe_comments_ibfk_1` FOREIGN KEY (`RecipeId`) REFERENCES `recipes` (`RecipeId`) ON DELETE CASCADE ON UPDATE CASCADE
);

CREATE TABLE `recipe_usersaved`(
	`UserSavedId` bigint unsigned NOT NULL AUTO_INCREMENT,
    `UserId` varchar(255) NOT NULL,
    `UserName` varchar(256) NOT NULL,
    `RecipeId` bigint unsigned DEFAULT NULL,
    PRIMARY KEY (`UserSavedId`),
    KEY `RecipeId` (`RecipeId`),
    CONSTRAINT `recipe_usersaved_ibfk_1` FOREIGN KEY (`RecipeId`) REFERENCES `recipes` (`RecipeId`) ON DELETE CASCADE ON UPDATE CASCADE
);

ALTER TABLE recipe_usersaved ADD RecipeTitle varchar(100) NOT NULL;


