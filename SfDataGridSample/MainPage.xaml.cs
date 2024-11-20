using Syncfusion.Maui.DataGrid;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace SfDataGridSample
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }


        public bool FilterRecords(object record)
        {
            var sampleData = record as SampleData;
            if (sampleData == null)
                return false;

            var namesToFilter = new List<string> { "Abinesh", "Balu", "Carry", "Dighi", "Gigne" };
            return namesToFilter.Contains(sampleData.Name);

        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            this.dataGrid.View.Filter = FilterRecords;
            this.dataGrid.View.RefreshFilter();
        }

        public interface InterfaceB
        {
            int TestProperty { get; set; }
        }

        public interface InterfaceA : InterfaceB
        {
            string Name { get; set; }
        }
        public class SampleData : InterfaceA
        {
            public int TestProperty { get; set; }
            public string Name { get; set; }
        }
    }
}


