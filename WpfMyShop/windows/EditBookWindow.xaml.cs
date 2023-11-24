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
using System.Windows.Shapes;
using WpfMyShop.model;

namespace WpfMyShop
{
    /// <summary>
    /// Interaction logic for EditBookWindow.xaml
    /// </summary>
    public partial class EditBookWindow : Window
    {
        public Book EditedBook { get; set; }
        BindingList<Book> books;
        public EditBookWindow(Book book, BindingList<Book> books)
        {
            InitializeComponent();
            this.EditedBook = book;
            this.books = books;
        }

        private void confirmBtn_Click(object sender, RoutedEventArgs e)
        {
            var sql = """
                update Books set name = @Name, author = @Author, image_url = @Image_url, year = @Year, 
                    cost = @Cost, price = @Price, stock = @Stock, genre_id = @GenreId, 
                    promo_price = @PromoPrice, sold = @Sold
                where id = @id
                select @id;
             """;
            var command = new SqlCommand(sql, DB.Instance.Connection);
            command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = EditedBook.Id;
            command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar).Value = EditedBook.Name;
            command.Parameters.Add("@Author", System.Data.SqlDbType.NVarChar).Value = EditedBook.Author;
            command.Parameters.Add("@Year", System.Data.SqlDbType.Int).Value = EditedBook.Year;
            command.Parameters.Add("@Image_url", System.Data.SqlDbType.NVarChar).Value = EditedBook.Image;
            command.Parameters.Add("@Cost", System.Data.SqlDbType.Int).Value = EditedBook.Cost;
            command.Parameters.Add("@Price", System.Data.SqlDbType.Int).Value = EditedBook.Price;
            command.Parameters.Add("@Stock", System.Data.SqlDbType.Int).Value = EditedBook.Stock;
            command.Parameters.Add("@GenreId", System.Data.SqlDbType.Int).Value = EditedBook.GenreId;
            command.Parameters.Add("@PromoPrice", System.Data.SqlDbType.Int).Value = EditedBook.PromoPrice;
            command.Parameters.Add("@Sold", System.Data.SqlDbType.Int).Value = EditedBook.Sold;

            int id = 0;
            try
            {
                id = (int)((Int32)command.ExecuteScalar());
                if (id > 0)
                {
                    // find index of book in array
                    int index = books.IndexOf(books.FirstOrDefault(book => book.Id == id));
                    books[index] = EditedBook;

                    MessageBox.Show("Edit successfully");
                    DialogResult = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Edit failed");
                DialogResult = false;
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = EditedBook;
            TextboxName.Focus();
        }
    }
}
