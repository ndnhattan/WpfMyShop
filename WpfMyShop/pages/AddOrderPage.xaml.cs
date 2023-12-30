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
using WpfMyShop.model;
using WpfMyShop.models;
using static System.Reflection.Metadata.BlobBuilder;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static WpfMyShop.pages.ChooseDiscountPage;

namespace WpfMyShop.pages
{
    /// <summary>
    /// Interaction logic for AddOrderPage.xaml
    /// </summary>
    public partial class AddOrderPage : Page
    {
        BindingList<Order> _orders;
        BindingList<OrderBook> ListOrderBook = new BindingList<OrderBook>() { };
        public static bool isAddFail = false;
        public event EventHandler ScreenClosed;
        Order order = new Order();
        Discount discount;
        public AddOrderPage(BindingList<Order> _orders)
        {
            InitializeComponent();
            this._orders = _orders;
            isAddFail = false;
        }

        private void confirmBtn_Click(object sender, RoutedEventArgs e)
        {
            int price = 0;
            int cost = 0;
            int customer_id = 0;
            try
            {
                customer_id = int.Parse(textboxIDCustomer.Text);
            } catch (Exception ex)
            {
                MessageBox.Show("Invalid ID");
                return;
            }

            string dateString = textboxDate.Text; 
            DateTime date;
            if (!(DateTime.TryParseExact(dateString, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)))
            {
                MessageBox.Show("Malformed string yyyy-MM-dd");
                return;
            }

            for (int i=0;i<ListOrderBook.Count;i++)
            {
                price += ListOrderBook[i].Price;
                cost += ListOrderBook[i].Cost;
            }
            if (discount != null)
            {
                if (discount.Currency.Equals("%"))
                {
                    price -= (int)(price * discount.Value * 0.01);
                }
                else
                {
                    price -= discount.Value;
                }
            }

            order.CustomerId = customer_id;
            order.ListOrderBook = ListOrderBook;
            order.Price = price;
            order.Cost = cost;
            order.Date = date;

            string sql = """
                insert into Orders(date,customer_id, cost, price, id_discount) values (@Date,@CustomerID, @Cost, @Price, @DiscountID)
                select ident_current('Orders');
             """;
            var command = new SqlCommand(sql, DB.Instance.Connection);
            command.Parameters.Add("@CustomerID", System.Data.SqlDbType.Int).Value = order.CustomerId;
            command.Parameters.Add("@Cost", System.Data.SqlDbType.Int).Value = order.Cost;
            command.Parameters.Add("@Price", System.Data.SqlDbType.Int).Value = order.Price;
            command.Parameters.Add("@Date", System.Data.SqlDbType.DateTime).Value = order.Date;
            int discountID = 0;
            if (discount != null)
            {
                discountID = order.Discount.Id;
            }
            command.Parameters.Add("@DiscountID", System.Data.SqlDbType.Int).Value = discountID;
            int id = 0;
            try
            {
                id = (int)((decimal)command.ExecuteScalar());
            } catch(Exception ex)
            {
                MessageBox.Show("Data dates are too far away now");
                return;
            }

            if (id > 0)
            {
                order.Id = id;
                _orders.Add(order);

                for (int i = 0; i < ListOrderBook.Count; i++)
                {
                    string sqlOrderBook = "insert into Order_Books(order_id, book_id, quantity) values (@OrderId, @BookId, @Quantity)";
                    var commandOrderBook = new SqlCommand(sqlOrderBook, DB.Instance.Connection);
                    commandOrderBook.Parameters.Add("@OrderId", System.Data.SqlDbType.Int).Value = order.Id;
                    commandOrderBook.Parameters.Add("@BookId", System.Data.SqlDbType.Int).Value = ListOrderBook[i].Id_Book;
                    commandOrderBook.Parameters.Add("@Quantity", System.Data.SqlDbType.Int).Value = ListOrderBook[i].Quantity;
                    commandOrderBook.ExecuteScalar();
                }
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

        private void ordersList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void deleteBookBtn_Click(object sender, RoutedEventArgs e)
        {
            var OrderBook = OrderBookList.SelectedItem;
            ListOrderBook.Remove((OrderBook)OrderBook);
        }

        private void ChooseDiscountPage_PageClosed(object sender, DiscountEventArgs e)
        {
            // Nhận thông tin về Discount ở đây
            discount = e.Discount;
            order.Discount = discount;
            textboxDiscount.Text = discount.Name;
        }

        private void btnChooseDiscount_Click(object sender, RoutedEventArgs e)
        {
            int total = 0;
            for (int i=0;i< ListOrderBook.Count;i++)
            {
                total = total + ListOrderBook[i].Price * ListOrderBook[i].Quantity;
            }

            ChooseDiscountPage page = new ChooseDiscountPage(total);
            ChooseDiscountPage.PageClosed += ChooseDiscountPage_PageClosed;
            NavigationService.Navigate(page);
        }
    }
}
