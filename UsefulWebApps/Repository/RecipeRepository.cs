using UsefulWebApps.Models.MyRecipes;
using UsefulWebApps.Repository.IRepository;
using MySqlConnector;
using Dapper;
using static Dapper.SqlMapper;
using UsefulWebApps.Models.ViewModels.MyRecipes;

namespace UsefulWebApps.Repository
{
    public class RecipeRepository : Repository<Recipe>, IRecipeRepository
    {
        private readonly MySqlConnection _connection;
        public RecipeRepository(MySqlConnection db) : base(db)
        {
            _connection = db;
        }
        //any Recipe model specific database methods here
        public async Task<(int count, List<Recipe> recipes)> Pagination(int limit, int offset, string searchString)
        {
            /* Basic limit offset pagination
            * 
            * The way that the OFFSET keyword works is that it discards the first n rows from the result set. 
            * It doesn't simply skip over them. Instead, it reads the rows and then discards them. 
            * This means that as you work into deeper and deeper pages of your result set, 
            * the performance of your query will degrade
            * 
            * use deferred join in mysql for more efficient pagination of large data
            * 
            * limit = page size
            * offset = page size * page - 1
            * 
            * https://planetscale.com/learn/courses/mysql-for-developers/indexes/fulltext-indexes?autoplay=1
            */

            string sqlMult = @"
                SELECT COUNT(*) FROM recipes;
                SELECT * FROM recipes ORDER BY RecipeId, RecipeTitle LIMIT @Limit OFFSET @Offset;
            ";
            string sqlMultFilter = @"
                SELECT COUNT(*) FROM recipes WHERE MATCH(RecipeTitle, Ingredients) AGAINST(@SearchString) ORDER BY RecipeId, RecipeTitle;
                SELECT * FROM recipes WHERE MATCH(RecipeTitle, Ingredients) AGAINST(@SearchString) ORDER BY RecipeId, RecipeTitle LIMIT @Limit OFFSET @Offset;
            ";
            GridReader gridReader = null;
            if (String.IsNullOrEmpty(searchString))
            {
                gridReader = await _connection.QueryMultipleAsync(sqlMult, new { Limit = limit, Offset = offset });
            }
            else
            {
                gridReader = await _connection.QueryMultipleAsync(sqlMultFilter, new { SearchString = searchString, Limit = limit, Offset = offset });
            }

            //count is the total number of recipes in database
            int count = await gridReader.ReadFirstAsync<int>();
            List<Recipe> recipes = (List<Recipe>)await gridReader.ReadAsync<Recipe>();
            await _connection.CloseAsync();
            return (count, recipes);
        }

        public async Task<List<Recipe>> GetRecipeById(int? id)
        {
            //returns x rows of a single recipe at RecipeId where x is the number of categories
            string sql = @"SELECT * FROM recipes
                JOIN recipe_categories_join ON recipe_categories_join.RecipeId = recipes.RecipeId
                JOIN recipe_categories ON recipe_categories_join.CategoryId = recipe_categories.CategoryId 
                JOIN recipe_courses ON recipe_courses.CourseId = recipes.CourseId
                JOIN recipe_cuisines ON recipe_cuisines.CuisineId = recipes.CuisineId
                JOIN recipe_difficulties ON recipe_difficulties.DifficultyId = recipes.DifficultyId
                WHERE recipes.RecipeId = @id;";

            //https://www.learndapper.com/relationships -- map the JOIN to C# objects
            //this is a list of 1 single recipe listed x times one for each category -- best to see this by running the above sql in workbench. 
            List<Recipe> recipe = (List<Recipe>)await _connection.QueryAsync<Recipe, RecipeCategories, RecipeCourses, RecipeCuisines, RecipeDifficulties, Recipe>(sql,
                (recipe, recipeCategories, recipeCourses, recipeCuisines, recipeDifficulties) => {
                    recipe.Categories.Add(recipeCategories);
                    recipe.Course = recipeCourses;
                    recipe.Cuisine = recipeCuisines;
                    recipe.Difficulty = recipeDifficulties;
                    return recipe;
                }, new { id }, splitOn: "CategoryId, CourseId, CuisineId, DifficultyId");

            await _connection.CloseAsync();
            return recipe;
        }

