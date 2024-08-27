using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using Dapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using UsefulWebApps.Models.ListBuddy;
using UsefulWebApps.Models.ViewModels.ListBuddy;
using static Dapper.SqlMapper;
using UsefulWebApps.Repository.IRepository;


namespace UsefulWebApps.Controllers
{
    public class ListBuddyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly MySqlConnection _connection;
        public ListBuddyController(MySqlConnection db, IUnitOfWork unitOfWork)
        {
            _connection = db;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }

        #region To Do List

        public async Task<IActionResult> ToDoList()
        {
            
            List<ToDoList> listItems = (List<ToDoList>)await _unitOfWork.ToDoList.GetAll();
            ToDoListVM toDoListVM = new()
            {
                ToDoListItems = listItems,
                ToDoList = new ToDoList()
            };
            await _connection.CloseAsync();
            return View(toDoListVM);
        }

        [HttpPost]
        [Route("/ListBuddy/ToDoListToggleComplete", Name = "toggleToDoComplete")]
        public async Task<IActionResult> ToDoListToggleComplete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            await _unitOfWork.ToDoList.ToDoListToggleComplete(id);
            return RedirectToAction("ToDoList");
        }

        [HttpPost]
        [Route("/ListBuddy/ToDoListCreate", Name = "createToDoItem")]
        public async Task<IActionResult> ToDoListCreate(ToDoList toDoList)
        {
            if (ModelState.IsValid)
            {
                bool success = await _unitOfWork.ToDoList.Add(toDoList);
                if (success)
                {
                    TempData["success"] = "To do item created successfully";
                }   
                else
                {
                    TempData["error"] = "Add To do item error. Please try again.";
                }
                return RedirectToAction("ToDoList");
            }
            TempData["error"] = "Add To do item error. Please try again.";
            return RedirectToAction("ToDoList");
        }

        [HttpPost]
        [Route("/ListBuddy/ToDoListDeleteItem", Name = "deleteToDoItem")]
        public async Task<IActionResult> ToDoListDeleteItem(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            await _unitOfWork.ToDoList.Delete(id);
            return RedirectToAction("ToDoList");
        }

        [HttpPost]
        [Route("/ListBuddy/ToDoListDeleteAll", Name = "deleteAllToDoList")]
        public async Task<IActionResult> ToDoListDeleteAll()
        {
            await _unitOfWork.ToDoList.DeleteAll();
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
        public async Task<IActionResult> GroceryList()
        {

            string sqlMult = @"
                SELECT * FROM grocery_list;
                SELECT * FROM grocery_categories;
            ";
            GridReader gridReader = await _connection.QueryMultipleAsync(sqlMult);
            List<GroceryList> groceryListItems = (List<GroceryList>)await gridReader.ReadAsync<GroceryList>();
            IEnumerable<GroceryCategories> groceryCategoriesEnum = await gridReader.ReadAsync<GroceryCategories>();
            await _connection.CloseAsync();

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
                GroceryList = new GroceryList(),
                GroceryCategoriesList = groceryListCategories,
                FilteredGroceryListItems = filteredGroceryListItems
            };
           
            return View(groceryListVM);
        }

        [HttpPost]
        [Route("/ListBuddy/GroceryListToggleComplete", Name = "toggleGroceryComplete")]
        public async Task<IActionResult> GroceryListToggleComplete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            await _connection.OpenAsync();
            MySqlTransaction txn = await _connection.BeginTransactionAsync();
            string sql = "SELECT Complete FROM grocery_list WHERE Id = @id";
            bool isComplete = await _connection.QuerySingleAsync<bool>(sql, new { id }, transaction: txn);
            string sql2 = String.Empty;
            if (isComplete)
            {
                sql2 = "UPDATE grocery_list SET Complete = False WHERE Id = @id";
            }
            else
            {
                sql2 = "UPDATE grocery_list SET Complete = True WHERE Id = @id";
            }
            await _connection.ExecuteAsync(sql2, new { id }, transaction: txn);
            await txn.CommitAsync();
            await _connection.CloseAsync();
            return RedirectToAction("GroceryList");
        }

        public async Task<IActionResult> GroceryListEdit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            string sqlMult = @"
                SELECT * FROM grocery_list WHERE Id = @id;
                SELECT * FROM grocery_categories;
            ";
            GridReader gridReader = await _connection.QueryMultipleAsync(sqlMult, new { id });
            GroceryList groceryList = await gridReader.ReadFirstAsync<GroceryList>();
            IEnumerable<GroceryCategories> groceryListCategoriesEnum = await gridReader.ReadAsync<GroceryCategories>();
            await _connection.CloseAsync();
            IEnumerable<SelectListItem> groceryListCategories = groceryListCategoriesEnum.Select(u => new SelectListItem
            {
                Text= u.Category,
                Value= u.Category
            });

            GroceryListEditVM groceryListEditVM = new()
            {
                Category = groceryList.Category,    
                GroceryList = groceryList,
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
                string sql = "UPDATE grocery_list SET GroceryItem = @groceryItem, Category = @category WHERE Id = @id";
                await _connection.ExecuteAsync(sql, new { groceryItem = groceryList.GroceryItem, category = groceryList.Category, id = groceryList.Id });
                TempData["success"] = "Grocery item updated successfully";
                await _connection.CloseAsync();
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
            string sql = "DELETE FROM grocery_list WHERE Id = @id";
            await _connection.ExecuteAsync(sql, new { id });
            await _connection.CloseAsync();
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
                string sql = "INSERT INTO grocery_list (GroceryItem, Category, Complete) VALUES (@newGroceryItem, @itemCategory, @isComplete)";
                await _connection.ExecuteAsync(sql, new { newGroceryItem = groceryListVM.GroceryList.GroceryItem, itemCategory = groceryListVM.GroceryList.Category, isComplete = groceryListVM.GroceryList.Complete });
                TempData["success"] = "Grocery item created successfully";
                await _connection.CloseAsync();
                return RedirectToAction("GroceryList");
            }
            TempData["error"] = "Add grocery item error. Please try again.";
            return RedirectToAction("GroceryList");
        }

        [HttpPost]
        [Route("/ListBuddy/GroceryListDeleteAll", Name = "deleteAllGroceryList")]
        public async Task<IActionResult> GroceryListDeleteAll()
        {
            await _connection.OpenAsync();
            MySqlTransaction txn = await _connection.BeginTransactionAsync();
            string sql = "DELETE FROM grocery_list WHERE Id >= 1";
            string sql2 = "ALTER TABLE grocery_list AUTO_INCREMENT = 1";
            await _connection.ExecuteAsync(sql, transaction: txn);
            await _connection.ExecuteAsync(sql2, transaction: txn);
            await txn.CommitAsync();
            await _connection.CloseAsync();
            return RedirectToAction("GroceryList");
        }
        #endregion
    }
}
