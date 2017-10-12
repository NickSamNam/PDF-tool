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
using Microsoft.Win32;
using Path = System.IO.Path;

namespace PDF_tool {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private PdfHandler _pdfHandler = new PdfHandler();

        public MainWindow() {
            InitializeComponent();
            Loaded += WindowLoaded;
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

        private async void Button_Click(object sender, RoutedEventArgs e) {
            var od = new OpenFileDialog() {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                AddExtension = true,
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "pdf",
                Multiselect = true,
                Filter = "PDF Documents (*.pdf)|*.pdf|All files (*.*)|*.*",
                ShowReadOnly = true,
                Title = "Open PDF Document(s)"
            };
            if (od.ShowDialog(this) ?? false) {
                await _pdfHandler.LoadAsync(od.FileNames);
            }
            var items = new List<ListBoxItem>();
            _pdfHandler.Input.ForEach(it => {
                items.Add(new ListBoxItem {Title = Path.GetFileName(it.FullPath), Path = it.FullPath});
            });
            LbPdfs.ItemsSource = items;
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e) {
            _pdfHandler.Input.ForEach(it => {
                var pages = Enumerable.Range(0, it.PageCount - 1);
                _pdfHandler.AddPagesToDoc(it, pages.ToArray());
            });
            var sd = new SaveFileDialog {
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                AddExtension = true,
                CheckPathExists = true,
                DefaultExt = "pdf",
                Filter = "PDF Documents (*.pdf)|*.pdf|All files (*.*)|*.*",
                Title = "Save PDF Document"
            };
            if (sd.ShowDialog(Owner) ?? false) {
                if (!await _pdfHandler.SaveAsync(sd.FileName)) {
                    MessageBox.Show("Document could not be saved.");
                }
            }
        }
    }
}