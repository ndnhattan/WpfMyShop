using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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

    public partial class CustomerPage : Page
    {
        public event System.ComponentModel.CancelEventHandler? Closing;
        int _rowsPerPage;

        public CustomerPage()
        {
            InitializeComponent();
        }


        public BindingList<Customer> _customers;

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
                    Label = "Tăng dần",
                    Value = "dob ASC",
                },
                new {
                    Label = "Giảm dần",
                    Value = "dob DESC",
                },
            };

            filterComboBox.ItemsSource = filters;
            filterComboBox.SelectedIndex = 0;

            if (!Dashboard.isLoaded)
            {
                if (Dashboard.page.Equals("AddCustomerPage"))
                {
                    AddCustomerPage page = new AddCustomerPage(_customers);
                    NavigationService.Navigate(page);
                    Dashboard.isLoaded = true;

                    if (!AddCustomerPage.isAddFail) // add successfully
                    {
                        int current = pagingComboBox.SelectedIndex;
                        LoadAllBooks("");
                        pagingComboBox.SelectedIndex = current;
                    }
                }
                else if (Dashboard.page.Equals("DetailCustomerPage"))
                {
                    DetailCustomerPage page = new DetailCustomerPage(_customers, Dashboard.selectedIndex);
                    NavigationService.Navigate(page);
                    Dashboard.isLoaded = true;
                }
            }
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
                select *, count(*) over() as Total from Customers
                {(_startDate != null && _endDate != null ? "where (birthday >= @Start) and (birthday <= @End)" : "")}
                order by 
                CASE WHEN @Filter = 'dob ASC' then birthday ELSE null END ASC,
                CASE WHEN @Filter = 'dob DESC' then birthday ELSE null END DESC
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
            _customers = new BindingList<Customer>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    int id = (int)reader["id"];
                    string fullName = (string)reader["fullname"];
                    string gender = (string)reader["gender"];
                    DateTime dob = (DateTime)reader["birthday"];
                    string email = (string)reader["email"];
                    string phoneNumber = (string)reader["phone_number"];
                    string address = (string)reader["address"];
                    var customer = new Customer()
                    {
                        Id = id,
                        FullName = fullName,
                        Gender = gender,
                        Email = email,
                        PhoneNumber = phoneNumber,
                        Address = address,
                        DOB = dob,
                    };

                    count = (int)reader["Total"];

                    _customers.Add(customer);
                }
                reader.Close();
            }

            orderGrid.ItemsSource = _customers;

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

                //if (!Dashboard.isLoaded)
                //{
                //    //if (Dashboard.page.Equals("DetailOrderPage"))
                //    //{
                //    //    DetailOrderPage page = new DetailOrderPage(_orders, Dashboard.selectedIndex);
                //    //    NavigationService.Navigate(page);
                //    //    Dashboard.isLoaded = true;
                //    //}
                //    //else if (Dashboard.page.Equals("AddOrderPage"))
                //    //{
                //    var page = new AddCustomerPage(_customers);
                //    NavigationService.Navigate(page);
                //    Dashboard.isLoaded = true;

                //    if (!AddOrderPage.isAddFail) // add successfully
                //    {
                //        int current = pagingComboBox.SelectedIndex;
                //        LoadAllBooks("");
                //        pagingComboBox.SelectedIndex = current;
                //    }
                //    //}
                //}
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


        private void addBtn_Click(object sender, RoutedEventArgs e)
        {
            var page = new AddCustomerPage(_customers);
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
            DetailCustomerPage page = new DetailCustomerPage(_customers, i);
            NavigationService.Navigate(page);
        }

        private void orderGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
