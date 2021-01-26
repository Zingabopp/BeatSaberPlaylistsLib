using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ImageExperiments.Converters
{
    [ValueConversion(typeof(Image), typeof(BitmapSource))]
    public class ImageToBitmapSourceConverter : IValueConverter
    {
        [DllImport("gdi32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool DeleteObject(IntPtr value);

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is null || !(value is Image myImage))
            {//ensure provided value is valid image.
                return null;
            }

            if (myImage.Height > Int16.MaxValue || myImage.Width > Int16.MaxValue)
            {//GetHbitmap will fail if either dimension is larger than max short value.
             //Throwing here to reduce cpu and resource usage when error can be detected early.
                throw new ArgumentException($"Cannot convert System.Drawing.Image with either dimension greater than {Int16.MaxValue} to BitmapImage.\nProvided image's dimensions: {myImage.Width}x{myImage.Height}", nameof(value));
            }

            using Bitmap bitmap = new Bitmap(myImage); //ensure Bitmap is disposed of after usefulness is fulfilled.
            IntPtr bmpPt = bitmap.GetHbitmap();
            try
            {
                BitmapSource bitmapSource =
                 System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                       bmpPt,
                       IntPtr.Zero,
                       Int32Rect.Empty,
                       BitmapSizeOptions.FromEmptyOptions());

                //freeze bitmapSource and clear memory to avoid memory leaks
                bitmapSource.Freeze();
                return bitmapSource;
            }
            finally
            { //done in a finally block to ensure this memory is not leaked regardless of exceptions.
                DeleteObject(bmpPt);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
