using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using WpfMyShop.model;
using WpfMyShop.models;
using WpfMyShop.windows;

namespace WpfMyShop.pages
{
    /// <summary>
    /// Interaction logic for DetailCustomerPage.xaml
    /// </summary>
    public partial class DetailCustomerPage : Page
    {
        public BindingList<Customer> _orders;
        int index;
        public DetailCustomerPage(BindingList<Customer> _orders, int index)
        {
            InitializeComponent();
            this._orders = _orders;
            this.index = index;

        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            // delete in database
            Customer order = _orders[index];
            string sql = """
                select id from Customers where id = @id; 
                delete from Customers where id = @id;
             """;
            var command = new SqlCommand(sql, DB.Instance.Connection);
            command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = order.Id;

            int id = (int)((Int32)command.ExecuteScalar());
            if (id > 0) // delete in db successfully
            {
                // delete data in bindingList
                Customer foundOrder = _orders.FirstOrDefault(order => order.Id == id);
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

        Customer order;
        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var sql = """
                update Customers set fullname = @FullName, 
                                    gender = @Gender,
                                    phone_number= @PhoneNumber,
                                    email = @Email,
                                    address = @Address,
                                    birthday = @DOB
                where id = @id
                select @id;
             """;
            var command = new SqlCommand(sql, DB.Instance.Connection);
            command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = order.Id;
            command.Parameters.Add("@FullName", System.Data.SqlDbType.NVarChar).Value = order.FullName;
            command.Parameters.Add("@Gender", System.Data.SqlDbType.NVarChar).Value = order.Gender;
            command.Parameters.Add("@PhoneNumber", System.Data.SqlDbType.NVarChar).Value = order.PhoneNumber;
            command.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar).Value = order.Email;
            command.Parameters.Add("@Address", System.Data.SqlDbType.NVarChar).Value = order.Address;
            command.Parameters.Add("@DOB", System.Data.SqlDbType.DateTime).Value = order.DOB;

            int id = 0;
            id = (int)((Int32)command.ExecuteScalar());
                if (id > 0)
                {
                    MessageBox.Show("Sửa thành công");

                    // Go back previous page
                    if (NavigationService.CanGoBack)
                    {
                        NavigationService.GoBack();
                    }
                }
                else
                {
                    // delete fail
                    MessageBox.Show("Sửa thất bại");
                }
            
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (index <= _orders.Count && index >= 0)
            {
                order = _orders[index];
                DataContext = order;
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
