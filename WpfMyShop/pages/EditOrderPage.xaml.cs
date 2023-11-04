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
using static System.Reflection.Metadata.BlobBuilder;

namespace WpfMyShop.pages
{
    /// <summary>
    /// Interaction logic for EditOrderPage.xaml
    /// </summary>
    public partial class EditOrderPage : Page
    {
        public static event EventHandler PageClosed;
        public Order EditedOrder { get; set; }
        bool? DialogResult {  get; set; }
        BindingList<Order> orders;
        BindingList<OrderBook> ListOrderBook;
        List<OrderBook> ListOrderBook2;
        public EditOrderPage(Order order, BindingList<Order> orders, bool? dialogResult)
        {
            InitializeComponent();
            this.orders = orders;
            this.EditedOrder = order;
            this.ListOrderBook = EditedOrder.ListOrderBook;
            ListOrderBook2 = EditedOrder.ListOrderBook.ToList();
            DialogResult = dialogResult;
        }

        private void ordersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataContext = EditedOrder;
            textboxIDCustomer.Focus();
        }

        private void confirmBtn_Click(object sender, RoutedEventArgs e)
        {
            var sql = """
                update Orders set date = @Date, customer_id = @CustomerID
                where id = @id
                select @id;
             """;
            var command = new SqlCommand(sql, DB.Instance.Connection);
            command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = EditedOrder.Id;
            command.Parameters.Add("@Date", System.Data.SqlDbType.DateTime).Value = EditedOrder.Date;
            command.Parameters.Add("@CustomerID", System.Data.SqlDbType.Int).Value = EditedOrder.CustomerId;

            int id = 0;
            try
            {
                id = (int)((Int32)command.ExecuteScalar());
                if (id > 0)
                {
                    // find index of book in array
                    int index = orders.IndexOf(orders.FirstOrDefault(order => order.Id == id));
                    orders[index] = EditedOrder;

                    var sql1 = """
                            delete from Order_Books where order_id = @OrderID
                        """;
                    var command1 = new SqlCommand(sql1, DB.Instance.Connection);
                    command1.Parameters.Add("@OrderID", System.Data.SqlDbType.Int).Value = EditedOrder.Id;
                    command1.ExecuteScalar();
                    for (int i=0;i<EditedOrder.ListOrderBook.Count;i++)
                    {
                        var sql2 = """
                            insert into Order_Books(order_id, book_id, quantity) values (@OrderID, @BookID, @Quantity)
                        """;
                        var command2 = new SqlCommand(sql2, DB.Instance.Connection);
                        command2.Parameters.Add("@BookID", System.Data.SqlDbType.Int).Value = EditedOrder.ListOrderBook[i].Id_Book;
                        command2.Parameters.Add("@OrderID", System.Data.SqlDbType.Int).Value = EditedOrder.Id;
                        command2.Parameters.Add("@Quantity", System.Data.SqlDbType.Int).Value = EditedOrder.ListOrderBook[i].Quantity;
                        command2.ExecuteScalar();
                    }

                    DialogResult = true;
                    
                    EditOrderWindow parentWindow = (EditOrderWindow)EditOrderWindow.GetWindow(this);

                    if (parentWindow != null)
                    {
                        // Đóng Window
                        PageClosed?.Invoke(this, EventArgs.Empty);
                        parentWindow.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Sửa thất bại");
                DialogResult = false;
            }
        }

        private void addBookBtn_Click(object sender, RoutedEventArgs e)
        {
            AddBookToOrderPage page = new AddBookToOrderPage();
            NavigationService.Navigate(page);

            page.PageFinished += (sender, args) =>
            {
                int id = args.Item1;
                int quantity = args.Item2;

                string sql = $@"SELECT * FROM Books WHERE id = @ID";
                var command = new SqlCommand(sql, DB.Instance.Connection);
                command.Parameters.Add("@ID", System.Data.SqlDbType.Int).Value = id;
                string name = "";
                int price = 0, cost = 0;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        name = (string)reader["name"];
                        price = (int)reader["price"];
                        cost = (int)reader["cost"];
                    }
                }

                OrderBook orderBook = new OrderBook()
                {
                    Id_Book = id,
                    Quantity = quantity,
                    Name = name,
                    Price = price * quantity,
                    Cost = cost * quantity
                };
                ListOrderBook.Add(orderBook);
                OrderBookList.ItemsSource = ListOrderBook;
            };
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            ListOrderBook = new BindingList<OrderBook>(ListOrderBook2);
            EditOrderWindow parentWindow = (EditOrderWindow)EditOrderWindow.GetWindow(this);

            if (parentWindow != null)
            {
                // Đóng Window
                parentWindow.Close();
            }
        }

        private void deleteBookBtn_Click(object sender, RoutedEventArgs e)
        {
            var OrderBook = OrderBookList.SelectedItem;
            ListOrderBook.Remove((OrderBook)OrderBook);
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = EditedOrder;
            textboxIDCustomer.Focus();
        }
    }
}
