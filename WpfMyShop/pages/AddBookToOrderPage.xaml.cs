using Microsoft.Data.SqlClient;
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
using WpfMyShop.model;
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

                string sql = """
                select stock from Books where id = @id
             """;
                var commnd = new SqlCommand(sql, DB.Instance.Connection);
                commnd.Parameters.Add("@id", System.Data.SqlDbType.NVarChar).Value = id;
                int stock = 0;
                try
                {
                    object result = commnd.ExecuteScalar();
                    if (result != null)
                    {
                        stock = (int)((Int32)result);
                        if (stock < quantity)
                        {
                            MessageBox.Show("Do not have enough books");
                            return;
                        }
                    }
                }
                catch (Exception ex) { }

            } catch(Exception ex)
            {
                MessageBox.Show("Invalid data");
                isSuccess = false;
                PageFinished?.Invoke(this, Tuple.Create(id, quantity));
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
