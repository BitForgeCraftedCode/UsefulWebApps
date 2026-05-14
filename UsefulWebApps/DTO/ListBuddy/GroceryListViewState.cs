using UsefulWebApps.Models.ListBuddy;

namespace UsefulWebApps.DTO.ListBuddy
{
    public class GroceryListViewState
    {
        public GroceryLists GroceryList { get; set; }

        public List<GroceryListItems> ListItems { get; set; }

        public IEnumerable<GroceryCategories> GroceryCategoriesEnum { get; set; }

        public List<UserGroceryCategories> UserGroceryCategories { get; set; }
    }
}
