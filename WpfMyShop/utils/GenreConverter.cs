using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using WpfMyShop.model;

namespace WpfMyShop.utils
{
    public class GenreConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int id = (int)value;
            string genre = "";
            string sql = """
                select name from Genres where id = @id;
             """;
            var command = new SqlCommand(sql, DB.Instance.Connection);
            command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = id;
            genre = (String)command.ExecuteScalar();
            return genre;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var sql = "select isnull((select id from Genres where name = @genre),0)";
            var command = new SqlCommand(sql, DB.Instance.Connection);
            command.Parameters.Add("@genre", System.Data.SqlDbType.NVarChar).Value = value.ToString();
            int id = (int)((Int32)command.ExecuteScalar());
            return id;
        }
    }
}
