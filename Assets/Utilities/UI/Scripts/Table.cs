using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using uGameCore;
using System.Linq;

namespace uGameCore.Utilities.UI {
	
	public class Table : MonoBehaviour {


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

			public	float	GetWidth( Table table ) {
				if (this.widthType == ColumnWidthType.Absolute) {
					return this.absoluteWidth;
				} else if (this.widthType == ColumnWidthType.Percentage) {
					return table.rectTransform.rect.width * this.widthPercentage;
				}
				return 0f;
			}

			public Column( Column other ) {
				return this.MemberwiseClone ();
			}

		}


		private	RectTransform	rectTransform { get { return this.GetComponent<RectTransform>(); } }

		/// <summary>
		/// Place where all table rows are put in.
		/// </summary>
		public	RectTransform	Container { get { return this.rectTransform; } }

		[SerializeField]	private	List<TableRow>	m_rows = new List<TableRow>();

		[SerializeField]	TableRow	m_headerRow = null;

		/// <summary>
		/// Gets all rows in content's first-level children.
		/// </summary>
		public List<TableRow> GetRowsInChildren () {
			var rows = new List<TableRow> (this.Container.transform.childCount);
			foreach (Transform child in this.Container.transform) {
				var row = child.GetComponent<TableRow> ();
				if (row != null)
					rows.Add (row);
			}
			return rows;
		}

		public	List<TableRow>	GetAllRows() {
			return m_rows;
		}

		public	int	RowsCount { get { return m_rows.Count; } }

		public	List<Column>	columns = new List<Column> (0);

		public	GameObject	tableEntryPrefab = null;

	//	public	GameObject	horizontalLayoutGroupPrefab = null;
		public	GameObject	tableRowPrefab = null;

		public	GameObject	columnHeaderEntryPrefab = null;

		public	int rowHeight = 30;


		public	event	Action<TableEntry>	onEntryCreated = delegate {};
		public	event	Action	onColumnHeadersCreated = delegate {};




		public	IEnumerable<TableEntry>	GetAllEntriesInColumn( string columnName ) {

			int columnIndex = this.columns.FindIndex (c => c.columnName == columnName);
			if (columnIndex < 0)
				yield break;

			foreach (var row in m_rows.WhereAlive()) {
				yield return row.Entries [columnIndex];
			}

		}

		public	float	GetTotalColumnsWidth() {
			float width = 0f;
			for (int i = 0; i < this.columns.Count; i++) {
				width += this.columns [i].GetWidth (this);
			}
			return width;
		}

		public	virtual	float	GetRowTopCoordinate( TableRow row, int rowIndex ) {

			if (row.IsHeaderRow)
				return 0f;

			if (m_headerRow) {
				// leave some space for headers
				return this.rowHeight * (rowIndex + 1);
			}

			return this.rowHeight * rowIndex;
		}


		/// <summary>
		/// Updates all rows.
		/// </summary>
		public	void	UpdateTable () {

			for (int i = 0; i < m_rows.Count; i++) {
				this.UpdateRow (m_rows [i], i);
			}

			if (m_headerRow) {
				this.UpdateRow (m_headerRow, 0);
			}

		}

		/// <summary>
		/// Updates all entries in a row.
		/// </summary>
		public	void	UpdateRow (TableRow row, int rowIndex) {

			row.Entries.RemoveAllDeadObjects ();
			MySetDirty (row);

			// create entries if they are not created
			int numEntriesToCreate = this.columns.Count - row.Entries.Count ;
			for (int i = 0; i < numEntriesToCreate; i++) {
				this.CreateEntry( row );
			}

			// TODO: delete extra entries, or better yet, rearrange columns if needed


			// set dimensions of row
			float top = this.GetRowTopCoordinate( row, rowIndex );
			float bottom = top - this.rowHeight;
			row.GetRectTransform ().SetRectAndAdjustAnchors (new Rect (0, bottom, this.GetTotalColumnsWidth (),
				top - bottom));
			MySetDirty (row.GetRectTransform ());

			// update entries
			float leftCoordinate = 0f;
			for (int i = 0; i < row.Entries.Count; i++) {
				UpdateTableEntry (row, rowIndex, i, leftCoordinate);
				leftCoordinate += this.columns [i].GetWidth (this);
			}

		}

