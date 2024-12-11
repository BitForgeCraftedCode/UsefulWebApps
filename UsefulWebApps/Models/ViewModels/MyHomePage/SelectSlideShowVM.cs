using UsefulWebApps.Models.MyHomePage;

namespace UsefulWebApps.Models.ViewModels.MyHomePage
{
    public class SelectSlideShowVM
    {
        public List<SlideShowFolder> SlideShowFolders { get; set; }
        public SlideShowFolder SelectedSlideShowFolder { get; set; }
    }
}
