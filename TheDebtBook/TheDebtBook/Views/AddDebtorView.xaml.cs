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

namespace TheDebtBook.Views
{
    /// <summary>
    /// Interaction logic for DebtorView.xaml
    /// </summary>
    public partial class AddDebtorView : Window
    {
        public AddDebtorView()
        {
            InitializeComponent();
        }

        private void AddButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
