using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace WpfMyShop.utils
{
    public class PriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int price = (int)value;
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");  // en-US /en-UK

            string result = price.ToString("#,### đ", cul.NumberFormat);
            return result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string priceString = value as string;
            if (string.IsNullOrEmpty(priceString))
            {
                return DependencyProperty.UnsetValue; // Trả về giá trị không hợp lệ nếu chuỗi rỗng hoặc null
            }

            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            try
            {
                // Loại bỏ ký tự "đ" và khoảng trắng sau đó thực hiện chuyển đổi
                priceString = priceString.Replace("đ", "").Trim();
                decimal price = decimal.Parse(priceString, NumberStyles.Currency, cul);
                int priceInt = (int)price;
                return priceInt;
            }
            catch (Exception)
            {
                return DependencyProperty.UnsetValue; // Trả về giá trị không hợp lệ nếu có lỗi trong quá trình chuyển đổi
            }
        }
    }
}
