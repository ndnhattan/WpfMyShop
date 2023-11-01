using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace WpfMyShop.pages
{
    /// <summary>
    /// Interaction logic for OrderPage.xaml
    /// </summary>
    public partial class OrderPage : Page
    {
        public OrderPage()
        {
            InitializeComponent();
        }

        public BindingList<Order> _orders;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadAllBooks("");
            var filters = new List<object>()
            {
                new {
                    Label = "Mới nhất",
                    Value = "new",
                },
                new {
                    Label = "Giá thấp -> cao",
                    Value = "price ASC",
                },
                new {
                    Label = "Giá cao -> thấp",
                    Value = "price DESC",
                },
            };

            filterComboBox.ItemsSource = filters;
            filterComboBox.SelectedIndex = 0;
        }

        int _totalPages = -1;
        int _totalItems = -1;
        int _currentPage = 1;
        DateTime? _startDate = null;
        DateTime? _endDate = null;
        string _filter = "";
        private void LoadAllBooks(string type)
        {
            var sql = $"""
                select *, count(*) over() as Total, cm.fullname from Orders od join Customers cm on od.customer_id = cm.id
                {(_startDate != null && _endDate != null ? "where (date >= @Start) and (date <= @End)" : "")}
                order by 
                case WHEN @Filter = 'new' Then date ELSE null END DESC,
                CASE WHEN @Filter = 'price ASC' then price ELSE null END ASC,
                CASE WHEN @Filter = 'price DESC' then price ELSE null END DESC
                offset @Skip rows fetch next @Take rows only
                """;
            var command = new SqlCommand(sql, DB.Instance.Connection);

            int _rowsPerPage = 10;
            command.Parameters.Add("@Skip", SqlDbType.Int)
                .Value = (_currentPage - 1) * _rowsPerPage;
            command.Parameters.Add("@Take", SqlDbType.Int)
                .Value = _rowsPerPage;

            if (_startDate != null && _endDate != null)
            {
                command.Parameters.AddWithValue("@Start", _startDate);
                command.Parameters.AddWithValue("@End", _endDate);
            }
            command.Parameters.AddWithValue("@Filter", _filter);

            int count = -1;
            _orders = new BindingList<Order>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = (int)reader["id"];
                    int customer_id = (int)reader["customer_id"];
                    int price = (int)reader["price"];
                    int cost = (int)reader["cost"];
                    DateTime date = (DateTime)reader["date"];
                    string customer_name = (string)reader["fullname"];

                    var order = new Order()
                    {
                        Id = id,
                        Price = price,
                        Cost = cost,
                        CustomerId = customer_id,
                        Date = date,
                        CustomerName = customer_name
                    };
                    count = (int)reader["Total"];

                    _orders.Add(order);
                }

                reader.Close();
            }
            orderGrid.ItemsSource = _orders;

            if (type != "changePage")
            {
                _totalItems = count;
                _totalPages = (_totalItems / _rowsPerPage) +
                    (((_totalItems % _rowsPerPage) == 0) ? 0 : 1);

                // Tạo thông tin phân trang cho combobox
                var pageInfos = new List<object>();

                for (int i = 1; i <= _totalPages; i++)
                {
                    pageInfos.Add(new
                    {
                        Page = i,
                        Total = _totalPages
                    });
                };

                pagingComboBox.ItemsSource = pageInfos;
                pagingComboBox.SelectedIndex = 0;
            }
        }

        private void pagingComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic info = pagingComboBox.SelectedItem;
            if (info != null)
            {
                if (info?.Page != _currentPage)
                {
                    _currentPage = info?.Page;
                    LoadAllBooks("changePage");

                }
            }
        }

        private void previousBtn_Click(object sender, RoutedEventArgs e)
        {
            if (pagingComboBox.SelectedIndex > 0)
            {
                pagingComboBox.SelectedIndex--;
            }
        }

        private void nextBtn_Click(object sender, RoutedEventArgs e)
        {
            if (pagingComboBox.SelectedIndex < _totalPages - 1)
            {
                pagingComboBox.SelectedIndex++;
            }
        }

        private void filterComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic info = filterComboBox.SelectedItem;
            if (info != null)
            {
                if (info?.Value != _filter)
                {
                    _filter = info?.Value;
                    LoadAllBooks("");
                }
            }
        }

        private void applyPriceBtn_Click(object sender, RoutedEventArgs e)
        {
            _startDate = startDate.SelectedDate;
            _endDate = endDate.SelectedDate;
            LoadAllBooks("");
        }

        private void DatePicker_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            DatePicker dp = (DatePicker)sender;
            Regex regex = new Regex("[^0-9/]"); //regex that matches allowed text
            e.Handled = regex.IsMatch(e.Text);
        }

        private void ListViewBook_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //int i = booksList.SelectedIndex;
            //DetailProductPage page = new DetailProductPage(i, _books);
            //NavigationService.Navigate(page);
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            //var page = new AddBookPage(_books);
            //NavigationService.Navigate(page);

            //if (!AddBookPage.isAddFail) // add successfully
            //{
            //    int current = pagingComboBox.SelectedIndex;
            //    LoadAllBooks("");
            //    pagingComboBox.SelectedIndex = current;
            //}
        }

       
    }
}
