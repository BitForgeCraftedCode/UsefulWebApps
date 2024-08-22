DROP TABLE grocery_list;

CREATE TABLE grocery_list (
	Id int NOT NULL AUTO_INCREMENT,
    GroceryItem varchar(100) NOT NULL,
    Category varchar(50) NOT NULL,
    Complete BOOLEAN NOT NULL,
    PRIMARY KEY (Id)
);

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

INSERT INTO grocery_list (GroceryItem, Category, Complete) VALUES ("Buy Beer", "Beverages", False);
INSERT INTO grocery_list (GroceryItem, Category, Complete) VALUES ("Chicken Nuggets", "Frozen", False);
INSERT INTO grocery_list (GroceryItem, Category, Complete) VALUES ("Dish soap", "Household", False);
INSERT INTO grocery_list (GroceryItem, Category, Complete) VALUES ("Black beans 2 15oz cans", "Canned", False);
INSERT INTO grocery_list (GroceryItem, Category, Complete) VALUES ("Pizza", "Frozen", False);
INSERT INTO grocery_list (GroceryItem, Category, Complete) VALUES ("Sponges", "Household", False);


DELETE FROM usefulwebapps.grocery_list WHERE Id >= 1;

ALTER TABLE usefulwebapps.grocery_list AUTO_INCREMENT = 1;