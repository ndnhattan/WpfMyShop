using System.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
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
using WpfMyShop.windows;
using log4net;
using log4net.Config;

namespace WpfMyShop
{
    /// <summary>
    /// Interaction logic for StartupWindow.xaml
    /// </summary>
    public partial class StartupWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(StartupWindow));
        public StartupWindow()
        {
            InitializeComponent();
            log4net.Util.LogLog.InternalDebugging = true;
            XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));
        }

        private void textEmail_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtEmail.Focus();
        }

        private void txtEmail_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtEmail.Text) && txtEmail.Text.Length > 0)
                textEmail.Visibility = Visibility.Collapsed;
            else
                textEmail.Visibility = Visibility.Visible;
        }


        private void textPassword_MouseDown(object sender, MouseButtonEventArgs e)
        {
            txtPasswordBox.Focus();
        }

        private void txtPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPasswordBox.Password) && txtPasswordBox.Password.Length > 0)
                textPassword.Visibility = Visibility.Collapsed;
            else
                textPassword.Visibility = Visibility.Visible;
        }


        private void Image_MouseUp(object sender, MouseButtonEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private async void logInButton_Click(object sender, RoutedEventArgs e)
        {
            //if (!string.IsNullOrEmpty(txtEmail.Text) && !string.IsNullOrEmpty(txtPasswordBox.Password))
            //{
            //    MessageBox.Show("Successfully Signed In");
            //}

            // Connect to db
            string username = txtEmail.Text;
            string password = txtPasswordBox.Password;

            var builder = new SqlConnectionStringBuilder();

            // server Tan
            //builder.DataSource = "DESKTOP-3A921M2";
            // server Quang
            //builder.DataSource = "DESKTOP-DKF8GU7\\SQLSERVER2016"; 
            // server Thien
            var nameServer = ConfigurationManager.AppSettings["NameServer"];
            if (nameServer.Equals("") || nameServer.Equals(null))
            {

                builder.DataSource = ".\\SQLEXPRESS01";// tên server demo
                //builder.DataSource = "DESKTOP-DKF8GU7\\SQLSERVER2016";
            }
            else
            {
                //builder.DataSource = "DESKTOP-3A921M2";
                builder.DataSource = nameServer;
            }

            var nameDatabase = ConfigurationManager.AppSettings["NameDatabase"];
            if (nameDatabase.Equals("") || nameDatabase.Equals(null))
            {
                builder.InitialCatalog = "my_shop"; // tên database demo
            }
            else
            {
                //builder.InitialCatalog = "my_shop"; // tên database demo
                builder.InitialCatalog = nameDatabase;
            }
            builder.UserID = username;
            builder.Password = password;
            builder.TrustServerCertificate = true;

            string connectionString = builder.ConnectionString;
            var connection = new SqlConnection(connectionString);

            loading.Visibility = Visibility.Visible;
            connection = await Task.Run(() => {
                var _connection = new SqlConnection(connectionString);

                try
                {
                    _connection.Open();
                }
                catch (Exception ex)
                {

                    _connection = null;
                    MessageBox.Show("Can not connect to server!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    MessageBox.Show(ex.Message);
                }
                // Test khi chạy quá nhanh
                //System.Threading.Thread.Sleep(3000);
                return _connection;
            });
            loading.Visibility = Visibility.Hidden;

            // save username, password if user check Remeber me
            if (connection != null)
            {
                if (RememberMe.IsChecked == true)
                {
                    var passwordInBytes = Encoding.UTF8.GetBytes(password);
                    var entropy = new byte[20];
                    using (var rng = new RNGCryptoServiceProvider())
                    {
                        rng.GetBytes(entropy);
                    }
                    var cypherText = ProtectedData.Protect(passwordInBytes, entropy,
                        DataProtectionScope.CurrentUser);
                    var passwordIn64 = Convert.ToBase64String(cypherText);
                    var entropyIn64 = Convert.ToBase64String(entropy);

                    var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["Username"].Value = username;
                    config.AppSettings.Settings["Password"].Value = passwordIn64;
                    config.AppSettings.Settings["Entropy"].Value = entropyIn64;
                    config.Save(ConfigurationSaveMode.Minimal);

                    ConfigurationManager.RefreshSection("appSettings");
                }
                else
                {  // delete username, password if user do not check Remeber me
                    var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["Username"].Value = "";
                    config.AppSettings.Settings["Password"].Value = "";
                    config.AppSettings.Settings["Entropy"].Value = "";
                    config.Save(ConfigurationSaveMode.Minimal);

                    ConfigurationManager.RefreshSection("appSettings");
                }
                DB.Instance.ConnectionString = connectionString;
                new Dashboard().Show();
                this.Close();
            }
            else
            {
                // Cannot connect to db
                //MessageBox.Show($"Cannot connect" );
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Load username, password and check RememberMe
            var passwordIn64 = ConfigurationManager.AppSettings["Password"];
            if (passwordIn64.Length != 0)
            {
                var entropyIn64 = ConfigurationManager.AppSettings["Entropy"];
                var cyperTextInBytes = Convert.FromBase64String(passwordIn64);
                var entropyInBytes = Convert.FromBase64String(entropyIn64);
                var passwordInBytes = ProtectedData.Unprotect(cyperTextInBytes, entropyInBytes,
                    DataProtectionScope.CurrentUser);
                var password = Encoding.UTF8.GetString(passwordInBytes);
                txtPasswordBox.Password = password;
                txtEmail.Text = ConfigurationManager.AppSettings["Username"];
                RememberMe.SetCurrentValue(CheckBox.IsCheckedProperty, true);
            }

            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;

            string bufferImgClose = "assets/close.png";
            string t_imgClose = $"{baseDirectory}{bufferImgClose}";
            var bitmapImgClose = new BitmapImage(new Uri(t_imgClose, UriKind.Absolute));

            string bufferImgEmail = "assets/user.jpg";
            string t_imgEmail = $"{baseDirectory}{bufferImgEmail}";
            var bitmapImgEmail = new BitmapImage(new Uri(t_imgEmail, UriKind.Absolute));
            imgEmail.Source = bitmapImgEmail;

            string bufferImgPassword = "assets/password.png";
            string t_imgPassword = $"{baseDirectory}{bufferImgPassword}";
            var bitmapImgPassword = new BitmapImage(new Uri(t_imgPassword, UriKind.Absolute));
            imgPassword.Source = bitmapImgPassword;

        }

        private void Button_Click_Setting(object sender, RoutedEventArgs e)
        {
            var screen = new SettingServerWindow();
            screen.Show();
        }
    }
}
