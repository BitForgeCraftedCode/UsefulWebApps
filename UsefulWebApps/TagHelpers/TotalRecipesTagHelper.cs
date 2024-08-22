using Microsoft.AspNetCore.Razor.TagHelpers;

namespace UsefulWebApps.TagHelpers
{
    public class TotalRecipesTagHelper : TagHelper
    {
        public string TotalRecipes { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "p";    // Replaces <ingredients> with <p> tag
            output.Content.SetContent("Total Recipes: " + TotalRecipes);
        }
    }
}
