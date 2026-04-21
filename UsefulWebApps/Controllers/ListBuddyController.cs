using Ganss.Xss;
using Google.Protobuf.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using UsefulWebApps.Models.Friends;
using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Models.ViewModels.ListBuddy;
using UsefulWebApps.Repository.IRepository;


namespace UsefulWebApps.Controllers
{
    [Authorize(Roles = "StandardUser, Admin")]
    [AutoValidateAntiforgeryToken]
    public class ListBuddyController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        private readonly HtmlSanitizer sanitizer;
        private readonly IUnitOfWork _unitOfWork;

        public ListBuddyController(UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;

            _unitOfWork = unitOfWork;

            sanitizer = new HtmlSanitizer();
            sanitizer.AllowedAttributes.UnionWith(new[] { "class", "data-list" });
        }
        public IActionResult Index()
        {
            return View();
        }

        #region Notes
        public async Task<IActionResult> MyNotes()
        {
            ClaimsPrincipal currentUser = this.User;
            string? userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return NotFound();

            List<Notes> myNotes = (List<Notes>)await _unitOfWork.Notes.GetAllWhere("UserId", userId);
            List<Notes> sharedWithMe = await _unitOfWork.NoteShares.GetNotesSharedWithUser(userId);
            Dictionary<long, List<string>> sharedToMap = await _unitOfWork.NoteShares.GetSharedToMapForOwner(userId);

            NotesVM notesVM = new()
            {
                MyNotes = myNotes,
                SharedWithMeNotes = sharedWithMe,
                SharedToFriends = sharedToMap
            };
            return View(notesVM);
        }

        public async Task<IActionResult> Note(long? id)
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
            return View(obj);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNote(long? id)
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

        public async Task<IActionResult> EditNote(long? id)
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
                (bool Success, bool WasConflict) result = await _unitOfWork.Notes.UpdateWithVersionCheck(obj);

                if (result.Success)
                {
                    TempData["success"] = "Note edited successfully.";
                    return RedirectToAction("Note", new { id = obj.Id });
                }

                if (result.WasConflict)
                {
                    // Load the latest version so the user can see what changed
                    Notes latest = await _unitOfWork.Notes.GetById(obj.Id);
                    //must clear model state for latest version to load up.
                    ModelState.Clear();
                    TempData["error"] = "This note was modified by someone else while you were editing. The current version is shown below. Please re-apply your changes.";
                    return View(latest); // show them the current state
                }

