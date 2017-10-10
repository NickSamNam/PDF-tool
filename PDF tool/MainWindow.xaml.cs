using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace PDF_tool {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        public MainWindow() {
            InitializeComponent();
            Loaded += WindowLoaded;
        }

        private async void WindowLoaded(object sender, RoutedEventArgs e) {
            Debug.WriteLine(await new ActivationHandler().ActivateAsync("26O87V3SW5V8QC57OCW4ASWV3"));
            Debug.WriteLine("Activated: " + new ActivationHandler().Validate());
        }
    }
}