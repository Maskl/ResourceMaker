using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media;

namespace ResourceMaker
{
  public class ColorEvent
  {
    public ColorEvent(SolidColorBrush color)
    {
      Color = color;
    }

    public SolidColorBrush Color { get; private set; }
  }
}
