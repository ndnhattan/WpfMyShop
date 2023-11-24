using Azure;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Reflection;
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

namespace WpfMyShop.pages
{
    /// <summary>
    /// Interaction logic for ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        int _rowsPerPage;
        public ProductPage()
        {
            InitializeComponent();
        }

        public BindingList<Book> _books;
        public BindingList<Genre> _genres;

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
                    Label = "Newest",
                    Value = "new",
                },
                new {
                    Label = "Best-seller",
                    Value = "best seller",
                },
                new {
                    Label = "low -> high price",
                    Value = "price ASC",
                },
                new {
                    Label = "high -> low price",
                    Value = "price DESC",
                },
            };

            filterComboBox.ItemsSource = filters;
            filterComboBox.SelectedIndex = 0;
            loadGenres();

            if (!Dashboard.isLoaded)
            {
                if (Dashboard.page.Equals("AddBookPage"))
                {
                    var page = new AddBookPage(_books);
                    NavigationService.Navigate(page);
                    Dashboard.isLoaded = true;

                    if (!AddBookPage.isAddFail) // add successfully
                    {
                        int current = pagingComboBox.SelectedIndex;
                        LoadAllBooks("");
                        pagingComboBox.SelectedIndex = current;
                    }
                }
                else if (Dashboard.page.Equals("DetailProductPage"))
                {
                    DetailProductPage page = new DetailProductPage(Dashboard.selectedIndex, _books);
                    NavigationService.Navigate(page);
                    Dashboard.isLoaded = true;
                }
            }
        }

        private void loadGenres()
        {
            var sql = """
                select * from Genres
                """;
            var command = new SqlCommand(sql, DB.Instance.Connection);

            int count = -1;
            _genres = new BindingList<Genre>();
            _genres.Add(new Genre() { Id = 0,Name="Tất cả",Description="" });
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = (int)reader["id"];
                    string name = (string)reader["name"];
                    string description = reader["description"] == DBNull.Value ? string.Empty : (string)reader["description"];

                    var genre = new Genre()
                    {
                        Id = id,
                        Name = name,
                        Description = description,
                    };

                    _genres.Add(genre);
                }

                reader.Close();
            }
            genreComboBox.ItemsSource = _genres;
            genreComboBox.SelectedIndex = 0;
        }

        int _totalPages = -1;
        int _totalItems = -1;
        int _currentPage = 1;
        int _startPrice = 0;
        int _endPrice = 0;
        string _filter = "";
        int _genre = 0;
        private void LoadAllBooks(string type)
        {
            var sql = $"""
                select *, count(*) over() as Total from Books
                where (name like @Keyword) and (promo_price >= @Start) 
                      {(_endPrice != 0 ? "and (promo_price <= @End)" : "")}
                      {(_genre != 0 ? "and (genre_id = @Genre)" : "")}
                order by 
                case WHEN @Filter = 'new' Then created_date ELSE null END DESC,
                case WHEN @Filter = 'best seller' Then sold ELSE null END DESC,
                CASE WHEN @Filter = 'price ASC' then promo_price ELSE null END ASC,
                CASE WHEN @Filter = 'price DESC' then promo_price ELSE null END DESC
                offset @Skip rows fetch next @Take rows only
                """;
            var command = new SqlCommand(sql, DB.Instance.Connection);

            //int _rowsPerPage = 10;
            command.Parameters.Add("@Skip", SqlDbType.Int)
                .Value = (_currentPage - 1) * _rowsPerPage;
            command.Parameters.Add("@Take", SqlDbType.Int)
                .Value = _rowsPerPage;
            var keyword = keywordTextBox.Text;
            command.Parameters.AddWithValue("@Keyword", "%" + keyword + "%");
            command.Parameters.AddWithValue("@Start", _startPrice);
            if (_endPrice > 0) command.Parameters.AddWithValue("@End", _endPrice);
            command.Parameters.AddWithValue("@Filter", _filter);
            if (_genre != 0) command.Parameters.AddWithValue("@Genre", _genre);

            int count = -1;
            _books = new BindingList<Book>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = (int)reader["id"];
                    string name = (string)reader["name"];
                    string author = (string)reader["author"];
                    int year = (int)reader["year"];
                    string image_url = (string)reader["image_url"];
                    int price = (int)reader["price"];
                    int promo_price = (int)reader["promo_price"];
                    int sold = (int)reader["sold"];
                    int stock = (int)reader["stock"];
                    int cost = (int)reader["cost"];
                    //int genre_id = (int)reader["genre_id"];
                    int genre_id = reader["genre_id"] == DBNull.Value ? 0 : (int)reader["genre_id"];
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
                    count = (int)reader["Total"];

                    _books.Add(book);
                }

                reader.Close();
            }
            booksList.ItemsSource = _books;

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

            //Title = $"Displaying {_books.Count} / {_totalItems}";
            
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

        private void keywordTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                LoadAllBooks("");
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
            _startPrice = string.IsNullOrEmpty(startPriceTextBox.Text) ? 0 : Int32.Parse(startPriceTextBox.Text);
            _endPrice = string.IsNullOrEmpty(endPriceTextBox.Text) ? 0 : Int32.Parse(endPriceTextBox.Text);
            LoadAllBooks("");
        }

        private void priceTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            // Define a regular expression to match only numeric input
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("[^0-9]+");

            // Replace any non-numeric characters with an empty string
            e.Handled = regex.IsMatch(e.Text);

        }

        private void ListViewBook_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            int i = booksList.SelectedIndex;
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["SelectedIndex"].Value = i.ToString();
            config.Save(ConfigurationSaveMode.Minimal);
            ConfigurationManager.RefreshSection("SelectedIndex");
            DetailProductPage page = new DetailProductPage(i, _books);
            NavigationService.Navigate(page);
        }

        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            var page = new AddBookPage(_books);
            NavigationService.Navigate(page);

            if (!AddBookPage.isAddFail) // add successfully
            {
                int current = pagingComboBox.SelectedIndex;
                LoadAllBooks("");
                pagingComboBox.SelectedIndex = current;
            }
        }

        private void genreComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            dynamic info = genreComboBox.SelectedItem;
            if (info != null)
            {
                if (info?.Id != _genre)
                {
                    _genre = info?.Id;
                    LoadAllBooks("");
                }
            }
        }

        
    }


}
