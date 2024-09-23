using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Models.ViewModels.ListBuddy;
using UsefulWebApps.Repository.IRepository;


namespace UsefulWebApps.Controllers
{
    [Authorize(Roles = "StandardUser, Admin")]
    public class ListBuddyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public ListBuddyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region To Do List

        public async Task<IActionResult> ToDoList()
        {
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            List<ToDoList> listItems = (List<ToDoList>)await _unitOfWork.ToDoList.GetAllWhere("UserId", userId);
            ToDoListVM toDoListVM = new()
            {
                ToDoListItems = listItems,
                ToDoList = new ToDoList
                {
                    UserId = userId,
                }
            };
            return View(toDoListVM);
        }

        [HttpPost]
        public async Task<IActionResult> ToDoListToggleComplete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            List<ToDoList> listItems = await _unitOfWork.ToDoList.ToDoListToggleComplete(id, userId);
            ToDoListVM toDoListVM = new()
            {
                ToDoListItems = listItems,
                ToDoList = new ToDoList
                {
                    UserId = userId,
                }
            };
            return PartialView("_ToDoListPartial", toDoListVM);
        }

        [HttpPost]
        public async Task<IActionResult> ToDoListCreate(ToDoList toDoList)
        {
            if (ModelState.IsValid)
            {
                List<ToDoList> listItems = await _unitOfWork.ToDoList.ToDoListAdd(toDoList);
                ToDoListVM toDoListVM = new()
                {
                    ToDoListItems = listItems,
                    ToDoList = new ToDoList
                    {
                        UserId = toDoList.UserId,
                    }
                };
                return PartialView("_ToDoListPartial", toDoListVM);
            }
            return StatusCode(400);
        }

        [HttpPost]
        public async Task<JsonResult> ToDoListDeleteItem(int? id)
        {
            if (id == null || id == 0)
            {
                return Json("error id was 0 or null");
            }
            await _unitOfWork.ToDoList.Delete(id);
            string jsonString = """
            { 
                "deleteId": "ID"
            }
            """;
            jsonString = jsonString.Replace("ID", $"{id}");
            return Json(jsonString);
        }

        [HttpPost]
        [Route("/ListBuddy/ToDoListDeleteAll", Name = "deleteAllToDoList")]
        public async Task<IActionResult> ToDoListDeleteAll()
        {
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            await _unitOfWork.ToDoList.DeleteAllWhere("UserId", userId);
            return RedirectToAction("ToDoList");
        }

        public async Task<IActionResult> ToDoListEdit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            } 
            ToDoList toDoList = await _unitOfWork.ToDoList.GetById(id);
            return View(toDoList);
        }

        [HttpPost]
        public async Task<IActionResult> ToDoListEdit(ToDoList obj)
        {
            if (ModelState.IsValid)
            {
                bool success = await _unitOfWork.ToDoList.Update(obj);
                if (success)
                {
                    TempData["success"] = "To do item updated successfully";
                }
                else
                {
                    TempData["error"] = "Update to do item error. Try again.";
                }
                return RedirectToAction("ToDoList");
            }
            TempData["error"] = "Update to do item error. Try again.";
            return RedirectToAction("ToDoList");
        }

        #endregion

        #region Grocery List

        private static GroceryListVM FormatGroceryListForDisplay(List<GroceryList> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum, string userId)
        {
            //for UI display we need a list of grocery items for each category
            //A list of lists where each individual list will contain the all items in a specific category
            List<List<GroceryList>> filteredGroceryListItems = new List<List<GroceryList>>();

            //for select list html to populate you need IEnum of SelectListItem with Text and Value populated
            //thus we must Select through the Query and return the new item
            IEnumerable<SelectListItem> groceryListCategories = groceryCategoriesEnum.Select(u => new SelectListItem
            {
                Text = u.Category,
                Value = u.Category
            });

            //filter out the List of lists
            //for each category filter out the grocery list items in that category to their own list 
            //add each new list to the filteredGroceryList variable
            foreach (SelectListItem glistCategory in groceryListCategories)
            {
                List<GroceryList> filter = groceryListItems.Where(x => x.Category == glistCategory.Text).ToList();
                //https://stackoverflow.com/questions/1191919/what-does-linq-return-when-the-results-are-empty
                //Empty enum returned if nothing in category found
                if (filter.Count > 0)
                {
                    filteredGroceryListItems.Add(filter);
                }
            }

            GroceryListVM groceryListVM = new()
            {
                GroceryList = new GroceryList
                {
                    UserId = userId,
                },
                GroceryCategoriesList = groceryListCategories,
                FilteredGroceryListItems = filteredGroceryListItems
            };
            return groceryListVM;
        }

        public async Task<IActionResult> GroceryList()
        {
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            (List<GroceryList> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum) result = await _unitOfWork.GroceryList.GetGroceryListItemsAndCategories("UserId", userId);

            GroceryListVM groceryListVM = FormatGroceryListForDisplay(result.groceryListItems, result.groceryCategoriesEnum, userId);

            return View(groceryListVM);
        }

        [HttpPost]
        public async Task<IActionResult> GroceryListToggleComplete(int? id, string userId)
        {
            if (id == null || id == 0 || userId == "")
            {
                return NotFound();
            }
            (List<GroceryList> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum) result = await _unitOfWork.GroceryList.GroceryListToggleComplete(id, userId);

            GroceryListVM groceryListVM = FormatGroceryListForDisplay(result.groceryListItems, result.groceryCategoriesEnum, userId);

            return PartialView("_GroceryListPartial", groceryListVM);
        }

        public async Task<IActionResult> GroceryListEdit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            (GroceryList groceryListItem, IEnumerable<GroceryCategories> groceryCategoriesEnum) result = await _unitOfWork.GroceryList.GetGroceryListItemAndCategoriesAtId(id);
            GroceryList groceryListItem = result.groceryListItem;
            IEnumerable<GroceryCategories> groceryCategoriesEnum = result.groceryCategoriesEnum;
            IEnumerable<SelectListItem> groceryListCategories = groceryCategoriesEnum.Select(u => new SelectListItem
            {
                Text = u.Category,
                Value = u.Category
            });

            GroceryListEditVM groceryListEditVM = new()
            {
                Category = groceryListItem.Category,
                GroceryList = groceryListItem,
                GroceryCategoriesList = groceryListCategories
            };
            return View(groceryListEditVM);
        }

        [HttpPost]
        public async Task<IActionResult> GroceryListEdit(GroceryList groceryList, GroceryListEditVM groceryListEditVM)
        {
            groceryList.Category = groceryListEditVM.Category;
            ModelState.Clear();
            TryValidateModel(groceryList);

            if (ModelState.IsValid)
            {
                bool success = await _unitOfWork.GroceryList.Update(groceryList);
                if (success) 
                {
                    TempData["success"] = "Grocery item updated successfully";
                }
                else
                {
                    TempData["error"] = "Update grocery item error. Please try again.";
                }
                return RedirectToAction("GroceryList");
            }
            TempData["error"] = "Update grocery item error. Please try again.";
            return RedirectToAction("GroceryListEdit");
        }

        [HttpPost]
        [Route("/ListBuddy/GroceryListDeleteItem", Name = "deleteGroceryItem")]
        public async Task<IActionResult> GroceryListDeleteItem(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            await _unitOfWork.GroceryList.Delete(id);
            return RedirectToAction("GroceryList");
        }

        [HttpPost]
        [Route("/ListBuddy/GroceryListCreate", Name = "createGroceryItem")]
        public async Task<IActionResult> GroceryListCreate(GroceryListVM groceryListVM)
        {
            //https://stackoverflow.com/questions/29309803/asp-net-mvc-modelstate-how-to-re-run-validation
            //add the VM Category to the GroceryList and re-validate
            if (groceryListVM.Category == "Please Select a Category")
            {
                TempData["error"] = "Please Select a Category. Please try again.";
                return RedirectToAction("GroceryList");
            }
            groceryListVM.GroceryList.Category = groceryListVM.Category;
            ModelState.Clear();
            TryValidateModel(groceryListVM.GroceryList);
            if (ModelState.IsValid)
            {
                bool success = await _unitOfWork.GroceryList.Add(groceryListVM.GroceryList);
                if (success)
                {
                    TempData["success"] = "Grocery item created successfully";
                }
                else
                {
                    TempData["error"] = "Add grocery item error. Please try again.";
                }
                return RedirectToAction("GroceryList");
            }
            TempData["error"] = "Add grocery item error. Please try again.";
            return RedirectToAction("GroceryList");
        }

        [HttpPost]
        [Route("/ListBuddy/GroceryListDeleteAll", Name = "deleteAllGroceryList")]
        public async Task<IActionResult> GroceryListDeleteAll()
        {
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            await _unitOfWork.GroceryList.DeleteAllWhere("UserId", userId);
            return RedirectToAction("GroceryList");
        }

        #endregion
    }
}
