using System;
using System.Collections.Generic;
using System.Configuration;
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
using WpfMyShop.pages;

namespace WpfMyShop
{
    /// <summary>
    /// Interaction logic for Dashboard.xaml
    /// </summary>
    public partial class Dashboard : Window
    {
        public static string page = "";
        public static int selectedIndex = -1;
        public static bool isLoaded = false; //avoid infinite loop
        public Dashboard()
        {
            InitializeComponent();
            MainScreen.Content = new DashboardPage();
        }

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                this.DragMove();
            }
        }
        private bool IsMaximized=false;
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                if(IsMaximized)
                {
                    this.WindowState = WindowState.Normal;
                    this.Width = 1080;
                    this.Height=720;
                    IsMaximized = false;
                }
                else
                {
                    this.WindowState = WindowState.Normal;
                    IsMaximized = true;
                }
            }
        }

        
        private void ProductBtn_Click(object sender, RoutedEventArgs e)
        {
            MainScreen.Content = new ProductPage();
        }

        private void orderBtn_Click(object sender, RoutedEventArgs e)
        {
            MainScreen.Content = new OrderPage();
        }

        private void settingBtn_Click(object sender, RoutedEventArgs e)
        {
            MainScreen.Content = new SettingPage();
        }

        private void Close_Window(object sender, System.ComponentModel.CancelEventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["Location"].Value = MainScreen.Content.ToString();
            config.Save(ConfigurationSaveMode.Minimal);
            ConfigurationManager.RefreshSection("Location");
        }
        private void genreBtn_Click(object sender, RoutedEventArgs e)
        {
            MainScreen.Content = new GenrePage();
        }

        private void btnDashboard_Click(object sender, RoutedEventArgs e)
        {
            MainScreen.Content = new DashboardPage();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            String location = ConfigurationManager.AppSettings["Location"];
            try
            {
                selectedIndex = int.Parse(ConfigurationManager.AppSettings["SelectedIndex"]);
            } catch(Exception ex) { MessageBox.Show(ex.Message.ToString()); }

            if (location!=null && location.Length>0)
            {
                int lastDotIndex = location.LastIndexOf(".");
                if (lastDotIndex != -1)
                {
                    page = location.Substring(lastDotIndex + 1);
                    if (page.Equals("AddBookPage") || page.Equals("ProductPage") || page.Equals("DetailProductPage"))
                    {
                        MainScreen.Content = new ProductPage();
                    }
                    else if (page.Equals("OrderPage") || page.Equals("DetailOrderPage") || page.Equals("AddOrderPage"))
                    {
                        MainScreen.Content = new OrderPage();
                    }
                    else if (page.Equals("GenrePage"))
                    {
                        MainScreen.Content = new GenrePage();
                    }
                    else if (page.Equals("SettingPage"))
                    {
                        MainScreen.Content = new SettingPage();
                    }
                    else if (page.Equals("DashboardPage"))
                    {
                        MainScreen.Content = new DashboardPage();
                    }
                    else if (page.Equals("AddCustomerPage") || page.Equals("CustomerPage") || page.Equals("DetailCustomerPage"))
                    {
                        MainScreen.Content = new CustomerPage();
                    }
                    else if (page.Equals("ReportFinancePage"))
                    {
                        MainScreen.Content = new ReportFinancePage();
                    }
                    else if (page.Equals("ReportProductPage"))
                    {
                        MainScreen.Content = new ReportProductPage();
                    }
                    else if (page.Equals("TopSellingProductPage"))
                    {
                        MainScreen.Content = new TopSellingProductPage();
                    }
                    //else if (page.Equals("ReportPage"))
                    //{
                    //    MainScreen.Content = new ReportPage();
                    //}
                }
            }
        }

        private void report_Click(object sender, RoutedEventArgs e)
        {
            MainScreen.Content = new ReportFinancePage();
        }

        private void reportProduct_Click(object sender, RoutedEventArgs e)
        {
            MainScreen.Content = new ReportProductPage();
        }

        private void TopSellingProduct_Click(object sender, RoutedEventArgs e)
        {
            MainScreen.Content = new TopSellingProductPage();
        }

        private void discountBtn_Click(object sender, RoutedEventArgs e)
        {
            MainScreen.Content = new DiscountPage();
        }

        private void customerBtn_Click(object sender, RoutedEventArgs e)
        {
            MainScreen.Content = new CustomerPage();
        }
    }
}
