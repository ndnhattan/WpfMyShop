using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
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
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Wordprocessing;
using LiveCharts;
using LiveCharts.Wpf;
using Microsoft.Data.SqlClient;
using Microsoft.Identity.Client;
using WpfMyShop.model;
using WpfMyShop.models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WpfMyShop.pages
{
    /// <summary>
    /// Interaction logic for ReportPage.xaml
    /// </summary>
    public partial class ReportFinancePage : Page
    {
        public ReportFinancePage()
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
            //listViewProduct.ItemsSource = _books;
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
            //listViewProduct.ItemsSource = _books;
            textQuantity.Text = count.ToString();
            textFinance.Text = finance.ToString();
            textProfit.Text = profit.ToString();
        }
        bool[] check;
        int[] _bookTemp;
        List<OrderBookSql> sortedList;
        string typeChart = "";
        string typeTime = "";
        public void totalFinanceInTime(string typeTime, string typeChart)
        {
            sortedList = new List<OrderBookSql>(_books.OrderBy(obj => obj.Date).ToList());
            if (typeTime == "WEEK" && typeChart == "FINANCE")
            {
                check = new bool[sortedList.Count];
                _bookTemp = new int[7];
                for (int i = 0; i < 7; i++)
                {
                    _bookTemp[i] = 0;
                }
                int c = 0;
                while (c < sortedList.Count)
                {
                    if (check[c] == false)
                    {
                        int result = sortedList[c].Price * sortedList[c].Quantity;
                        for (int j = c + 1; j < sortedList.Count; j++)
                        {
                            if (sortedList[c].Date == sortedList[j].Date)
                            {
                                result = result + sortedList[j].Price * sortedList[j].Quantity;
                                check[c] = true;
                            }

                        }
                        int dayOfWeek = (int)sortedList[c].Date.DayOfWeek;
                        _bookTemp[dayOfWeek - 1] = result;
                    }
                    c++;
                }


            }
            else if (typeTime == "WEEK" && typeChart == "PROFIT")
            {
                check = new bool[sortedList.Count];
                _bookTemp = new int[7];
                for (int i = 0; i < 7; i++)
                {
                    _bookTemp[i] = 0;
                }
                int c = 0;
                while (c < sortedList.Count)
                {
                    if (check[c] == false)
                    {
                        int result = (sortedList[c].Price - sortedList[c].Cost) * sortedList[c].Quantity;
                        for (int j = c + 1; j < sortedList.Count; j++)
                        {
                            if (sortedList[c].Date == sortedList[j].Date)
                            {
                                result = result + (sortedList[c].Price - sortedList[c].Cost) * sortedList[j].Quantity;
                                check[c] = true;
                            }

                        }
                        int dayOfWeek = (int)sortedList[c].Date.DayOfWeek;
                        _bookTemp[dayOfWeek - 1] = result;
                    }
                    c++;
                }


            }
            else if (typeTime == "MONTH" && typeChart == "FINANCE")
            {
                check = new bool[sortedList.Count];
                _bookTemp = new int[31];
                for (int i = 0; i < 31; i++)
                {
                    _bookTemp[i] = 0;
                }
                int c = 0;
                while (c < sortedList.Count)
                {
                    if (check[c] == false)
                    {
                        int result = sortedList[c].Price * sortedList[c].Quantity;
                        for (int j = c + 1; j < sortedList.Count; j++)
                        {
                            if (sortedList[c].Date == sortedList[j].Date)
                            {
                                result = result + sortedList[j].Price * sortedList[j].Quantity;
                                check[c] = true;
                            }

                        }
                        int dayOfMonth = (int)sortedList[c].Date.Day;
                        _bookTemp[dayOfMonth - 1] = result;
                    }
                    c++;
                }

            }
            else if (typeTime == "MONTH" && typeChart == "PROFIT")
            {
                check = new bool[sortedList.Count];
                _bookTemp = new int[31];
                for (int i = 0; i < 31; i++)
                {
                    _bookTemp[i] = 0;
                }
                int c = 0;
                while (c < sortedList.Count)
                {
                    if (check[c] == false)
                    {
                        int result = (sortedList[c].Price - sortedList[c].Cost) * sortedList[c].Quantity;
                        for (int j = c + 1; j < sortedList.Count; j++)
                        {
                            if (sortedList[c].Date == sortedList[j].Date)
                            {
                                result = result + (sortedList[c].Price - sortedList[c].Cost) * sortedList[j].Quantity;
                                check[c] = true;
                            }

                        }
                        int dayOfMonth = (int)sortedList[c].Date.Day;
                        _bookTemp[dayOfMonth - 1] = result;
                    }
                    c++;
                }

            }
            else if (typeTime == "YEAR" && typeChart == "FINANCE")
            {
                check = new bool[sortedList.Count];
                for (int i = 0; i < check.Length; i++)
                {
                    check[i] = false;
                }
                _bookTemp = new int[12];
                for (int i = 0; i < 12; i++)
                {
                    _bookTemp[i] = 0;
                }
                //DateTime firstDayOfMonth = new DateTime(sortedList[0].Date.Year, sortedList[0].Date.Month, 1);
                //DateTime lastDayOfMonth = new DateTime(sortedList[0].Date.Year, sortedList[0].Date.Month, DateTime.DaysInMonth(sortedList[0].Date.Year, sortedList[0].Date.Month));
                int c = 0;
                while (c < sortedList.Count)
                {
                    if (check[c] == false)
                    {
                        int month = sortedList[c].Date.Month;
                        int result = sortedList[c].Price * sortedList[c].Quantity;
                        for (int j = c + 1; j < sortedList.Count; j++)
                        {
                            int month1 = sortedList[j].Date.Month;
                            if (month == month1 && check[j] == false)
                            {
                                result = result + sortedList[j].Price * sortedList[j].Quantity;
                                check[j] = true;
                            }

                        }
                        _bookTemp[month - 1] = result;
                        check[c] = true;
                    }
                    c++;
                }

            }
            else if (typeTime == "YEAR" && typeChart == "PROFIT")
            {
                check = new bool[sortedList.Count];
                for (int i = 0; i < check.Length; i++)
                {
                    check[i] = false;
                }
                _bookTemp = new int[12];
                for (int i = 0; i < 12; i++)
                {
                    _bookTemp[i] = 0;
                }
                //DateTime firstDayOfMonth = new DateTime(sortedList[0].Date.Year, sortedList[0].Date.Month, 1);
                //DateTime lastDayOfMonth = new DateTime(sortedList[0].Date.Year, sortedList[0].Date.Month, DateTime.DaysInMonth(sortedList[0].Date.Year, sortedList[0].Date.Month));
                int c = 0;
                while (c < sortedList.Count)
                {
                    if (check[c] == false)
                    {
                        int month = sortedList[c].Date.Month;
                        int result = (sortedList[c].Price - sortedList[c].Cost) * sortedList[c].Quantity;
                        for (int j = c + 1; j < sortedList.Count; j++)
                        {
                            int month1 = sortedList[j].Date.Month;
                            if (month == month1 && check[j] == false)
                            {
                                result = result + (sortedList[c].Price - sortedList[c].Cost) * sortedList[j].Quantity;
                                check[j] = true;
                            }

                        }
                        _bookTemp[month - 1] = result;
                        check[c] = true;
                    }
                    c++;
                }

            }
            else if (typeTime == "TIME" && typeChart == "FINANCE")
            {
                check = new bool[sortedList.Count];
                _bookTemp = new int[30];
                for (int i = 0; i < 30; i++)
                {
                    _bookTemp[i] = 0;
                }
                int c = 0;
                while (c < sortedList.Count)
                {
                    if (check[c] == false)
                    {
                        int result = sortedList[c].Price * sortedList[c].Quantity;
                        for (int j = c + 1; j < sortedList.Count; j++)
                        {
                            if (sortedList[c].Date == sortedList[j].Date)
                            {
                                result = result + sortedList[j].Price * sortedList[j].Quantity;
                                check[c] = true;
                            }

                        }
                        int dayOfMonth = (int)sortedList[c].Date.Day;
                        _bookTemp[dayOfMonth - 1] = result;
                    }
                    c++;
                }

            }
            else if (typeTime == "TIME" && typeChart == "PROFIT")
            {
                check = new bool[sortedList.Count];
                _bookTemp = new int[30];
                for (int i = 0; i < 30; i++)
                {
                    _bookTemp[i] = 0;
                }
                int c = 0;
                while (c < sortedList.Count)
                {
                    if (check[c] == false)
                    {
                        int result = (sortedList[c].Price - sortedList[c].Cost) * sortedList[c].Quantity;
                        for (int j = c + 1; j < sortedList.Count; j++)
                        {
                            if (sortedList[c].Date == sortedList[j].Date)
                            {
                                result = result + (sortedList[c].Price - sortedList[c].Cost) * sortedList[j].Quantity;
                                check[c] = true;
                            }

                        }
                        int dayOfMonth = (int)sortedList[c].Date.Day;
                        _bookTemp[dayOfMonth - 1] = result;
                    }
                    c++;
                }

            }
            else
            {
                MessageBox.Show("Enter the data type again");
            }

        }
        private void TimeButton_Click(object sender, RoutedEventArgs e)
        {
            typeTime = "TIME";
            textTime.Visibility = Visibility.Visible;
            btnWeek.Background = Brushes.Gray;
            btnMonth.Background = Brushes.Gray;
            btnYear.Background = Brushes.Gray;
            btnTime.Background = Brushes.Green;
        }

        private void WeekButton_Click(object sender, RoutedEventArgs e)
        {
            typeTime = "WEEK";
            textTime.Visibility = Visibility.Hidden;
            btnWeek.Background = Brushes.Green;
            btnMonth.Background = Brushes.Gray;
            btnYear.Background = Brushes.Gray;
            btnTime.Background = Brushes.Gray;
        }

        private void MonthButton_Click(object sender, RoutedEventArgs e)
        {
            typeTime = "MONTH";
            textTime.Visibility = Visibility.Hidden;
            btnWeek.Background = Brushes.Gray;
            btnMonth.Background = Brushes.Green;
            btnYear.Background = Brushes.Gray;
            btnTime.Background = Brushes.Gray;
        }

        private void YearButton_click(object sender, RoutedEventArgs e)
        {
            typeTime = "YEAR";
            textTime.Visibility = Visibility.Hidden;
            btnWeek.Background = Brushes.Gray;
            btnMonth.Background = Brushes.Gray;
            btnYear.Background = Brushes.Green;
            btnTime.Background = Brushes.Gray;
        }

        private void FinanceButton_click(object sender, RoutedEventArgs e)
        {
            typeChart = "FINANCE";
            btnFinance.Background = Brushes.Green;
            btnProfit.Background = Brushes.Gray;
        }

        private void ProfitButton_click(object sender, RoutedEventArgs e)
        {
            typeChart = "PROFIT";
            btnFinance.Background = Brushes.Gray;
            btnProfit.Background = Brushes.Green;
        }

        private void endDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //_startDate = startDate.SelectedDate;
            //_endDate = endDate.SelectedDate;
            //loadAllBooksTime();
        }

        private void startDate_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            //_startDate = startDate.SelectedDate;
            //_endDate = endDate.SelectedDate;
            //loadAllBooksTime();
        }

        private void applyReportBtn_Click(object sender, RoutedEventArgs e)
        {
            if (typeChart == "" || typeTime == "")
            {
                MessageBox.Show("Select full data");
                return;
            }
            if (typeTime == "WEEK")
            {
                ChartContainer.Children.Clear();
                loadAllBooks("WEEK");
                totalFinanceInTime("WEEK", typeChart);
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
                    Title = "Revenue by week"
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
                chart.LegendLocation = LegendLocation.Bottom;
                ChartContainer.Children.Add(chart);
                chart.Visibility = Visibility.Hidden;
                if (_books.Count > 0)
                {
                    chart.Visibility = Visibility.Visible;
                }
            }
            else if (typeTime == "MONTH")
            {
                typeTime = "MONTH";
                ChartContainer.Children.Clear();
                loadAllBooks("MONTH");
                totalFinanceInTime("MONTH", typeChart);
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
                    Title = "Revenue by month"
                }
            };
                chart.AxisX.Add(new Axis()
                {
                    Foreground = Brushes.Black,
                    FontWeight = FontWeights.Bold,
                    Title = "Day",
                    Labels = new List<string> { "1", "2", "3", "4","5", "6", "7", "8", "9", "10", "11", "12", "13", "14", "15",
                    "16", "17", "18", "19", "20","21", "22", "23", "24","25", "26", "27", "28", "29", "30","31" }
                });
                chart.AxisY.Add(new Axis()
                {
                    Foreground = Brushes.Black,
                    FontWeight = FontWeights.Bold,
                });
                chart.LegendLocation = LegendLocation.Bottom;
                ChartContainer.Children.Add(chart);
                chart.Visibility = Visibility.Hidden;
                if (_books.Count > 0)
                {
                    chart.Visibility = Visibility.Visible;
                }
            }
            else if (typeTime == "YEAR")
            {
                typeTime = "YEAR";
                ChartContainer.Children.Clear();
                loadAllBooks("YEAR");
                totalFinanceInTime("YEAR", typeChart);
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
                    Title = "Revenue by year"
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
                chart.LegendLocation = LegendLocation.Bottom;
                ChartContainer.Children.Add(chart);
                chart.Visibility = Visibility.Hidden;
                if (_books.Count > 0)
                {
                    chart.Visibility = Visibility.Visible;
                }
            }
            //Nhap thoi gian trong khoang 10 ngay 
            else if (typeTime == "TIME")
            {
                typeTime = "TIME";
                _startDate = startDate.SelectedDate;
                _endDate = endDate.SelectedDate;
                if (_startDate == null || _endDate == null) {
                    MessageBox.Show("Please enter entire data");
                    return;
                }
                ChartContainer.Children.Clear();
                loadAllBooksTime();
                totalFinanceInTime("TIME", typeChart);
                if (_books.Count==0)
                {
                    MessageBox.Show("During this time, no books were sold");
                    return;
                }
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
                    Title = $"Revenue from {_startDate} to {_endDate}"
                }
            };
                chart.AxisX.Add(new Axis()
                {
                    Foreground = Brushes.Black,
                    FontWeight = FontWeights.Bold,
                    Title = "Date",
                    Labels = new List<string> { "1", "2", "3", "4","5", "6", "7", "8", "9", "10",
                     }

                });
                chart.AxisY.Add(new Axis()
                {
                    Foreground = Brushes.Black,
                    FontWeight = FontWeights.Bold,
                });
                chart.LegendLocation = LegendLocation.Bottom;
                ChartContainer.Children.Add(chart);
                chart.Visibility = Visibility.Hidden;
                if (_books.Count > 0)
                {
                    chart.Visibility = Visibility.Visible;
                }
            }


        }
    }
}
