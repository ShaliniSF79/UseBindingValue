using Syncfusion.Maui.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace SfDataGridSample
{
	public class EmployeeViewModel : IDisposable, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		private void NotifyPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}


		public EmployeeViewModel()
		{
			employees = new ObservableCollection<DictionaryModel>();
		}

		private ObservableCollection<DictionaryModel> employees;

		public ObservableCollection<DictionaryModel> Employees
		{
			get
			{
				return employees;
			}
			set
			{
				this.employees = value;
				NotifyPropertyChanged(nameof(Employees));
			}
		}

		Random r = new Random();
		Dictionary<string, string> loginID = new Dictionary<string, string>();
		Dictionary<string, string> gender = new Dictionary<string, string>();


		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool isdisposable)
		{
			if (Employees != null)
			{
				Employees.Clear();
			}
		}
	}
}
