using Ganss.Xss;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Models.MyRecipes;
using UsefulWebApps.Models.ViewModels.MyRecipes;
using UsefulWebApps.Repository.IRepository;

namespace UsefulWebApps.Controllers
{
    [Authorize(Roles = "StandardUser, Admin")]
    [AutoValidateAntiforgeryToken]
    public class MyRecipesController : Controller
    {
        private IWebHostEnvironment Environment;
        private const int PageSize = 10;
        private readonly HtmlSanitizer sanitizer;
        private readonly IUnitOfWork _unitOfWork;
        public MyRecipesController(IUnitOfWork unitOfWork, IWebHostEnvironment _environment)
        {
            Environment = _environment;
            _unitOfWork = unitOfWork;

            sanitizer = new HtmlSanitizer();
            sanitizer.AllowedAttributes.UnionWith(new[] { "class", "data-list" });
        }

        public async Task<IActionResult> Index(int page, string searchString, string categories, bool ascending = true)
        {
            List<int> selectedCategoryIds = new();

            if (!string.IsNullOrEmpty(categories))
            {
                selectedCategoryIds = categories
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .ToList();
            }

            if (page == 0)
                page = 1;
            
            //limit is the number of recipes per page
            int limit = PageSize;
            int offset = (limit * (page - 1));

            (int count, List<Recipe> recipes) result;
            if (selectedCategoryIds.Any())
            {
                result = await _unitOfWork.Recipe.PaginationWithTagFilter(limit, offset, selectedCategoryIds, ascending);
            }
            else
            {
                result = await _unitOfWork.Recipe.Pagination(limit, offset, searchString, ascending);
            }

            (
                List<RecipeCategories> recipeCategories,
                List<RecipeCourses> recipeCourses,
                List<RecipeCuisines> recipeCuisines,
                List<RecipeDifficulties> recipeDifficulties
            ) resultB = await _unitOfWork.Recipe.GetCategoriesForCreateDisplay();

            List<RecipeCategories> recipeCategories = resultB.recipeCategories;
            List<RecipeCourses> recipeCourses = resultB.recipeCourses;
            List<RecipeCuisines> recipeCuisines = resultB.recipeCuisines;
            List<RecipeDifficulties> recipeDifficulties = resultB.recipeDifficulties;

            // Persist UI state
            foreach (var cat in recipeCategories)
            {
                if (selectedCategoryIds.Contains(cat.CategoryId))
                    cat.IsChecked = true;
            }

            //count is the total number of recipes in database when search sting in empty or the total
            //number of retured recipes that match the search string
            int count = result.count;
            List<Recipe> recipes = result.recipes;

            int totalPages = (int)Math.Ceiling(count / (double)limit);

            RecipeIndexVM recipeIndexVM = new()
            {
                Recipes = recipes,
                RecipeCategories = recipeCategories,
                RecipeCourses = recipeCourses,
                RecipeCuisines = recipeCuisines,
                RecipeDifficulties = recipeDifficulties,
                CategoriesQuery = categories,
                Ascending = ascending,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalRecipes = count,
                SearchString = searchString,
            };
            
            return View(recipeIndexVM);
        }

        //Post/Redirect/Get (PRG) pattern
        /*
            | Operation      | Verb |
            | -------------- | ---- |
            | Submit form    | POST |
            | Navigate pages | GET  |
            | Bookmark       | GET  |
            | Share link     | GET  |
            | Refresh        | GET  |
            | Filter submit  | POST |
            | Search submit  | POST |
            | Pagination     | GET  |

         */
        [HttpPost]
        public async Task<IActionResult> Index(RecipeIndexVM model, int page, string searchString = "", bool ascending = true)
        {
            //model.RecipeCategories null on Search Title and Ingredients form
            if(model.RecipeCategories == null)
            {
                return RedirectToAction("Index", new { page = page, searchString = searchString, ascending = model.Ascending });
            }
                
            if (page == 0)
                page = 1;
            
            // Get selected category IDs
            List<int> selectedCategoryIds = model.RecipeCategories
                .Where(c => c.IsChecked)
                .Select(c => c.CategoryId)
                .ToList();

            if (selectedCategoryIds.Count == 0) 
                return RedirectToAction("Index", new { page = page, searchString = searchString, ascending = model.Ascending });
            
            return RedirectToAction("Index", new
            {
                page = page,
                ascending = model.Ascending,
                categories = string.Join(",", selectedCategoryIds)
            });
        }

        public async Task<IActionResult> Recipe(int? id, int page, string searchString = "", string categories = "", bool ascending = true)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            string userName = currentUser.FindFirstValue(ClaimTypes.Name);

