using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace WpfMyShop.pages
{
    /// <summary>
    /// Interaction logic for SettingPage.xaml
    /// </summary>
    public partial class SettingPage : Page
    {
        int _itemsPerPage;
        public SettingPage()
        {
            InitializeComponent();
            TextBoxPaging.Text = ConfigurationManager.AppSettings["ItemsPerPage"];
            if (TextBoxPaging.Text != null && TextBoxPaging.Text.Length > 0)
            {
                _itemsPerPage = int.Parse(TextBoxPaging.Text);
            }
        }

        private void changePagingButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _itemsPerPage = int.Parse(TextBoxPaging.Text);
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["ItemsPerPage"].Value = _itemsPerPage.ToString();
                config.Save(ConfigurationSaveMode.Minimal);
                ConfigurationManager.RefreshSection("appSettings");
            } catch(Exception ex)
            {
                MessageBox.Show("Số đã nhập không hợp lệ");
            }
        }
    }
}