		public	void	UpdateRow (TableRow row) {

			if (null == row)
				return;

			if (m_headerRow == row) {
				this.UpdateRow (row, 0);
				return;
			}

			int index = m_rows.IndexOf (row);
			if (index < 0)
				return;

			this.UpdateRow (row, index);

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
		/// Creates entry out of prefab.
		/// </summary>
		public	virtual	TableEntry	CreateEntry ( TableRow row ) {
			
			var go = this.tableEntryPrefab.InstantiateAsUIElement (row.transform);

			var entry = go.AddComponentIfDoesntExist<TableEntry> ();
			entry.tableRow = row;

			// add it to list of entries
			row.Entries.Add (entry);

			MySetDirty (row.transform);
			MySetDirty (go);
			MySetDirty (entry);
			MySetDirty (row);

			return entry;
		}

		private	void	SetEntryPositionAndSize (RectTransform rt, Column column) {



		}

		/// <summary>
		/// Updates position, size and name of table entry.
		/// </summary>
		public	virtual	void	UpdateTableEntry ( TableRow row, int rowIndex, int columnIndex, float leftCoordinate ) {

			var entry = row.Entries [columnIndex];
			var column = this.columns [columnIndex];

			// set it's position and dimensions
			float entryWidth = column.GetWidth (this);
			float top = this.GetRowTopCoordinate (row, rowIndex);
			float bottom = top - this.rowHeight ;
			entry.GetRectTransform().SetRectAndAdjustAnchors( new Rect(leftCoordinate, bottom, entryWidth, this.rowHeight) );

			MySetDirty (entry.GetRectTransform ());

			// set game object's name
			if (entry.gameObject.name != column.columnName) {
				entry.gameObject.name = column.columnName;
				MySetDirty (entry.gameObject);
			}

			if (row.IsHeaderRow) {
				// set entry's text
				var textComponent = entry.textComponent;
				if (textComponent) {
					textComponent.text = column.columnName;
					MySetDirty (textComponent);
				}
			}

		}

		/// <summary>
		/// Retreives text from attached Text component.
		/// </summary>
		public	string	GetEntryText ( TableRow row, int columnIndex ) {

			var textComponent = row.entries [columnIndex].textComponent;

			if (textComponent != null)
				return textComponent.text;

			return "";
		}

		public	TableEntry	GetEntry( int rowIndex, int columnIndex ) {

			return this.GetAllRows () [rowIndex].Entries [columnIndex];

		}


//		public	void	SetColumns( List<Column> newColumns ) {
//
//			if (newColumns == this.columns)
//				return;
//
//			foreach (var newColumn in newColumns) {
//
//				// find this column by name
//				int columnIndex = this.columns.FindIndex( c => c.columnName == newColumn.columnName );
//
//				if (columnIndex < 0) {
//					// column with this name doesn't exist
//					// insert it
//
//				} else {
//					// column with this name exists
//					// replace it to match index
//
//				}
//
//			}
//
//		}

		/// <summary>
		/// Sets width of each column based on it's text. It does not update table.
		/// </summary>
		public	void	SetColumnWidthsBasedOnText() {

			if (null == m_headerRow)
				return;

			for (int i = 0; i < this.columns.Count; i++) {
				var textComponent = m_headerRow.Entries [i].textComponent;

				// multiply column width by ratio between row height and preffered height => this will maintain aspect ratio of text component
				float columnWidth = textComponent.preferredWidth * this.rowHeight / (float) textComponent.preferredHeight ;

				this.columns [i].widthType = ColumnWidthType.Absolute;
				this.columns [i].absoluteWidth = columnWidth;
			}

			MySetDirty (this);
		}


		/// <summary>
		/// Adds new row to the table.
		/// </summary>
		public	TableRow	AddRow () {
			
			TableRow row = this.CreateRow ();

			m_rows.Add (row);

			MySetDirty (this);

			return row;
		}

		public	void	RemoveRow (int rowIndex) {

			var rows = this.GetAllRows ();

			var row = rows [rowIndex];

			DestroyRow (row);

		//	m_rows.RemoveAt (rowIndex);

		}

		private	void	DestroyRow (TableRow row) {

			// destroy row's game object - it will also destroy all it's entries
			MyDestroy( row.gameObject );

			MySetDirty (this.Container.transform);

		}

		/// <summary>
		/// Removes all rows from table.
		/// </summary>
		public	void	Clear () {

			foreach (var row in m_rows.WhereAlive ()) {
				DestroyRow (row);
			}

			m_rows.Clear ();

			MySetDirty (this);
		}

		public	void	EnsureNumberOfRows (int numberOfRows)
		{
			m_rows.RemoveAllDeadObjects ();

			int numRowsToAdd = numberOfRows - this.GetAllRows ().Count;

			for (int i = 0; i < numRowsToAdd; i++) {
				this.AddRow ();
			}

			MySetDirty (this);

		}

		/// <summary>
		/// Adds or removes rows from table, so that new number of rows is equal to specified value.
		/// </summary>
		public	void	SetNumberOfRows (int numberOfRows)
		{
			m_rows.RemoveAllDeadObjects ();

			var rows = m_rows;

			int numToDelete = rows.Count - numberOfRows ;
			int numToAdd = - numToDelete ;

			for (int i = 0; i < numToDelete; i++) {
				DestroyRow (rows [rows.Count - 1 - i]);
			}

			for (int i = 0; i < numToAdd; i++) {
				this.AddRow ();
			}

			MySetDirty (this);
		}


		/// <summary>
		/// Creates one row in a table. It creates entries for each column.
		/// </summary>
		protected	virtual	TableRow	CreateRow () {

			GameObject rowGameObject = this.tableRowPrefab.InstantiateAsUIElement (this.Container.transform);
			TableRow row = rowGameObject.AddComponentIfDoesntExist<TableRow> ();
			row.table = this;


			// create entries
			for (int i = 0; i < this.columns.Count; i++) {
				
				var column = this.columns [i];

				TableEntry entry = this.CreateEntry (row);

				var entryGameObject = entry.gameObject;

				entryGameObject.name = column.columnName;

				// add layout element if it doesn't exist
			//	if (null == entryGameObject.GetComponentInChildren<ILayoutElement> ())
			//		entryGameObject.AddComponent<LayoutElement> ();
				

				MySetDirty (entryGameObject);
				MySetDirty (entryGameObject.GetRectTransform ());


				// invoke event
				onEntryCreated (entry);
			}


			MySetDirty (this.Container.transform);
			MySetDirty (rowGameObject);
			MySetDirty (row);
			MySetDirty (row.GetRectTransform ());


			return row;
		}


		/// <summary>
		/// Creates header row if it doesn't exist.
		/// </summary>
		public	virtual	void	CreateHeader () {
			
			if (null == m_headerRow) {
				// headers row doesn't exist
				// create it

				m_headerRow = this.CreateRow ();
				m_headerRow.isHeaderRow = true;

				m_headerRow.gameObject.name = "Table headers";

				MySetDirty (m_headerRow);
				MySetDirty (m_headerRow.gameObject);

				onColumnHeadersCreated ();
			}

		}

		/// <summary>
		/// Gets the row which represents table header.
		/// </summary>
		public	TableRow	GetHeaderRow () {

			return m_headerRow;

		}

		public	void	DestroyHeader () {
			
			if (m_headerRow != null) {
				MyDestroy (m_headerRow.gameObject);
				MySetDirty (this.Container.transform);
				m_headerRow = null;
			}

		}


		protected	static	void	MyDestroy( UnityEngine.Object obj ) {

			if (Application.isEditor && !Application.isPlaying) {
				// edit mode => we have to destroy objects using DestroyImmediate
				DestroyImmediate (obj, false);
			} else {
				Destroy (obj);
			}

		}

		protected	internal	static	void	MySetDirty( UnityEngine.Object obj ) {

			if (Application.isEditor && !Application.isPlaying) {
				Utilities.MarkObjectAsDirty (obj);
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
			if (null == this.GetHeaderRow ()) {
				this.CreateHeader ();
				this.MarkForRebuild ();
			}

		}


	}

}
