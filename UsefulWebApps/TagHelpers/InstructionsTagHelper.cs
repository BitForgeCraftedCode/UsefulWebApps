using Microsoft.AspNetCore.Razor.TagHelpers;

namespace UsefulWebApps.TagHelpers
{
    public class InstructionsTagHelper : TagHelper
    {
        public string HtmlInstructionsContent { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";    // Replaces <instructions> with <div> tag
            output.Content.SetHtmlContent(HtmlInstructionsContent);
        }
    }
}
