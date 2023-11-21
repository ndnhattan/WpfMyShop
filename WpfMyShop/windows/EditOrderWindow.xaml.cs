using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfMyShop.model;
using WpfMyShop.models;
using WpfMyShop.pages;

namespace WpfMyShop.windows
{
    /// <summary>
    /// Interaction logic for EditOrderWindow.xaml
    /// </summary>
    public partial class EditOrderWindow : Window
    {
        public Order EditedOrder { get; set; }
        BindingList<Order> orders;
        BindingList<OrderBook> ListOrderBook;
        public EditOrderWindow(Order order, BindingList<Order> orders)
        {
            InitializeComponent();
            this.orders = orders;
            this.EditedOrder = order;
            this.ListOrderBook = EditedOrder.ListOrderBook;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = EditedOrder;
            EditOrderPage.PageClosed += HandlePageClosed;
            MainScreenEditOrder.Content = new EditOrderPage(EditedOrder, orders, DialogResult);
        }

        private void HandlePageClosed(object? sender, EventArgs e)
        {
            DialogResult = true;
            EditOrderPage.PageClosed -= HandlePageClosed;
        }
    }
}
