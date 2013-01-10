using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace ResourceMaker
{
    public class Resource
    {
        [Category("Main")]
        [DisplayName("Layer")]
        public string Name { get; set; }

        [Category("Main")]
        [DisplayName("Layer")]
        public string Layer { get; set; }

        [Category("Main")]
        [DisplayName("Crop")]
        public Int32Rect Crop { get; set; }

        [Category("Main")]
        [DisplayName("Forbidden Areas")]
        public ObservableCollection<Rect> ForbiddenAreas { get; set; }

        [Category("Transparency")]
        [DisplayName("Transparent Color")]
        public Color TransparentColor { get; set; }

        [Category("Transparency")]
        [DisplayName("Has transparent color")]
        public bool HasTransparentColor { get; set; }
    }
}
