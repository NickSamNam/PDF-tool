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
using System.Windows.Shapes;

namespace PDF_tool {
    /// <summary>
    /// Interaction logic for ActivationWindow.xaml
    /// </summary>
    public partial class ActivationWindow : Window {
        public ActivationWindow() {
            InitializeComponent();
            tb_productKey_0.TextChanged += OnTextChange;
            tb_productKey_1.TextChanged += OnTextChange;
            tb_productKey_2.TextChanged += OnTextChange;
            tb_productKey_3.TextChanged += OnTextChange;
            tb_productKey_4.TextChanged += OnTextChange;
        }

        private void OnTextChange(object sender, TextChangedEventArgs e) {
            var tb = (TextBox) sender;
            if (tb.Text.Length == tb.MaxLength) {
                tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            else if (tb.Text.Length == 0) {
                tb.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
            }
        }

        private async void btn_ok_Click(object sender, RoutedEventArgs e) {
            if (tb_productKey_0.Text.Length != 5 || tb_productKey_1.Text.Length != 5 ||
                tb_productKey_2.Text.Length != 5 || tb_productKey_3.Text.Length != 5 ||
                tb_productKey_4.Text.Length != 5) {
                MessageBox.Show(this, "Product key should be 25 characters long.");
                return;
            }
            switch (await ActivationHandler.ActivateAsync(tb_productKey_0.Text + tb_productKey_1.Text +
                                                          tb_productKey_2.Text + tb_productKey_3.Text +
                                                          tb_productKey_4.Text)) {
                case ActivationResponse.Succesful:
                    DialogResult = true;
                    return;
                case ActivationResponse.InvalidProductKey:
                    MessageBox.Show(this, "Product key invalid.");
                    return;
                case ActivationResponse.ProductKeyTaken:
                    MessageBox.Show(this, "Product key taken.");
                    return;
                default:
                    MessageBox.Show(this, "Product could not be activated.");
                    return;
            }
        }

        private void btn_cancel_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
        }
    }
}