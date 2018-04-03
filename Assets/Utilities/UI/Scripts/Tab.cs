using UnityEngine;
using UnityEngine.UI;

namespace uGameCore.Utilities.UI {
	
	public class Tab : MonoBehaviour {

		public	TabView	tabView;
		public	RectTransform	button ;
		public	RectTransform	panel ;



		void Start()
		{
			// add button click listener which will change active tab
			this.button.GetComponentInChildren<Button>().onClick.AddListener( () => { this.tabView.SwitchTab(this); } );

		}

	}

}
