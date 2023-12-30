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
using log4net;
using log4net.Config;

namespace WpfMyShop.windows
{
    /// <summary>
    /// Interaction logic for SettingServerWindow.xaml
    /// </summary>
    public partial class SettingServerWindow : Window
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(SettingServerWindow));
        public SettingServerWindow()
        {
            InitializeComponent();
            log4net.Util.LogLog.InternalDebugging = true;
            XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));
        }
        private void btn_Accept_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                config.AppSettings.Settings["NameServer"].Value = $"{textBoxServer.Text}";
                config.AppSettings.Settings["NameDatabase"].Value = textBoxDatabase.Text;
                config.Save(ConfigurationSaveMode.Minimal);

                ConfigurationManager.RefreshSection("appSettings");

                ConfigurationManager.RefreshSection("appSettings");
                this.Close();
            }catch(Exception ex)
            {
                log.Debug(ex.Message);
            }
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            textBoxServer.Text = ConfigurationManager.AppSettings["NameServer"];
            textBoxDatabase.Text = ConfigurationManager.AppSettings["NameDatabase"];
        }
    }
}
