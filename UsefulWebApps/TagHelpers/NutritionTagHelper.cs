using Microsoft.AspNetCore.Razor.TagHelpers;
namespace UsefulWebApps.TagHelpers
{
    public class NutritionTagHelper : TagHelper
    {
        public string HtmlNutritionContent { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";    // Replaces <nutrition> with <div> tag
            output.Content.SetHtmlContent(HtmlNutritionContent);
        }
    }
}
