using Ganss.Xss;
using Google.Protobuf.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using UsefulWebApps.Helpers;
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

        private async Task<IEnumerable<SelectListItem>> GetFriendsSelectListAsync(string userId)
        {
            // Get friends to populate dropdown
            (List<UserProfiles> profiles, List<Friendships> friendships) result = await _unitOfWork.Friendships.GetFriendsWithProfiles(userId);
            return result.profiles.Select(p => new SelectListItem
            {
                //?? null-coalescing operator
                Text = p.DisplayName ?? p.UserId,
                Value = p.UserId
            });
        }
        private async Task<bool> AreFriendsAsync(string userId, string otherUserId)
        {
            Friendships? friendship = await _unitOfWork.Friendships.GetExisting(userId, otherUserId);
            return friendship != null && friendship.Status == FriendshipStatus.Accepted;
        }

        #region Notes
        public async Task<IActionResult> MyNotes()
        {
            string? userId = User.GetUserId();
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
            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

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

            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            Notes note = await _unitOfWork.Notes.GetById(id);

            // Only the owner can share their note
            if (note.UserId != userId)
            {
                TempData["error"] = "You can only share your own notes.";
                return RedirectToAction("MyNotes");
            }

            IEnumerable<SelectListItem> friendsList = await GetFriendsSelectListAsync(userId);

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
            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            if (!await AreFriendsAsync(userId, vm.SelectedFriendUserId))
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
            string? userId = User.GetUserId();
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
            string? userId = User.GetUserId();
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

            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            ToDoLists toDoList = await _unitOfWork.ToDoLists.GetById(id);

            // Only the owner can share their list
            if (toDoList.UserId != userId)
            {
                TempData["error"] = "You can only share your own lists.";
                return RedirectToAction("MyToDoLists");
            }

            IEnumerable<SelectListItem> friendsList = await GetFriendsSelectListAsync(userId);

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
            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            if (!await AreFriendsAsync(userId, vm.SelectedFriendUserId))
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
            string? userId = User.GetUserId();
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
            string? userId = User.GetUserId();
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

        private IActionResult ToDoListPartialResult(bool success, ToDoLists toDoList, List<ToDoItems> listItems, long listId)
        {
            ToDoListVM vm = new ToDoListVM
            {
                ToDoList = toDoList,
                ToDoListItems = listItems,
                ToDoItem = new ToDoItems { ListId = listId }
            };

            if (!success)
                Response.Headers["X-Concurrency-Conflict"] = "true";

            return PartialView("_ToDoListPartial", vm);
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

            if(!result.success)
                await _unitOfWork.RollbackAsync();
            else
                await _unitOfWork.CommitAsync();

            return ToDoListPartialResult(result.success, result.toDoList, result.listItems, (long)listId);
        }


        //transaction method -- Concurrent
        [HttpPost]
        public async Task<IActionResult> ToDoListSortItem(long? id, long? listId, int newSortOrder, int listVersion)
        {
            if (id == null || id == 0 || listId == null || listId == 0)
                return NotFound();

            await _unitOfWork.OpenConnectionAsync();
            await _unitOfWork.BeginTxnAsync();
            (bool success, bool wasConflict, ToDoLists toDoList, List<ToDoItems> listItems) result = await _unitOfWork.ToDoLists.ToDoListSortItem((long)id, (long)listId, newSortOrder, listVersion);

            if (!result.success)
                await _unitOfWork.RollbackAsync();
            else
                await _unitOfWork.CommitAsync();

            return ToDoListPartialResult(result.success, result.toDoList, result.listItems, (long)listId);
        }

        public IActionResult CreateToDoList()
        {
            string? userId = User.GetUserId();
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
                    await _unitOfWork.RollbackAsync();
                else
                    await _unitOfWork.CommitAsync();

                return ToDoListPartialResult(result.success, result.toDoList, result.listItems, toDoItem.ListId);
            }
            return StatusCode(400);
        }

        //transaction method -- Concurrent
        [HttpPost]
        public async Task<IActionResult> ToDoListDeleteItem(long? id, long? listId, int listVersion)
        {
            if (id == null || id == 0 || listId == 0 || listId == null)
                return NotFound();

            await _unitOfWork.OpenConnectionAsync();
            await _unitOfWork.BeginTxnAsync();
            (bool success, bool wasConflict, ToDoLists toDoList, List<ToDoItems> listItems) result = await _unitOfWork.ToDoItems.DeleteWithVersionCheck((long)id, (long)listId, listVersion);

            if (!result.success)
                await _unitOfWork.RollbackAsync();
            else
                await _unitOfWork.CommitAsync();

            return ToDoListPartialResult(result.success, result.toDoList, result.listItems, (long)listId);
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
            ToDoLists parentList = await _unitOfWork.ToDoLists.GetById(toDoItem.ListId);
            toDoItem.ListVersion = parentList.Version;
            return View(toDoItem);
        }

        //transaction method -- Concurrent
        [HttpPost]
        public async Task<IActionResult> ToDoListEdit(ToDoItems obj)
        {
            if (ModelState.IsValid)
            {
                await _unitOfWork.OpenConnectionAsync();
                await _unitOfWork.BeginTxnAsync();
                (bool success, bool wasConflict) result = await _unitOfWork.ToDoItems.UpdateWithVersionCheck(obj);
                if (result.success == false && result.wasConflict == true)
                {
                    await _unitOfWork.RollbackAsync();
                    ToDoItems latest = await _unitOfWork.ToDoItems.GetById(obj.Id);
                    ToDoLists parentList = await _unitOfWork.ToDoLists.GetById(latest.ListId);
                    latest.ListVersion = parentList.Version;
                    ModelState.Clear();
                    TempData["error"] = "This list was modified by someone else while you were editing. The current item is shown. Please re-apply your changes.";
                    return View(latest);
                }
                if (result.success)
                {
                    await _unitOfWork.CommitAsync();
                    TempData["success"] = "To do item updated successfully.";
                }
                else
                {
                    await _unitOfWork.RollbackAsync();
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

        private static GroceryListNewVM NewFormatGroceryListForDisplay(List<GroceryListItems> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum, List<UserGroceryCategories> userGroceryCategories, long listId)
        {
            //for UI display we need a list of grocery items for each category
            //A list of lists where each individual list will contain the all items in a specific category
            List<List<GroceryListItems>> filteredGroceryListItems = new List<List<GroceryListItems>>();

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
                List<GroceryListItems> filter = groceryListItems.Where(x => x.Category == glistCategory.Category).ToList();
                //https://stackoverflow.com/questions/1191919/what-does-linq-return-when-the-results-are-empty
                //Empty enum returned if nothing in category found
                if (filter.Count > 0)
                {
                    filteredGroceryListItems.Add(filter);
                }
            }

            GroceryListNewVM groceryListVM = new()
            {
                GroceryListItem = new GroceryListItems
                {
                    ListId = listId,
                },
                GroceryCategoriesList = groceryListCategories,
                FilteredGroceryListItems = filteredGroceryListItems,
                UserSortedGroceryCategories = userGroceryCategories
            };
            return groceryListVM;
        }

        public async Task<IActionResult> MyGroceryLists()
        {
            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            List<GroceryLists> myGroceryLists = (List<GroceryLists>)await _unitOfWork.GroceryLists.GetAllWhere("UserId", userId);
            List<GroceryLists> sharedWithMe = await _unitOfWork.GroceryListShares.GetGroceryListsSharedWithUser(userId);
            Dictionary<long, List<string>> sharedToMap = await _unitOfWork.GroceryListShares.GetSharedToMapForOwner(userId);

            MyGroceryListsVM myGroceryListVM = new()
            {
                MyGroceryLists = myGroceryLists,
                SharedWithMeGroceryLists = sharedWithMe,
                SharedToFriends = sharedToMap,
            };
            return View(myGroceryListVM);
        }

        public async Task<IActionResult> GroceryList(long? listId)
        {
            if (listId == null || listId == 0)
            {
                return NotFound();
            }
            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            GroceryLists groceryList = await _unitOfWork.GroceryLists.GetById(listId);
            (List<GroceryListItems> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum, List<UserGroceryCategories> userGroceryCategories) result = await _unitOfWork.GroceryLists.GetAllItemsAndCategoriesInList(listId);
            GroceryListNewVM groceryListVM = NewFormatGroceryListForDisplay(result.groceryListItems, result.groceryCategoriesEnum, result.userGroceryCategories, (long)listId);
            groceryListVM.GroceryList = groceryList;
            return View(groceryListVM);
        }

        //transaction method -- Concurrent (check it)
        [HttpPost]
        public async Task<IActionResult> GroceryListToggleComplete(long? id, long? listId, int listVersion)
        {
            if (id == null || id == 0 || listId == 0 || listId == null)
                return NotFound();

            await _unitOfWork.OpenConnectionAsync();
            await _unitOfWork.BeginTxnAsync();
            (
            bool success,
            bool wasConflict,
            GroceryLists groceryList,
            List<GroceryListItems> groceryListItems,
            IEnumerable<GroceryCategories> groceryCategoriesEnum,
            List<UserGroceryCategories> userGroceryCategories) result = await _unitOfWork.GroceryLists.GroceryListToggleComplete(id, listId, listVersion);
            if (!result.success)
            {
                await _unitOfWork.RollbackAsync();
                Response.Headers["X-Concurrency-Conflict"] = "true";
            }
            else
                await _unitOfWork.CommitAsync();

            GroceryListNewVM groceryListVM = NewFormatGroceryListForDisplay(result.groceryListItems, result.groceryCategoriesEnum, result.userGroceryCategories, (long)listId);
            groceryListVM.GroceryList = result.groceryList;
            return PartialView("_GroceryListPartial", groceryListVM);
        }

        //transaction method -- Concurrent (check it)
        [HttpPost]
        public async Task<IActionResult> GroceryListSortCategories(long? listId, int newSortOrder, int listVersion, string category)
        {
            if (listId == 0 || listId == null)
                return NotFound();

            await _unitOfWork.OpenConnectionAsync();
            await _unitOfWork.BeginTxnAsync();
            (
            bool success,
            bool wasConflict,
            GroceryLists groceryList,
            List<GroceryListItems> groceryListItems,
            IEnumerable<GroceryCategories> groceryCategoriesEnum,
            List<UserGroceryCategories> userGroceryCategories) result = await _unitOfWork.GroceryLists.GroceryListSortCategories(listId, newSortOrder, listVersion, category);
            if (!result.success)
            {
                await _unitOfWork.RollbackAsync();
                Response.Headers["X-Concurrency-Conflict"] = "true";
            }
            else
                await _unitOfWork.CommitAsync();

            GroceryListNewVM groceryListVM = NewFormatGroceryListForDisplay(result.groceryListItems, result.groceryCategoriesEnum, result.userGroceryCategories, (long)listId);
            groceryListVM.GroceryList = result.groceryList;
            return PartialView("_GroceryListPartial", groceryListVM);
        }

        public async Task<IActionResult> GroceryListEdit(long? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            (GroceryListItems groceryListItem, IEnumerable<GroceryCategories> groceryCategoriesEnum) result = await _unitOfWork.GroceryListItems.GetGroceryListItemAndCategoriesAtId(id);
            GroceryListItems groceryListItem = result.groceryListItem;
            IEnumerable<GroceryCategories> groceryCategoriesEnum = result.groceryCategoriesEnum;
            IEnumerable<SelectListItem> groceryListCategories = groceryCategoriesEnum.Select(u => new SelectListItem
            {
                Text = u.Category,
                Value = u.Category
            });
            GroceryLists parentList = await _unitOfWork.GroceryLists.GetById(groceryListItem.ListId);
            groceryListItem.ListVersion = parentList.Version;
            GroceryListEditVM groceryListEditVM = new()
            {
                Category = groceryListItem.Category,
                GroceryListItem = groceryListItem,
                GroceryCategoriesList = groceryListCategories
            };
            return View(groceryListEditVM);
        }

        //transaction method -- Concurrent (check it)
        [HttpPost]
        public async Task<IActionResult> GroceryListEdit(GroceryListItems groceryListItem, GroceryListEditVM groceryListEditVM)
        {
            groceryListItem.Category = groceryListEditVM.Category;
            ModelState.Clear();
            TryValidateModel(groceryListItem);

            if (ModelState.IsValid)
            {
                await _unitOfWork.OpenConnectionAsync();
                await _unitOfWork.BeginTxnAsync();
                (bool success, bool wasConflict) result = await _unitOfWork.GroceryListItems.GroceryListUpdate(groceryListItem);
                if (result.success == false && result.wasConflict == true) 
                {
                    await _unitOfWork.RollbackAsync();
                    (GroceryListItems groceryListItem, IEnumerable<GroceryCategories> groceryCategoriesEnum) resultB = await _unitOfWork.GroceryListItems.GetGroceryListItemAndCategoriesAtId(groceryListItem.Id);
                    GroceryListItems latest = resultB.groceryListItem;
                    IEnumerable<GroceryCategories> groceryCategoriesEnum = resultB.groceryCategoriesEnum;
                    IEnumerable<SelectListItem> groceryListCategories = groceryCategoriesEnum.Select(u => new SelectListItem
                    {
                        Text = u.Category,
                        Value = u.Category
                    });
                    GroceryLists parentList = await _unitOfWork.GroceryLists.GetById(latest.ListId);
                    latest.ListVersion = parentList.Version;
                    GroceryListEditVM latestVM = new()
                    {
                        Category = latest.Category,
                        GroceryListItem = latest,
                        GroceryCategoriesList = groceryListCategories
                    };
                    ModelState.Clear();
                    TempData["error"] = "This list was modified by someone else while you were editing. The current item is shown. Please re-apply your changes.";
                    return View(latestVM);
                   
                }
                else if(result.success)
                {
                    await _unitOfWork.CommitAsync();
                    TempData["success"] = "Grocery item updated successfully.";
                }
                else
                {
                    await _unitOfWork.RollbackAsync();
                    TempData["error"] = "Update grocery item error. Please try again.";
                }
                return RedirectToAction("GroceryList", new { listId = groceryListItem.ListId });
            }
            TempData["error"] = "Update grocery item error. Please try again.";
            return RedirectToAction("MyGroceryLists");
        }

        //transaction method -- Concurrent (check it)
        [HttpPost]
        public async Task<IActionResult> GroceryListDeleteItem(long? id, long? listId, int listVersion)
        {
            if (id == null || id == 0 || listId == 0 || listId == null)
                return NotFound();

            await _unitOfWork.OpenConnectionAsync();
            await _unitOfWork.BeginTxnAsync();
            (
            bool success,
            bool wasConflict,
            GroceryLists groceryList,
            List<GroceryListItems> groceryListItems,
            IEnumerable<GroceryCategories> groceryCategoriesEnum,
            List<UserGroceryCategories> userGroceryCategories) result = await _unitOfWork.GroceryListItems.DeleteGroceryListItem(id, listId, listVersion);
            if (!result.success)
            {
                await _unitOfWork.RollbackAsync();
                Response.Headers["X-Concurrency-Conflict"] = "true";
            }
            else
                await _unitOfWork.CommitAsync();

            GroceryListNewVM groceryListVM = NewFormatGroceryListForDisplay(result.groceryListItems, result.groceryCategoriesEnum, result.userGroceryCategories, (long)listId);
            groceryListVM.GroceryList = result.groceryList;
            return PartialView("_GroceryListPartial", groceryListVM);

        }

        //transaction method -- Concurrent (check it)
        [HttpPost]
        public async Task<IActionResult> GroceryListAddItem(GroceryListItems groceryListItem, GroceryListNewVM groceryListVM)
        {
            //https://stackoverflow.com/questions/29309803/asp-net-mvc-modelstate-how-to-re-run-validation
            //add the VM Category to the GroceryList and re-validate
            groceryListItem.Category = groceryListVM.Category;
            ModelState.Clear();
            TryValidateModel(groceryListItem);
            if (ModelState.IsValid)
            {
                await _unitOfWork.OpenConnectionAsync();
                await _unitOfWork.BeginTxnAsync();
                (
                bool success,
                bool wasConflict,
                GroceryLists groceryList,
                List<GroceryListItems> groceryListItems,
                IEnumerable<GroceryCategories> groceryCategoriesEnum,
                List<UserGroceryCategories> userGroceryCategories) result = await _unitOfWork.GroceryListItems.GroceryListAddItem(groceryListItem);
                if (!result.success)
                {
                    await _unitOfWork.RollbackAsync();
                    Response.Headers["X-Concurrency-Conflict"] = "true";
                }
                else
                    await _unitOfWork.CommitAsync();

                GroceryListNewVM groceryListNewVM = NewFormatGroceryListForDisplay(result.groceryListItems, result.groceryCategoriesEnum, result.userGroceryCategories, groceryListItem.ListId);
                groceryListNewVM.GroceryList = result.groceryList;
                return PartialView("_GroceryListPartial", groceryListNewVM);

            }
            return StatusCode(400);
        }

        [HttpPost]
        [Route("/ListBuddy/GroceryListDeleteAll", Name = "deleteAllGroceryList")]
        public async Task<IActionResult> GroceryListDeleteAll()
        {
            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

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
