INSERT INTO recipes (RecipeTitle, RecipeDescription, CourseId, CuisineId, DifficultyId, PrepTime, CookTime, Rating, Servings, Nutrition, Ingredients, Instructions, Notes)
VALUES ("Easy Lemon Pepper Chicken",
"These Easy Lemon Pepper Chicken Breasts are so tender and juicy.",
7,16,1,10,20,4,4,"Serving: 1serving Calories: 253.4kcal Carbohydrates: 3.53g Protein: 34.45g Fat: 10.43g Sodium: 656.68mg Fiber: 0.15g",
"2 boneless skinless chicken breasts
2 Tbsp all-purpose flour
1 Tbsp lemon pepper seasoning
1 Tbsp cooking oil
1 clove garlic, minced
1/2 cup chicken broth
1 Tbsp butter
1 tsp lemon juice
1 Tbsp chopped fresh parsley (optional)
1/8 tsp freshly cracked black pepper",
"1 Use a sharp knife to carefully fillet the chicken breasts into two thinner peices (or use thin-cut chicken breasts).
2 Combine the flour and lemon pepper seasoning in a bowl. Sprinkle the mixture over both sides of the chicken breast pieces and then rub it in until the chicken is fully coated.
3 Heat the cooking oil in a large skillet over medium. When the skillet and oil are very hot, add the chicken and cook on each side until golden brown (about 5 minutes per side). Remove the cooked chicken to a clean plate and cover to keep warm.
4 Add the butter and minced garlic to the skillet and sauté for about one minute.
5 Add the chicken broth to the skillet and whisk to dissolve all the browned bits from the bottom of the skillet. Add the lemon juice and allow the sauce to simmer in the skillet for 3-5 minutes, or until it has reduced slightly. Taste the sauce and add salt if needed (I did not add any).
6 Finally, return the chicken to the skillet and spoon the sauce over top. Allow the chicken to heat through. Season with a little freshly cracked pepper and fresh chopped parsley (optional), then serve.",
"Cook this now!!!");

INSERT INTO recipe_categories_join VALUES (1,6),(1,21),(1,27);

INSERT INTO recipes (RecipeTitle, RecipeDescription, CourseId, CuisineId, DifficultyId, PrepTime, CookTime, Rating, Servings, Nutrition, Ingredients, Instructions, Notes)
VALUES ("Easy Buffalo Chicken",
"This spicy buffalo chicken is so easy i make it all the time",
7,16,1,10,420,5,7,"Serving: 1serving<br/> Calories: 253.4 kcal<br/> Carbohydrates: 3.53g <br/> Protein: 34.45g<br/> Fat: 10.43g<br/> Sodium: 656.68mg<br/> Fiber: 0.15g<br/>",
"2 - 4  boneless skinless chicken breasts<br/>
1 - 1.5 cups sweet baby rays buffalo sauce<br/>
1 packet of ranch dressing mix<br/>
1 - 2 table spoons butter<br/>",
"1. place chicken in slow cooker <br/>
2. add in ranch packet<br/>
3. add in hot sauce<br/>
4. cook on low 6-7 hours<br/>
5. shred chicken and stir in butter<br/>
6. cook on low 1 more hour",
"Enjoy");

INSERT INTO recipe_categories_join VALUES (2,6),(2,9),(2,20),(2,25),(2,27);

