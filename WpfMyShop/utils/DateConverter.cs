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
    public class DateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is DateTime dateTime)
            {
                // Chuyển đổi DateTime thành chuỗi với định dạng "yyyy-MM-dd"
                return dateTime.ToString("yyyy-MM-dd");
            }
            return DependencyProperty.UnsetValue; // Trả về DependencyProperty.UnsetValue nếu giá trị không hợp lệ.
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
