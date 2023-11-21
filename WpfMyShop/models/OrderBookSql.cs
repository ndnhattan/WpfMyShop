using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMyShop.models
{
    //Dung de lam report finance,product,topSellingProduct
    public class OrderBookSql : INotifyPropertyChanged, ICloneable
    {
        public string Name { get; set; }   
        public string Image { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
        public int Cost { get; set; }
        public int Price {  get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