        public async Task<(
            List<Recipe> recipe,
            List<RecipeCategories> recipeCategories,
            List<RecipeCourses> recipeCourses,
            List<RecipeCuisines> recipeCuisines,
            List<RecipeDifficulties> recipeDifficulties
            )> GetRecipeAndCategoriesForEditDisplay(int? id)
        {
            //returns x rows of a single recipe at RecipeId where x is the number of categories
            string sql = @"SELECT * FROM recipes
                JOIN recipe_categories_join ON recipe_categories_join.RecipeId = recipes.RecipeId
                JOIN recipe_categories ON recipe_categories_join.CategoryId = recipe_categories.CategoryId 
                JOIN recipe_courses ON recipe_courses.CourseId = recipes.CourseId
                JOIN recipe_cuisines ON recipe_cuisines.CuisineId = recipes.CuisineId
                JOIN recipe_difficulties ON recipe_difficulties.DifficultyId = recipes.DifficultyId
                WHERE recipes.RecipeId = @id;";

            string sqlMult = @"
                SELECT * FROM recipe_categories;
                SELECT * FROM recipe_courses;
                SELECT * FROM recipe_cuisines;
                SELECT * FROM recipe_difficulties;
            ";
            await _connection.OpenAsync();
            MySqlTransaction txn = await _connection.BeginTransactionAsync();
            //https://www.learndapper.com/relationships -- map the JOIN to C# objects
            //this is a list of 1 single recipe listed x times one for each category -- best to see this by running the above sql in workbench. 
            List<Recipe> recipe = (List<Recipe>)await _connection.QueryAsync<Recipe, RecipeCategories, RecipeCourses, RecipeCuisines, RecipeDifficulties, Recipe>(sql,
                (recipe, recipeCategories, recipeCourses, recipeCuisines, recipeDifficulties) => {
                    recipe.Categories.Add(recipeCategories);
                    recipe.Course = recipeCourses;
                    recipe.Cuisine = recipeCuisines;
                    recipe.Difficulty = recipeDifficulties;
                    return recipe;
                }, new { id }, transaction: txn, splitOn: "CategoryId, CourseId, CuisineId, DifficultyId");

            GridReader gridReader = await _connection.QueryMultipleAsync(sqlMult, transaction: txn);

            List<RecipeCategories> recipeCategories = (List<RecipeCategories>)await gridReader.ReadAsync<RecipeCategories>();
            List<RecipeCourses> recipeCourses = (List<RecipeCourses>)await gridReader.ReadAsync<RecipeCourses>();
            List<RecipeCuisines> recipeCuisines = (List<RecipeCuisines>)await gridReader.ReadAsync<RecipeCuisines>();
            List<RecipeDifficulties> recipeDifficulties = (List<RecipeDifficulties>)await gridReader.ReadAsync<RecipeDifficulties>();

            await txn.CommitAsync();
            await _connection.CloseAsync();
            return (recipe, recipeCategories, recipeCourses, recipeCuisines, recipeDifficulties);
        }

        public async Task<bool> UpdateRecipe(RecipeVM recipeVM, List<Object> checkedCategoriesParams)
        {
            int rowsEffected1 = 0;
            int rowsEffected2 = 0;
            int rowsEffected3 = 0;
            await _connection.OpenAsync();
            MySqlTransaction txn = await _connection.BeginTransactionAsync();
            string sql = @"UPDATE recipes
                    SET RecipeTitle = @recipeTitle, RecipeDescription = @recipeDescription, CourseId = @courseId,
                    CuisineId = @cuisineId, DifficultyId = @difficultyId, PrepTime = @prepTime, CookTime = @cookTime,
                    Rating = @rating, Servings = @servings, Nutrition = @nutrition, Ingredients = @ingredients,
                    Instructions = @instructions, Notes = @notes
                    WHERE RecipeId = @id";

            //to update the many to many -- delete all the records then insert the new categories
            string sql2 = @"DELETE FROM recipe_categories_join WHERE RecipeId = @id";
            string sql3 = @"INSERT INTO recipe_categories_join (RecipeId, CategoryId) VALUES (@id, @categoryId)";
            rowsEffected1 = await _connection.ExecuteAsync(sql, new
            {
                recipeTitle = recipeVM.Recipe.RecipeTitle,
                recipeDescription = recipeVM.Recipe.RecipeDescription,
                courseId = recipeVM.Recipe.Course.CourseId,
                cuisineId = recipeVM.Recipe.Cuisine.CuisineId,
                difficultyId = recipeVM.Recipe.Difficulty.DifficultyId,
                prepTime = recipeVM.Recipe.PrepTime,
                cookTime = recipeVM.Recipe.CookTime,
                rating = recipeVM.Recipe.Rating,
                servings = recipeVM.Recipe.Servings,
                nutrition = recipeVM.Recipe.Nutrition,
                ingredients = recipeVM.Recipe.Ingredients,
                instructions = recipeVM.Recipe.Instructions,
                notes = recipeVM.Recipe.Notes,
                id = recipeVM.Recipe.RecipeId
            }, transaction: txn);
            rowsEffected2 = await _connection.ExecuteAsync(sql2, new { id = recipeVM.Recipe.RecipeId }, transaction: txn);
            rowsEffected3 = await _connection.ExecuteAsync(sql3, checkedCategoriesParams, transaction: txn);

            await txn.CommitAsync();
            await _connection.CloseAsync();
            return (rowsEffected1 > 0 && rowsEffected2 > 0 && rowsEffected3 > 0) ? true : false;
        }

