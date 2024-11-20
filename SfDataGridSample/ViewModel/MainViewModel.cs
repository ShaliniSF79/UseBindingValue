using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SfDataGridSample
{
    public class MainViewModel
    {
        public ObservableCollection<InterfaceA> Items { get; set; }
        public ObservableCollection<InterfaceA> FilteredItems { get; set; }

        public MainViewModel()
        {
            Items = new ObservableCollection<InterfaceA>
        {
            new SampleData { TestProperty = 1, Name = "Abinesh" },
            new SampleData { TestProperty = 2, Name = "Balu" },
            new SampleData { TestProperty = 3, Name = "Carry" },
            new SampleData { TestProperty = 4, Name = "Dighi" },
            new SampleData { TestProperty = 5, Name = "Gigne" },
            new SampleData { TestProperty = 6, Name = "Lieji" },
            new SampleData { TestProperty = 7, Name = "Oopi" },
            new SampleData { TestProperty = 8, Name = "shai" },
            new SampleData { TestProperty = 9, Name = "Xiosn" },
            new SampleData { TestProperty = 10, Name = "john" },
            new SampleData { TestProperty = 11, Name = "kelly" },
            new SampleData { TestProperty = 12, Name = "rand" },
            new SampleData { TestProperty = 13, Name = "yojo" },
            new SampleData { TestProperty = 14, Name = "singf" },
            new SampleData { TestProperty = 15, Name = "yalu" },
            new SampleData { TestProperty = 16, Name = "indu" },
            new SampleData { TestProperty = 17, Name = "jill" },
            new SampleData { TestProperty = 18, Name = "Bkizdlu" },
            new SampleData { TestProperty = 19, Name = "kisa" },
        };
        }
    }
}

