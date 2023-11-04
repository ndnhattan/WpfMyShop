using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMyShop.model;

namespace WpfMyShop.models
{
    public class OrderBook : INotifyPropertyChanged, ICloneable
    {
        public int Id_Book { get; set; }
        //public int Id_Order { get; set; }
        //public BindingList<Book> ListBook { get; set; }
        //public BindingList<int> ListBookQuantity { get; set; }
        public String Name { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public int Cost { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
