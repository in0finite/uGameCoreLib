using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using uGameCore;

namespace uGameCore.Utilities.UI {
	
	public class Table : MonoBehaviour {


		/// <summary>
		/// Entry in a table.
		/// </summary>
		public class Entry {
			public	GameObject	gameObject = null;
			public	object	cachedValue = null;

		}

		public enum ColumnWidthType {
			Percentage,
			Absolute
		}

		/// <summary>
		/// Contains parameters for a column.
		/// </summary>
		[Serializable]
		public class Column
		{
			public	ColumnWidthType	widthType = ColumnWidthType.Percentage ;
			public	float	widthPercentage = 0.2f;
			public	float	absoluteWidth = 80f ;
			public	string	columnName = "";
			/// <summary> Assign this function if you want to enable sorting for this column. </summary>
			public	Func<TableRow, int, IComparable> compareValueSelector = null ;
		}


		private	RectTransform	rectTransform { get { return this.GetComponent<RectTransform>(); } }

		/// <summary>
		/// Place where all table rows are put in.
		/// </summary>
		public	RectTransform	Content { get { return this.GetComponent<ScrollRect> ().content; } }

	//	private	List<TableRow>	m_rows = new List<TableRow>();

		/// <summary>
		/// To be refactored - use GetAllRows() instead.
		/// </summary>
		public List<TableRow> GetRows { get {
				var rows = new List<TableRow> (this.Content.transform.childCount);
				foreach (Transform child in this.Content.transform) {
					var row = child.GetComponent<TableRow> ();
					if (row != null)
						rows.Add (row);
				}
				return rows;
			}
		}

		public	List<TableRow>	GetAllRows() {
			return this.GetRows;
		}

		public	List<Column>	columns = new List<Column> (0);

		public	GameObject	tableEntryPrefab = null;

		public	GameObject	horizontalLayoutGroupPrefab = null;

		public	GameObject	columnHeaderEntryPrefab = null;

		public	int rowHeight = 30;


		/// <summary> Assign this delegate to create table entry yourself. Arguments are prefab, parent, column index. </summary>
		public	Func<GameObject, Transform, int, Entry>	function_createEntry ;
		public	Action<TableRow, int>	function_updateEntry ;
		public	Func<TableRow, int, string> function_getEntryText ;
	//	public	Func<List<Column>>	function_getColumns = () => new List<Column> (0);

		public	event	Action<Entry>	onEntryCreated = delegate {};
		public	event	Action	onColumnHeadersCreated = delegate {};




		private	Table()
		{

			function_createEntry = CreateEntry ;
			function_updateEntry = UpdateTableEntry ;
			function_getEntryText = GetEntryText ;

		}


		/// <summary>
		/// Updates all rows.
		/// </summary>
		public	void	UpdateTable () {

			foreach (var row in this.GetRows) {

				this.UpdateRow (row);

			}

		}

		/// <summary>
		/// Updates all entries in a row.
		/// </summary>
		public	void	UpdateRow (TableRow row) {

			for (int i = 0; i < row.entries.Count; i++) {
				function_updateEntry (row, i);
			}

		}


		private	void	Rebuild () {
			
			// destroy entries for each row

			// destroy rows

			// destroy headers


			// create headers

			// create rows

			// create entries for each row


		}


		/// <summary>
		/// Default function for creating entry. It creates game object out of prefab.
		/// </summary>
		public	Entry	CreateEntry ( GameObject prefab, Transform parent, int columnIndex ) {
			
			var go = prefab.InstantiateAsUIElement (parent);

			var entry = new Entry ();
			entry.gameObject = go;

			return entry;
		}

		private	void	SetEntryPositionAndSize (RectTransform rt, Column column) {



		}

		/// <summary>
		/// Default function for updating entry. It updates text of attached Text component.
		/// </summary>
		public	void	UpdateTableEntry ( TableRow row, int columnIndex ) {
			
			var entry = row.entries[columnIndex];

			var textComponent = entry.gameObject.GetComponentInChildren<Text> ();

			if (textComponent != null) {
				textComponent.text = function_getEntryText (row, columnIndex);
			}

		}

		/// <summary>
		/// Default function for getting entry text. It retreives text from attached Text component.
		/// </summary>
		public	string	GetEntryText ( TableRow row, int columnIndex ) {

			var textComponent = row.entries [columnIndex].gameObject.GetComponentInChildren<Text> ();

			if (textComponent != null)
				return textComponent.text;

			return "";
		}


		/// <summary>
		/// Adds new row to the table.
		/// </summary>
		public	TableRow		AddRow () {
			
			// create horizontal group
			var go = this.horizontalLayoutGroupPrefab.InstantiateAsUIElement (this.Content);

			// add table row script if it doesn't exist
			var row = go.AddComponentIfDoesntExist<TableRow>();

			this.CreateRow (row.entries, this.tableEntryPrefab, row.transform, row.GetComponent<RectTransform> ());


			return row;
		}

		public	void	RemoveRow (int rowIndex) {

			var rows = this.GetRows;

			var row = rows [rowIndex];

			DestroyRow (row);

		//	m_rows.RemoveAt (rowIndex);

		}

