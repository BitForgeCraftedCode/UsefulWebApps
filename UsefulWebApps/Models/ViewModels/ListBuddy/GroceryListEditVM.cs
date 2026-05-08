using Microsoft.AspNetCore.Mvc.Rendering;
using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.Models.ViewModels.ListBuddy
{
    public class GroceryListEditVM
    {
        public string Category { get; set; }
        public GroceryListItems GroceryListItem { get; set; }
        public IEnumerable<SelectListItem> GroceryCategoriesList { get; set; }
    }
}
