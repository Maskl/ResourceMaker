using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Caliburn.Micro;
using System.Windows.Media;
using Sklodowski.ResourceMaker.Extensions;
using Sklodowski.ResourceMaker.Features.ResourceFileWindow;

namespace Sklodowski.ResourceMaker.Features.ResourceWindow
{
    [Export(typeof(ResourceViewModel))]
    public class ResourceViewModel : PropertyChangedBase, IViewAware
    {
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
        public ResourceViewModel(Resource resource, ResourceFile resourceFile, string currentDirectory)
        {
            Resource = resource;
            ResourceFile = resourceFile;
            CurrentDirectory = currentDirectory;

            ResourceBitmap = CreateCroppedBitmapResolutionInvariant(new Uri(CurrentDirectory + ResourceFile.BitmapFileName), resource.Crop);
        }

        public void DeleteResource()
        {
            if (MessageBox.Show("Are you sure? (Action can not be undone)", "Delete a resource?", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                ResourceFile.Resources.Remove(Resource);
                dialogWindow.Close();
            }
        }

        public void CloseWindow()
        {
            dialogWindow.Close();
        }

        public void BitmapMouseDown(Point mousePosition)
        {
            if (NewForbiddenAreaTemporary == null && !IsForbiddenAreaUnderMousePointer(mousePosition))
                NewForbiddenAreaStart = mousePosition;
        }

        public void BitmapMouseUp(Point mousePosition)
        {
            if (NewForbiddenAreaTemporary.HasValue)
            {
                CreateNewForbiddenArea(NewForbiddenAreaTemporary.Value);
                NewForbiddenAreaStart = null;
                NewForbiddenAreaTemporary = null;
                return;
            }

            var forbiddenArea = GetForbiddenAreaUnderMousePointer(mousePosition);
            Resource.ForbiddenAreas.Remove(forbiddenArea);
        }

        public void BitmapMouseMove(Point mousePosition)
        {
            if (NewForbiddenAreaStart != null)
            {
                var p1 = new Point(Math.Min(mousePosition.X, NewForbiddenAreaStart.Value.X),
                                   Math.Min(mousePosition.Y, NewForbiddenAreaStart.Value.Y));
                var p2 = new Point(Math.Max(mousePosition.X, NewForbiddenAreaStart.Value.X),
                                   Math.Max(mousePosition.Y, NewForbiddenAreaStart.Value.Y));

                NewForbiddenAreaTemporary = new Int32Rect((int)p1.X, (int)p1.Y, (int)(p2 - p1).X, (int)(p2 - p1).Y);


                Mouse.OverrideCursor = (mousePosition.X < NewForbiddenAreaStart.Value.X) == (mousePosition.Y < NewForbiddenAreaStart.Value.Y) ? Cursors.SizeNWSE : Cursors.SizeNESW;
                return;
            }

            if (IsForbiddenAreaUnderMousePointer(mousePosition))
            {
                Mouse.OverrideCursor = Cursors.No;
                return;

            }
            Mouse.OverrideCursor = Cursors.Cross;
        }

        public void BitmapMouseLeave(Point mousePosition)
        {
            Mouse.OverrideCursor = Cursors.Arrow;
        }

        private bool IsForbiddenAreaUnderMousePointer(Point mousePosition)
        {
            return Resource.ForbiddenAreas.Any(forbiddenArea => forbiddenArea.Contains(mousePosition));
        }

        private Int32Rect GetForbiddenAreaUnderMousePointer(Point mousePosition)
        {
            return Resource.ForbiddenAreas.FirstOrDefault(forbiddenArea => forbiddenArea.Contains(mousePosition));
        }

        private void CreateNewForbiddenArea(Int32Rect rect)
        {
            if (rect.Height < 5 || rect.Width < 5)
            {
                MessageBox.Show("Too small.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Resource.ForbiddenAreas.Select(forbiddenArea => forbiddenArea).Any(
                roth => rect.X < roth.X + roth.Width && rect.X + rect.Width > roth.X && rect.Y < roth.Y + roth.Height && rect.Y + rect.Height > roth.Y))
            {
                MessageBox.Show("Forbidden area shouldn't intersect any other.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Resource.ForbiddenAreas.Add(rect);
        }

        private bool _isNoBitmapLoaded;
        public bool IsNoBitmapLoaded
        {
            get { return _isNoBitmapLoaded; }
            set { _isNoBitmapLoaded = value; NotifyOfPropertyChange(() => IsNoBitmapLoaded); }
        }

        private Point? _newForbiddenAreaStart;
        public Point? NewForbiddenAreaStart
        {
            get { return _newForbiddenAreaStart; }
            set { _newForbiddenAreaStart = value; NotifyOfPropertyChange(() => NewForbiddenAreaStart); }
        }

        private Int32Rect? _newForbiddenAreaTemporary;
        public Int32Rect? NewForbiddenAreaTemporary
        {
            get { return _newForbiddenAreaTemporary; }
            set { _newForbiddenAreaTemporary = value; NotifyOfPropertyChange(() => NewForbiddenAreaTemporary); }
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

        private Window dialogWindow;
        public void AttachView(object view, object context = null)
        {
            dialogWindow = view as Window;
            if (ViewAttached != null)
            {
                ViewAttached(this,
                             new ViewAttachedEventArgs() {Context = context, View = view});
            }
        }

        public object GetView(object context = null)
        {
            return dialogWindow;
        }

        public event EventHandler<ViewAttachedEventArgs> ViewAttached;
    }
}
