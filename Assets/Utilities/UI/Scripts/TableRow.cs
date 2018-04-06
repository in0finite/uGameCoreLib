using System.Collections.Generic;
using UnityEngine;

namespace uGameCore.Utilities.UI {

	public class TableRow : MonoBehaviour {

		[HideInInspector]	[SerializeField]	internal	Table	table = null;
		public Table Table { get { return this.table; } }

		[HideInInspector]	[SerializeField]	internal	List<TableEntry> entries = new List<TableEntry>();
		public List<TableEntry> Entries { get { return this.entries; } }

	//	public	object	value = null;

		[HideInInspector]	[SerializeField]	internal	bool	isHeaderRow = false;
		public bool IsHeaderRow { get { return this.isHeaderRow; } }


		public	TableEntry	FindEntryByColumnName( string columnName ) {

			// find column with this name
			int columnIndex = this.Table.columns.FindIndex( c => c.columnName == columnName );
			if (columnIndex < 0)
				return null;

			return this.Entries [columnIndex];
		}

	}

}
