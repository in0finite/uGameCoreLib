using System.Collections.Generic;
using UnityEngine;

namespace uGameCore.Utilities.UI {

	public class TableRow : MonoBehaviour {

		[HideInInspector]	[SerializeField]	internal	Table	table = null;

		[HideInInspector]	[SerializeField]	internal	List<TableEntry> entries = new List<TableEntry>();
		public List<TableEntry> Entries { get { return this.entries; } }

		public	object	value = null;

		[HideInInspector]	[SerializeField]	internal	bool	isHeaderRow = false;
		public bool IsHeaderRow { get { return this.isHeaderRow; } }


	}

}
