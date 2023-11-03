using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
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
using static System.Reflection.Metadata.BlobBuilder;

namespace WpfMyShop.pages
{
    /// <summary>
    /// Interaction logic for DetailOrderPage.xaml
    /// </summary>
    public partial class DetailOrderPage : Page
    {
        public BindingList<Order> _orders;
        int index;
        public DetailOrderPage(BindingList<Order> _orders, int index)
        {
            InitializeComponent();
            this._orders = _orders;
            this.index = index;
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            // delete in database
            Order order = _orders[index];
            string sql = """
                select id from Orders where id = @id;
                delete from Orders where id = @id;
                delete from Order_Books where order_id = @id
             """;
            var command = new SqlCommand(sql, DB.Instance.Connection);
            command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = order.Id;

            int id = (int)((Int32)command.ExecuteScalar());
            if (id > 0) // delete in db successfully
            {
                // delete data in bindingList
                Order foundOrder = _orders.FirstOrDefault(order => order.Id == id);
                _orders.Remove(foundOrder);
                MessageBox.Show("Xóa thành công");

                // Go back previous page
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
            else
            {
                // delete fail
                MessageBox.Show("Xóa thất bại");
            }
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ordersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            
            if (index <= _orders.Count && index >= 0)
            {
                DataContext = _orders[index];
            }
            else
            {
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
        }

        private void btnSeeCustomer_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
