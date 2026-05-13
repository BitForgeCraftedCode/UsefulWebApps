using Microsoft.AspNetCore.Mvc.Rendering;
using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.Models.ViewModels.ListBuddy
{
    public class GroceryListVM
    {
        public string Category { get; set; }
        public GroceryLists GroceryList { get; set; }
        public GroceryListItems GroceryListItem { get; set; }
        public IEnumerable<SelectListItem> GroceryCategoriesList { get; set; }
        public List<List<GroceryListItems>> FilteredGroceryListItems { get; set; }
        public List<UserGroceryCategories> UserSortedGroceryCategories { get; set; }
    }
}
