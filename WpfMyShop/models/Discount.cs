using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfMyShop.models
{
    public class Discount : INotifyPropertyChanged, ICloneable
    {
        public int Id { get; set; }
        public int Value { get; set; }
        public int Condition { get; set; }
        public string Currency {  get; set; }
        public string Name { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
