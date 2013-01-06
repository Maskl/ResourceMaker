using System.ComponentModel.Composition;
using Caliburn.Micro;
using System.Windows.Media;

namespace ResourceMaker
{
    [Export(typeof(ResourceViewModel))]
    public class ResourceViewModel
    {
        private readonly IEventAggregator _events;

        [ImportingConstructor]
        public ResourceViewModel(IEventAggregator events)
        {
            _events = events;
        }

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
