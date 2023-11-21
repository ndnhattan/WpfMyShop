using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WpfMyShop.model;

namespace WpfMyShop.models
{
    public class Order : INotifyPropertyChanged, ICloneable
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int CustomerId { get; set; }
        public int Cost { get; set; }
        public int Price { get; set; }
        public string CustomerName { get; set; }
        public Discount Discount { get; set; }
        public BindingList<OrderBook> ListOrderBook { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
