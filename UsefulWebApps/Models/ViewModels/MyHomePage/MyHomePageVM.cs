using UsefulWebApps.Models.MyHomePage;

namespace UsefulWebApps.Models.ViewModels.MyHomePage
{
    public class MyHomePageVM
    {
        public List<SlideShowImages> SlideShowImagesToDisplay { get; set; }
        public List<string> DefaultSlideShowImagesToDisplay { get; set; }
        public List<QuickLinks> QuickLinksToDisplay { get; set; }
        public Quotes RandomQuote { get; set; }
    }
}