            await _unitOfWork.OpenConnectionAsync();
            await _unitOfWork.BeginTxnAsync();
            RecipePageVM RecipePageVM = await _unitOfWork.Recipe.GetRecipeAndCommentsById(id);
            await _unitOfWork.CommitAsync();
            RecipePageVM.RecipeComment.UserId = userId;
            RecipePageVM.RecipeComment.UserName = userName;
            RecipePageVM.RecipeUserSaved.UserId = userId;
            RecipePageVM.RecipeUserSaved.UserName = userName;
            RecipePageVM.ReturnPage = page;
            RecipePageVM.ReturnSearchString = searchString;
            RecipePageVM.ReturnCategories = categories;
            RecipePageVM.ReturnAscending = ascending;

            (List<GroceryList> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum, List<UserGroceryCategories> userGroceryCategories) groceryResult = await _unitOfWork.GroceryList.GetGroceryListItemsAndCategories("UserId", userId);
            RecipePageVM.GroceryCategoriesList = groceryResult.groceryCategoriesEnum.Select(u => new SelectListItem
            {
                Text = u.Category,
                Value = u.Category
            });
            RecipePageVM.AddIngredientToGrocery = new AddRecipeIngredientToGroceryVM
            {
                RecipeId = RecipePageVM.Recipe.RecipeId,
            };
            return View(RecipePageVM);
        }
        [HttpPost]
        [Route("/MyRecipes/AddIngredientToGroceryList", Name = "addIngredientToGroceryList")]
        //in html asp-for="AddIngredientToGrocery.GroceryItem" so Bind Prefix AddIngredientToGrocery so model validates
        public async Task<IActionResult> AddIngredientToGroceryList([Bind(Prefix = "AddIngredientToGrocery")] AddRecipeIngredientToGroceryVM addIngredientToGroceryVM)
        {
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!ModelState.IsValid)
            {
                TempData["error"] = "Add ingredient to grocery list error. Please try again.";
                return RedirectToAction("Recipe", new { id = addIngredientToGroceryVM.RecipeId });
            }

            GroceryList groceryList = new()
            {
                GroceryItem = addIngredientToGroceryVM.GroceryItem.Trim(),
                Category = addIngredientToGroceryVM.Category,
                Complete = false,
                UserId = userId
            };

            await _unitOfWork.OpenConnectionAsync();
            await _unitOfWork.BeginTxnAsync();
            await _unitOfWork.GroceryList.GroceryListAdd(groceryList);
            await _unitOfWork.CommitAsync();

