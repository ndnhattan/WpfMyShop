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
using WpfMyShop.windows;

namespace WpfMyShop.pages
{
    /// <summary>
    /// Interaction logic for ChooseDiscountPage.xaml
    /// </summary>
    public partial class ChooseDiscountPage : Page
    {
        int total;
        public static event EventHandler<DiscountEventArgs> PageClosed;
        public class DiscountEventArgs : EventArgs
        {
            public Discount Discount { get; set; }

            public DiscountEventArgs(Discount discount)
            {
                Discount = discount;
            }
        }
        public ChooseDiscountPage(int total)
        {
            InitializeComponent();
            this.total = total;
        }

        public BindingList<Discount> _discounts;
        private void loadAllDiscounts()
        {
            {
                var sql = """
                select * from Discount where condition <= @Total
                """;
                var command = new SqlCommand(sql, DB.Instance.Connection);
                command.Parameters.Add("@Total", System.Data.SqlDbType.Int).Value = total;
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

        private void btn_ChooseDiscount_Click(object sender, RoutedEventArgs e)
        {
            Discount discount = (Discount)discountsList.SelectedItem;
            
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
            PageClosed?.Invoke(this, new DiscountEventArgs(discount));
        }

        private void btn_CencelDiscount_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
            {
                NavigationService.GoBack();
            }
        }
    }
}
