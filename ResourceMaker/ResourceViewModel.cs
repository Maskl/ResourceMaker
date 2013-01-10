using System;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using System.Windows.Media;

namespace ResourceMaker
{
    [Export(typeof(ResourceViewModel))]
    public class ResourceViewModel : PropertyChangedBase
    {
        private readonly IEventAggregator _events;

        static CroppedBitmap CreateCroppedBitmapResolutionInvariant(Uri uri, Int32Rect crop)
        {
            var bitmap = new BitmapImage(uri);
            var rect = new Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight);
            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(bitmap, rect);
            }
            var resizedImage = new RenderTargetBitmap(
                (int)rect.Width, (int)rect.Height,
                96, 96,
                PixelFormats.Default);
            resizedImage.Render(drawingVisual);
            return new CroppedBitmap(resizedImage, crop);
        }

        [ImportingConstructor]
        public ResourceViewModel(IEventAggregator events, Resource resource, ResourceFile resourceFile, string currentDirectory)
        {
            _events = events;
            Resource = resource;
            ResourceFile = resourceFile;
            CurrentDirectory = currentDirectory;

            ResourceBitmap = CreateCroppedBitmapResolutionInvariant(new Uri(CurrentDirectory + ResourceFile.BitmapFileName), resource.Crop);
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


        private CroppedBitmap _resourceBitmap;
        public CroppedBitmap ResourceBitmap
        {
            get { return _resourceBitmap; }
            set { _resourceBitmap = value; NotifyOfPropertyChange(() => ResourceBitmap); }
        }

        private ResourceFile _resourceFile;
        public ResourceFile ResourceFile
        {
            get { return _resourceFile; }
            set { _resourceFile = value; NotifyOfPropertyChange(() => ResourceFile); }
        }

        private Resource _resource;
        public Resource Resource
        {
            get { return _resource; }
            set { _resource = value; NotifyOfPropertyChange(() => Resource); }
        }

        private string _currentDirectory;
        public string CurrentDirectory
        {
            get { return _currentDirectory; }
            set { _currentDirectory = value; NotifyOfPropertyChange(() => CurrentDirectory); }
        }
    }
}