            TempData["success"] = "Ingredient added to grocery list successfully.";
            return RedirectToAction("Recipe", new { id = addIngredientToGroceryVM.RecipeId });
        }
        public async Task<IActionResult> SavedRecipes(int page, string searchString = "", string categories = "", bool ascending = true)
        {
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            List<RecipeUserSaved> recipeUserSaved = await _unitOfWork.Recipe.GetUserSavedRecipes(userId);
            SavedRecipesVM savedRecipesVM = new SavedRecipesVM 
            { 
                RecipeUserSaved = recipeUserSaved,
                ReturnPage = page,
                ReturnSearchString = searchString,
                ReturnCategories = categories,
                ReturnAscending = ascending
            };
            if(recipeUserSaved.Count == 0)
            {
                TempData["success"] = "You don't have any saved recipes.";
            }
            return View(savedRecipesVM);
        }

        [HttpPost]
        [Route("/MyRecipes/PostComment", Name = "postComment")]
        public async Task<IActionResult> PostComment(RecipeComment recipeComment)
        {
            if (ModelState.IsValid)
            {
               
                int? id = recipeComment.RecipeId;
                bool success = await _unitOfWork.Recipe.AddRecipeComment(recipeComment);
                if (success)
                {
                    TempData["success"] = "Posted comment successfully";
                    return RedirectToAction("Recipe", new { id });
                }
                else
                {
                    TempData["error"] = "Post comment error. Please try again.";
                    return RedirectToAction("Index");
                }
            }
            TempData["error"] = "Post comment error. Please try again.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("/MyRecipes/UserSavedRecipe", Name = "userSavedRecipe")]
        public async Task<IActionResult> UserSavedRecipe(RecipeUserSaved recipeUserSaved)
        {
            if (ModelState.IsValid)
            {
                int? id = recipeUserSaved.RecipeId;
                await _unitOfWork.OpenConnectionAsync();
                await _unitOfWork.BeginTxnAsync();
                bool success = await _unitOfWork.Recipe.AddUserSavedRecipe(recipeUserSaved);
                if (success)
                {
                    await _unitOfWork.CommitAsync();
                    TempData["success"] = "Recipe saved to your list successfully";
                    return RedirectToAction("Recipe", new { id });
                }
                else
                {
                    await _unitOfWork.RollbackAsync();
                    TempData["error"] = "You can only save 10 recipes to your list";
                    return RedirectToAction("Recipe", new { id });
                }
            }
            TempData["error"] = "Save recipe to list error. Please try again.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Route("/MyRecipes/DeleteUserSavedRecipe", Name = "deleteUserSavedRecipe")]
        public async Task<IActionResult> DeleteUserSavedRecipe(int? id)
        {
            
            if (id == null || id == 0)
            {
                TempData["error"] = "Delete saved recipe error. Please try again";
                return RedirectToAction("SavedRecipes");
            }
            bool success = await _unitOfWork.Recipe.DeleteUserSavedRecipe(id);
            if (success)
            {
                TempData["success"] = "Saved recipe deleted successfully";
            }
            else
            {
                TempData["error"] = "Delete saved recipe error. Please try again";
            }
            return RedirectToAction("SavedRecipes");
        }

        public async Task<IActionResult> EditRecipe(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            

            await _unitOfWork.OpenConnectionAsync();
            await _unitOfWork.BeginTxnAsync();  
            (
                List<Recipe> recipe, 
                List<RecipeCategories> recipeCategories,
                List<RecipeCourses> recipeCourses, 
                List<RecipeCuisines> recipeCuisines, 
                List<RecipeDifficulties> recipeDifficulties
            ) result = await _unitOfWork.Recipe.GetRecipeAndCategoriesForEditDisplay(id);
            await _unitOfWork.CommitAsync();
            //https://www.learndapper.com/relationships -- map the JOIN to C# objects
            //this is a list of 1 single recipe listed x times one for each category -- best to see this by running the above sql in workbench. 
            List<Recipe> recipe = result.recipe;

            //since we sql SELECT on 1 id GroupBy returns 1 group with x num recipe rows
            //foreach group get the First recipe and add the categories to it
            //this returns a list with 1 recipe in it that now has List<RecipeCategories> filled
            List<Recipe> filteredRecipe = recipe.GroupBy(r => r.RecipeId).Select(g =>
            {
                Recipe singleRecipe = g.First();
                //select each recipe in the group and return the list of categories
                singleRecipe.Categories = g.Select(r => r.Categories.Single()).ToList();
                return singleRecipe;
            }).ToList();

            List<RecipeCategories> recipeCategories = result.recipeCategories;
            List<RecipeCourses> recipeCourses = result.recipeCourses;
            List<RecipeCuisines> recipeCuisines = result.recipeCuisines;
            List<RecipeDifficulties> recipeDifficulties = result.recipeDifficulties;

            //add IsChecked to recipeCategories
            for (int i = 0; i < recipeCategories.Count; i++)
            {
                for (int j = 0; j < filteredRecipe[0].Categories.Count; j++)
                {
                    if (recipeCategories[i].CategoryId == filteredRecipe[0].Categories[j].CategoryId)
                    {
                        recipeCategories[i].IsChecked = true;
                    }
                }
            }
            //add IsChecked to recipeCourses
            for (int i = 0; i < recipeCourses.Count; i++)
            {
                if (recipeCourses[i].CourseId == filteredRecipe[0].Course.CourseId)
                {
                    recipeCourses[i].IsChecked = true;
                }
            }
            //add IsChecked to recipeCuisines
            for (int i = 0; i < recipeCuisines.Count; i++)
            {
                if (recipeCuisines[i].CuisineId == filteredRecipe[0].Cuisine.CuisineId)
                {
                    recipeCuisines[i].IsChecked = true;
                }
            }
            for (int i = 0; i < recipeDifficulties.Count; i++)
            {
                if (recipeDifficulties[i].DifficultyId == filteredRecipe[0].Difficulty.DifficultyId)
                {
                    recipeDifficulties[i].IsChecked = true;
                }
            }

            RecipeVM recipeVM = new()
            {
                Recipe = filteredRecipe[0],
                RecipeCategories = recipeCategories,
                RecipeCourses = recipeCourses,
                RecipeCuisines = recipeCuisines,
                RecipeDifficulties = recipeDifficulties
            };

            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            string role = currentUser.FindFirstValue(ClaimTypes.Role);

            if (recipeVM.Recipe.UserId != userId && role != "Admin")
            {
                TempData["error"] = $"Only the author {recipeVM.Recipe.UserName} can edit this recipe";
                return RedirectToAction("Recipe", new { id } );
            }

            return View(recipeVM);
        }

        [HttpPost]
        public async Task<IActionResult> EditRecipe(RecipeVM recipeVM)
        {
            recipeVM.Recipe.Ingredients = sanitizer.Sanitize(recipeVM.Recipe.Ingredients);
            recipeVM.Recipe.Instructions = sanitizer.Sanitize(recipeVM.Recipe.Instructions);
            recipeVM.Recipe.Notes = sanitizer.Sanitize(recipeVM.Recipe.Notes);
            recipeVM.Recipe.Nutrition = sanitizer.Sanitize(recipeVM.Recipe.Nutrition);
            
            //set Recipe.Course equal to the new chosen course
            //basically map the users choice to the Recipe model
            foreach (RecipeCourses course in recipeVM.RecipeCourses)
            {
                if (recipeVM.Recipe.CourseId == course.CourseId)
                {
                    recipeVM.Recipe.Course = course;
                }
            }
            foreach (RecipeCuisines cuisine in recipeVM.RecipeCuisines)
            {
                if (recipeVM.Recipe.CuisineId == cuisine.CuisineId)
                {
                    recipeVM.Recipe.Cuisine = cuisine;
                }
            }
            foreach (RecipeDifficulties difficulty in recipeVM.RecipeDifficulties)
            {
                if (recipeVM.Recipe.DifficultyId == difficulty.DifficultyId)
                {
                    recipeVM.Recipe.Difficulty = difficulty;
                }
            }
            recipeVM.Recipe.Categories = recipeVM.RecipeCategories;
            
            ModelState.Clear();
            TryValidateModel(recipeVM);

            if (ModelState.IsValid)
            {
                string oldFilePathDb = recipeVM.Recipe.ImagePath;
               
                //remove old image if there and new image is not null
                if (oldFilePathDb != null && recipeVM.ImageFile != null)
                {
                    string oldImageStoragePath = Path.Combine(this.Environment.WebRootPath, oldFilePathDb);
                    if (System.IO.File.Exists(oldImageStoragePath))
                    {
                        System.IO.File.Delete(oldImageStoragePath);
                    }
                }
                if (recipeVM.ImageFile != null)
                {
                    //generate a unique file name
                    string fileName = $"{Guid.NewGuid()}-{Path.GetFileName(recipeVM.ImageFile.FileName)}";
                    //get filepath for physical storage location
                    string storageFilePath = Path.Combine(this.Environment.WebRootPath, "images/recipes/", fileName);
                    //get filepath for database
                    string filePathDb = Path.Combine("images/recipes/", Path.GetFileName(storageFilePath));
                    //upload image -- copy image to wwwroot
                    using (var stream = System.IO.File.Create(storageFilePath))
                    {
                        await recipeVM.ImageFile.CopyToAsync(stream);
                    }
                    //set database path in recipe model
                    recipeVM.Recipe.ImagePath = filePathDb;
                }
                await _unitOfWork.OpenConnectionAsync();
                await _unitOfWork.BeginTxnAsync();
                bool success = await _unitOfWork.Recipe.UpdateRecipe(recipeVM);
                if (success)
                {
                    await _unitOfWork.CommitAsync();
                    TempData["success"] = "Recipe updated successfully";
                }
                else
                {
                    await _unitOfWork.RollbackAsync();
                    TempData["error"] = "Update recipe error. Please try again.";
                }
                //return RedirectToAction("Index");
                return RedirectToAction("Recipe", new { id = recipeVM.Recipe.RecipeId });
            }
            TempData["error"] = "Update recipe error. Please try again.";
            return View(recipeVM);
        }

        public async Task<IActionResult> CreateRecipe()
        {
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            string userName = currentUser.FindFirstValue(ClaimTypes.Name);
            (
                List<RecipeCategories> recipeCategories,
                List<RecipeCourses> recipeCourses,
                List<RecipeCuisines> recipeCuisines,
                List<RecipeDifficulties> recipeDifficulties
            ) result = await _unitOfWork.Recipe.GetCategoriesForCreateDisplay();

            List<RecipeCategories> recipeCategories = result.recipeCategories;
            List<RecipeCourses> recipeCourses = result.recipeCourses;
            List<RecipeCuisines> recipeCuisines = result.recipeCuisines;
            List<RecipeDifficulties> recipeDifficulties = result.recipeDifficulties;

            RecipeVM recipeVM = new()
            {
                Recipe = new Recipe 
                { 
                    UserId = userId,
                    UserName = userName,
                },
                RecipeCategories = recipeCategories,
                RecipeCourses = recipeCourses,
                RecipeCuisines = recipeCuisines,
                RecipeDifficulties = recipeDifficulties
            };
            return View(recipeVM);
        }

        [HttpPost]
        public async Task<IActionResult> CreateRecipe(RecipeVM recipeVM)
        {
            recipeVM.Recipe.Ingredients = sanitizer.Sanitize(recipeVM.Recipe.Ingredients);
            recipeVM.Recipe.Instructions = sanitizer.Sanitize(recipeVM.Recipe.Instructions);
            recipeVM.Recipe.Notes = sanitizer.Sanitize(recipeVM.Recipe.Notes);
            recipeVM.Recipe.Nutrition = sanitizer.Sanitize(recipeVM.Recipe.Nutrition);
            //set Recipe.Course equal to the chosen course
            //basically map the users choice to the Recipe model
            foreach (RecipeCourses course in recipeVM.RecipeCourses)
            {
                if (recipeVM.Recipe.CourseId == course.CourseId)
                {
                    recipeVM.Recipe.Course = course;
                }
            }
            foreach (RecipeCuisines cuisine in recipeVM.RecipeCuisines)
            {
                if (recipeVM.Recipe.CuisineId == cuisine.CuisineId)
                {
                    recipeVM.Recipe.Cuisine = cuisine;
                }
            }
            foreach (RecipeDifficulties difficulty in recipeVM.RecipeDifficulties)
            {
                if (recipeVM.Recipe.DifficultyId == difficulty.DifficultyId)
                {
                    recipeVM.Recipe.Difficulty = difficulty;
                }
            }
            recipeVM.Recipe.Categories = recipeVM.RecipeCategories;
            

            ModelState.Clear();
            TryValidateModel(recipeVM);
            if (ModelState.IsValid)
            {
                if (recipeVM.ImageFile != null)
                {
                    //generate a unique file name
                    string fileName = $"{Guid.NewGuid()}-{Path.GetFileName(recipeVM.ImageFile.FileName)}";
                    //get filepath for physical storage location
                    string storageFilePath = Path.Combine(this.Environment.WebRootPath, "images/recipes/", fileName);
                    //get filepath for database
                    string filePathDb = Path.Combine("images/recipes/", Path.GetFileName(storageFilePath));
                    //upload image -- copy image to wwwroot
                    using (var stream = System.IO.File.Create(storageFilePath))
                    {
                        await recipeVM.ImageFile.CopyToAsync(stream);
                    }
                    //set database path in recipe model
                    recipeVM.Recipe.ImagePath = filePathDb;
                }
                await _unitOfWork.OpenConnectionAsync();
                await _unitOfWork.BeginTxnAsync();
                bool success = await _unitOfWork.Recipe.AddRecipe(recipeVM);
                if (success)
                {
                    await _unitOfWork.CommitAsync();
                    TempData["success"] = "Recipe created successfully";
                }
                else
                {
                    await _unitOfWork.RollbackAsync();
                    TempData["error"] = "Create recipe error. Please try again.";
                }
                return RedirectToAction("Index");
            }
            TempData["error"] = "Create recipe error. Please try again.";
            return View(recipeVM);
        }

        public async Task<IActionResult> DeleteRecipe(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            Recipe recipe = await _unitOfWork.Recipe.GetRecipeById(id);

            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            string role = currentUser.FindFirstValue(ClaimTypes.Role);

            if (recipe.UserId != userId && role != "Admin")
            {
                TempData["error"] = $"Only the author {recipe.UserName} can delete this recipe";
                return RedirectToAction("Recipe", new { id });
            }

            return View(recipe);
        }

        [HttpPost, ActionName("DeleteRecipe")]
        public async Task<IActionResult> DeleteRecipeFromDb(int? id, string imagePath)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            //remove image if there
            if (imagePath != null)
            {
                string imageStoragePath = Path.Combine(this.Environment.WebRootPath, imagePath);
                if (System.IO.File.Exists(imageStoragePath))
                {
                    System.IO.File.Delete(imageStoragePath);
                }
            }
            await _unitOfWork.OpenConnectionAsync();
            await _unitOfWork.BeginTxnAsync();
            bool success = await _unitOfWork.Recipe.DeleteRecipe(id);
            if (success) 
            {
                await _unitOfWork.CommitAsync();
                TempData["success"] = "Recipe deleted successfully";
            }
            else
            {
                await _unitOfWork.RollbackAsync();
                TempData["error"] = "Delete recipe error. Please try again";
            }
            return RedirectToAction("Index");
        }

    }
}
