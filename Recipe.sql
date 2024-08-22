DROP TABLE recipe_categories_join;
DROP TABLE recipes;
/*
recipes is the child table of recipe_courses, recipe_cuisines, and recipe_difficulties

the app will not allow the deletion of recipe_courses, recipe_cuisines, recipe_difficulties, and recipe_categories

i plan to allow for editing and adding of cuisines, and categories 

No edit or add of difficulties or courses

Future note: primary keys best to be unsigned bigint-- why waste half the points
RecipeId bigint unsigned NOT NULL AUTO_INCREMENT
*/
CREATE TABLE recipes (
	RecipeId int NOT NULL AUTO_INCREMENT,
    RecipeTitle varchar(100) NOT NULL,
    RecipeDescription varchar(200),
    CourseId int,
    FOREIGN KEY (CourseId) REFERENCES recipe_courses(CourseId) ON UPDATE CASCADE ON DELETE RESTRICT,
    CuisineId int,
    FOREIGN KEY (CuisineId) REFERENCES recipe_cuisines(CuisineId) ON UPDATE CASCADE ON DELETE RESTRICT,
    DifficultyId int,
    FOREIGN KEY (DifficultyId) REFERENCES recipe_difficulties(DifficultyId) ON UPDATE CASCADE ON DELETE RESTRICT,
    PrepTime smallint unsigned NOT NULL,
    CookTime smallint unsigned NOT NULL,
    Rating tinyint unsigned NOT NULL,
    Servings tinyint unsigned NOT NULL,
    Nutrition varchar(2000),
    Ingredients varchar(3000) NOT NULL,
    Instructions varchar(3000) NOT NULL,
    Notes varchar(2000),
    PRIMARY KEY (RecipeId)
);

alter table recipes add fulltext index `fulltext`(RecipeTitle, Ingredients);

show create table recipes;

/*
MANY TO MANY
One recipe can have many categories
One category can have many recipies

*/
DROP TABLE recipe_categories;
CREATE TABLE recipe_categories (
	CategoryId int NOT NULL AUTO_INCREMENT,
    Category varchar(50) NOT NULL,
    PRIMARY KEY (CategoryId)
);

INSERT INTO recipe_categories (Category) 
VALUES ("Beverages"),("Sides"),("Breakfast"),
("Lunch"),("Brunch"),("Dinner"),
("Breads"),("Appetizers"),("Main Dish"),
("Dessert"),("Soups"),("Stews & Chili"),
("Pasta, Sauces, & Noodles"),("Salad & Dressings"),("Grilling"),
("Smoked"),("Burgers"),("Sandwiches"),
("Pizza"),("Slow & Pressure Cooker"),("Skillet & Stir-Fries"),
("Oven Baked & Broiled"),("Beans, Grains, & Rice"),("Casseroles"),
("Diet"),("Meatless & Vegan"),("Poultry"),
("Beef"),("Pork"),("Lamb"),
("Duck"),("Turkey"),("Sausages"),
("Seafood"),("Fruit"),("Vegetables"),
("Brownies"),("Cookies & Biscuits"),("Cakes & Cupcakes"),
("Custards & Puddings"),("Pies, Tarts, Cobblers, & Crisp"),("Chocolates & Candie"),
("Pastries"),("Frozen");

CREATE TABLE recipe_categories_join (
	RecipeId int,
    CategoryId int,
	PRIMARY KEY (RecipeId, CategoryId),
    FOREIGN KEY (RecipeId) REFERENCES recipes(RecipeId),
    FOREIGN KEY (CategoryId) REFERENCES recipe_categories(CategoryId)
);

/*
ONE TO MANY -- once recipe can have only one course but one course can have many recipes

Parent tables to recipes
*/
DROP TABLE recipe_courses;
CREATE TABLE recipe_courses (
	CourseId int NOT NULL AUTO_INCREMENT,
    Course varchar(50) NOT NULL,
    PRIMARY KEY (CourseId)
);

INSERT INTO recipe_courses (Course) 
VALUES ("Hors D'Oeuvre"),("Amuse-Bouche"),("Soup"),("Appetizer"),("Salad"),("Palate Cleanser"),("Main Course"),("Dessert"),("Mignardise");

DROP TABLE recipe_cuisines;
CREATE TABLE recipe_cuisines (
	CuisineId INT NOT NULL AUTO_INCREMENT,
    Cuisine varchar(50) NOT NULL,
    PRIMARY KEY (CuisineId)
);

INSERT INTO recipe_cuisines (Cuisine)
VALUES ("Italian"),("Indian"),("Mexican"),
("Japanese"),("French"),("Chinese"),
("Middle Eastern"),("Thai"),("Greek"),
("Brazilian"),("Spanish"),("Vietnamese"),
("Korean"),("African"),("Caribbean"),("American"),("Russian");

INSERT INTO recipe_cuisines (cuisine) VALUES ("SOME OTHER VALUE");

DROP TABLE recipe_difficulties;
CREATE TABLE recipe_difficulties (
	DifficultyId INT NOT NULL AUTO_INCREMENT,
    Difficulty varchar(25) NOT NULL,
    PRIMARY KEY (DifficultyId)
);

INSERT INTO recipe_difficulties (Difficulty)
VALUES ("Easy"),("Medium"),("Hard"),("Pro Chef")

