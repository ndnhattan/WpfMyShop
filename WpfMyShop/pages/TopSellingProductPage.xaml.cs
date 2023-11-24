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
    /// Interaction logic for TopSellingProductPage.xaml
    /// </summary>
    public partial class TopSellingProductPage : Page
    {
        public TopSellingProductPage()
        {
            InitializeComponent();
        }
        private void DatePicker_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }
        bool[] check;
        BindingList<OrderBookSql> _books;
        BindingList<OrderBookSql> _newBooks;
        DateTime? _startDate = null;
        DateTime? _endDate = null;
        public void loadAllBooksTime()
        {

            var sql = $"""
                select count(*) over() as Total, name,image_url,quantity,O.cost,O.price,date
                from Books as B join Order_Books as OB join Orders as O
                on O.id = OB.order_id on OB.book_id = B.id
                {(_startDate != null && _endDate != null ? "where (date >= @Start) and (date <= @End) " : "")}

                """;
            var command = new SqlCommand(sql, DB.Instance.Connection);
            if (_startDate != null && _endDate != null)
            {
                command.Parameters.AddWithValue("@Start", _startDate);
                command.Parameters.AddWithValue("@End", _endDate);
            }
            int count = 0;
            int finance = 0;
            int profit = 0;
            _books = new BindingList<OrderBookSql>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string name = (string)reader["name"];
                    string imageUrl = (string)reader["image_url"];
                    int quantity = (int)reader["quantity"];
                    int cost = (int)reader["cost"];
                    int price = (int)reader["price"];
                    DateTime date = (DateTime)reader["date"];
                    var book = new OrderBookSql()
                    {
                        Name = name,
                        Image = imageUrl,
                        Quantity = quantity,
                        Date = date,
                        Cost = cost,
                        Price = price
                    };
                    count = count + book.Quantity;
                    finance = finance + book.Price * book.Quantity;
                    profit = profit + (book.Price - book.Cost) * book.Quantity;
                    _books.Add(book);
                }
                reader.Close();
            }
            check = new bool[_books.Count];
            for (int i = 0; i < _books.Count; i++)
            {
                for (int j = i + 1; j < _books.Count; j++)
                {
                    if (_books[i].Name == _books[j].Name)
                    {
                        _books[i].Quantity = _books[i].Quantity + _books[j].Quantity;
                        _books.RemoveAt(j);
                        j--;

                    }
                }
                check[i] = true;
            }
            listViewProduct.ItemsSource = _books;
            listViewProduct.SelectedIndex = 0;
            textQuantity.Text = _books.Count.ToString();
        }
        public void loadAllBooks(string Timename)
        {
            var sql = $"""
                select count(*) over() as Total, name,image_url,quantity,O.cost,O.price,date
                from Books as B join Order_Books as OB join Orders as O
                on O.id = OB.order_id on OB.book_id = B.id
                where date >= DATEADD({Timename}, DATEDIFF({Timename}, 0, GETDATE()), 0)
                and date<DATEADD({Timename}, DATEDIFF({Timename}, 0, GETDATE()) +1, 0)

                """;
            var command = new SqlCommand(sql, DB.Instance.Connection);
            int count = 0;
            int finance = 0;
            int profit = 0;
            _books = new BindingList<OrderBookSql>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    string name = (string)reader["name"];
                    string imageUrl = (string)reader["image_url"];
                    int quantity = (int)reader["quantity"];
                    int cost = (int)reader["cost"];
                    int price = (int)reader["price"];
                    DateTime date = (DateTime)reader["date"];
                    var book = new OrderBookSql()
                    {
                        Name = name,
                        Image = imageUrl,
                        Quantity = quantity,
                        Date = date,
                        Cost = cost,
                        Price = price
                    };
                    count = count + book.Quantity;
                    finance = finance + book.Price * book.Quantity;
                    profit = profit + (book.Price - book.Cost) * book.Quantity;
                    _books.Add(book);
                }
                reader.Close();
            }
            check = new bool[_books.Count];
            for (int i = 0; i < _books.Count; i++)
            {
                for (int j = i+1; j < _books.Count ; j++)
                {
                    if (_books[i].Name == _books[j].Name)
                    {
                        _books[i].Quantity = _books[i].Quantity + _books[j].Quantity;
                        _books.RemoveAt(j);
                        j--;

                    }
                }
                check[i] = true;
            }
            listViewProduct.ItemsSource = _books;
            listViewProduct.SelectedIndex = 0;
            textQuantity.Text = _books.Count.ToString();
        }
        string typeTime = "";
        private void WeekButton_Click(object sender, RoutedEventArgs e)
        {
            btnWeek.Background = Brushes.Green;
            btnMonth.Background = Brushes.Gray;
            btnYear.Background = Brushes.Gray;
            btnTime.Background = Brushes.Gray;
            typeTime = "WEEK";
            loadAllBooks(typeTime);
        }

        private void MonthButton_Click(object sender, RoutedEventArgs e)
        {
            btnWeek.Background = Brushes.Gray;
            btnMonth.Background = Brushes.Green;
            btnYear.Background = Brushes.Gray;
            btnTime.Background = Brushes.Gray;
            typeTime = "MONTH";
            loadAllBooks("MONTH");
            if (_books.Count == 0)
            {
                MessageBox.Show("No books sold during the month");
                return;
            }
        }

        private void YearButton_click(object sender, RoutedEventArgs e)
        {
            btnWeek.Background = Brushes.Gray;
            btnMonth.Background = Brushes.Gray;
            btnYear.Background = Brushes.Green;
            btnTime.Background = Brushes.Gray;
            typeTime = "YEAR";
            loadAllBooks(typeTime);

        }

        private void TimeButton_Click(object sender, RoutedEventArgs e)
        {
            btnWeek.Background = Brushes.Gray;
            btnMonth.Background = Brushes.Gray;
            btnYear.Background = Brushes.Gray;
            btnTime.Background = Brushes.Green;
            textTime.Visibility = Visibility.Visible;
        }

        private void startDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _startDate = startDate.SelectedDate;
            _endDate = endDate.SelectedDate;
            loadAllBooksTime();
        }

        private void endDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            _startDate = startDate.SelectedDate;
            _endDate = endDate.SelectedDate;
            loadAllBooksTime();
        }

    }
}
