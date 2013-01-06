using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace ResourceMaker
{
    [DisplayName("Resource file")]
    public class ResourceFile
    {
        [Category("Bitmap")]
        [DisplayName("File name")]
        [ReadOnly(true)]
        public string BitmapFileName { get; set; }

        [Category("Bitmap")]
        [DisplayName("Transparent color")]
        public Color TransparentColor { get; set; }

        [Category("Resources")]
        [DisplayName("Layers")]
        public ObservableCollection<string> Layers { get; set; }

        [Category("Resources")]
        [DisplayName("Resources")]
        public ObservableCollection<Resource> Resources { get; set; }
    }
}
