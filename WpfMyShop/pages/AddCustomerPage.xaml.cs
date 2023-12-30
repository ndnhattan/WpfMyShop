using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using static WpfMyShop.pages.ChooseDiscountPage;
using WpfMyShop.model;
using WpfMyShop.models;
using System.Text.RegularExpressions;
using System.Windows.Controls.Primitives;

namespace WpfMyShop.pages
{
    /// <summary>
    /// Interaction logic for AddCustomerPage.xaml
    /// </summary>
    public partial class AddCustomerPage : Page
    {
        BindingList<Customer> customers;
        public event EventHandler ScreenClosed;
        public static bool isAddFail = false;

        public AddCustomerPage(BindingList<Customer> customers)
        {
            InitializeComponent();
            this.customers = customers;
            isAddFail = false;
        }
    
        private void DatePicker_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            DatePicker dp = (DatePicker)sender;
            Regex regex = new Regex("[^0-9/]"); //regex that matches allowed text
            e.Handled = regex.IsMatch(e.Text);
        }


        private void confirmBtn_Click(object sender, RoutedEventArgs e)
        {
            if (TextboxName.Text.Length <= 0 || TextboxGender.Text.Length <= 0 ||
                TextboxPhoneNumber.Text.Length <= 0 || TextboxEmail.Text.Length <= 0 || TextboxAddress.Text.Length <= 0)
            {
                MessageBox.Show("Please fill the entire form");
                isAddFail = true;
            }

            if (!isAddFail)
            {
                int id = 0;
                var customer = new Customer()
                {
                    FullName = TextboxName.Text,
                    Gender = TextboxGender.Text,
                    PhoneNumber = TextboxPhoneNumber.Text,
                    Address = TextboxAddress.Text,
                    Email = TextboxEmail.Text,
                    DOB = DOBDate.SelectedDate
                };

                string sql = """
                insert into Customers(fullname, gender, phone_number, email, address, birthday) 
                values(@FullName, @Gender, @PhoneNumber, @Email, @Address, @DOB);
                select ident_current('Customers');
             """;
                try
                {
                    var command = new SqlCommand(sql, DB.Instance.Connection);
                    command.Parameters.Add("@FullName", System.Data.SqlDbType.NVarChar).Value = customer.FullName;
                    command.Parameters.Add("@Gender", System.Data.SqlDbType.NVarChar).Value = customer.Gender;
                    command.Parameters.Add("@PhoneNumber", System.Data.SqlDbType.NVarChar).Value = customer.PhoneNumber;
                    command.Parameters.Add("@Email", System.Data.SqlDbType.NVarChar).Value = customer.Email;
                    command.Parameters.Add("@Address", System.Data.SqlDbType.NVarChar).Value = customer.Address;
                    command.Parameters.Add("@DOB", System.Data.SqlDbType.DateTime).Value = customer.DOB;

                    id = (int)((decimal)command.ExecuteScalar());
                }
                catch (Exception ex) { }

                if (id > 0)
                {
                    customer.Id = id;
                    customers.Add(customer);
                    MessageBox.Show("Add successfully");
                }
                else
                {
                    MessageBox.Show("Add failed");
                }
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
                ScreenClosed?.Invoke(this, EventArgs.Empty);
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
