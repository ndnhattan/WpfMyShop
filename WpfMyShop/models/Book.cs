using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMyShop.model
{
    public class Book : INotifyPropertyChanged, ICloneable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Author { get; set; }
        public int Year { get; set; }
        public int Cost { get; set; }
        public int Price { get; set; }
        public int PromoPrice { get; set; }
        public bool IsPromo { get; set; }
        public int Stock { get; set; }
        public int Sold { get; set; }
        public int GenreId { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
