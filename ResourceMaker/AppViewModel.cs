using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Windows;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;

namespace ResourceMaker
{
    public class Filters
    {
        public string Name { get; set; }
        public string Category { get; set; }
    }

    [Export(typeof(AppViewModel))]
    public class AppViewModel : PropertyChangedBase, IHandle<ColorEvent>
    {
        private readonly IWindowManager _windowManager;
        private readonly IEventAggregator _eventAggregator;

        [ImportingConstructor]
        public AppViewModel(IWindowManager windowManager, IEventAggregator eventAggregator)
        {
            _windowManager = windowManager;
            _eventAggregator = eventAggregator;
            eventAggregator.Subscribe(this);
            _resourceFile = new ResourceFile();
            _resourceFile.BitmapFileName = "bitmap.bmp";
            _resourceFile.Layers = new ObservableCollection<string>();
            _resourceFile.Layers.Add("Woda");
            _resourceFile.Layers.Add("Teren");
            _resourceFile.Layers.Add("Drzewa");
            _resourceFile.Layers.Add("Budynki");

            _filters = new Filters();
        }

        public void OpenWindow()
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.Manual;
            settings.WindowStyle = WindowStyle.ToolWindow;
            settings.ShowInTaskbar = false;
            _windowManager.ShowDialog(new ResourceViewModel(_eventAggregator), null, settings);
        }

        private SolidColorBrush _color;
        public SolidColorBrush Color
        {
            get { return _color; }
            set { _color = value; NotifyOfPropertyChange(() => Color); }
        }

        private ResourceFile _resourceFile;
        public ResourceFile ResourceFile
        {
            get { return _resourceFile; }
            set { _resourceFile = value; NotifyOfPropertyChange(() => ResourceFile); }
        }

        private Filters _filters;
        public Filters Filters
        {
            get { return _filters; }
            set { _filters = value; NotifyOfPropertyChange(() => Filters); }
        }

        public void Handle(ColorEvent message)
        {
            Color = message.Color;
        }
    }
}