INSERT INTO recipes (RecipeTitle, CourseId, CuisineId, DifficultyId, PrepTime, CookTime, Rating, Servings, Nutrition, Ingredients, Instructions, Notes)
VALUES ("Strawberries Romanoff",
4,17,2,10,0,5,6,"Serving: 1serving<br/> Calories: 253.4 kcal<br/> Carbohydrates: 3.53g <br/> Protein: 34.45g<br/> Fat: 10.43g<br/> Sodium: 656.68mg<br/> Fiber: 0.15g<br/>",
"2 pint strawberries<br/>
4 tbs sugar<br/>
4 tbs Grand Marnier<br/>
1/4 cup powdered sugar<br/>
1 cup Cream<br/>
1/4 cup sour cream",
"1. In a medium bowl, combine hulled and quartered strawberries, 4 Tbsp sugar and 4 Tbsp liqueur, stir to combine then cover and refrigerate at least 1 hour and up to 2 hours, stirring once or twice.<br/>
2.  in a large mixing bowl, combine 1 cup cold heavy cream and 1/4 cup powdered sugar, and beat with an electric mixer until stiff peaks form.
Using a spatula, fold in 1/4 cup sour cream just until well blended.<br/>
3. To serve, stir strawberries then divide between 6 serving glasses or bowls.<br/>
4. Spoon cream over strawberries, dividing evenly<br/>
5. Serve right away or chill and enjoy within 2 hours of assembly.<br/>",
"Enjoy");

INSERT INTO recipe_categories_join VALUES (3,8),(3,10),(3,35);

INSERT INTO recipes (RecipeTitle, CourseId, CuisineId, DifficultyId, PrepTime, CookTime, Rating, Servings, Nutrition, Ingredients, Instructions, Notes)
VALUES ("Chicken Enchilada Casserole",
7,3,2,20,60,5,8,"Serving: 1serving<br/> Calories: 253.4 kcal<br/> Carbohydrates: 3.53g <br/> Protein: 34.45g<br/> Fat: 10.43g<br/> Sodium: 656.68mg<br/> Fiber: 0.15g<br/>",
"14 oz jar Enchilada sauce<br/>
3 Cups shredded Monterey Jack cheese<br/>
6 corn tortillas<br/>
2 chicken breasts<br/>",
"1. Cut each chicken breast in about 3 pieces, so that it cooks faster and put it in a small pot.
Pour Enchilada sauce over it and cook covered on low to medium heat until chicken is cooked through, about 20 minutes.
No water is needed, the chicken will cook in the Enchilada sauce.
Make sure you stir occasionally so that it doesn't stick to the bottom.<br/>
2. Remove chicken from the pot and shred with two forks.
Preheat oven to 375 F degrees.<br/>
3. Start layering the casserole.
Start with about ¼ cup of the leftover Enchilada sauce over the bottom of a baking dish.
I used a longer baking dish, so that I can put 2 corn tortillas across.
Place 2 tortillas on the bottom, top with ⅓ of the chicken and ⅓ of the remaining sauce.
Sprinkle with ⅓ of the cheese and repeat starting with 2 more tortillas, then chicken, sauce, cheese.
Repeat with last layer with the remaining ingredients, tortillas, chicken, sauce and cheese.<br/>
4. Bake for 20 to 30 minutes uncovered, until bubbly and cheese has melted and started to brown on top.
Serve warm.<br/>",
"Enjoy");

INSERT INTO recipe_categories_join VALUES (4,6),(4,24),(4,27);
SELECT LAST_INSERT_ID();
SELECT * FROM recipes
JOIN recipe_categories_join ON recipe_categories_join.RecipeId = recipes.RecipeId
JOIN recipe_categories ON recipe_categories_join.CategoryId = recipe_categories.CategoryId 
WHERE recipe_categories.CategoryId = '27' AND recipes.CuisineId = 16;

SELECT * FROM recipes
JOIN recipe_categories_join ON recipe_categories_join.RecipeId = recipes.RecipeId
JOIN recipe_categories ON recipe_categories_join.CategoryId = recipe_categories.CategoryId;

SELECT * FROM recipes
JOIN recipe_categories_join ON recipe_categories_join.RecipeId = recipes.RecipeId
JOIN recipe_categories ON recipe_categories_join.CategoryId = recipe_categories.CategoryId 
JOIN recipe_courses ON recipe_courses.CourseId = recipes.CourseId
JOIN recipe_cuisines ON recipe_cuisines.CuisineId = recipes.CuisineId
JOIN recipe_difficulties ON recipe_difficulties.DifficultyId = recipes.DifficultyId
WHERE recipe_categories.CategoryId = '27' AND recipes.CuisineId = 16;

