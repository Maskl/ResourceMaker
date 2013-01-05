using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Windows;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace ResourceMaker
{
  [Export(typeof(AppViewModel))]
  public class AppViewModel : PropertyChangedBase, IHandle<ColorEvent>
  {
    private SolidColorBrush _Color;

      private readonly IWindowManager _windowManager;

    // Get the event aggregator through the constructor and store it in a field so we can publish messages later.
    private readonly IEventAggregator _events;

    [ImportingConstructor]
    public AppViewModel(IWindowManager windowManager, IEventAggregator events)
    {
        _windowManager = windowManager;
        _events = events;
        // Get the event aggregator through the constructor and subscribe this ColorViewModel so it can listen for ColorEvent messages.
        events.Subscribe(this);
    }

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

    public void OpenWindow()
    {
        dynamic settings = new ExpandoObject();
        settings.WindowStartupLocation = WindowStartupLocation.Manual;
        settings.WindowStyle = WindowStyle.ToolWindow;
        settings.ShowInTaskbar = false;   

        _events.Publish(new ColorEvent(new SolidColorBrush(Colors.Red)));
        _windowManager.ShowDialog(new ColorViewModel(_windowManager, _events), null, settings);
    }
  }
}
