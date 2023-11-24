using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
using System.Windows.Shapes;

namespace WpfMyShop.windows
{
    /// <summary>
    /// Interaction logic for SettingServerWindow.xaml
    /// </summary>
    public partial class SettingServerWindow : Window
    {
        public SettingServerWindow()
        {
            InitializeComponent();
        }
        private void btn_Accept_Click(object sender, RoutedEventArgs e)
        {
            var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["NameServer"].Value = $"{textBoxServer.Text}";
            config.AppSettings.Settings["NameDatabase"].Value = textBoxDatabase.Text;
            config.Save(ConfigurationSaveMode.Minimal);

            ConfigurationManager.RefreshSection("appSettings");


       

            ConfigurationManager.RefreshSection("appSettings");
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textBoxServer.Text = ConfigurationManager.AppSettings["NameServer"];
            textBoxDatabase.Text = ConfigurationManager.AppSettings["NameDatabase"];
        }
    }
}
