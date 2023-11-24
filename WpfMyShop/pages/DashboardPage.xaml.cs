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
using Microsoft.Data.SqlClient;
using WpfMyShop.model;
using WpfMyShop.models;

namespace WpfMyShop.pages
{
    /// <summary>
    /// Interaction logic for DashboardPage.xaml
    /// </summary>
    public partial class DashboardPage : Page
    {
        public DashboardPage()
        {
            InitializeComponent();
            totalProduct();
            loadTopStockBook();
            loadAllNewOrderInWeek();
            loadAllNewOrderInMonth();
        }
        private void totalProduct()
        {
            var sql = """
                select count(*) as Total from Books
                """;
            var command = new SqlCommand(sql, DB.Instance.Connection);
            var reader = command.ExecuteReader();
            reader.Read();
            int totalProduct = (int)reader["Total"];
            textTotalProduct.Text = $"{totalProduct}";
            reader.Close();

        }
        private void topSellingProduct()
        {

        }
        BindingList<Book> _books;
        public void loadTopStockBook()
        {
            var sql = """
                select top(10)name,sold,image_url from Books
                order by sold DESC
                """;
            var command = new SqlCommand(sql, DB.Instance.Connection);
            _books = new BindingList<Book>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    //int id = (int)reader["id"];
                    string name = (string)reader["name"];
                    //string author = (string)reader["author"];
                    //int year = (int)reader["year"];
                    string image_url = (string)reader["image_url"];
                    //int price = (int)reader["price"];
                    //int promo_price = (int)reader["promo_price"];
                    //int sold = (int)reader["sold"];
                    int sold = (int)reader["sold"];
                    //int cost = (int)reader["cost"];
                    //int genre_id = (int)reader["genre_id"];

                    var book = new Book()
                    {
                        //Id = id,
                        Name = name,
                        //Author = author,
                        //Year = year,
                        Image = image_url,
                        //Price = price,
                        //PromoPrice = promo_price,
                        //Sold = sold,
                        //Cost = cost,
                        //Stock = stock,
                        Sold = sold
                        //GenreId = genre_id
                    };
                    //count = (int)reader["Total"];

                    _books.Add(book);
                }

                reader.Close();
            }
            listViewTopBookStock.ItemsSource = _books;

        }
        BindingList<Order> _orderWeek;
        public void loadAllNewOrderInWeek()
        {
            var sql = """
                select *,count(*) over() as TotalWeek from Orders
                where date >= DATEADD(WEEK, DATEDIFF(WEEK, 0, GETDATE()), 0)
                and date<DATEADD(WEEK, DATEDIFF(WEEK, 0, GETDATE()) +1, 0)
                """;
            
            var command = new SqlCommand(sql, DB.Instance.Connection);
            _orderWeek = new BindingList<Order>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    //int id = (int)reader["id"];
                    int id = (int)reader["id"];
                    int customerId = (int)reader["customer_id"];
                    DateTime date = (DateTime)reader["date"];
                    int cost = (int)reader["cost"];
                    int price = (int)reader["price"];

                    var Order = new Order()
                    {
                        Id = id,
                        CustomerId = customerId,
                        Date = date,
                        Cost = cost,
                        Price = price
                    };
                    int countWeek = (int)reader["TotalWeek"];
                    textTotalNewOrderInWeek.Text = $"{countWeek}";
                    _orderWeek.Add(Order);
                }

                reader.Close();
            }
            listViewTotalOrderInWeek.ItemsSource = _orderWeek;
        }
        BindingList<Order> _orderMonth;
        public void loadAllNewOrderInMonth()
        {
            var sql = """
                select *,count(*) over() as TotalMonth from Orders
                where date >= DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0)
                and date<DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) +1, 0)
                """;

            var command = new SqlCommand(sql, DB.Instance.Connection);
            _orderMonth = new BindingList<Order>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    //int id = (int)reader["id"];
                    int id = (int)reader["id"];
                    int customerId = (int)reader["customer_id"];
                    DateTime date = (DateTime)reader["date"];
                    int cost = (int)reader["cost"];
                    int price = (int)reader["price"];

                    var Order = new Order()
                    {
                        Id = id,
                        CustomerId = customerId,
                        Date = date,
                        Cost = cost,
                        Price = price
                    };
                    int countMonth = (int)reader["TotalMonth"];
                    textTotalNewOrderInMonth.Text = $"{countMonth}";

                    _orderMonth.Add(Order);
                }

                reader.Close();
            }
            listViewTotalOrderInMonth.ItemsSource = _orderMonth;
        }
    }
    
}
