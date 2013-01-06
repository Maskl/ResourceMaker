using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Windows;
using System.Windows.Media.Imaging;
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
            IsNoBitmapLoaded = true;
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

        public void LoadBitmap()
        {
            var fdialog = new Microsoft.Win32.OpenFileDialog();
            var res = fdialog.ShowDialog();
            if (!res.Value)
                return;

            PrepareNewResourceFile(fdialog.FileName);
        }

        public void PrepareNewResourceFile(string bitmapFileName)
        {
            var uri = new Uri(bitmapFileName);
            ResourcesBitmap = new BitmapImage(uri);
            IsNoBitmapLoaded = false;

            ResourceFile = new ResourceFile
                             {
                                 BitmapFileName = System.IO.Path.GetFileName(uri.LocalPath),
                                 TransparentColor = Colors.Magenta,
                                 Layers = new ObservableCollection<string>()
                             };
        }

        public void Exit()
        {
            Application.Current.Shutdown();
        }

        public void About()
        {
            MessageBox.Show("Application by Marek Skłodowski, Poznań University of Technology 2013");
        }

        public bool CanSave()
        {
            return !IsNoBitmapLoaded;
        }

        public void Save()
        {
            MessageBox.Show("Save");
        }

        public bool CanSaveAs()
        {
            return !IsNoBitmapLoaded;
        }

        private bool _isNoBitmapLoaded;
        public bool IsNoBitmapLoaded
        {
            get { return _isNoBitmapLoaded; }
            set { _isNoBitmapLoaded = value; NotifyOfPropertyChange(() => IsNoBitmapLoaded); }
        }

        private BitmapImage _resourcesBitmap;
        public BitmapImage ResourcesBitmap
        {
            get { return _resourcesBitmap; }
            set { _resourcesBitmap = value; NotifyOfPropertyChange(() => ResourcesBitmap); }
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
