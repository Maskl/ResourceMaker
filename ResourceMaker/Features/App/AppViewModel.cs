using System;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using System.ComponentModel.Composition;
using System.Windows.Media;
using Sklodowski.ResourceMaker.Extensions;
using Sklodowski.ResourceMaker.Features.ResourceWindow;

namespace Sklodowski.ResourceMaker.Features.App
{
    public class Filters
    {
        public string Name { get; set; }
        public string Layer { get; set; }
    }

    [Export(typeof(AppViewModel))]
    public class AppViewModel : PropertyChangedBase
    {
        private readonly IWindowManager _windowManager;

        [ImportingConstructor]
        public AppViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
            New();
        }

        public void OpenResourceWindow(Resource resource)
        {
            dynamic settings = new ExpandoObject();
            settings.WindowStartupLocation = WindowStartupLocation.Manual;
            settings.WindowStyle = WindowStyle.ToolWindow;
            settings.ShowInTaskbar = false;
            _windowManager.ShowDialog(new ResourceViewModel(resource, ResourceFile, CurrentDirectory), null, settings);
            UpdateFilters();
        }

        public void LoadBitmap()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Image files (*.jpg, *.png, *.bmp, *.tga, *.gif) | *.jpg; *.png; *.bmp; *.tga; *.gif";
            var res = dialog.ShowDialog();
            if (!res.Value)
                return;

            PrepareNewResourceFile(dialog.FileName);
        }

        public void PrepareNewResourceFile(string bitmapFileName)
        {
            var uri = new Uri(bitmapFileName);
            ResourcesBitmap = new BitmapImage(uri);
            IsNoBitmapLoaded = false;
            CurrentDirectory = Path.GetDirectoryName(uri.LocalPath) + Path.AltDirectorySeparatorChar;

            ResourceFile = new ResourceFile
                             {
                                 BitmapFileName = Path.GetFileName(uri.LocalPath),
                                 TransparentColor = Colors.Magenta,
                                 HasTransparentColor = true,
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
            ResourcesToRender = new ObservableCollection<Resource>();
        }

        public void UpdateFilters()
        {
            ResourcesToRender.Clear();
            foreach (var resource in ResourceFile.Resources)
            {
                if (resource.Name != null && Filters.Name != null)
                {
                    var r = resource.Name.Trim().ToLower();
                    var f = Filters.Name.Trim().ToLower();
                    if (!r.Contains(f) && !f.Contains(r))
                        continue;
                }

                if (resource.Layer != null && Filters.Layer != null)
                {
                    var r = resource.Layer.Trim().ToLower();
                    var f = Filters.Layer.Trim().ToLower();
                    if (!r.Contains(f) && !f.Contains(r))
                        continue;
                }

                ResourcesToRender.Add(resource);
            }
        }

        public void Open()
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            dialog.Filter = "Resource files (*.resource) | *.resource";
            var res = dialog.ShowDialog();
            if (!res.Value)
                return;

            // Reset current resource and bitmap.
            New();

            try
            {
                // Load resource data.
                using (var stream = new FileStream(dialog.FileName, FileMode.Open))
                {
                    using (var reader = new BinaryReader(stream))
                    {
                        ResourceFile = ResourceFile.Load(reader);
                        reader.Close();
                    }
                }

                // Load bitmap.
                var uri = new Uri(dialog.FileName);
                CurrentDirectory = Path.GetDirectoryName(uri.LocalPath) + Path.AltDirectorySeparatorChar;
                ResourcesBitmap = new BitmapImage(new Uri(CurrentDirectory + ResourceFile.BitmapFileName));
                IsNoBitmapLoaded = false;
                UpdateFilters();
            }
            catch (Exception)
            {
                MessageBox.Show("Problem while opening resource file.");
            }
        }

        public void Save()
        {
            if (IsNoBitmapLoaded)
            {
                MessageBox.Show("No bitmap loaded", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var dialog = new Microsoft.Win32.SaveFileDialog();
            dialog.Filter = "Resource files (*.resource) | *.resource";
            var res = dialog.ShowDialog();
            if (!res.Value)
                return;

            try
            {
                using (var stream = new FileStream(dialog.FileName, FileMode.Create))
                {
                    using (var writer = new BinaryWriter(stream))
                    {
                        ResourceFile.Save(writer);
                        writer.Close();
                    }
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Problem while saving resource file.");
            }
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
            {
                OpenResourceWindow(resource);
            }
        }

        public void BitmapMouseMove(Point mousePosition)
        {
            if (NewResourceStart != null)
            {
                var p1 = new Point(Math.Min(mousePosition.X, NewResourceStart.Value.X),
                                   Math.Min(mousePosition.Y, NewResourceStart.Value.Y));
                var p2 = new Point(Math.Max(mousePosition.X, NewResourceStart.Value.X),
                                   Math.Max(mousePosition.Y, NewResourceStart.Value.Y));
            
                NewResourceTemporary = new Int32Rect((int) p1.X, (int) p1.Y, (int) (p2 - p1).X, (int) (p2 - p1).Y);


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
            return ResourceFile != null && ResourceFile.Resources.Any(resource => resource.Crop.Contains(mousePosition));
        }

        private Resource GetResourceUnderMousePointer(Point mousePosition)
        {
            return ResourceFile == null ? null : ResourceFile.Resources.FirstOrDefault(resource => resource.Crop.Contains(mousePosition));
        }

        private void CreateNewResource(Int32Rect rect)
        {
            if (rect.Height < 30 || rect.Width < 30)
            {
                MessageBox.Show("Too small.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (ResourceFile.Resources.Select(resource => resource.Crop).Any(
                roth => rect.X < roth.X + roth.Width && rect.X + rect.Width > roth.X && rect.Y < roth.Y + roth.Height && rect.Y + rect.Height > roth.Y))
            {
                MessageBox.Show("Resource shouldn't intersect any other.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var res = new Resource
                          {
                              Name = "Unnamed",
                              Layer = "Default",
                              Crop = rect,
                              TransparentColor = ResourceFile.TransparentColor,
                              HasTransparentColor = ResourceFile.HasTransparentColor,
                              ForbiddenAreas = new ObservableCollection<Int32Rect>()
                          };
            ResourceFile.Resources.Add(res);
            UpdateFilters();
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

        private Int32Rect? _newResourceTemporary;
        public Int32Rect? NewResourceTemporary
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
            set { _filters = value; NotifyOfPropertyChange(() => Filters);}
        }

        private BitmapImage _resourcesBitmap;
        public BitmapImage ResourcesBitmap
        {
            get { return _resourcesBitmap; }
            set { _resourcesBitmap = value; NotifyOfPropertyChange(() => ResourcesBitmap); }
        }

        private string _currentDirectory;
        public string CurrentDirectory
        {
            get { return _currentDirectory; }
            set { _currentDirectory = value; NotifyOfPropertyChange(() => CurrentDirectory); }
        }

        private ObservableCollection<Resource> _resourcesToRender;
        public ObservableCollection<Resource> ResourcesToRender
        {
            get { return _resourcesToRender; }
            set { _resourcesToRender = value; NotifyOfPropertyChange(() => ResourcesToRender); }
        }
    }
}
