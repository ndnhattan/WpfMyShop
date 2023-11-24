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
using LiveCharts.Wpf;
using LiveCharts;
using Microsoft.Data.SqlClient;
using WpfMyShop.model;
using WpfMyShop.models;
using System.Collections;

namespace WpfMyShop.pages
{
    /// <summary>
    /// Interaction logic for ReportProductPage.xaml
    /// </summary>
    public partial class ReportProductPage : Page
    {
        public ReportProductPage()
        {
            InitializeComponent();
        }
        private void DatePicker_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

        }

        private void ReportPage_Loaded(object sender, RoutedEventArgs e)
        {

        }
        BindingList<OrderBookSql> _books;
        DateTime? _startDate = null;
        DateTime? _endDate = null;
        public void loadAllBooksTime()
        {

            var sql = $"""
                select count(*) over() as Total, name,image_url,quantity,O.cost,O.price,date
                from Books as B join Order_Books as OB join Orders as O
                on O.id = OB.order_id on OB.book_id = B.id
                {(_startDate != null && _endDate != null ? "where (date >= @Start) and (date <= @End)" : "")}

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
            listViewProduct.ItemsSource = _books;
            listViewProduct.SelectedIndex = 0;
            textQuantity.Text = count.ToString();
            textFinance.Text = finance.ToString();
            textProfit.Text = profit.ToString();
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
            listViewProduct.ItemsSource = _books;
            listViewProduct.SelectedIndex = 0;
            textQuantity.Text = count.ToString();
            textFinance.Text = finance.ToString();
            textProfit.Text = profit.ToString();
        }
        string typeTime = "";
        bool[] check;
        List<OrderBookSql> sortedList;
        int[] _bookTemp;
        private void totalProductInTime(string typeTime, OrderBookSql book)
        {
            sortedList = new List<OrderBookSql>(_books.OrderBy(obj => obj.Date).ToList());
            if (typeTime == "WEEK")
            {
                _bookTemp = new int[7];
                for (int i = 0; i < 7; i++)
                {
                    _bookTemp[i] = 0;
                }
                for (int i = 0; i < _books.Count; i++)
                {
                    if (_books[i].Name == book.Name)
                    {
                        int dayOfWeek = (int)_books[i].Date.DayOfWeek;
                        _bookTemp[dayOfWeek - 1] = _books[i].Quantity;
                    }
                }
            }
            else if (typeTime == "MONTH")
            {
                _bookTemp = new int[31];
                for (int i = 0; i < 31; i++)
                {
                    _bookTemp[i] = 0;
                }
                for (int i = 0; i < _books.Count; i++)
                {
                    if (_books[i].Name == book.Name)
                    {
                        int dayOfMonth = (int)_books[i].Date.Day;
                        _bookTemp[dayOfMonth ] = _books[i].Quantity;
                    }
                }
            }
            else if (typeTime == "YEAR")
            {
                check=new bool[_books.Count];
                _bookTemp = new int[12];
                for (int i = 0; i < 12; i++)
                {
                    _bookTemp[i] = 0;
                }
                
                for (int i = 0; i < _books.Count; i++)
                {
                    int result = 0;
                    int month = _books[i].Date.Month;
                    for(int j = 0 ;j < _books.Count;j++)
                    {
                        int month1 = _books[j].Date.Month;
                        if(month1 == month && _books[j].Name==book.Name) 
                        {
                            result = result + _books[j].Quantity;
                            check[j] = true;
                        }
                    }
                    _bookTemp[month-1]= result;
                }
            }
            else if (typeTime == "TIME")
            {
                _bookTemp = new int[31];
                for (int i = 0; i < 31; i++)
                {
                    _bookTemp[i] = 0;
                }
                for (int i = 0; i < _books.Count; i++)
                {
                    if (_books[i].Name == book.Name)
                    {
                        int dayOfMonth = (int)_books[i].Date.Day;
                        _bookTemp[dayOfMonth] = _books[i].Quantity;
                    }
                }
            }
        }
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
                MessageBox.Show("Không bán được cuốn sách nào trong tháng");
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
            typeTime = "TIME";
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

        private void listViewProduct_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChartContainer.Children.Clear();
            int index = listViewProduct.SelectedIndex;
            if (index == -1)
            {
                index = 0;
            }
            if (typeTime == "WEEK")
            {
                if(_books.Count == 0)
                {
                    MessageBox.Show("No books were sold during the week");
                    return;
                }
                totalProductInTime(typeTime, _books[index]);
                ChartValues<int> chartValues = new ChartValues<int>(_bookTemp);
                CartesianChart chart = new CartesianChart();
                chart.Series = new LiveCharts.SeriesCollection()
                 {

                    new ColumnSeries()
                     {
                        ColumnPadding=5,
                        MaxColumnWidth=20,
                        Values = chartValues,
                        Stroke = Brushes.Red,
                        Fill = Brushes.Yellow,
                        StrokeThickness = 2,
                        Title = "Number of books per week"
                    }
                };
                chart.AxisX.Add(new Axis()
                {
                    Foreground = Brushes.Black,
                    FontWeight = FontWeights.Bold,
                    Title = "Day",
                    Labels = new List<string> { "2", "3", "4", "5", "6", "7", "CN" }
                });
                chart.AxisY.Add(new Axis()
                {
                    Foreground = Brushes.Black,
                    FontWeight = FontWeights.Bold,
                });
                ChartContainer.Children.Add(chart);
                chart.LegendLocation= LegendLocation.Bottom;
                chart.Margin = new Thickness(0, 10, 0, 0);
                chart.Visibility = Visibility.Hidden;
                if (_books.Count > 0)
                {
                    chart.Visibility = Visibility.Visible;
                }
            }
            else if (typeTime == "MONTH")
            {
                if (_books.Count == 0)
                {
                    MessageBox.Show("No books sold during the month");
                    return;
                }
                totalProductInTime(typeTime, _books[index]);
                ChartValues<int> chartValues = new ChartValues<int>(_bookTemp);
                CartesianChart chart = new CartesianChart();
                chart.Series = new LiveCharts.SeriesCollection()
                 {

                    new ColumnSeries()
                     {
                        ColumnPadding=5,
                        MaxColumnWidth=20,
                        Values = chartValues,
                        Stroke = Brushes.Red,
                        Fill = Brushes.Yellow,
                        StrokeThickness = 2,
                        Title = "Number of books by month"
                    }
                };
                chart.AxisX.Add(new Axis()
                {
                    Foreground = Brushes.Black,
                    FontWeight = FontWeights.Bold,
                    Title = "Date",
                    Labels = new List<string> { "1", "2", "3", "4","5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15",
                    "16", "17", "18", "19", "20","21", "22", "23", "24","25", "26", "27", "28", "29", "30","31" }
                });
                chart.AxisY.Add(new Axis()
                {
                    Foreground = Brushes.Black,
                    FontWeight = FontWeights.Bold,
                });
                ChartContainer.Children.Add(chart);
                chart.LegendLocation = LegendLocation.Bottom;
                chart.Margin = new Thickness(0, 10, 0, 0);
                chart.Visibility = Visibility.Hidden;
                if (_books.Count > 0)
                {
                    chart.Visibility = Visibility.Visible;
                }
            }
            else if (typeTime == "YEAR")
            {
                if (_books.Count == 0)
                {
                    MessageBox.Show("No books sold during the year");
                    return;
                }
                totalProductInTime(typeTime, _books[index]);
                ChartValues<int> chartValues = new ChartValues<int>(_bookTemp);
                CartesianChart chart = new CartesianChart();
                chart.Series = new LiveCharts.SeriesCollection()
                 {

                    new ColumnSeries()
                     {
                        ColumnPadding=5,
                        MaxColumnWidth=20,
                        Values = chartValues,
                        Stroke = Brushes.Red,
                        Fill = Brushes.Yellow,
                        StrokeThickness = 2,
                        Title = "Number of books by year"
                    }
                };
                chart.AxisX.Add(new Axis()
                {
                    Foreground = Brushes.Black,
                    FontWeight = FontWeights.Bold,
                    Title = "Month",
                    Labels = new List<string> { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12" }
                });
                chart.AxisY.Add(new Axis()
                {
                    Foreground = Brushes.Black,
                    FontWeight = FontWeights.Bold,
                });
                ChartContainer.Children.Add(chart);
                chart.LegendLocation = LegendLocation.Bottom;
                chart.Margin=new Thickness(0,10, 0, 0);
                chart.Visibility = Visibility.Hidden;
                if (_books.Count > 0)
                {
                    chart.Visibility = Visibility.Visible;
                }
            }
            //Nhap thoi gian trong khoang 30 ngay
            else if (typeTime == "TIME")
            {
                if (_books.Count == 0)
                {
                    MessageBox.Show("No books sold during this time");
                    return;
                }
                totalProductInTime(typeTime, _books[index]);
                ChartValues<int> chartValues = new ChartValues<int>(_bookTemp);
                CartesianChart chart = new CartesianChart();
                chart.Series = new LiveCharts.SeriesCollection()
                 {

                    new ColumnSeries()
                     {
                        ColumnPadding=5,
                        MaxColumnWidth=20,
                        Values = chartValues,
                        Stroke = Brushes.Red,
                        Fill = Brushes.Yellow,
                        StrokeThickness = 2,
                        Title = $"Quantity of book from {_startDate} to {_endDate}"
                    }
                };
                chart.AxisX.Add(new Axis()
                {
                    Foreground = Brushes.Black,
                    FontWeight = FontWeights.Bold,
                    Title = "Date",
                    Labels = new List<string> { "1", "2", "3", "4","5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15",
                    "16", "17", "18", "19", "20","21", "22", "23", "24","25", "26", "27", "28", "29", "30","31" }
                });
                chart.AxisY.Add(new Axis()
                {
                    Foreground = Brushes.Black,
                    FontWeight = FontWeights.Bold,
                });
                ChartContainer.Children.Add(chart);
                chart.LegendLocation = LegendLocation.Bottom;
                chart.Margin = new Thickness(0, 10, 0, 0);
                chart.Visibility = Visibility.Hidden;
                if (_books.Count > 0)
                {
                    chart.Visibility = Visibility.Visible;
                }
            }

        }

    }
}
