using Microsoft.AspNetCore.Razor.TagHelpers;

namespace UsefulWebApps.TagHelpers
{
    public class NotesTagHelper : TagHelper
    {
        public string HtmlNotesContent { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            output.TagName = "div";    // Replaces <notes> with <div> tag
            output.Content.SetHtmlContent(HtmlNotesContent);
        }
    }
}
