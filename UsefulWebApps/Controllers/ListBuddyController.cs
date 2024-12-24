using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Models.ViewModels.ListBuddy;
using UsefulWebApps.Repository.IRepository;
using Ganss.Xss;


namespace UsefulWebApps.Controllers
{
    [Authorize(Roles = "StandardUser, Admin")]
    [AutoValidateAntiforgeryToken]
    public class ListBuddyController : Controller
    {
        private HtmlSanitizer sanitizer = new HtmlSanitizer();
        private readonly IUnitOfWork _unitOfWork;

        public ListBuddyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region Notes
        public async Task<IActionResult> MyNotes()
        {
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            List<Notes> notes = (List<Notes>)await _unitOfWork.Notes.GetAllWhere("UserId", userId);
            NotesVM notesVM = new()
            {
                Notes = notes
            };
            return View(notesVM);
        }

        public async Task<IActionResult> Note(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Notes note = await _unitOfWork.Notes.GetById(id);
            return View(note);
        }

        public IActionResult CreateNote() 
        {
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            Notes note = new()
            {
                UserId = userId
            };
            return View(note); 
        }

        [HttpPost]
        public async Task<IActionResult> CreateNote(Notes obj)
        {
            obj.Note = sanitizer.Sanitize(obj.Note);
            if (ModelState.IsValid)
            {
                bool success = await _unitOfWork.Notes.Add(obj);
                if (success)
                {
                    TempData["success"] = "Note created successfully.";
                    return RedirectToAction("MyNotes");
                }
                TempData["error"] = "Create note error. Try again.";
                return RedirectToAction("MyNotes");
            }
            TempData["error"] = "Create note error. Try again.";
            return RedirectToAction("MyNotes");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNote(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            bool success = await _unitOfWork.Notes.Delete(id);
            if (success)
            {
                TempData["success"] = "Note deleted successfully.";
                return RedirectToAction("MyNotes");
            }
            TempData["error"] = "Delete note error. Try again.";
            return RedirectToAction("MyNotes");
        }

        public async Task<IActionResult> EditNote(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Notes note = await _unitOfWork.Notes.GetById(id);
            return View(note);
        }

        [HttpPost]
        public async Task<IActionResult> EditNote(Notes obj)
        {
            obj.Note = sanitizer.Sanitize(obj.Note);
            if (ModelState.IsValid)
            {
                bool success = await _unitOfWork.Notes.Update(obj);
                if (success)
                {
                    TempData["success"] = "Note edited successfully.";
                    return RedirectToAction("Note", new { id = obj.Id });
                }
                TempData["error"] = "Edit note error. Try again.";
                return RedirectToAction("MyNotes");
            }
            TempData["error"] = "Edit note error. Try again.";
            return RedirectToAction("MyNotes");
        }
        #endregion

        #region To Do List

        public async Task<IActionResult> MyToDoLists() 
        {
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            List<string> myToDoLists = await _unitOfWork.ToDoList.GetMyToDoLists(userId);
            MyToDoListsVM myToDoListsVM = new()
            {
                MyToDoLists = myToDoLists,
            };
            return View(myToDoListsVM); 
        }

        public async Task<IActionResult> ToDoList(string list)
        {
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            List<ToDoList> listItems = await _unitOfWork.ToDoList.GetAllItemsInList(userId, list);
            ToDoListVM toDoListVM = new()
            {
                ToDoListItems = listItems,
                ToDoList = new ToDoList
                {
                    UserId = userId,
                    ListTitle = list,
                }
            };
            return View(toDoListVM);
        }
        
        [HttpPost]
        public async Task<IActionResult> ToDoListToggleComplete(int? id, string listTitle)
        {
            if (id == null || id == 0 || listTitle == "" || listTitle == null)
            {
                return NotFound();
            }
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            List<ToDoList> listItems = await _unitOfWork.ToDoList.ToDoListToggleComplete(id, userId, listTitle);
            ToDoListVM toDoListVM = new()
            {
                ToDoListItems = listItems,
                ToDoList = new ToDoList
                {
                    UserId = userId,
                    ListTitle = listTitle
                }
            };
            return PartialView("_ToDoListPartial", toDoListVM);
        }

        public IActionResult CreateToDoList()
        {
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            ToDoList toDoList = new ToDoList
            {
                UserId = userId
            };

            return View(toDoList);
        }
        //create a new ToDoList
        [HttpPost]
        public async Task<IActionResult> CreateToDoList(ToDoList toDoList)
        {

            if (ModelState.IsValid) 
            {
                bool success = await _unitOfWork.ToDoList.Add(toDoList);
                if (success)
                {
                    TempData["success"] = "To do list created successfully.";
                    return RedirectToAction("ToDoList", new { list = toDoList.ListTitle });
                }
                else
                {
                    TempData["error"] = "Create to do list error. Try again.";
                    return RedirectToAction("MyToDoLists");
                }
            }
            TempData["error"] = "Create to do list error. Try again.";
            return RedirectToAction("MyToDoLists");
        }

        //add new item to already existing ToDoList
        [HttpPost]
        public async Task<IActionResult> ToDoListAddItem(ToDoList toDoList)
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
                        ListTitle= toDoList.ListTitle
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
        public async Task<IActionResult> ToDoListDeleteAll(ToDoList toDoList)
        {
            toDoList.ToDoItem = "dummy item";
            ModelState.Clear();
            TryValidateModel(toDoList);
            if (ModelState.IsValid) 
            {
                bool success = await _unitOfWork.ToDoList.DeleteAllItemsInList(toDoList.UserId, toDoList.ListTitle);
                if (success)
                {
                    TempData["success"] = "To do list deleted successfully.";
                }
                else
                {
                    TempData["error"] = "Delete to do list error. Try again.";
                }
                return RedirectToAction("MyToDoLists");
            }
            TempData["error"] = "Delete to do list error. Try again.";
            return RedirectToAction("MyToDoLists");
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
                    TempData["success"] = "To do item updated successfully.";
                }
                else
                {
                    TempData["error"] = "Update to do item error. Try again.";
                }
                return RedirectToAction("ToDoList", new { list = obj.ListTitle });
            }
            TempData["error"] = "Update to do item error. Try again.";
            return RedirectToAction("MyToDoLists");
        }

