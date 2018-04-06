using UnityEngine;
using uGameCore.Utilities.UI;
using System.Linq;
using System.Collections.Generic;

namespace uGameCore {

	/// <summary>
	/// Starts listening on LAN when specified tab in a TabView is activated, and populates table with found servers.
	/// </summary>
	public class LANScan2UI : MonoBehaviour
	{

		public	TabView	tabView = null;
		public	string	tabName = "LAN";

		public	Table	table = null;

		[Range(1, 10)]
		public	float	refreshTime = 4f ;

		private	readonly	string	delayedStopMethodName = "DelayedStop";



		void Start ()
		{
			// register to tab-switched event
			if (this.tabView) {
				this.tabView.onSwitchedTab += this.OnTabSwitched ;
			}

			// register to broadcast received event
			NetBroadcast.onReceivedBroadcast += this.OnReceivedBroadcast ;

		}

		private void StopListeningLater() {
			
			this.CancelInvoke (this.delayedStopMethodName);
			this.Invoke (this.delayedStopMethodName, this.refreshTime);

		}

		private void DelayedStop() {

			if (NetBroadcast.IsListening ())
				NetBroadcast.StopBroadcastingAndListening ();

		}

		void OnTabSwitched() {

			if (this.tabView.ActiveTab && this.tabView.ActiveTab.tabButtonText == this.tabName) {
				// our tab was opened
				if (!NetBroadcast.IsListening ()) {
					// clear table
					if (this.table)
						this.table.Clear ();
					// start listening
					NetBroadcast.StartListening ();
					// stop after some time
					this.StopListeningLater ();
				}
			}

		}

		void OnReceivedBroadcast( NetBroadcast.BroadcastData data ) {

			if (null == this.table)
				return;

			EnsureColumnsMatchBroadcastData( this.table, data );

			// try to find this server in a table
			TableRow rowWithServer = FindRowWithServer( this.table, data );

			if (rowWithServer) {
				// this server already exists in table
				// update it's values

				PopulateTableRow (rowWithServer, data);

			} else {
				// this server doesn't exist
				// add it to table

				rowWithServer = this.table.AddRow ();

				this.table.UpdateRow (rowWithServer);

				PopulateTableRow (rowWithServer, data);
			}

		}


		public	static	void	EnsureColumnsMatchBroadcastData( Table table, NetBroadcast.BroadcastData broadcastData ) {
			
			var newColumnNames = new List<string>(1 + broadcastData.KeyValuePairs.Count);
			newColumnNames.Add ("IP");
			newColumnNames.AddRange (broadcastData.KeyValuePairs.Select (pair => pair.Key));


			bool sameColumns = table.columns.Select (c => c.columnName).SequenceEqual (newColumnNames);

//			for (int i = 0; i < table.columns.Count; i++) {
//				if (table.columns [i].columnName != newColumnNames [i]) {
//					sameColumns = false;
//					break;
//				}
//			}

			if (!sameColumns) {
				// we'll make this easy: rebuild the whole table :D

				// first, save all table data
				var allData = new List<NetBroadcast.BroadcastData> (table.RowsCount);
				foreach (var row in table.GetAllRows().WhereAlive()) {
					allData.Add (GetBroadcastDataFromRow (row));
				}

				// now rebuild the table

				table.Clear();
				table.DestroyHeader ();

				// create new columns
				var newColumns = new List<Table.Column>();
				foreach (var columnName in newColumnNames) {
					var column = new Table.Column ();
					column.columnName = columnName;
					newColumns.Add (column);
				}
				table.columns = newColumns;
				table.CreateHeader ();

				// we need column widths - first, update header, and then obtain width from text component
				table.UpdateRow( table.GetHeaderRow() );
				table.SetColumnWidthsBasedOnText ();

				// restore saved data
				foreach (var rowData in allData) {
					var row = table.AddRow ();
					PopulateTableRow (row, rowData);
				}

				// update table
				table.UpdateTable ();

			}

		}

		public	static	NetBroadcast.BroadcastData	GetBroadcastDataFromRow( TableRow row ) {
			
			string fromAddress = "";
			Dictionary<string, string> dict = new Dictionary<string, string> ();

			TableEntry entryIP = row.FindEntryByColumnName ("IP");
			if (entryIP)
				fromAddress = entryIP.entryText;

			for (int i = 0; i < row.Table.columns.Count; i++) {
				var column = row.Table.columns [i];
				if (column.columnName == "IP")
					continue;
				var entry = row.Entries [i];
				dict.AddOrSet (column.columnName, entry.entryText);
			}


			var broadcastData = new NetBroadcast.BroadcastData (fromAddress, dict);
			return broadcastData;
		}

		public	static	TableRow	FindRowWithServer( Table table, NetBroadcast.BroadcastData broadcastData ) {

			TableEntry entry = table.GetAllEntriesInColumn ("IP").FirstOrDefault( e => e.entryText == broadcastData.FromAddress );
			if (entry)
				return entry.TableRow;
			
			return null;
		}

		public	static	void	PopulateTableRow( TableRow row, NetBroadcast.BroadcastData data ) {
			
			PopulateTableEntry (row, "IP", data.FromAddress);

			foreach (var pair in data.KeyValuePairs) {
				PopulateTableEntry (row, pair.Key, pair.Value);
			}

		}

		public	static	void	PopulateTableEntry( TableRow row, string columnName, string value ) {
			
			TableEntry entry = row.FindEntryByColumnName (columnName);
			if (entry)
				entry.entryText = value;

		}

	}

}
