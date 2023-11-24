using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.Data.SqlClient;
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
using System.Windows.Shapes;
using WpfMyShop.model;
using WpfMyShop.models;
using static System.Reflection.Metadata.BlobBuilder;

namespace WpfMyShop.windows
{
    /// <summary>
    /// Interaction logic for EditDiscountWindow.xaml
    /// </summary>
    public partial class EditDiscountWindow : Window
    {
        public Discount EditedDiscount { get; set; }
        BindingList<Discount> discounts;
        public EditDiscountWindow(Discount discount, BindingList<Discount> discounts)
        {
            InitializeComponent();
            this.EditedDiscount = discount;
            this.discounts = discounts;
        }

        private void confirmBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int value = int.Parse(TextboxValue.Text);
                int condition = int.Parse(TextboxCondition.Text);
            } catch(Exception ex)
            {
                MessageBox.Show("Invalid data");
                return;
            }

            if (!TextboxCurrency.Text.Equals("%") && !TextboxCurrency.Text.Equals("VNĐ")) 
            {
                MessageBox.Show("Invalid data");
                return;
            }

            var sql = """
                update Discount set name = @Name, value = @Value, condition = @Condition, currency = @Currency
                where id = @id
                select @id;
             """;
            var command = new SqlCommand(sql, DB.Instance.Connection);
            command.Parameters.Add("@id", System.Data.SqlDbType.Int).Value = EditedDiscount.Id;
            command.Parameters.Add("@Value", System.Data.SqlDbType.Int).Value = EditedDiscount.Value;
            command.Parameters.Add("@Name", System.Data.SqlDbType.Text).Value = EditedDiscount.Name;
            command.Parameters.Add("@Currency", System.Data.SqlDbType.Text).Value = EditedDiscount.Currency;
            command.Parameters.Add("@Condition", System.Data.SqlDbType.Int).Value = EditedDiscount.Condition;

            int id = 0;
            try
            {
                id = (int)((Int32)command.ExecuteScalar());
                if (id > 0)
                {
                    // find index of book in array
                    int index = discounts.IndexOf(discounts.FirstOrDefault(discount => discount.Id == id));
                    discounts[index] = EditedDiscount;

                    MessageBox.Show("Edit successfully");
                    DialogResult = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message.ToString());
                DialogResult = false;
            }
        }

        private void cancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            DataContext = EditedDiscount;
        }
    }
}
