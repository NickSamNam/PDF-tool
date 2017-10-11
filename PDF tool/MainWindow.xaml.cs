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
            toolbar.Loaded += ToolbarLoaded;
        }

        private void ToolbarLoaded(object sender, RoutedEventArgs e) {
            var toolBar = sender as ToolBar;
            if (toolBar?.Template.FindName("OverflowGrid", toolBar) is FrameworkElement overflowGrid)
                overflowGrid.Visibility = Visibility.Collapsed;

            if (toolBar?.Template.FindName("MainPanelBorder", toolBar) is FrameworkElement mainPanelBorder)
                mainPanelBorder.Margin = new Thickness(0);
        }

        private async void WindowLoaded(object sender, RoutedEventArgs e) {
            while (!await HandleActivation()) {
                if (MessageBox.Show(this, "In order to use this product you need to activate it.",
                        "Product activation.", MessageBoxButton.OKCancel) == MessageBoxResult.Cancel)
                    Environment.Exit(0);
            }
        }

        /// <summary>
        /// Check activation and show activation screen if needed.
        /// </summary>
        /// <returns>Returns true if the product is activated.</returns>
        private async Task<bool> HandleActivation() {
            if (await ActivationHandler.ValidateAsync()) return true;
            if (new ActivationWindow().ShowDialog() ?? false)
                return await ActivationHandler.ValidateAsync();
            return false;
        }
    }
}