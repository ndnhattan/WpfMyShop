using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfMyShop.model;
using WpfMyShop.models;
using static System.Reflection.Metadata.BlobBuilder;

namespace WpfMyShop.pages
{
    /// <summary>
    /// Interaction logic for SettingPage.xaml
    /// </summary>
    public partial class SettingPage : System.Windows.Controls.Page
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
            }
            catch (Exception ex)
            {
                MessageBox.Show("Invalid data");
            }
        }

        private void importBtn_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            dlg.DefaultExt = ".xlsx";
            dlg.Filter = "Excel Files (*.xlsx)|*.xlsx";


            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;

                var document = SpreadsheetDocument.Open(filename, false);
                var wbPart = document.WorkbookPart!;
                var sheets = wbPart.Workbook.Descendants<Sheet>()!;

                var sheet = sheets.FirstOrDefault(s => s.Name == "Category");
                var wsPart = (WorksheetPart)(wbPart!.GetPartById(sheet.Id!));
                var stringTable = wbPart
                    .GetPartsOfType<SharedStringTablePart>()
                    .FirstOrDefault()!;
                var cells = wsPart.Worksheet.Descendants<Cell>();

                int row = 3;
                Cell idCell;

                do
                {
                    idCell = cells.FirstOrDefault(
                    c => c?.CellReference == $"B{row}"
                    )!;

                    if (idCell?.InnerText.Length > 0)
                    {
                        string id = idCell.InnerText;
                        Cell nameCell = cells.FirstOrDefault(
                            c => c?.CellReference == $"C{row}"
                        )!;
                        string stringId = nameCell!.InnerText;
                        string name = stringTable.SharedStringTable
                                .ElementAt(int.Parse(stringId))
                                .InnerText;
                        Cell descCell = cells.FirstOrDefault(
                            c => c?.CellReference == $"D{row}"
                        )!;
                        string description = null;
                        if (descCell?.InnerText.Length > 0)
                        {
                            string descId = descCell!.InnerText;
                            description = stringTable.SharedStringTable
                                    .ElementAt(int.Parse(descId))
                                    .InnerText;
                        }

                        insertGenre(name, description);
                    }
                    row++;
                } while (idCell?.InnerText.Length > 0);

                var sheet2 = sheets.FirstOrDefault(s => s.Name == "Products");
                var wsPart2 = (WorksheetPart)(wbPart!.GetPartById(sheet2.Id!));
                var stringTable2 = wbPart
                    .GetPartsOfType<SharedStringTablePart>()
                    .FirstOrDefault()!;
                var cells2 = wsPart2.Worksheet.Descendants<Cell>();

                int row2 = 3;
                Cell idCell2;

                do
                {
                    idCell2 = cells2.FirstOrDefault(
                    c => c?.CellReference == $"B{row2}"
                    )!;

                    if (idCell2?.InnerText.Length > 0)
                    {
                        string id2 = idCell2.InnerText;
                        Cell bookNameCell = cells2.FirstOrDefault(
                            c => c?.CellReference == $"C{row2}"
                        )!;
                        string nameId = bookNameCell!.InnerText;
                        string bookName = stringTable2.SharedStringTable
                                .ElementAt(int.Parse(nameId))
                                .InnerText;
                        Cell authorCell = cells2.FirstOrDefault(
                            c => c?.CellReference == $"D{row2}"
                        )!;
                        string authorId = authorCell!.InnerText;
                        string author = stringTable2.SharedStringTable
                                .ElementAt(int.Parse(authorId))
                                .InnerText;
                        Cell costCell = cells2.FirstOrDefault(
                            c => c?.CellReference == $"E{row2}"
                        )!;
                        int cost = int.Parse(costCell!.InnerText);
                        Cell priceCell = cells2.FirstOrDefault(
                            c => c?.CellReference == $"F{row2}"
                        )!;
                        int price = int.Parse(priceCell!.InnerText);
                        Cell yearCell = cells2.FirstOrDefault(
                            c => c?.CellReference == $"G{row2}"
                        )!;
                        int year = int.Parse(yearCell!.InnerText);
                        Cell stockCell = cells2.FirstOrDefault(
                            c => c?.CellReference == $"H{row2}"
                        )!;
                        int stock = int.Parse(stockCell!.InnerText);
                        Cell genreCell = cells2.FirstOrDefault(
                            c => c?.CellReference == $"I{row2}"
                        )!;
                        int? genre = null;
                        if (genreCell?.InnerText.Length > 0)
                        {
                            
                            genre = int.Parse(genreCell!.InnerText);
                        }
                        Cell promoPriceCell = cells2.FirstOrDefault(
                            c => c?.CellReference == $"J{row2}"
                        )!;
                        int promoPrice = int.Parse(promoPriceCell!.InnerText);
                        Cell imageUrlCell = cells2.FirstOrDefault(
                            c => c?.CellReference == $"K{row2}"
                        )!;
                        string imageUrlId = imageUrlCell!.InnerText;
                        string imageUrl = stringTable2.SharedStringTable
                                .ElementAt(int.Parse(imageUrlId))
                                .InnerText;
                        Cell soldCell = cells2.FirstOrDefault(
                            c => c?.CellReference == $"L{row2}"
                        )!;
                        int sold = int.Parse(soldCell!.InnerText);

                        insertBook(bookName, author, cost, price, year, stock, genre, promoPrice, imageUrl, sold);
                    }
                    row2++;
                } while (idCell2?.InnerText.Length > 0);
            }
        }

        private void insertBook(string bookName, string author, int cost, int price, int year, int stock, int? genre, int promoPrice, string imageUrl, int sold)
        {
            var book = new Book()
            {
                Name = bookName,
                Image = imageUrl,
                Author = author,
                Year = year,
                Price = price,
                PromoPrice = promoPrice,
                Sold = sold,
                Cost = cost,
                Stock = stock
            };

            string sql = $"""
                insert into Books(name, author, year, image_url, cost, price, stock{(genre != null ? ", genre_id" : "")}, promo_price, sold) 
                values(@Name, @Author, @Year, @Image, @Cost, @Price, @Stock{(genre != null ? ", @Genre_id" : "")}, @PromoPrice, @Sold);
                select ident_current('Books');
             """;

            int id = 0;
            try
            {
                var command = new SqlCommand(sql, DB.Instance.Connection);
                command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar).Value = book.Name;
                command.Parameters.Add("@Author", System.Data.SqlDbType.NVarChar).Value = book.Author;
                command.Parameters.Add("@Year", System.Data.SqlDbType.Int).Value = book.Year;
                command.Parameters.Add("@Image", System.Data.SqlDbType.NVarChar).Value = book.Image;
                command.Parameters.Add("@Cost", System.Data.SqlDbType.Int).Value = book.Cost;
                command.Parameters.Add("@Price", System.Data.SqlDbType.Int).Value = book.Price;
                command.Parameters.Add("@Stock", System.Data.SqlDbType.Int).Value = book.Stock;
                if (genre != null)
                {
                    command.Parameters.Add("@Genre_id", System.Data.SqlDbType.Int).Value = genre;
                }
                command.Parameters.Add("@PromoPrice", System.Data.SqlDbType.Int).Value = book.PromoPrice;
                command.Parameters.Add("@Sold", System.Data.SqlDbType.Int).Value = book.Sold;

                id = (int)((decimal)command.ExecuteScalar());
            }
            catch (Exception ex) { }

            if (id > 0)
            {
                book.Id = id;
            }
            else
            {
                MessageBox.Show("Excel file structure is wrong!");
            }
        }

        private void insertGenre(string name, string? description)
        {
            var newGenre = new Genre()
            {
                Name = name,
                Description = description
            };

            var sql = $"""
                insert into Genres (name{(newGenre.Description != null ? ",description" : "")})
                values(@Name{(newGenre.Description != null ? ",@Description" : "")})
                select ident_current('Genres');
                """;
            var command = new SqlCommand(sql, DB.Instance.Connection);
            command.Parameters.Add("@Name", System.Data.SqlDbType.NVarChar).Value = newGenre.Name;
            if (newGenre.Description != null)
            {
                command.Parameters.Add("@Description", System.Data.SqlDbType.NVarChar).Value = newGenre.Description;
            }
            int id = (int)((decimal)command.ExecuteScalar());
            if (id > 0)
            {
                newGenre.Id = id;
            }
            else
            {
                MessageBox.Show("Excel file structure is wrong!");
            }
        }
    }
}
