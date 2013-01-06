using System.Collections.ObjectModel;

namespace ResourceMaker
{
    public class ResourceFile
    {
        public string BitmapFileName { get; set; }
        public ObservableCollection<string> Layers { get; set; }
    }
}
