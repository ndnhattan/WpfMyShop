using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;
using WpfMyShop.model;
using WpfMyShop.models;

namespace WpfMyShop.windows
{
    /// <summary>
    /// Interaction logic for EditGenreWindow.xaml
    /// </summary>
    public partial class EditGenreWindow : Window
    {
        public Genre editGenre {  get; set; }
        public EditGenreWindow(Genre info)
        {
            InitializeComponent();
            editGenre = (Genre)info.Clone();
        }
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            
            DialogResult = true;
            
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = editGenre;
        }
    }
}
