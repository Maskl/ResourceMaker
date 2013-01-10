using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace ResourceMaker
{
    static class Int32ExtensionMethods
    {
        public static bool Contains(this Int32Rect instance, Point point)
        {
            return new Rect(instance.X, instance.Y, instance.Width, instance.Height).Contains(point);
        }
    }
}
