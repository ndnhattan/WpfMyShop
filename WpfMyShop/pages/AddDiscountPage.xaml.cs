using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Vml;
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
using static System.Reflection.Metadata.BlobBuilder;

namespace WpfMyShop.pages
{
    /// <summary>
    /// Interaction logic for AddDiscountPage.xaml
    /// </summary>
    public partial class AddDiscountPage : Page
    {
        BindingList<Discount> _discounts;
        public event EventHandler ScreenClosed;
        public static bool isAddFail = false;
        public AddDiscountPage(BindingList<Discount> discounts)
        {
            InitializeComponent();
            this._discounts = discounts;
            isAddFail = false;
        }

        private void confirmBtn_Click(object sender, RoutedEventArgs e)
        {
            int value = 0;
            int condition = 0;
            string currency = "";
            string name = "";
            int id = 0;
            try
            {
                value = int.Parse(TextboxValue.Text);
                condition = int.Parse(TextboxCondition.Text);
                currency = TextboxCurrency.Text;
                name = TextboxName.Text;
                if (currency!="VNĐ" && currency != "%")
                {
                    MessageBox.Show("Invalid data");
                    return;
                }

                string sql = """
                insert into Discount(value, condition, currency, name) values (@Value, @Condition, @Currency, @Name)
                select ident_current('Discount');
             """;
                var command = new SqlCommand(sql, DB.Instance.Connection);
                command.Parameters.Add("@Value", System.Data.SqlDbType.NVarChar).Value = value;
                command.Parameters.Add("@Condition", System.Data.SqlDbType.NVarChar).Value = condition;
                command.Parameters.Add("@Currency", System.Data.SqlDbType.NVarChar).Value = currency;
                command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar).Value = name;
                id = (int)((decimal)command.ExecuteScalar());
            }
            catch (Exception ex){}

            if (id > 0)
            {
                var discount = new Discount()
                {
                    Id = id,
                    Value = value,
                    Condition = condition
                };
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

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}
