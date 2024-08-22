using Microsoft.AspNetCore.Razor.TagHelpers;

namespace UsefulWebApps.TagHelpers
{
    public class IngredientsTagHelper : TagHelper
    {
        public string HtmlIngredientsContent { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";    // Replaces <ingredients> with <div> tag
            output.Content.SetHtmlContent(HtmlIngredientsContent);
        }
    }
}
