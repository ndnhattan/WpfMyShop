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

namespace WpfMyShop.pages
{
    /// <summary>
    /// Interaction logic for DetailProductPage.xaml
    /// </summary>
    public partial class DetailProductPage : Page
    {
        int index;
        BindingList<Book> books;
        public DetailProductPage(int index, BindingList<Book> books)
        {
            InitializeComponent();
            this.index = index;
            this.books = books;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (index <= books.Count && index >= 0)
            {
                DataContext = books[index];
            }
            else
            {
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            // delete in database
            Book book = books[index];
            string sql = """
                select id from Books where id = @id;
                delete from Books where id = @id;
             """;
            var command = new SqlCommand(sql, DB.Instance.Connection);
            command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = book.Id;

            int id = (int)((Int32)command.ExecuteScalar());
            if (id > 0) // delete in db successfully
            {
                // delete data in bindingList
                Book foundBook = books.FirstOrDefault(book => book.Id == id);
                books.Remove(foundBook);
                MessageBox.Show("Xóa thành công");

                // Go back previous page
                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();
                }
            }
            else
            {
                // delete fail
                MessageBox.Show("Xóa thất bại");
            }
        }

        Book _backup;
        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            var book = books[index];
            _backup = (Book)book.Clone();
            var screen = new EditBookWindow(book, books);

            if (screen.ShowDialog()!.Value == true)
            {
                screen.EditedBook.CopyProperties(book);
            }
            else
            {
                _backup.CopyProperties(book);
            }
        }
    }
}
