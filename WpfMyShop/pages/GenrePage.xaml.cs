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
using Microsoft.Data.SqlClient;
using WpfMyShop.model;
using WpfMyShop.models;
using WpfMyShop.utils;
using WpfMyShop.windows;

namespace WpfMyShop.pages
{
    /// <summary>
    /// Interaction logic for GenrePage.xaml
    /// </summary>
    public partial class GenrePage : Page
    {
        public static bool isAddFail = false;
        public GenrePage()
        {
            InitializeComponent();
        }
        private void bookComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
        public BindingList<Genre> _genres;
        private void loadAllGenres()
        {
            {
                var sql = """
                select * from Genres
                """;
                var command = new SqlCommand(sql, DB.Instance.Connection);

                int count = -1;
                _genres = new BindingList<Genre>();
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
                genreListView.ItemsSource = _genres;
            }
        }


        

        private void btn_Accept_Click(object sender, RoutedEventArgs e)
        {
            if (textBoxGenre.Text != null)
            {
                isAddFail = true;
            }
            if (isAddFail = true)
            {
                var newGenre = new Genre()
                {
                    Name = textBoxGenre.Text,
                    Description = textBoxGenreDescription.Text
                };

                var sql = """
                insert into Genres (name,description)
                values(@Name,@Description)
                select ident_current('Genres');
                """;
                var command = new SqlCommand(sql, DB.Instance.Connection);
                command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar).Value = newGenre.Name;
                command.Parameters.Add("@Description", System.Data.SqlDbType.NVarChar).Value = newGenre.Description;
                int id = (int)((decimal)command.ExecuteScalar());
                if (id > 0)
                {
                    newGenre.Id = id;
                    _genres.Add(newGenre);
                    //MessageBox.Show("Thêm thành công");
                }
                else
                {
                    MessageBox.Show("Add failed");
                }
                genreListView.ItemsSource = _genres;
            }

        }

        private void btn_DeleteGenre_Click(object sender, RoutedEventArgs e)
        {
            var genre = (Genre) genreListView.SelectedItem;
            var sql = """
                delete from Genres
                where name = @Name;5
                """;
            var command = new SqlCommand(sql, DB.Instance.Connection);
            command.Parameters.Add("@Id", System.Data.SqlDbType.Int).Value = genre.Id;
            command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar).Value = genre.Name;
            //command.Parameters.Add("@Description", System.Data.SqlDbType.NVarChar).Value = genre.Description;
            command.ExecuteNonQuery();
            _genres.Remove(genre);
        }
        Genre _backup;
        private void btn_EditGenre_Click(object sender, RoutedEventArgs e)
        {
            var genre = (Genre)genreListView.SelectedItem;
            _backup = (Genre)genre.Clone();
            var screen = new EditGenreWindow(genre);
            if (screen.ShowDialog()!.Value == true)
            {
                screen.editGenre.CopyProperties(genre);
                var sql = """
                update Genres
                set name=@Name
                where name = @oldName;
                """;
                var command = new SqlCommand(sql, DB.Instance.Connection);
                //command.Parameters.Add("@Id", System.Data.SqlDbType.Int).Value = editGenre.Id;
                command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar).Value = genre.Name;
                command.Parameters.Add("@oldName", System.Data.SqlDbType.NVarChar).Value = _backup.Name;
                command.Parameters.Add("@Description", System.Data.SqlDbType.NVarChar).Value = genre.Description;
                command.ExecuteNonQuery();
            }
            else
            {
                _backup.CopyProperties(genre);
            }

        }

        private void GenrePage_loaded(object sender, RoutedEventArgs e)
        {
            loadAllGenres();
        }
    }
}
