using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Media;
using Sklodowski.ResourceMaker.Features.ResourceWindow;

namespace Sklodowski.ResourceMaker.Features.App
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

        [Category("Bitmap")]
        [DisplayName("Has transparent color")]
        public bool HasTransparentColor { get; set; }

        [Category("Data")]
        [DisplayName("Resources")]
        public ObservableCollection<Resource> Resources { get; set; }

        public void Save(BinaryWriter writer)
        {
            writer.Write(BitmapFileName);
            writer.Write(HasTransparentColor ? 1 : 0);
            writer.Write(HasTransparentColor ? TransparentColor.R : 0);
            writer.Write(HasTransparentColor ? TransparentColor.G : 0);
            writer.Write(HasTransparentColor ? TransparentColor.B : 0);
            writer.Write(HasTransparentColor ? TransparentColor.A : 0);

            var layers = GetAllLayersNames();
            writer.Write(layers.Count);
            foreach (var layer in layers)
            {
                writer.Write(layer);
            }

            writer.Write(Resources.Count);
            foreach (var resource in Resources)
            {
                resource.Save(writer);
            }
        }

        private HashSet<string> GetAllLayersNames()
        {
            var set = new HashSet<string>();
            foreach (var resource in Resources)
            {
                set.Add(resource.Layer);
            }
            return set;
        }

        public static ResourceFile Load(BinaryReader reader)
        {
            var resourceFile = new ResourceFile();
            resourceFile.BitmapFileName = reader.ReadString();
            resourceFile.HasTransparentColor = reader.ReadInt32() == 1;
            var r = reader.ReadByte();
            var g = reader.ReadByte();
            var b = reader.ReadByte();
            var a = reader.ReadByte();
            resourceFile.TransparentColor = Color.FromArgb(a, r, g, b);

            var layersCount = reader.ReadInt32();
            for (var i = 0; i < layersCount; ++i)
            {
                reader.ReadString();
            }

            resourceFile.Resources = new ObservableCollection<Resource>();
            var resourcesCount = reader.ReadInt32();
            for (var i = 0; i < resourcesCount; ++i)
            {
                var resource = Resource.Load(reader);
                resourceFile.Resources.Add(resource);
            }

            return resourceFile;
        }
    }
}
