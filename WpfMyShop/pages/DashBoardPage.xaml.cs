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

namespace WpfMyShop.pages
{
    /// <summary>
    /// Interaction logic for DashBoardPage.xaml
    /// </summary>
    public partial class DashBoardPage : Page
    {
        public DashBoardPage()
        {
            InitializeComponent();
            totalProduct();
            loadTopStockBook();
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
                select top(5)name,stock,image_url from Books
                order by stock
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
                    int stock = (int)reader["stock"];
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
                        Stock = stock,
                        //GenreId = genre_id
                    };
                    //count = (int)reader["Total"];

                    _books.Add(book);
                }

                reader.Close();
            }
            listViewTopBookStock.ItemsSource = _books;
        }
    }
}
