using System.Text.Json.Serialization;

namespace SfDataGridSample
{
	internal class QueryResult
	{
		public long SchemaVersion { get; set; }

		/// <summary>
		/// Gets or sets the header row for the table.
		/// </summary>
		public IEnumerable<TableColumn>? Header { get; set; }

		/// <summary>
		/// Gets or sets the data rows in the table.
		/// </summary>
		public List<TableRow>? Data { get; set; }
	}
}
