using System.Windows;

namespace Sklodowski.ResourceMaker.Extensions
{
    static class Int32RectExtensionMethods
    {
        public static bool Contains(this Int32Rect instance, Point point)
        {
            return new Rect(instance.X, instance.Y, instance.Width, instance.Height).Contains(point);
        }
    }
}
