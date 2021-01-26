using ImageExperiments.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;

namespace ImageExperiments.Views
{
    /// <summary>
    /// Interaction logic for DrawSettingsView.xaml
    /// </summary>
    public partial class DrawSettingsView : UserControl
    {
        public DrawSettingsViewModel DrawSettingsViewModel { get; }
        public DrawSettingsView()
        {
            DrawSettingsViewModel = new DrawSettingsViewModel();
            InitializeComponent();
            DataContext = DrawSettingsViewModel;
        }
    }
}
