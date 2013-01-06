using System.Dynamic;
using System.Windows;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Windows.Media;

namespace ResourceMaker
{
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

        public void Handle(ColorEvent message)
        {
            Color = message.Color;
        }
    }
}
