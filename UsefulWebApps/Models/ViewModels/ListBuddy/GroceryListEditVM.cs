using Microsoft.AspNetCore.Mvc.Rendering;
using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.Models.ViewModels.ListBuddy
{
    public class GroceryListEditVM
    {
        public string Category { get; set; }
        public GroceryList GroceryList { get; set; }
        public IEnumerable<SelectListItem> GroceryCategoriesList { get; set; }
    }
}
