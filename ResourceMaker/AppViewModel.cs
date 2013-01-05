using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace ResourceMaker
{
  [Export(typeof(AppViewModel))]
  public class AppViewModel : PropertyChangedBase, IHandle<ColorEvent>
  {
    private SolidColorBrush _Color;

    [ImportingConstructor]
    public AppViewModel(ColorViewModel colorModel, IEventAggregator events)
    {
      ColorModel = colorModel;

      // Get the event aggregator through the constructor and subscribe this ColorViewModel so it can listen for ColorEvent messages.
      events.Subscribe(this);
    }

    public ColorViewModel ColorModel { get; private set; }

    // This property is for changing the color of the rectangle.
    public SolidColorBrush Color
    {
      get { return _Color; }
      set
      {
        _Color = value;
        NotifyOfPropertyChange(() => Color);
      }
    }

    // This method is called after a ColorEvent message is published from somewhere else in the application.
    public void Handle(ColorEvent message)
    {
      Color = message.Color;
    }
  }
}
