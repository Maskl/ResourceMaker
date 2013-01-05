﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.Windows;
using Caliburn.Micro;
using System.Windows.Media;

namespace ResourceMaker
{
  [Export(typeof(ColorViewModel))]
  public class ColorViewModel
  {

      private readonly IWindowManager _windowManager;

    // Get the event aggregator through the constructor and store it in a field so we can publish messages later.
    private readonly IEventAggregator _events;

    [ImportingConstructor]
    public ColorViewModel(IWindowManager windowManager, IEventAggregator events)
    {
        _windowManager = windowManager;
      _events = events;
    }

    // When a radio button is clicked, publish a message containing the appropriate color:

    public void Red()
    {
      _events.Publish(new ColorEvent(new SolidColorBrush(Colors.Red)));
    }

    public void Green()
    {
      _events.Publish(new ColorEvent(new SolidColorBrush(Colors.Green)));
    }

    public void Blue()
    {
      _events.Publish(new ColorEvent(new SolidColorBrush(Colors.Blue)));
    }
  }
}
