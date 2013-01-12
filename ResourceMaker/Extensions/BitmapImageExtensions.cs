using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Sklodowski.ResourceMaker.Extensions
{
    static class BitmapImageExtensions
    {
        public static CroppedBitmap CreateCroppedBitmapResolutionInvariant(this BitmapImage bitmap, Int32Rect crop)
        {
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
    }
}
