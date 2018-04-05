using UnityEngine;
using UnityEngine.UI;

namespace uGameCore.Utilities.UI {
	
	public class Tab : MonoBehaviour {

		public	TabView	tabView;
		public	RectTransform	button ;
		public	RectTransform	panel ;

		private	Color	m_originalButtonColor = Color.white;
		public Color originalButtonColor { get { return this.m_originalButtonColor; } }

		/// <summary> Image component attached to button. </summary>
		public Image buttonImageComponent { get { if(this.button) return this.button.GetComponent<Image>(); else return null; } }

		public Text buttonTextComponent { get { if(this.button) return this.button.GetComponentInChildren<Text>(); else return null; } }

		public Button buttonComponent { get { if(this.button) return this.button.GetComponent<Button>(); else return null; } }

		public	string	tabButtonText {
			get {
				if (this.buttonTextComponent)
					return this.buttonTextComponent.text;
				else
					return null;
			}
		}



		void Awake()
		{
			
		}

		void Start()
		{
			// add button click listener which will change active tab
			this.buttonComponent.onClick.AddListener( () => { this.tabView.SwitchTab(this); } );

			// remember normal color of button
			m_originalButtonColor = this.buttonImageComponent.color;

		}

	}

}
