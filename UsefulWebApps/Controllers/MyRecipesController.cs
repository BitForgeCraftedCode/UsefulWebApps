using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using UsefulWebApps.Models.MyRecipes;
using UsefulWebApps.Models.ViewModels.MyRecipes;
using UsefulWebApps.Repository.IRepository;
using Ganss.Xss;

namespace UsefulWebApps.Controllers
{
    public class MyRecipesController : Controller
    {
        private HtmlSanitizer sanitizer = new HtmlSanitizer();
        private readonly IUnitOfWork _unitOfWork;
        public MyRecipesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(int page, string searchString)
        {
            
            if (page == 0)
            {
                page = 1;
            }
            //limit is the number of recipes per page
            int limit = 10;
            int offset = (limit * (page - 1));

            (int count, List<Recipe> recipes) result = await _unitOfWork.Recipe.Pagination(limit, offset, searchString);

            //count is the total number of recipes in database when search sting in empty or the total
            //number of retured recipes that match the search string
            int count = result.count;
            List<Recipe> recipes = result.recipes;

            int totalPages = (int)Math.Ceiling(count / (double)limit);

            RecipeIndexVM recipeIndexVM = new()
            {
                Recipes = recipes,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalRecipes = count,
                SearchString = searchString,
            };
            
            return View(recipeIndexVM);
        }

        public async Task<IActionResult> Recipe(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            string userName = currentUser.FindFirstValue(ClaimTypes.Name);

            RecipePageVM RecipePageVM = await _unitOfWork.Recipe.GetRecipeAndCommentsById(id);
            RecipePageVM.RecipeComment.UserId = userId;
            RecipePageVM.RecipeComment.UserName = userName;
            RecipePageVM.RecipeUserSaved.UserId = userId;
            RecipePageVM.RecipeUserSaved.UserName = userName;
            return View(RecipePageVM);
        }

        [Authorize(Roles = "StandardUser, Admin")]
        public async Task<IActionResult> SavedRecipes()
        {
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            List<RecipeUserSaved> recipeUserSaved = await _unitOfWork.Recipe.GetUserSavedRecipes(userId);
            SavedRecipesVM savedRecipesVM = new SavedRecipesVM 
            { 
                RecipeUserSaved = recipeUserSaved,
            };
            if(recipeUserSaved.Count == 0)
            {
                TempData["success"] = "You don't have any saved recipes.";
            }
            return View(savedRecipesVM);
        }

        [Authorize(Roles = "StandardUser, Admin")]
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

        [Authorize(Roles = "StandardUser, Admin")]
        [HttpPost]
        [Route("/MyRecipes/UserSavedRecipe", Name = "userSavedRecipe")]
        public async Task<IActionResult> UserSavedRecipe(RecipeUserSaved recipeUserSaved)
        {
            if (ModelState.IsValid)
            {
                int? id = recipeUserSaved.RecipeId;
                bool success = await _unitOfWork.Recipe.AddUserSavedRecipe(recipeUserSaved);
                if (success)
                {
                    TempData["success"] = "Recipe saved to your list successfully";
                    return RedirectToAction("Recipe", new { id });
                }
                else
                {
                    TempData["error"] = "You can only save 10 recipes to your list";
                    return RedirectToAction("Recipe", new { id });
                }
            }
            TempData["error"] = "Save recipe to list error. Please try again.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "StandardUser, Admin")]
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

        [Authorize(Roles = "StandardUser, Admin")]
        public async Task<IActionResult> EditRecipe(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
          
            (
                List<Recipe> recipe, 
                List<RecipeCategories> recipeCategories,
                List<RecipeCourses> recipeCourses, 
                List<RecipeCuisines> recipeCuisines, 
                List<RecipeDifficulties> recipeDifficulties
            ) result = await _unitOfWork.Recipe.GetRecipeAndCategoriesForEditDisplay(id);

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

        [Authorize(Roles = "StandardUser, Admin")]
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
                bool success = await _unitOfWork.Recipe.UpdateRecipe(recipeVM);
                if (success)
                {
                    TempData["success"] = "Recipe updated successfully";
                }
                else
                {
                    TempData["error"] = "Update recipe error. Please try again.";
                }
                //return RedirectToAction("Index");
                return RedirectToAction("Recipe", new { id = recipeVM.Recipe.RecipeId });
            }
            TempData["error"] = "Update recipe error. Please try again.";
            return View(recipeVM);
        }

        [Authorize(Roles = "StandardUser, Admin")]
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

        [Authorize(Roles = "StandardUser, Admin")]
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
                bool success = await _unitOfWork.Recipe.AddRecipe(recipeVM);
                if (success)
                {
                    TempData["success"] = "Recipe created successfully";
                }
                else
                {
                    TempData["error"] = "Create recipe error. Please try again.";
                }
                return RedirectToAction("Index");
            }
            TempData["error"] = "Create recipe error. Please try again.";
            return View(recipeVM);
        }

        [Authorize(Roles = "StandardUser, Admin")]
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

        [Authorize(Roles = "StandardUser, Admin")]
        [HttpPost, ActionName("DeleteRecipe")]
        public async Task<IActionResult> DeleteRecipeFromDb(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            bool success = await _unitOfWork.Recipe.DeleteRecipe(id);
            if (success) 
            {
                TempData["success"] = "Recipe deleted successfully";
            }
            else
            {
                TempData["error"] = "Delete recipe error. Please try again";
            }
            return RedirectToAction("Index");
        }
    }
}
