using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows;
using System.Windows.Media;

namespace ResourceMaker.Features.Resource
{
    public class Resource
    {
        [Category("Main")]
        [DisplayName("Name")]
        public string Name { get; set; }

        [Category("Main")]
        [DisplayName("Layer")]
        public string Layer { get; set; }

        [Category("Main")]
        [DisplayName("Crop")]
        public Int32Rect Crop { get; set; }

        [Category("Main")]
        [DisplayName("Forbidden Areas")]
        public ObservableCollection<Int32Rect> ForbiddenAreas { get; set; }

        [Category("Transparency")]
        [DisplayName("Transparent Color")]
        public Color TransparentColor { get; set; }

        [Category("Transparency")]
        [DisplayName("Has transparent color")]
        public bool HasTransparentColor { get; set; }

        public void Save(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(HasTransparentColor ? 1 : 0);
            writer.Write(HasTransparentColor ? TransparentColor.R : 0);
            writer.Write(HasTransparentColor ? TransparentColor.G : 0);
            writer.Write(HasTransparentColor ? TransparentColor.B : 0);
            writer.Write(HasTransparentColor ? TransparentColor.A : 0);
            writer.Write(Layer);
            writer.Write(Crop.X);
            writer.Write(Crop.Y);
            writer.Write(Crop.Width);
            writer.Write(Crop.Height);
            writer.Write(ForbiddenAreas.Count);
            foreach (var forbiddenArea in ForbiddenAreas)
            {
                var x1 = forbiddenArea.X;
                var y1 = forbiddenArea.Y;
                var x2 = forbiddenArea.X + forbiddenArea.Width;
                var y2 = forbiddenArea.Y + forbiddenArea.Height;
                writer.Write(x1);
                writer.Write(y1);
                writer.Write(x2);
                writer.Write(y1);
                writer.Write(x2);
                writer.Write(y2);
                writer.Write(x1);
                writer.Write(y2);
            }
        }

        public static Resource Load(BinaryReader reader)
        {
            var resource = new Resource();
            resource.Name = reader.ReadString();
            resource.HasTransparentColor = reader.ReadInt32() == 1;

            var r = reader.ReadByte();
            var g = reader.ReadByte();
            var b = reader.ReadByte();
            var a = reader.ReadByte();
            resource.TransparentColor = Color.FromArgb(a, r, g, b);

            resource.Layer = reader.ReadString();

            var cropX = reader.ReadInt32();
            var cropY = reader.ReadInt32();
            var cropW = reader.ReadInt32();
            var cropH = reader.ReadInt32();
            resource.Crop = new Int32Rect(cropX, cropY, cropW, cropH);

            resource.ForbiddenAreas = new ObservableCollection<Int32Rect>();
            var forbiddenAreas = reader.ReadInt32();
            for (var i = 0; i < forbiddenAreas; ++i)
            {
                var x1A = reader.ReadInt32();
                var y1A = reader.ReadInt32();
                var x2A = reader.ReadInt32();
                var y1B = reader.ReadInt32();
                var x2B = reader.ReadInt32();
                var y2A = reader.ReadInt32();
                var x1B = reader.ReadInt32();
                var y2B = reader.ReadInt32();
                if (x1A != x1B || x2A != x2B || y1A != y1B || y2A != y2B)
                    throw new Exception("Invalid forbidden area (non rectangle)");

                resource.ForbiddenAreas.Add(new Int32Rect(x1A, y1A, x2A - x1A, y2A - y1A));
            }

            return resource;
        }
    }
}