                TempData["error"] = "Edit note error. Try again.";
                return RedirectToAction("MyNotes");
            }
            TempData["error"] = "Edit note error. Try again.";
            return View(obj);
        }
        //shared notes methods
        public async Task<IActionResult> ShareNote(long? id)
        {
            if (id == null || id == 0) return NotFound();

            ClaimsPrincipal currentUser = this.User;
            string? userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return NotFound();

            Notes note = await _unitOfWork.Notes.GetById(id);

            // Only the owner can share their note
            if (note.UserId != userId)
            {
                TempData["error"] = "You can only share your own notes.";
                return RedirectToAction("MyNotes");
            }

            // Get friends to populate dropdown
            (List<UserProfiles> profiles, List<Friendships> friendships) result = await _unitOfWork.Friendships.GetFriendsWithProfiles(userId);

            IEnumerable<SelectListItem> friendsList = result.profiles.Select(p => new SelectListItem
            {
                //?? null-coalescing operator
                Text = p.DisplayName ?? p.UserId,
                Value = p.UserId
            });

            ShareNoteVM vm = new()
            {
                NoteId = note.Id,
                NoteTitle = note.NoteTitle,
                FriendsList = friendsList
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ShareNote(ShareNoteVM vm)
        {
            ClaimsPrincipal currentUser = this.User;
            string? userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return NotFound();

            // Server-side: verify these two users are actually friends
            Friendships? friendship = await _unitOfWork.Friendships.GetExisting(userId, vm.SelectedFriendUserId);
            if (friendship == null || friendship.Status != FriendshipStatus.Accepted)
            {
                TempData["error"] = "You can only share notes with friends.";
                return RedirectToAction("MyNotes");
            }

            // Verify the note belongs to the current user
            Notes note = await _unitOfWork.Notes.GetById(vm.NoteId);
            if (note.UserId != userId)
            {
                TempData["error"] = "You can only share your own notes.";
                return RedirectToAction("MyNotes");
            }

            bool success = await _unitOfWork.NoteShares.ShareNote(vm.NoteId, vm.SelectedFriendUserId);
            TempData[success ? "success" : "error"] = success
                ? "Note shared successfully."
                : "Could not share note. It may already be shared with this friend.";

            return RedirectToAction("Note", new { id = vm.NoteId });
        }

        [HttpPost]
        public async Task<IActionResult> UnshareNote(long noteId)
        {
            ClaimsPrincipal currentUser = this.User;
            string? userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return NotFound();

            Notes note = await _unitOfWork.Notes.GetById(noteId);
            if (note.UserId != userId)
            {
                TempData["error"] = "You can only manage sharing on your own notes.";
                return RedirectToAction("MyNotes");
            }

            bool success = await _unitOfWork.NoteShares.UnshareNote(noteId);
            TempData[success ? "success" : "error"] = success
                ? "Note unshared."
                : "Could not unshare note.";

            return RedirectToAction("MyNotes");
        }

        #endregion

        #region To Do List
        [HttpPost]
        public async Task<IActionResult> UnshareToDoList(long listId)
        {
            ClaimsPrincipal currentUser = this.User;
            string? userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return NotFound();

            ToDoLists toDoList = await _unitOfWork.ToDoLists.GetById(listId);
            // Only the owner can share their list
            if (toDoList.UserId != userId)
            {
                TempData["error"] = "You can only manage sharing on your own lists.";
                return RedirectToAction("MyToDoLists");
            }
            bool success = await _unitOfWork.ToDoListShares.UnshareToDoList(listId);
            TempData[success ? "success" : "error"] = success
                ? "List unshared."
                : "Could not unshare list.";

            return RedirectToAction("MyToDoLists");
        }
        public async Task<IActionResult> ShareToDoList(long? id)
        {
            if (id == null || id == 0) return NotFound();

            ClaimsPrincipal currentUser = this.User;
            string? userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return NotFound();

            ToDoLists toDoList = await _unitOfWork.ToDoLists.GetById(id);

            // Only the owner can share their list
            if (toDoList.UserId != userId)
            {
                TempData["error"] = "You can only share your own lists.";
                return RedirectToAction("MyToDoLists");
            }

            // Get friends to populate dropdown
            (List<UserProfiles> profiles, List<Friendships> friendships) result = await _unitOfWork.Friendships.GetFriendsWithProfiles(userId);

            IEnumerable<SelectListItem> friendsList = result.profiles.Select(p => new SelectListItem
            {
                //?? null-coalescing operator
                Text = p.DisplayName ?? p.UserId,
                Value = p.UserId
            });

            ShareToDoListVM vm = new()
            {
                ListId = toDoList.Id,
                ListTitle = toDoList.ListTitle,
                FriendsList = friendsList
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ShareToDoList(ShareToDoListVM vm)
        {
            ClaimsPrincipal currentUser = this.User;
            string? userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return NotFound();

            // Server-side: verify these two users are actually friends
            Friendships? friendship = await _unitOfWork.Friendships.GetExisting(userId, vm.SelectedFriendUserId);
            if (friendship == null || friendship.Status != FriendshipStatus.Accepted)
            {
                TempData["error"] = "You can only share lists with friends.";
                return RedirectToAction("MyToDoLists");
            }
            ToDoLists toDoList = await _unitOfWork.ToDoLists.GetById(vm.ListId);
            // Verify the list belongs to the current user
            if (toDoList.UserId != userId)
            {
                TempData["error"] = "You can only share your own lists.";
                return RedirectToAction("MyToDoLists");
            }
            bool success = await _unitOfWork.ToDoListShares.ShareToDoList(vm.ListId, vm.SelectedFriendUserId);
            TempData[success ? "success" : "error"] = success
                ? "List shared successfully."
                : "Could not share list. It may already be shared with this friend.";

            return RedirectToAction("ToDoList", new { listId = vm.ListId });
        }
        public async Task<IActionResult> MyToDoLists() 
        {
            ClaimsPrincipal currentUser = this.User;
            string? userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return NotFound();

            List<ToDoLists> myToDoLists = (List<ToDoLists>)await _unitOfWork.ToDoLists.GetAllWhere("UserId", userId);
            List<ToDoLists> sharedWithMe = await _unitOfWork.ToDoListShares.GetToDoListsSharedWithUser(userId);
            Dictionary<long, List<string>> sharedToMap = await _unitOfWork.ToDoListShares.GetSharedToMapForOwner(userId);

            MyToDoListsVM myToDoListsVM = new()
            {
                MyToDoLists = myToDoLists,
                SharedWithMeToDoLists = sharedWithMe,
                SharedToFriends = sharedToMap
            };
            return View(myToDoListsVM); 
        }

        public async Task<IActionResult> ToDoList(long? listId)
        {
            if (listId == null || listId == 0)
            {
                return NotFound();
            }
            ClaimsPrincipal currentUser = this.User;
            string? userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return NotFound();

            ToDoLists toDoList = await _unitOfWork.ToDoLists.GetById(listId);

            List<ToDoItems> listItems = await _unitOfWork.ToDoLists.GetAllItemsInList(listId);
            ToDoListVM toDoListVM = new()
            {
                ToDoList = toDoList,
                ToDoListItems = listItems,
                ToDoItem = new ToDoItems { ListId = (long)listId },
            };
            return View(toDoListVM);
        }

        //transaction method -- Concurrent
        [HttpPost]
        public async Task<IActionResult> ToDoListToggleComplete(long? id, long? listId, int listVersion)
        {
            if (id == null || id == 0 || listId == 0 || listId == null)
                return NotFound();

            await _unitOfWork.OpenConnectionAsync();
            await _unitOfWork.BeginTxnAsync();
            (bool success, bool wasConflict, ToDoLists toDoList, List<ToDoItems> listItems) result = await _unitOfWork.ToDoLists.ToDoListToggleComplete((long)id, (long)listId, listVersion);

            if (!result.success)
            {
                await _unitOfWork.RollbackAsync();
                ToDoListVM conflictVM = new()
                {
                    ToDoList = result.toDoList,
                    ToDoListItems = result.listItems,
                    ToDoItem = new ToDoItems { ListId = (long)listId },
                };
                // Return the refreshed list so JS can update the UI + signal the conflict
                Response.Headers["X-Concurrency-Conflict"] = "true";
                return PartialView("_ToDoListPartial", conflictVM);
            }

            await _unitOfWork.CommitAsync();
            ToDoListVM toDoListVM = new()
            {
                ToDoList = result.toDoList,
                ToDoListItems = result.listItems,
                ToDoItem = new ToDoItems { ListId = (long)listId },
            };
            return PartialView("_ToDoListPartial", toDoListVM);
        }

        public IActionResult CreateToDoList()
        {
            ClaimsPrincipal currentUser = this.User;
            string? userId = currentUser.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return NotFound();

            ToDoLists toDoList = new ToDoLists
            {
                UserId = userId
            };

            return View(toDoList);
        }
        //create a new ToDoList
        [HttpPost]
        public async Task<IActionResult> CreateToDoList(ToDoLists toDoList)
        {

            if (ModelState.IsValid) 
            {
                bool success = await _unitOfWork.ToDoLists.Add(toDoList);
                if (success)
                {
                    TempData["success"] = "To do list created successfully.";
                }
                else
                {
                    TempData["error"] = "Create to do list error. Try again.";
                }
                return RedirectToAction("MyToDoLists");
            }
            TempData["error"] = "Create to do list error. Try again.";
            return RedirectToAction("MyToDoLists");
        }

        //transaction method -- Concurrent
        //add new item to already existing ToDoList 
        [HttpPost]
        public async Task<IActionResult> ToDoListAddItem(ToDoItems toDoItem)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.OpenConnectionAsync();
                await _unitOfWork.BeginTxnAsync();
                (bool success, bool wasConflict, ToDoLists toDoList, List<ToDoItems> listItems) result = await _unitOfWork.ToDoItems.ToDoListAddItem(toDoItem);
                if (!result.success)
                {
                    await _unitOfWork.RollbackAsync();
                    ToDoListVM conflictVM = new()
                    {
                        ToDoList = result.toDoList,
                        ToDoListItems = result.listItems,
                        ToDoItem = new ToDoItems { ListId = toDoItem.ListId },
                    };
                    // Return the refreshed list so JS can update the UI + signal the conflict
                    Response.Headers["X-Concurrency-Conflict"] = "true";
                    return PartialView("_ToDoListPartial", conflictVM);
                }
                await _unitOfWork.CommitAsync();
                ToDoListVM toDoListVM = new()
                {
                    ToDoList = result.toDoList,
                    ToDoListItems = result.listItems,
                    ToDoItem = new ToDoItems { ListId = toDoItem.ListId },
                };
                return PartialView("_ToDoListPartial", toDoListVM);
            }
            return StatusCode(400);
        }

        // needs version check/concurrency
        [HttpPost]
        public async Task<JsonResult> ToDoListDeleteItem(long? id)
        {
            if (id == null || id == 0)
            {
                return Json("error id was 0 or null");
            }
            await _unitOfWork.ToDoItems.Delete(id);
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
        public async Task<IActionResult> ToDoListDeleteAll(ToDoLists toDoList)
        {
            if (ModelState.IsValid) 
            {
                bool success = await _unitOfWork.ToDoLists.Delete(toDoList.Id);
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

        public async Task<IActionResult> ToDoListEdit(long? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            } 
            ToDoItems toDoItem = await _unitOfWork.ToDoItems.GetById(id);
            return View(toDoItem);
        }

        // needs version check/concurrency
        [HttpPost]
        public async Task<IActionResult> ToDoListEdit(ToDoItems obj)
        {
            if (ModelState.IsValid)
            {
                bool success = await _unitOfWork.ToDoItems.Update(obj);
                if (success)
                {
                    TempData["success"] = "To do item updated successfully.";
                }
                else
                {
                    TempData["error"] = "Update to do item error. Try again.";
                }
                return RedirectToAction("ToDoList", new { listId = obj.ListId });
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

        //transaction method
        [HttpPost]
        public async Task<IActionResult> GroceryListToggleComplete(long? id, string userId)
        {
            if (id == null || id == 0 || userId == "")
            {
                return NotFound();
            }
            await _unitOfWork.OpenConnectionAsync();
            await _unitOfWork.BeginTxnAsync();
            (List<GroceryList> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum, List<UserGroceryCategories> userGroceryCategories) result = await _unitOfWork.GroceryList.GroceryListToggleComplete(id, userId);
            await _unitOfWork.CommitAsync();
            GroceryListVM groceryListVM = FormatGroceryListForDisplay(result.groceryListItems, result.groceryCategoriesEnum, result.userGroceryCategories, userId);

            return PartialView("_GroceryListPartial", groceryListVM);
        }

        //transaction method
        [HttpPost]
        public async Task<IActionResult> GroceryListSortCategories(int sortOrder, string category, string userId)
        {
            await _unitOfWork.OpenConnectionAsync();
            await _unitOfWork.BeginTxnAsync();
            (List<GroceryList> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum, List<UserGroceryCategories> userGroceryCategories) result = await _unitOfWork.GroceryList.GroceryListSortCategories(sortOrder, category, userId);
            await _unitOfWork.CommitAsync();
            GroceryListVM groceryListVM = FormatGroceryListForDisplay(result.groceryListItems, result.groceryCategoriesEnum, result.userGroceryCategories, userId);

            return PartialView("_GroceryListPartial", groceryListVM);
        }

        public async Task<IActionResult> GroceryListEdit(long? id)
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

        //transaction method
        [HttpPost]
        public async Task<IActionResult> GroceryListEdit(GroceryList groceryList, GroceryListEditVM groceryListEditVM)
        {
            groceryList.Category = groceryListEditVM.Category;
            ModelState.Clear();
            TryValidateModel(groceryList);

            if (ModelState.IsValid)
            {
                await _unitOfWork.OpenConnectionAsync();
                await _unitOfWork.BeginTxnAsync();
                bool success = await _unitOfWork.GroceryList.GroceryListUpdate(groceryList);
                if (success) 
                {
                    await _unitOfWork.CommitAsync();
                    TempData["success"] = "Grocery item updated successfully.";
                }
                else
                {
                    await _unitOfWork.RollbackAsync();
                    TempData["error"] = "Update grocery item error. Please try again.";
                }
                return RedirectToAction("GroceryList");
            }
            TempData["error"] = "Update grocery item error. Please try again.";
            return RedirectToAction("GroceryListEdit");
        }

        [HttpPost]
        public async Task<JsonResult> GroceryListDeleteItem(long? id)
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

        //transaction method
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
                await _unitOfWork.OpenConnectionAsync();
                await _unitOfWork.BeginTxnAsync();
                (List<GroceryList> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum, List<UserGroceryCategories> userGroceryCategories) result = await _unitOfWork.GroceryList.GroceryListAdd(groceryListVM.GroceryList);
                await _unitOfWork.CommitAsync();
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

        //transaction method
        [HttpPost]
        public async Task<JsonResult> SaveUserGroceryList(string userId)
        {
            if (userId == null || userId == "")
            {
                return Json("error userId was null");
            }
            //jquery ajax handles the toast
            await _unitOfWork.OpenConnectionAsync();
            await _unitOfWork.BeginTxnAsync();
            bool success = await _unitOfWork.GroceryList.SaveUserGroceryList(userId);
            if (!success)
            {
                await _unitOfWork.RollbackAsync();  
                return Json("failed to save list");
            }
            await _unitOfWork.CommitAsync();
            return Json("success");
        }

        //transaction method
        [HttpPost]
        public async Task<IActionResult> UseSavedGroceryList(string userId)
        {
            if (ModelState.IsValid) 
            {
                await _unitOfWork.OpenConnectionAsync();
                await _unitOfWork.BeginTxnAsync();
                bool success = await _unitOfWork.GroceryList.UseSavedGroceryList(userId);
                if (success)
                {
                    await _unitOfWork.CommitAsync();
                    TempData["success"] = "Grocery template loaded successfully.";
                }
                else
                {
                    await _unitOfWork.RollbackAsync();
                    TempData["error"] = "You currently don't have a saved list. Save one first.";
                }
                return RedirectToAction("GroceryList");
            }
            TempData["error"] = "Use grocery template error. Please try again.";
            return RedirectToAction("GroceryList");
        }

        public IActionResult ShareGroceryList(string userId) 
        {
            ShareGroceryListVM shareGroceryListVM = new()
            {
                UserId = userId
            };
            return View(shareGroceryListVM);  
        }

        //transaction method
        [HttpPost]
        public async Task<IActionResult> ShareGroceryList(ShareGroceryListVM shareGroceryListVM)
        {
            if (!ModelState.IsValid) { return View(); }
            IdentityUser friend = await _userManager.FindByEmailAsync(shareGroceryListVM.Friend.Email.Trim());
            
            if (friend == null)
            {
                TempData["error"] = "Share list error. Please try again. Make sure you have the correct user name and email.";
                return View();
            };
            await _unitOfWork.OpenConnectionAsync();
            await _unitOfWork.BeginTxnAsync();
            bool success = await _unitOfWork.GroceryList.ShareGroceryList(shareGroceryListVM.UserId, friend.Id);
            if (success)
            {
                await _unitOfWork.CommitAsync();
                TempData["success"] = "Grocery list shared.";
            }
            else
            {
                await _unitOfWork.RollbackAsync();
                TempData["error"] = "Share list error. Please try again. Make sure your friend has a list.";
            }
            return RedirectToAction("GroceryList");
        }

        #endregion
    }
}