		private	void	DestroyRow (TableRow row) {

			// destroy all entries and horizontal group
			MyDestroy( row.gameObject );

			row.entries.Clear ();

		}

		/// <summary>
		/// Removes all rows from table.
		/// </summary>
		public	void	Clear () {

			foreach (var row in this.GetRows) {
				DestroyRow (row);
			}

		//	m_rows.Clear ();

		}

		public	void	EnsureNumberOfRows (int numberOfRows)
		{

			int numRowsToAdd = numberOfRows - this.GetRows.Count;

			for (int i = 0; i < numRowsToAdd; i++) {
				this.AddRow ();
			}

		}

		/// <summary>
		/// Adds or removes rows from table, so that new number of rows is equal to specified value.
		/// </summary>
		public	void	SetNumberOfRows (int numberOfRows)
		{
			var rows = this.GetRows;

			int numToDelete = rows.Count - numberOfRows ;
			int numToAdd = - numToDelete ;

			for (int i = 0; i < numToDelete; i++) {
				DestroyRow (rows [rows.Count - 1 - i]);
			}

			for (int i = 0; i < numToAdd; i++) {
				this.AddRow ();
			}

		}


		/// <summary>
		/// Creates one row in a table. It creates entries for each column, sets their positions and dimensions, and does some more stuff.
		/// </summary>
		private	void	CreateRow ( List<Entry> rowEntriesList, GameObject entryPrefab, Transform parent, RectTransform horizontalGroup ) {

			float totalColumnWidth = 0;

			for (int i = 0; i < this.columns.Count; i++) {
				
				var column = this.columns [i];

				var entry = function_createEntry (entryPrefab, parent, i);

				var entryGameObject = entry.gameObject;

				entryGameObject.name = column.columnName;

				// add layout element if it doesn't exist
				if (null == entryGameObject.GetComponentInChildren<ILayoutElement> ())
					entryGameObject.AddComponent<LayoutElement> ();

				// set width and height
				if (column.widthType == ColumnWidthType.Absolute) {

					entryGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2( column.absoluteWidth, this.rowHeight ) ;

					totalColumnWidth += column.absoluteWidth ;

				} else if (column.widthType == ColumnWidthType.Percentage) {

					float width = this.rectTransform.rect.width * column.widthPercentage;

					entryGameObject.GetComponent<RectTransform>().sizeDelta = new Vector2( width, this.rowHeight ) ;

					totalColumnWidth += width;
				}

				// add it to list of entries
				rowEntriesList.Add (entry);

				// invoke event
				onEntryCreated (entry);
			}

			// set size of horizontal group game object
			horizontalGroup.sizeDelta = new Vector2( totalColumnWidth, this.rowHeight );


		}


		public	void	CreateHeaders () {

			var headersRow = this.GetHeadersRow ();

			if (null == headersRow) {
				// headers row doesn't exist
				// create it as a game object with horizontal layout

				var headersContainerGameObject = this.horizontalLayoutGroupPrefab.InstantiateAsUIElement (this.Content);
				headersContainerGameObject.name = "Table headers";

				headersRow = headersContainerGameObject.AddComponentIfDoesntExist<TableHeadersRow> ();

				// add content size fitter
			//	headersContainerGameObject.AddComponentIfDoesntExist<ContentSizeFitter> ();

			} else {
				// headers row is already created
				// destroy it's entries

				foreach (var entry in headersRow.entries) {
					MyDestroy (entry.gameObject);
				}
				headersRow.entries.Clear ();
			}

			// create entries

			this.CreateRow (headersRow.entries, this.columnHeaderEntryPrefab, headersRow.transform, headersRow.GetComponent<RectTransform>() );

			// set entries' text
			for (int i = 0; i < headersRow.entries.Count; i++) {
				var textComponent = headersRow.entries [i].gameObject.GetComponentInChildren<Text> ();
				if (textComponent)
					textComponent.text = this.columns [i].columnName;
			}



			onColumnHeadersCreated ();

		}

		/// <summary>
		/// Gets the row which represents table headers.
		/// </summary>
		public	TableHeadersRow	GetHeadersRow () {

			foreach (Transform child in this.Content.transform) {

				var headersRow = child.GetComponent<TableHeadersRow> ();
				if (headersRow != null) {
					// found headers row
					return headersRow;
				}
			}

			return null;
		}

		public	void	DestroyHeaders () {

			var headersRow = GetHeadersRow ();

			if (headersRow != null) {
				MyDestroy (headersRow.gameObject);
			}

		}


		private	static	void	MyDestroy (UnityEngine.Object obj) {

			if (Application.isEditor && !Application.isPlaying) {
				DestroyImmediate (obj, false);
			} else {
				Destroy (obj);
			}

		}


		public	void	MarkForRebuild () {

			LayoutRebuilder.MarkLayoutForRebuild (this.transform as RectTransform);

		}




		void Awake ()
		{


		}

		void Start ()
		{

			// create headers if they are not created
			if (null == this.GetHeadersRow ()) {
				this.CreateHeaders ();
				this.MarkForRebuild ();
			}

		}


	}

}
