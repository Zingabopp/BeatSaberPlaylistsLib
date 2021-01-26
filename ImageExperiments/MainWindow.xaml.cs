using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace ImageExperiments
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private static readonly string ImagePath = Path.Combine("Images", "testCover.jpg");
        private static readonly string AlternateImagePath = Path.Combine("Images", "BeatSaverMapper.png");
        private static readonly Converters.ImageToBitmapSourceConverter imageConverter = new Converters.ImageToBitmapSourceConverter();
        private System.Drawing.Image _image;

        public System.Drawing.Image TestImage
        {
            get { return _image; }
            set
            {
                _image = value;
                imgDynamic.Source = (BitmapSource)imageConverter.Convert(value, typeof(BitmapSource), null, System.Globalization.CultureInfo.CurrentCulture);
                NotifyPropertyChanged();
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            TestImage = (System.Drawing.Image)Bitmap.FromFile(ImagePath);

        }







        public event PropertyChangedEventHandler PropertyChanged;
        public void NotifyPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DrawSettings drawSettings = drawSettingsView.DrawSettingsViewModel.GetDrawSettings();
            var image = (System.Drawing.Image)Bitmap.FromFile(ImagePath);
            using (MemoryStream ms = new MemoryStream())
            {
                ImageUtilities.DrawString(txtInput.Text, image, drawSettings);
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                ms.Seek(0, SeekOrigin.Begin);
                TestImage = System.Drawing.Image.FromStream(ms);
            }
        }
    }
}
