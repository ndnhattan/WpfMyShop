using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
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
using WpfMyShop.models;

namespace WpfMyShop.pages
{
    /// <summary>
    /// Interaction logic for AddBookToOrderPage.xaml
    /// </summary>
    public partial class AddBookToOrderPage : Page
    {
        public event EventHandler<Tuple<int, int>> PageFinished;
        bool isSuccess = true;
        public AddBookToOrderPage()
        {
            InitializeComponent();
            isSuccess = true;
        }

        private void confirmBtn_Click(object sender, RoutedEventArgs e)
        {
            int quantity = 0;
            int id = 0;
            try
            {
                quantity = int.Parse(TextboxQuantity.Text);
                id = int.Parse(TextboxID.Text);
            } catch(Exception ex)
            {
                MessageBox.Show("Dữ liệu không hợp lệ");
                isSuccess = false;
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }

            if (isSuccess){
                PageFinished?.Invoke(this, Tuple.Create(id, quantity));
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}