SELECT * FROM recipes
JOIN recipe_categories_join ON recipe_categories_join.RecipeId = recipes.RecipeId
JOIN recipe_categories ON recipe_categories_join.CategoryId = recipe_categories.CategoryId 
JOIN recipe_courses ON recipe_courses.CourseId = recipes.CourseId
JOIN recipe_cuisines ON recipe_cuisines.CuisineId = recipes.CuisineId
JOIN recipe_difficulties ON recipe_difficulties.DifficultyId = recipes.DifficultyId
WHERE recipes.RecipeId = 2;

SELECT * FROM recipes WHERE RecipeId = 2;

/*Delete a Recipe*/
DELETE FROM recipe_categories_join WHERE RecipeId = 9;
DELETE FROM recipes WHERE RecipeId = 9;

/*updating one to many recipe difficuly is easy just upate the recipes table*/
UPDATE recipes
SET RecipeDescription = null, DifficultyId = 2
WHERE recipes.RecipeId = 2;

/*to update the many to many it may be best to delete all the records then insert the new categories*/
DELETE FROM recipe_categories_join WHERE RecipeId = 2;
INSERT INTO recipe_categories_join VALUES (2,6),(2,9),(2,20),(2,25),(2,27),(2,30);

SELECT * FROM recipes
JOIN recipe_categories_join ON recipe_categories_join.RecipeId = recipes.RecipeId
WHERE recipe_categories_join.CategoryId = '27' AND recipes.CuisineId = 16;

SELECT * FROM recipes
JOIN recipe_categories_join ON recipe_categories_join.RecipeId = recipes.RecipeId
JOIN recipe_categories ON recipe_categories_join.CategoryId = recipe_categories.CategoryId;

/*For pagination*/
SELECT * FROM recipes ORDER BY RecipeId, RecipeTitle;
SELECT COUNT(*) FROM recipes;
/*page 1*/
SELECT * FROM recipes ORDER BY RecipeId, RecipeTitle LIMIT 5 OFFSET 0;
/*page 2*/
SELECT * FROM recipes ORDER BY RecipeId, RecipeTitle LIMIT 5 OFFSET 5;
/*page 3*/
SELECT * FROM recipes ORDER BY RecipeId, RecipeTitle LIMIT 5 OFFSET 10;

/*for searching*/
SELECT * FROM recipes WHERE RecipeTitle LIKE '%slow cooker%';

SELECT * FROM recipes WHERE RecipeTitle LIKE '%slow cooker%' ORDER BY RecipeId, RecipeTitle LIMIT 5 OFFSET 0;

SELECT * FROM recipes WHERE RecipeTitle LIKE '%%' ORDER BY RecipeId, RecipeTitle LIMIT 5 OFFSET 0;

SELECT * FROM recipes WHERE MATCH(RecipeTitle, Ingredients) AGAINST("slow cooker crock pot chicken") ORDER BY RecipeId, RecipeTitle LIMIT 5 OFFSET 0;

SELECT * FROM recipes WHERE MATCH(RecipeTitle, Ingredients) AGAINST("slow cooker crock pot chicken") ORDER BY RecipeId, RecipeTitle;

SELECT * FROM recipes WHERE MATCH(RecipeTitle, Ingredients) AGAINST("slow cooker crock pot chicken") AND CuisineId = '16' AND CourseId = '7' ORDER BY RecipeId, RecipeTitle;

SELECT * FROM recipes WHERE CuisineId = '16' AND CourseId = '7' ORDER BY RecipeId, RecipeTitle;

SELECT * FROM recipes WHERE MATCH(RecipeTitle, Ingredients) AGAINST("") ORDER BY RecipeId, RecipeTitle;