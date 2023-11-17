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
using WpfMyShop.utils;
using WpfMyShop.windows;
using static System.Reflection.Metadata.BlobBuilder;

namespace WpfMyShop.pages
{
    /// <summary>
    /// Interaction logic for DiscountPage.xaml
    /// </summary>
    public partial class DiscountPage : Page
    {
        public DiscountPage()
        {
            InitializeComponent();
        }

        public BindingList<Discount> _discounts;
        private void loadAllDiscounts()
        {
            {
                var sql = """
                select * from Discount
                """;
                var command = new SqlCommand(sql, DB.Instance.Connection);


                _discounts = new BindingList<Discount>();
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int id = (int)reader["id"];
                        int value = (int)reader["value"];
                        int condition = (int)reader["condition"];
                        string currency = (string)reader["currency"];
                        string name = (string)reader["name"];

                        var discount = new Discount()
                        {
                            Id = id,
                            Value = value,
                            Condition = condition,
                            Currency = currency,
                            Name = name
                        };

                        _discounts.Add(discount);
                    }

                    reader.Close();
                }
                discountsList.ItemsSource = _discounts;
            }
        }

        private void btn_DeleteDiscount_Click(object sender, RoutedEventArgs e)
        {
            Discount discount = (Discount)discountsList.SelectedItem;

            string sql = """
                select id from Discount where id = @id;
                delete from Discount where id = @id;
             """;
            var command = new SqlCommand(sql, DB.Instance.Connection);
            command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = discount.Id;

            int id = (int)((Int32)command.ExecuteScalar());
            if (id > 0) // delete in db successfully
            {
                _discounts.Remove((Discount)discount);
                MessageBox.Show("Xóa thành công");
            }
            else
            {
                // delete fail
                MessageBox.Show("Xóa thất bại");
            }
        }

        Discount _backup;
        private void btn_EditDiscount_Click(object sender, RoutedEventArgs e)
        {
            Discount discount = (Discount)discountsList.SelectedItem;
            if (discount != null)
            {
                _backup = (Discount)discount.Clone();
                var screen = new EditDiscountWindow(discount, _discounts);

                if (screen.ShowDialog()!.Value == true)
                {
                    screen.EditedDiscount.CopyProperties(discount);
                }
                else
                {
                    _backup.CopyProperties(discount);
                }
            }
        }

        private void discountsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            loadAllDiscounts();

            if (!Dashboard.isLoaded)
            {
                if (Dashboard.page.Equals("AddDiscountPage"))
                {
                    var page = new AddDiscountPage(_discounts);
                    NavigationService.Navigate(page);
                    Dashboard.isLoaded = true;

                    if (!AddDiscountPage.isAddFail) // add successfully
                    {
                        loadAllDiscounts();
                    }
                }
            }
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            var page = new AddDiscountPage(_discounts);
            NavigationService.Navigate(page);

            if (!AddDiscountPage.isAddFail) // add successfully
            {
                loadAllDiscounts();
            }
        }
    }
}
