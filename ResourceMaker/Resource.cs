using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Media;

namespace ResourceMaker
{
    public class Resource
    {
        public string Name { get; set; }
        public string Layer { get; set; }
        public Color TransparentColor { get; set; }
        public Rect Crop { get; set; }
        public ObservableCollection<Rect> ForbiddenAreas { get; set; }
    }
}