        #endregion

        #region Grocery List

        private static GroceryListVM FormatGroceryListForDisplay(List<GroceryList> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum, List<UserGroceryCategories> userGroceryCategories, string userId)
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
            foreach (UserGroceryCategories glistCategory in userGroceryCategories)
            {
                List<GroceryList> filter = groceryListItems.Where(x => x.Category == glistCategory.Category).ToList();
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
                FilteredGroceryListItems = filteredGroceryListItems,
                UserSortedGroceryCategories = userGroceryCategories
            };
            return groceryListVM;
        }

        public async Task<IActionResult> GroceryList()
        {
            ClaimsPrincipal currentUser = this.User;
            string userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            (List<GroceryList> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum, List<UserGroceryCategories> userGroceryCategories) result = await _unitOfWork.GroceryList.GetGroceryListItemsAndCategories("UserId", userId);

            GroceryListVM groceryListVM = FormatGroceryListForDisplay(result.groceryListItems, result.groceryCategoriesEnum, result.userGroceryCategories, userId);

            return View(groceryListVM);
        }

        [HttpPost]
        public async Task<IActionResult> GroceryListToggleComplete(int? id, string userId)
        {
            if (id == null || id == 0 || userId == "")
            {
                return NotFound();
            }
            (List<GroceryList> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum, List<UserGroceryCategories> userGroceryCategories) result = await _unitOfWork.GroceryList.GroceryListToggleComplete(id, userId);

            GroceryListVM groceryListVM = FormatGroceryListForDisplay(result.groceryListItems, result.groceryCategoriesEnum, result.userGroceryCategories, userId);

            return PartialView("_GroceryListPartial", groceryListVM);
        }

        [HttpPost]
        public async Task<IActionResult> GroceryListSortCategories(int sortOrder, string category, string userId)
        {
            (List<GroceryList> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum, List<UserGroceryCategories> userGroceryCategories) result = await _unitOfWork.GroceryList.GroceryListSortCategories(sortOrder, category, userId);
            GroceryListVM groceryListVM = FormatGroceryListForDisplay(result.groceryListItems, result.groceryCategoriesEnum, result.userGroceryCategories, userId);

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
                bool success = await _unitOfWork.GroceryList.GroceryListUpdate(groceryList);
                if (success) 
                {
                    TempData["success"] = "Grocery item updated successfully.";
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
        public async Task<JsonResult> GroceryListDeleteItem(int? id)
        {
            if (id == null || id == 0)
            {
                return Json("error id was 0 or null");
            }
            await _unitOfWork.GroceryList.Delete(id);
            string jsonString = """
            { 
                "deleteId": "ID"
            }
            """;
            jsonString = jsonString.Replace("ID", $"{id}");
            return Json(jsonString);
        }

        [HttpPost]
        public async Task<IActionResult> GroceryListCreate(GroceryListVM groceryListVM)
        {
            //https://stackoverflow.com/questions/29309803/asp-net-mvc-modelstate-how-to-re-run-validation
            //add the VM Category to the GroceryList and re-validate
            groceryListVM.GroceryList.Category = groceryListVM.Category;
            ModelState.Clear();
            TryValidateModel(groceryListVM.GroceryList);
            if (ModelState.IsValid)
            {
                (List<GroceryList> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum, List<UserGroceryCategories> userGroceryCategories) result = await _unitOfWork.GroceryList.GroceryListAdd(groceryListVM.GroceryList);
                GroceryListVM newGroceryListVM = FormatGroceryListForDisplay(result.groceryListItems, result.groceryCategoriesEnum, result.userGroceryCategories, groceryListVM.GroceryList.UserId);
                return PartialView("_GroceryListPartial", newGroceryListVM);
            }
            return StatusCode(400);
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

        [HttpPost]
        public async Task<JsonResult> SaveUserGroceryList(string userId)
        {
            if (userId == null || userId == "")
            {
                return Json("error userId was null");
            }
            //jquery ajax handles the toast
            bool success = await _unitOfWork.GroceryList.SaveUserGroceryList(userId);
            if (!success)
            {
                return Json("failed to save list");
            }
            return Json("success");
        }

        #endregion
    }
}
