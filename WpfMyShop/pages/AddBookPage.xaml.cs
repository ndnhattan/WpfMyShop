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

namespace WpfMyShop.pages
{
    /// <summary>
    /// Interaction logic for AddBookPage.xaml
    /// </summary>
    public partial class AddBookPage : Page
    {
        BindingList<Book> books;
        public event EventHandler ScreenClosed;
        public static bool isAddFail = false;

        public AddBookPage(BindingList<Book> books)
        {
            InitializeComponent();
            this.books = books;
            isAddFail = false;
        }

        private void confirmBtn_Click(object sender, RoutedEventArgs e)
        {
            string sqlGenre = """
                select id from Genres where name = @Genre
             """;
            var commandGenre = new SqlCommand(sqlGenre, DB.Instance.Connection);
            commandGenre.Parameters.Add("@Genre", System.Data.SqlDbType.NVarChar).Value = TextboxGenre.Text;
            int id = 0;
            try
            {
                object result = commandGenre.ExecuteScalar();
                if (result != null)
                {
                    id = (int)((Int32)result);
                }
            }
            catch (Exception ex) { }

            int genre_id = 0;
            if (id > 0)
            {
                genre_id = id;
            }
            else
            {
                MessageBox.Show("Genre is inavailable");
                isAddFail = true;
            }

            if (TextboxName.Text.Length <= 0 || TextboxImage.Text.Length <= 0 || TextboxAuthor.Text.Length <= 0 || TextboxYear.Text.Length <= 0 ||
                TextboxPrice.Text.Length <= 0 || TextboxPromoPrice.Text.Length <= 0 || TextboxSold.Text.Length <= 0 || TextboxCost.Text.Length <= 0 || TextboxStock.Text.Length <= 0)
            {
                MessageBox.Show("Please fill the entire form");
                isAddFail = true;
            }

            if (!isAddFail)
            {
                var book = new Book()
                {
                    Name = TextboxName.Text,
                    Image = TextboxImage.Text,
                    Author = TextboxAuthor.Text,
                    Year = int.Parse(TextboxYear.Text),
                    Price = int.Parse(TextboxPrice.Text),
                    PromoPrice = int.Parse(TextboxPromoPrice.Text),
                    Sold = int.Parse(TextboxSold.Text),
                    Cost = int.Parse(TextboxCost.Text),
                    Stock = int.Parse(TextboxStock.Text),
                    GenreId = genre_id
                };

                string sql = """
                insert into Books(name, author, year, image_url, cost, price, stock, genre_id, promo_price, sold) 
                values(@Name, @Author, @Year, @Image, @Cost, @Price, @Stock, @Genre_id, @PromoPrice, @Sold);
                select ident_current('Books');
             """;
                try
                {
                    var command = new SqlCommand(sql, DB.Instance.Connection);
                    command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar).Value = book.Name;
                    command.Parameters.Add("@Author", System.Data.SqlDbType.NVarChar).Value = book.Author;
                    command.Parameters.Add("@Year", System.Data.SqlDbType.Int).Value = book.Year;
                    command.Parameters.Add("@Image", System.Data.SqlDbType.NVarChar).Value = book.Image;
                    command.Parameters.Add("@Cost", System.Data.SqlDbType.Int).Value = book.Cost;
                    command.Parameters.Add("@Price", System.Data.SqlDbType.Int).Value = book.Price;
                    command.Parameters.Add("@Stock", System.Data.SqlDbType.Int).Value = book.Stock;
                    command.Parameters.Add("@Genre_id", System.Data.SqlDbType.Int).Value = book.GenreId;
                    command.Parameters.Add("@PromoPrice", System.Data.SqlDbType.Int).Value = book.PromoPrice;
                    command.Parameters.Add("@Sold", System.Data.SqlDbType.Int).Value = book.Sold;

                    id = (int)((decimal)command.ExecuteScalar());
                } catch (Exception ex) { }

                if (id > 0)
                {
                    book.Id = id;
                    books.Add(book);
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
