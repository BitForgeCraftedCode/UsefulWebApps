using Ganss.Xss;
using Google.Protobuf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using UsefulWebApps.DTO.ListBuddy;
using UsefulWebApps.Helpers;
using UsefulWebApps.Hubs;
using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Models.Notifications;
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
        private readonly IFriendAccessService _friendAccessService;
        private readonly IHubContext<AppHub> _hubContext;
        public ListBuddyController(UserManager<IdentityUser> userManager, IUnitOfWork unitOfWork, IFriendAccessService friendAccessService, IHubContext<AppHub> hubContext)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _friendAccessService = friendAccessService;

            sanitizer = new HtmlSanitizer();
            sanitizer.AllowedAttributes.UnionWith(new[] { "class", "data-list" });
            _hubContext = hubContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region Notes
        public async Task<IActionResult> MyNotes()
        {
            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            List<Notes> myNotes = (await _unitOfWork.Notes.GetAllWhere("UserId", userId)).ToList();
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
                    TempData["warning"] = "This note was modified by someone else while you were editing. The current version is shown below. Please re-apply your changes.";
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
                TempData["warning"] = "You can only share your own notes.";
                return RedirectToAction("MyNotes");
            }

            IEnumerable<SelectListItem> friendsList = await _friendAccessService.GetFriendsSelectListAsync(userId);

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

            if (!await _friendAccessService.AreFriendsAsync(userId, vm.SelectedFriendUserId))
            {
                TempData["warning"] = "You can only share notes with friends.";
                return RedirectToAction("MyNotes");
            }

            // Verify the note belongs to the current user
            Notes note = await _unitOfWork.Notes.GetById(vm.NoteId);
            if (note.UserId != userId)
            {
                TempData["warning"] = "You can only share your own notes.";
                return RedirectToAction("MyNotes");
            }

            bool success = await _unitOfWork.NoteShares.ShareNote(vm.NoteId, vm.SelectedFriendUserId);
            if (success)
            {
                string message = $"{User.Identity?.Name} shared note '{note.NoteTitle}' with you.";
                await _hubContext.Clients.User(vm.SelectedFriendUserId).SendAsync("ReceiveNotification", message);

                //update notifications table
                Notifications notification = new()
                {
                    UserId = vm.SelectedFriendUserId,
                    SenderUserId = userId,
                    Message = message,
                    NotificationType = NotificationType.NoteShared.ToString(),
                    RelatedEntityId = note.Id
                };
                await _unitOfWork.Notifications.Add(notification);

            }
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
                TempData["warning"] = "You can only manage sharing on your own notes.";
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
                TempData["warning"] = "You can only manage sharing on your own lists.";
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
                TempData["warning"] = "You can only share your own lists.";
                return RedirectToAction("MyToDoLists");
            }

            IEnumerable<SelectListItem> friendsList = await _friendAccessService.GetFriendsSelectListAsync(userId);

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

            if (!await _friendAccessService.AreFriendsAsync(userId, vm.SelectedFriendUserId))
            {
                TempData["warning"] = "You can only share lists with friends.";
                return RedirectToAction("MyToDoLists");
            }
            ToDoLists toDoList = await _unitOfWork.ToDoLists.GetById(vm.ListId);
            // Verify the list belongs to the current user
            if (toDoList.UserId != userId)
            {
                TempData["warning"] = "You can only share your own lists.";
                return RedirectToAction("MyToDoLists");
            }
            bool success = await _unitOfWork.ToDoListShares.ShareToDoList(vm.ListId, vm.SelectedFriendUserId);
            if (success)
            {
                string message = $"{User.Identity?.Name} shared to do list '{toDoList.ListTitle}' with you.";
                await _hubContext.Clients.User(vm.SelectedFriendUserId).SendAsync("ReceiveNotification", message);

                //update notifications table
                Notifications notification = new()
                {
                    UserId = vm.SelectedFriendUserId,
                    SenderUserId = userId,
                    Message = message,
                    NotificationType = NotificationType.ToDoListShared.ToString(),
                    RelatedEntityId = toDoList.Id
                };
                await _unitOfWork.Notifications.Add(notification);
            }
            TempData[success ? "success" : "error"] = success
                ? "List shared successfully."
                : "Could not share list. It may already be shared with this friend.";

            return RedirectToAction("ToDoList", new { listId = vm.ListId });
        }

        public async Task<IActionResult> MyToDoLists() 
        {
            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            List<ToDoLists> myToDoLists = (await _unitOfWork.ToDoLists.GetAllWhere("UserId", userId)).ToList();
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

        private IActionResult ToDoListPartialResult(bool success, ToDoListViewState viewState, long listId)
        {
            ToDoListVM vm = new ToDoListVM
            {
                ToDoList = viewState.ToDoList,
                ToDoListItems = viewState.ListItems,
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
            (bool success, bool wasConflict, ToDoListViewState viewState) result = await _unitOfWork.ToDoLists.ToDoListToggleComplete((long)id, (long)listId, listVersion);

            if(!result.success)
                await _unitOfWork.RollbackAsync();
            else
                await _unitOfWork.CommitAsync();

            return ToDoListPartialResult(result.success, result.viewState, (long)listId);
        }


        //transaction method -- Concurrent
        [HttpPost]
        public async Task<IActionResult> ToDoListSortItem(long? id, long? listId, int newSortOrder, int listVersion)
        {
            if (id == null || id == 0 || listId == null || listId == 0)
                return NotFound();

            await _unitOfWork.OpenConnectionAsync();
            await _unitOfWork.BeginTxnAsync();
            (bool success, bool wasConflict, ToDoListViewState viewState) result = await _unitOfWork.ToDoLists.ToDoListSortItem((long)id, (long)listId, newSortOrder, listVersion);

            if (!result.success)
                await _unitOfWork.RollbackAsync();
            else
                await _unitOfWork.CommitAsync();

            return ToDoListPartialResult(result.success, result.viewState, (long)listId);
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
                (bool success, bool wasConflict, ToDoListViewState viewState) result = await _unitOfWork.ToDoItems.ToDoListAddItem(toDoItem);

                if (!result.success)
                    await _unitOfWork.RollbackAsync();
                else
                    await _unitOfWork.CommitAsync();

                return ToDoListPartialResult(result.success, result.viewState, toDoItem.ListId);
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
            (bool success, bool wasConflict, ToDoListViewState viewState) result = await _unitOfWork.ToDoItems.DeleteWithVersionCheck((long)id, (long)listId, listVersion);

            if (!result.success)
                await _unitOfWork.RollbackAsync();
            else
                await _unitOfWork.CommitAsync();

            return ToDoListPartialResult(result.success, result.viewState, (long)listId);
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
                    TempData["warning"] = "This list was modified by someone else while you were editing. The current item is shown. Please re-apply your changes.";
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

        private static GroceryListVM FormatGroceryListForDisplay(GroceryLists grocerylist, List<GroceryListItems> groceryListItems, IEnumerable<GroceryCategories> groceryCategoriesEnum, List<UserGroceryCategories> userGroceryCategories, long listId, bool isSharedWithCurrentUser = false)
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

            GroceryListVM groceryListVM = new()
            {
                GroceryList = grocerylist,
                GroceryListItem = new GroceryListItems
                {
                    ListId = listId,
                },
                GroceryCategoriesList = groceryListCategories,
                FilteredGroceryListItems = filteredGroceryListItems,
                UserSortedGroceryCategories = userGroceryCategories,
                IsSharedWithCurrentUser = isSharedWithCurrentUser,
            };
            return groceryListVM;
        }

        private static GroceryListEditVM FormatGroceryListEditForDisplay(GroceryLists parentList, GroceryListItems groceryListItem, IEnumerable<GroceryCategories> groceryCategoriesEnum)
        {
            IEnumerable<SelectListItem> groceryListCategories = groceryCategoriesEnum.Select(u => new SelectListItem
            {
                Text = u.Category,
                Value = u.Category
            });

            groceryListItem.ListVersion = parentList.Version;

            GroceryListEditVM vm = new()
            {
                Category = groceryListItem.Category,
                GroceryListItem = groceryListItem,
                GroceryCategoriesList = groceryListCategories
            };

            return vm;
        }
        private IActionResult GroceryListPartialResult(bool success, GroceryListViewState viewState, long listId)
        {
            GroceryListVM vm = FormatGroceryListForDisplay(
                viewState.GroceryList,
                viewState.ListItems,
                viewState.GroceryCategoriesEnum,
                viewState.UserGroceryCategories,
                listId);

            if (!success)
                Response.Headers["X-Concurrency-Conflict"] = "true";

            return PartialView("_GroceryListPartial", vm);
        }

        public async Task<IActionResult> MyGroceryLists()
        {
            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            List<GroceryLists> myGroceryLists = (await _unitOfWork.GroceryLists.GetAllWhere("UserId", userId)).ToList();
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

            GroceryListViewState viewState = await _unitOfWork.GroceryLists.GetAllItemsAndCategoriesInList(listId);
            bool isSharedWithCurrentUser = await _unitOfWork.GroceryListShares.IsGroceryListSharedWithUser((long)listId, userId);
            GroceryListVM groceryListVM = FormatGroceryListForDisplay(viewState.GroceryList, viewState.ListItems, viewState.GroceryCategoriesEnum, viewState.UserGroceryCategories, (long)listId, isSharedWithCurrentUser);
            return View(groceryListVM);
        }

        public async Task<IActionResult> CreateGroceryList()
        {
            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            GroceryLists groceryList = new GroceryLists
            {
                UserId = userId
            };

            return View(groceryList);
        }

        [HttpPost]
        public async Task<IActionResult> CreateGroceryList(GroceryLists groceryList)
        {
            if (ModelState.IsValid)
            {
                bool success = await _unitOfWork.GroceryLists.Add(groceryList);
                if (success)
                {
                    TempData["success"] = "Grocery list created successfully.";
                }
                else
                {
                    TempData["error"] = "Create grocery list error. Try again.";
                }
                return RedirectToAction("MyGroceryLists");
            }
            TempData["error"] = "Create grocery list error. Try again.";
            return RedirectToAction("MyGroceryLists");
        }

        //transaction method -- Concurrent
        [HttpPost]
        public async Task<IActionResult> GroceryListToggleComplete(long? id, long? listId, int listVersion)
        {
            if (id == null || id == 0 || listId == 0 || listId == null)
                return NotFound();

            await _unitOfWork.OpenConnectionAsync();
            await _unitOfWork.BeginTxnAsync();
            (bool success, bool wasConflict, GroceryListViewState viewState) result = await _unitOfWork.GroceryLists.GroceryListToggleComplete(id, listId, listVersion);
            if (!result.success)
                await _unitOfWork.RollbackAsync();
            else
                await _unitOfWork.CommitAsync();

            return GroceryListPartialResult(result.success, result.viewState, (long)listId);
        }

        //transaction method -- Concurrent
        [HttpPost]
        public async Task<IActionResult> GroceryListSortCategories(long? listId, int newSortOrder, int listVersion, string category)
        {
            if (listId == 0 || listId == null)
                return NotFound();

            await _unitOfWork.OpenConnectionAsync();
            await _unitOfWork.BeginTxnAsync();
            (bool success, bool wasConflict, GroceryListViewState viewState) result = await _unitOfWork.GroceryLists.GroceryListSortCategories(listId, newSortOrder, listVersion, category);
            if (!result.success)
                await _unitOfWork.RollbackAsync();
            else
                await _unitOfWork.CommitAsync();

            return GroceryListPartialResult(result.success, result.viewState, (long)listId);
        }

        public async Task<IActionResult> GroceryListEdit(long? id, long? listId)
        {
            if (id == null || id == 0 || listId == null || listId == 0)
                return NotFound();
            
            (GroceryLists parentList, GroceryListItems groceryListItem, IEnumerable<GroceryCategories> groceryCategoriesEnum) result = await _unitOfWork.GroceryListItems.GetGroceryListItemAndCategoriesAtId(id, listId);
            GroceryListEditVM vm = FormatGroceryListEditForDisplay(result.parentList, result.groceryListItem, result.groceryCategoriesEnum);
            return View(vm);
        }

        //transaction method -- Concurrent
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
                    (GroceryLists parentList, GroceryListItems groceryListItem, IEnumerable<GroceryCategories> groceryCategoriesEnum) resultB = await _unitOfWork.GroceryListItems.GetGroceryListItemAndCategoriesAtId(groceryListItem.Id, groceryListItem.ListId);
                    GroceryListEditVM latestVM = FormatGroceryListEditForDisplay(resultB.parentList, resultB.groceryListItem, resultB.groceryCategoriesEnum);
                    ModelState.Clear();
                    TempData["warning"] = "This list was modified by someone else while you were editing. The current item is shown. Please re-apply your changes.";
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

        //transaction method -- Concurrent
        [HttpPost]
        public async Task<IActionResult> GroceryListDeleteItem(long? id, long? listId, int listVersion)
        {
            if (id == null || id == 0 || listId == 0 || listId == null)
                return NotFound();

            await _unitOfWork.OpenConnectionAsync();
            await _unitOfWork.BeginTxnAsync();
            (bool success, bool wasConflict, GroceryListViewState viewState) result = await _unitOfWork.GroceryListItems.DeleteGroceryListItem(id, listId, listVersion);
            if (!result.success)
                await _unitOfWork.RollbackAsync();
            else
                await _unitOfWork.CommitAsync();

            return GroceryListPartialResult(result.success, result.viewState, (long)listId);
        }

        //transaction method -- Concurrent
        [HttpPost]
        public async Task<IActionResult> GroceryListAddItem(GroceryListItems groceryListItem, GroceryListVM groceryListVM)
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
                (bool success, bool wasConflict, GroceryListViewState viewState) result = await _unitOfWork.GroceryListItems.GroceryListAddItem(groceryListItem);
                if (!result.success)
                    await _unitOfWork.RollbackAsync();
                else
                    await _unitOfWork.CommitAsync();

                return GroceryListPartialResult(result.success, result.viewState, groceryListItem.ListId);
            }
            return StatusCode(400);
        }

        [HttpPost]
        [Route("/ListBuddy/GroceryListDeleteAll", Name = "deleteAllGroceryList")]
        public async Task<IActionResult> GroceryListDeleteAll(GroceryLists groceryList)
        {
            if (ModelState.IsValid)
            {
                bool success = await _unitOfWork.GroceryLists.Delete(groceryList.Id);
                if (success)
                {
                    TempData["success"] = "Grocery list deleted successfully.";
                }
                else
                {
                    TempData["error"] = "Delete grocery list error. Try again.";
                }
                return RedirectToAction("MyGroceryLists");
            }
            TempData["error"] = "Delete grocery list error. Try again.";
            return RedirectToAction("MyGroceryLists");
        }

        //transaction method
        [HttpPost]
        public async Task<JsonResult> SaveUserGroceryList(string userId, long? listId)
        {
            if (userId == null || userId == "" || listId == null || listId == 0)
            {
                return Json("error userId was null");
            }
            //jquery ajax handles the toast
            await _unitOfWork.OpenConnectionAsync();
            await _unitOfWork.BeginTxnAsync();
            bool success = await _unitOfWork.GroceryListItems.SaveUserGroceryListTemplate(userId, listId);
            if (!success)
            {
                await _unitOfWork.RollbackAsync();  
                return Json("failed to save list");
            }
            await _unitOfWork.CommitAsync();
            return Json("success");
        }

        //transaction method -- Concurrent
        [HttpPost]
        public async Task<IActionResult> UseSavedGroceryList(string userId, long? listId, int listVersion)
        {
            if (userId == null || userId == "" || listId == null || listId == 0)
                return NotFound();

            if (ModelState.IsValid) 
            {
                await _unitOfWork.OpenConnectionAsync();
                await _unitOfWork.BeginTxnAsync();
                (bool success, bool wasConflict) result = await _unitOfWork.GroceryListItems.UseSavedGroceryListTemplate(userId, listId, listVersion);
                if (result.wasConflict)
                {
                    await _unitOfWork.RollbackAsync();
                    TempData["warning"] = "This grocery list was modified by someone else before the saved template could be loaded.";
                    return RedirectToAction("GroceryList", new { listId });
                }
                if (!result.success)
                {
                    await _unitOfWork.RollbackAsync();
                    TempData["warning"] = "You currently don't have a saved list. Save one first.";
                    return RedirectToAction("GroceryList", new { listId });
                }
                await _unitOfWork.CommitAsync();
                TempData["success"] = "Grocery template loaded successfully.";
                return RedirectToAction("GroceryList", new { listId });
            }
            TempData["error"] = "Use grocery template error. Please try again.";
            return RedirectToAction("MyGroceryLists");
        }

        public async Task<IActionResult> ShareGroceryList(long? id) 
        {
            if (id == null || id == 0) return NotFound();

            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            GroceryLists groceryList = await _unitOfWork.GroceryLists.GetById(id);

            // Only the owner can share their list
            if (groceryList.UserId != userId)
            {
                TempData["warning"] = "You can only share your own lists.";
                return RedirectToAction("MyGroceryLists");
            }

            IEnumerable<SelectListItem> friendsList = await _friendAccessService.GetFriendsSelectListAsync(userId);

            ShareGroceryListVM vm = new()
            {
                ListId = groceryList.Id,
                ListTitle = groceryList.ListTitle,
                FriendsList = friendsList
            };
            return View(vm);
        }

        //transaction method
        [HttpPost]
        public async Task<IActionResult> ShareGroceryList(ShareGroceryListVM vm)
        {
            if (!ModelState.IsValid) 
            {
                TempData["error"] = "Share list error. Try again.";
                return RedirectToAction("MyGroceryLists");
            }
            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            if (!await _friendAccessService.AreFriendsAsync(userId, vm.SelectedFriendUserId))
            {
                TempData["warning"] = "You can only share lists with friends.";
                return RedirectToAction("MyGroceryLists");
            }
            GroceryLists groceryList = await _unitOfWork.GroceryLists.GetById(vm.ListId);
            // Verify the list belongs to the current user
            if (groceryList.UserId != userId)
            {
                TempData["warning"] = "You can only share your own lists.";
                return RedirectToAction("MyGroceryLists");
            }
            //share it
            bool success = await _unitOfWork.GroceryListShares.ShareGroceryList(vm.ListId, vm.SelectedFriendUserId);
            if (success)
            {
                string message = $"{User.Identity?.Name} shared grocery list '{groceryList.ListTitle}' with you.";
                await _hubContext.Clients.User(vm.SelectedFriendUserId).SendAsync("ReceiveNotification", message);

                //update notifications table
                Notifications notification = new()
                {
                    UserId = vm.SelectedFriendUserId,
                    SenderUserId = userId,
                    Message = message,
                    NotificationType = NotificationType.GroceryListShared.ToString(),
                    RelatedEntityId = groceryList.Id
                };
                await _unitOfWork.Notifications.Add(notification);
            }
            TempData[success ? "success" : "error"] = success
                ? "List shared successfully."
                : "Could not share list. It may already be shared with this friend.";
            return RedirectToAction("GroceryList", new { listId = vm.ListId });
        }

        [HttpPost]
        public async Task<IActionResult> UnShareGroceryList(long listId) 
        {
            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            GroceryLists groceryList = await _unitOfWork.GroceryLists.GetById(listId);
            // Only the owner can share their list
            if (groceryList.UserId != userId)
            {
                TempData["warning"] = "You can only manage sharing on your own lists.";
                return RedirectToAction("MyGroceryLists");
            }
            bool success = await _unitOfWork.GroceryListShares.UnshareGroceryList(listId);
            TempData[success ? "success" : "error"] = success
                ? "List unshared."
                : "Could not unshare list.";

            return RedirectToAction("MyGroceryLists");
        }

        [HttpPost]
        public async Task<IActionResult> NotifyGroceryListEditingDone(long listId)
        {
            if (listId == 0) return NotFound();

            string? userId = User.GetUserId();
            if (string.IsNullOrEmpty(userId)) return NotFound();

            GroceryLists groceryList = await _unitOfWork.GroceryLists.GetById(listId);
            if (groceryList == null) return NotFound();

            bool isOwner = groceryList.UserId == userId;
            bool isSharedWithCurrentUser = await _unitOfWork.GroceryListShares.IsGroceryListSharedWithUser(listId, userId);

            if (!isOwner && !isSharedWithCurrentUser)
            {
                TempData["warning"] = "You can only send done editing notifications for grocery lists you own or lists shared with you.";
                return RedirectToAction("MyGroceryLists");
            }

            string message = $"{User.Identity?.Name} is done editing grocery list '{groceryList.ListTitle}'.";
            List<string> sharedUserIds = await _unitOfWork.GroceryListShares.GetSharedUserIdsForList(listId);
            /*
             * If owner of list send send notifications to everyone in sharedUserIds
             * else send notifications to the owner and other shared friends
             * 
             * append to add owner userId to list and Where removes the current user's userId from the list
             */
            List<string> recipientUserIds = isOwner ? sharedUserIds : sharedUserIds.Append(groceryList.UserId).Where(uid => uid != userId).Distinct().ToList();

            if (!recipientUserIds.Any())
            {
                TempData["warning"] = isOwner
                    ? "This grocery list is not shared with any friends yet."
                    : "There is no one else to notify for this grocery list.";
                return RedirectToAction("GroceryList", new { listId });
            }

            foreach (string recipientUserId in recipientUserIds)
            {
                await SendGroceryListDoneEditingNotification(recipientUserId, userId, message, groceryList.Id);
            }

            TempData["success"] = isOwner
                ? "Friends shared on this grocery list have been notified."
                : "The owner and other friends shared on this grocery list have been notified.";
            return RedirectToAction("GroceryList", new { listId });
        }
        private async Task SendGroceryListDoneEditingNotification(string recipientUserId, string senderUserId, string message, long groceryListId)
        {
            await _hubContext.Clients.User(recipientUserId).SendAsync("ReceiveNotification", message);

            Notifications notification = new()
            {
                UserId = recipientUserId,
                SenderUserId = senderUserId,
                Message = message,
                NotificationType = NotificationType.GroceryListDoneEditing.ToString(),
                RelatedEntityId = groceryListId
            };
            await _unitOfWork.Notifications.Add(notification);
        }
        #endregion
    }
}
