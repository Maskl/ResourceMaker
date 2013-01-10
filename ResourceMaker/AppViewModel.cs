using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
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
            New();
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
                                 Layers = new ObservableCollection<string> {"Default"},
                                 Resources = new ObservableCollection<Resource>()
                             };
        }

        public void New()
        {
            IsNoBitmapLoaded = true;
            NewResourceStart = null;
            NewResourceTemporary = null;
            ResourceFile = null;
            ResourcesBitmap = null;
            Filters = new Filters();
        }

        public void Open()
        {
            MessageBox.Show("Open");
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
            return CanSave();
        }

        public void SaveAs()
        {
            MessageBox.Show("Save as");
        }

        public void Exit()
        {
            Application.Current.Shutdown();
        }

        public void About()
        {
            MessageBox.Show("Application by Marek Skłodowski, Poznań University of Technology 2013");
        }

        public void BitmapMouseDown(Point mousePosition)
        {
            if (NewResourceTemporary == null && !IsResourceUnderMousePointer(mousePosition))
                NewResourceStart = mousePosition;
        }

        public void BitmapMouseUp(Point mousePosition)
        {
            if (NewResourceTemporary.HasValue)
            {
                CreateNewResource(NewResourceTemporary.Value);
                NewResourceStart = null;
                NewResourceTemporary = null;
                return;
            }

            var resource = GetResourceUnderMousePointer(mousePosition);
            if (resource != null)
                MessageBox.Show(resource.Crop.ToString());
        }

        public void BitmapMouseMove(Point mousePosition)
        {
            if (NewResourceStart != null)
            {
                var p1 = new Point(Math.Min(mousePosition.X, NewResourceStart.Value.X),
                                   Math.Min(mousePosition.Y, NewResourceStart.Value.Y));
                var p2 = new Point(Math.Max(mousePosition.X, NewResourceStart.Value.X),
                                   Math.Max(mousePosition.Y, NewResourceStart.Value.Y));
                NewResourceTemporary = new Rect(p1, p2 - p1);

                Mouse.OverrideCursor = (mousePosition.X < NewResourceStart.Value.X) == (mousePosition.Y < NewResourceStart.Value.Y) ? Cursors.SizeNWSE : Cursors.SizeNESW;
                return;
            }

            if (IsResourceUnderMousePointer(mousePosition))
            {
                Mouse.OverrideCursor = Cursors.Hand;
                return;
                
            }
            Mouse.OverrideCursor = Cursors.Cross;
        }

        public void BitmapMouseLeave(Point mousePosition)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private bool IsResourceUnderMousePointer(Point mousePosition)
        {
            return ResourceFile.Resources.Any(resource => resource.Crop.Contains(mousePosition));
        }

        private Resource GetResourceUnderMousePointer(Point mousePosition)
        {
            return ResourceFile.Resources.FirstOrDefault(resource => resource.Crop.Contains(mousePosition));
        }

        private void CreateNewResource(Rect rect)
        {
            if (rect.Height < 30 || rect.Width < 30)
            {
                MessageBox.Show("Too small.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (ResourceFile.Resources.Select(resource => resource.Crop).Any(
                roth => rect.Left < roth.Right && rect.Right > roth.Left && rect.Top < roth.Bottom && rect.Bottom > roth.Top))
            {
                MessageBox.Show("Resource shouldn't intersect any other.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var res = new Resource
                          {
                              Name = "Unnamed",
                              Layer = ResourceFile.Layers.Count > 0 ? ResourceFile.Layers[0] : null,
                              Crop = rect,
                              TransparentColor = ResourceFile.TransparentColor,
                              ForbiddenAreas = new ObservableCollection<Rect>()
                          };
            ResourceFile.Resources.Add(res);
        }

        private bool _isNoBitmapLoaded;
        public bool IsNoBitmapLoaded
        {
            get { return _isNoBitmapLoaded; }
            set { _isNoBitmapLoaded = value; NotifyOfPropertyChange(() => IsNoBitmapLoaded); }
        }

        private Point? _newResourceStart;
        public Point? NewResourceStart
        {
            get { return _newResourceStart; }
            set { _newResourceStart = value; NotifyOfPropertyChange(() => NewResourceStart); }
        }

        private Rect? _newResourceTemporary;
        public Rect? NewResourceTemporary
        {
            get { return _newResourceTemporary; }
            set { _newResourceTemporary = value; NotifyOfPropertyChange(() => NewResourceTemporary); }
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

        private BitmapImage _resourcesBitmap;
        public BitmapImage ResourcesBitmap
        {
            get { return _resourcesBitmap; }
            set { _resourcesBitmap = value; NotifyOfPropertyChange(() => ResourcesBitmap); }
        }

        public void Handle(ColorEvent message)
        {
            Color = message.Color;
        }
    }
}
