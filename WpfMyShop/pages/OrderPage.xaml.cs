using Azure;
using Microsoft.Data.SqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
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
        public event System.ComponentModel.CancelEventHandler? Closing;
        int _rowsPerPage;
        public OrderPage()
        {
            InitializeComponent();
        }

        public BindingList<Order> _orders;

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {

            try
            {
                _rowsPerPage = int.Parse(ConfigurationManager.AppSettings["ItemsPerPage"].ToString());
            }
            catch (Exception ex)
            {
                _rowsPerPage = 10;
            }

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

            //int _rowsPerPage = 10;
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
            BindingList<int> arrayListDiscountID = new BindingList<int> { };
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
                    object discountID = reader["id_discount"];
                    if (discountID != null && !Convert.IsDBNull(discountID))
                    {
                        int id_discount = (int)discountID;
                        arrayListDiscountID.Add(id_discount);
                    }
                    else
                    {
                        arrayListDiscountID.Add(0);
                    }

                    var order = new Order()
                    {
                        Id = id,
                        Price = price,
                        Cost = cost,
                        CustomerId = customer_id,
                        Date = date,
                        CustomerName = customer_name,
                    };

                    count = (int)reader["Total"];

                    _orders.Add(order);
                }
                reader.Close();
            }

            for (int i = 0; i < arrayListDiscountID.Count; i++)
            {
                if (arrayListDiscountID[i] != 0)
                {
                    sql = "select * from Discount where id = @id";
                    command = new SqlCommand(sql, DB.Instance.Connection);
                    command.Parameters.Add("@id", SqlDbType.Int).Value = arrayListDiscountID[i];
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int id = (int)reader["id"];
                            int condition = (int)reader["condition"];
                            int value = (int)reader["value"];
                            string name = (string)reader["name"];
                            string currency = (string)reader["currency"];

                            var discount = new Discount()
                            {
                                Id = id,
                                Value = value,
                                Condition = condition,
                                Name = name,
                                Currency = currency
                            };

                            _orders[i].Discount = discount;
                        }
                        reader.Close();
                    }
                }
            }

            for (int i = 0; i < _orders.Count; i++)
            {
                var sql1 = $@"SELECT * FROM Order_Books WHERE order_id = @orderID";
                var command1 = new SqlCommand(sql1, DB.Instance.Connection);
                command1.Parameters.Add("@orderID", SqlDbType.Int).Value = _orders[i].Id;
                BindingList<int> listBookIndex = new BindingList<int> { };
                BindingList<int> quantityList = new BindingList<int> { };
                BindingList<OrderBook> orderBookList = new BindingList<OrderBook> { };
                using (var reader1 = command1.ExecuteReader())
                {
                    while (reader1.Read())
                    {
                        int book_id = (int)reader1["book_id"];
                        int quantity = (int)reader1["quantity"];
                        quantityList.Add(quantity);
                        listBookIndex.Add(book_id);
                    }
                }

                BindingList<Book> books = new BindingList<Book>();
                for (int j = 0; j < listBookIndex.Count; j++)
                {
                    var sql2 = $@"SELECT * FROM Books WHERE id = @ID";
                    var command2 = new SqlCommand(sql2, DB.Instance.Connection);
                    command2.Parameters.Add("@ID", SqlDbType.Int).Value = listBookIndex[j];
                    using (var reader2 = command2.ExecuteReader())
                    {
                        while (reader2.Read())
                        {
                            int id = (int)reader2["id"];
                            string name = (string)reader2["name"];
                            string author = (string)reader2["author"];
                            int year = (int)reader2["year"];
                            string image_url = (string)reader2["image_url"];
                            int price = (int)reader2["price"];
                            int promo_price = (int)reader2["promo_price"];
                            int sold = (int)reader2["sold"];
                            int stock = (int)reader2["stock"];
                            int cost = (int)reader2["cost"];
                            int genre_id = (int)reader2["genre_id"];

                            var book = new Book()
                            {
                                Id = id,
                                Name = name,
                                Author = author,
                                Year = year,
                                Image = image_url,
                                Price = price,
                                PromoPrice = promo_price,
                                Sold = sold,
                                Cost = cost,
                                Stock = stock,
                                GenreId = genre_id,
                                IsPromo = price == promo_price ? false : true,
                            };

                            books.Add(book);

                            OrderBook order = new OrderBook()
                            {
                                Price = price * quantityList[j],
                                Cost = cost * quantityList[j],
                                Name = name,
                                Quantity = quantityList[j],
                                Id_Book = id
                            };
                            orderBookList.Add(order);
                        }
                    }
                }
                _orders[i].ListOrderBook = orderBookList;
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

                if (!Dashboard.isLoaded)
                {
                    if (Dashboard.page.Equals("DetailOrderPage"))
                    {
                        DetailOrderPage page = new DetailOrderPage(_orders, Dashboard.selectedIndex);
                        NavigationService.Navigate(page);
                        Dashboard.isLoaded = true;
                    }
                    else if (Dashboard.page.Equals("AddOrderPage"))
                    {
                        var page = new AddOrderPage(_orders);
                        NavigationService.Navigate(page);
                        Dashboard.isLoaded = true;

                        if (!AddOrderPage.isAddFail) // add successfully
                        {
                            int current = pagingComboBox.SelectedIndex;
                            LoadAllBooks("");
                            pagingComboBox.SelectedIndex = current;
                        }
                    }
                }
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
            var page = new AddOrderPage(_orders);
            NavigationService.Navigate(page);

            if (!AddOrderPage.isAddFail) // add successfully
            {
                int current = pagingComboBox.SelectedIndex;
                LoadAllBooks("");
                pagingComboBox.SelectedIndex = current;
            }
        }

        private void DataGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
             int i = orderGrid.SelectedIndex;
             var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
             config.AppSettings.Settings["SelectedIndex"].Value = i.ToString();
             config.Save(ConfigurationSaveMode.Minimal);
             ConfigurationManager.RefreshSection("SelectedIndex");
             DetailOrderPage page = new DetailOrderPage(_orders, i);
             NavigationService.Navigate(page);
        }

        private void orderGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