        public async Task<(
            List<RecipeCategories> recipeCategories,
            List<RecipeCourses> recipeCourses,
            List<RecipeCuisines> recipeCuisines,
            List<RecipeDifficulties> recipeDifficulties
            )> GetCategoriesForCreateDisplay()
        {
            //get the categories, courses, cuisines, and difficulties from the database for display on form
            string sqlMult = @"
                SELECT * FROM recipe_categories;
                SELECT * FROM recipe_courses;
                SELECT * FROM recipe_cuisines;
                SELECT * FROM recipe_difficulties;
            ";
            GridReader gridReader = await _connection.QueryMultipleAsync(sqlMult);

            List<RecipeCategories> recipeCategories = (List<RecipeCategories>)await gridReader.ReadAsync<RecipeCategories>();
            List<RecipeCourses> recipeCourses = (List<RecipeCourses>)await gridReader.ReadAsync<RecipeCourses>();
            List<RecipeCuisines> recipeCuisines = (List<RecipeCuisines>)await gridReader.ReadAsync<RecipeCuisines>();
            List<RecipeDifficulties> recipeDifficulties = (List<RecipeDifficulties>)await gridReader.ReadAsync<RecipeDifficulties>();
            return (recipeCategories, recipeCourses, recipeCuisines, recipeDifficulties);
        }

        public async Task<bool> AddRecipe(RecipeVM recipeVM)
        {
            int rowsEffected1 = 0;
            int rowsEffected2 = 0;
            //make a checked categories parameter list for sql INSERT
            List<Object> checkedCategoriesParams = new List<Object>();
            await _connection.OpenAsync();
            MySqlTransaction txn = await _connection.BeginTransactionAsync();
            string sql = @"INSERT INTO recipes 
                    (
                        RecipeTitle, 
                        RecipeDescription, 
                        CourseId, 
                        CuisineId, 
                        DifficultyId, 
                        PrepTime, 
                        CookTime, 
                        Rating, 
                        Servings, 
                        Nutrition, 
                        Ingredients,   
                        Instructions,   
                        Notes
                    )
                    VALUES 
                    (
                        @recipeTitle,   
                        @recipeDescription, 
                        @courseId, 
                        @cuisineId, 
                        @difficultyId, 
                        @prepTime, 
                        @cookTime, 
                        @rating, 
                        @servings, 
                        @nutrition, 
                        @ingredients, 
                        @instructions, 
                        @notes
                    )";
            rowsEffected1 = await _connection.ExecuteAsync(sql, new
            {
                recipeTitle = recipeVM.Recipe.RecipeTitle,
                recipeDescription = recipeVM.Recipe.RecipeDescription,
                courseId = recipeVM.Recipe.Course.CourseId,
                cuisineId = recipeVM.Recipe.Cuisine.CuisineId,
                difficultyId = recipeVM.Recipe.Difficulty.DifficultyId,
                prepTime = recipeVM.Recipe.PrepTime,
                cookTime = recipeVM.Recipe.CookTime,
                rating = recipeVM.Recipe.Rating,
                servings = recipeVM.Recipe.Servings,
                nutrition = recipeVM.Recipe.Nutrition,
                ingredients = recipeVM.Recipe.Ingredients,
                instructions = recipeVM.Recipe.Instructions,
                notes = recipeVM.Recipe.Notes
            }, transaction: txn);

            //need the id before adding to recipe_categories_join
            //not sure on the best way but for now using LAST_INSERT_ID() should get the job done. 
            //https://stackoverflow.com/questions/56378699/insert-on-a-child-table-and-update-fk-on-parent
            //https://stackoverflow.com/questions/3837990/last-insert-id-mysql
            //https://stackoverflow.com/questions/19714308/mysql-how-to-insert-into-table-that-has-many-to-many-relationship
            string sql2 = @"SELECT RecipeId FROM recipes WHERE RecipeId = LAST_INSERT_ID()";
            //string sql2 = @"LAST_INSERT_ID()";
            int idLastRecipeInsert = await _connection.QuerySingleAsync<int>(sql2, transaction: txn);

            foreach (RecipeCategories category in recipeVM.Recipe.Categories)
            {
                if (category.IsChecked == true)
                {
                    checkedCategoriesParams.Add(
                        new { id = idLastRecipeInsert, categoryId = category.CategoryId }
                    );
                }
            }
            string sql3 = @"INSERT INTO recipe_categories_join (RecipeId, CategoryId) VALUES (@id, @categoryId)";

            rowsEffected2 = await _connection.ExecuteAsync(sql3, checkedCategoriesParams, transaction: txn);
            await txn.CommitAsync();
            await _connection.CloseAsync();
            return (rowsEffected1 > 0 && rowsEffected2 > 0) ? true : false;
        }

    }
}
