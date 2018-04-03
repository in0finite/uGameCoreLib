using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace uGameCore.Utilities.UI {
	
	public class TabView : MonoBehaviour {
		

	//	private	RectTransform	m_rectTransform = null;
	//	public	RectTransform	rectTransform { get { return m_rectTransform; } private set { m_rectTransform = value; } }
		public	RectTransform	rectTransform { get { return this.GetComponent<RectTransform>(); } }

	//	private	List<Tab>	m_tabs = new List<Tab>();
		
		public List<Tab> Tabs { get {
				var tabs = new List<Tab> (this.transform.childCount / 2);
				foreach (Transform child in this.transform) {
					var tab = child.GetComponent<Tab> ();
					if (tab != null)
						tabs.Add (tab);
				}
				return tabs;
			}
		}

		private	Tab m_activeTab = null;
		public Tab ActiveTab { get { return this.m_activeTab; } }


		public	Func<string, RectTransform>	createTabPanelFunction ;
		public	Func<string, RectTransform>	createTabButtonFunction ;

		public	Action<RectTransform>	setTabButtonPositionFunction ;
		public	Action<RectTransform>	setTabPanelPositionFunction ;

		public	Action<Tab>	activateTabFunction ;
		public	Action<Tab>	deactivateTabFunction ;


		public	event Action<Tab>	onTabAdded = delegate {};

		public	GameObject	tabButtonPrefab = null;
		public	GameObject	tabPanelPrefab = null;

		public	bool	bestFitForButtonText = false;

		public	int		tabHeight = 30 ;

		/// <summary>
		/// Tabs shown in inspector. The actual real tabs are located in child objects.
		/// </summary>
		[SerializeField]	private	List<string>	m_tabsForInspector = new List<string>();



		private	TabView() {

			createTabPanelFunction = CreateTabPanel;
			createTabButtonFunction = CreateTabButton;
			setTabButtonPositionFunction = SetPositionOfTabButton;
			setTabPanelPositionFunction = SetPositionOfTabPanel;
			activateTabFunction = ActivateTab;
			deactivateTabFunction = DeactivateTab;

		}

		public Tab AddTab ( string name ) {
			
			// add tab button as child of this game object
			var tabButton = createTabButtonFunction( name );

			// set position of tab button
			setTabButtonPositionFunction (tabButton);

			// panel will also be the child of this game object
			var panel = createTabPanelFunction (name);

			// set position of panel
			setTabPanelPositionFunction (panel);


			// attach tab script
			Tab tab = tabButton.gameObject.AddComponentIfDoesntExist<Tab> ();
			tab.tabView = this;
			tab.button = tabButton;
			tab.panel = panel;


		//	m_tabs.Add (tab);

			this.onTabAdded (tab);

			return tab;
		}

		public	Vector2		NormalizeCoordinates (Vector2 coord) {

			return new Vector2( coord.x / GetWidth (), coord.y / GetHeight () );

		}

		public	int	GetWidth () {

			return (int) this.rectTransform.rect.width;
		}

		public	int	GetHeight () {

			return (int) this.rectTransform.rect.height;
		}

//		public	int	GetParentWidth () {
//
//			if (null == this.transform.parent)
//				return Screen.width;
//
//			return (int) (this.transform.parent as RectTransform).rect.width;
//		}
//
//		public	int	GetParentHeight () {
//
//			if (null == this.transform.parent)
//				return Screen.height;
//
//			return (int) (this.transform.parent as RectTransform).rect.height;
//		}

		public int	GetWidthOfAllTabs () {

			int width = 0;

			foreach (var tab in this.Tabs) {
			//	width += tab.tab.GetComponentInChildren<Text> ().preferredWidth;
				width += (int) tab.button.rect.width ;
			}

			return width;
		}

		public	RectTransform CreateTabButton ( string tabName ) {

		//	var button = DataBinder.CreateButton (this.transform);

			var button = Instantiate (this.tabButtonPrefab);
			button.transform.SetParent (this.transform, false);

			button.gameObject.name = tabName + " button";

			var textComponent = button.GetComponentInChildren<Text> ();

			// set button text
			textComponent.text = tabName ;

			// set best fit for button text
			if (this.bestFitForButtonText) {
				textComponent.resizeTextForBestFit = true;
				textComponent.horizontalOverflow = HorizontalWrapMode.Overflow;	// disable multiline text
				textComponent.verticalOverflow = VerticalWrapMode.Overflow ;	// fit to width
			}

			return button.GetComponent<RectTransform> ();
		}

		public	void	SetPositionOfTabButton (RectTransform tabButton) {

			var textComponent = tabButton.GetComponentInChildren<Text> ();

			float preferredWidth = textComponent.preferredWidth;
			float preferredHeight = textComponent.preferredHeight;

			int currentWidth = GetWidthOfAllTabs ();

			// multiply button width by ratio between tabs height and preffered height => this will maintain aspect ratio of text component
			float buttonWidth = preferredWidth * this.tabHeight / (float) preferredHeight ;

			tabButton.anchorMin = NormalizeCoordinates (new Vector2 (currentWidth, GetHeight() - this.tabHeight ));
			tabButton.anchorMax = NormalizeCoordinates (new Vector2 (currentWidth + buttonWidth, GetHeight() ));
			tabButton.offsetMin = tabButton.offsetMax = Vector2.zero;

		}

		public	RectTransform CreateTabPanel ( string tabName ) {

		//	GameObject panelGameObject = new GameObject (tabName, typeof(RectTransform), typeof(Image));
			GameObject panelGameObject = Instantiate( this.tabPanelPrefab );

			panelGameObject.name = tabName + " panel";

			panelGameObject.transform.SetParent (this.transform, false);

			return panelGameObject.GetComponent<RectTransform> ();
		}

		public	void	SetPositionOfTabPanel (RectTransform rt) {
			
			rt.anchorMin = Vector2.zero ;
			rt.anchorMax = NormalizeCoordinates (new Vector2 ( GetWidth(), GetHeight () - this.tabHeight ));
			rt.offsetMin = rt.offsetMax = Vector2.zero ;

		}


		private	static	void	MyDestroy( UnityEngine.Object obj ) {

			if (Application.isEditor && !Application.isPlaying) {
				// edit mode => we have to destroy objects using DestroyImmediate
				DestroyImmediate (obj, false);
			} else {
				Destroy (obj);
			}

		}


		public	void	RemoveAllTabs () {

			foreach (var tab in this.Tabs) {
				
				MyDestroy (tab.button.gameObject);
				MyDestroy (tab.panel.gameObject);

			}

		//	m_tabs.Clear ();

		//	m_activeTab = null;
		}


		public	void	SwitchTab (Tab newActiveTab) {

			if (newActiveTab == m_activeTab)
				return;
			
			foreach( var tab in this.Tabs ) {

				if (tab == newActiveTab) {
					// this is the new active tab
					// activate new tab
					activateTabFunction (tab);
				} else {
					// this is not the active tab
					// deactivate it
					deactivateTabFunction (tab);
				}
			}

			m_activeTab = newActiveTab;

		}

		public	static	void	ActivateTab (Tab tab) {
			tab.panel.gameObject.SetActive (true);
		}

		public	static	void	DeactivateTab (Tab tab) {
			tab.panel.gameObject.SetActive (false);
		}


		public	void	ApplyTabsFromInspector () {

			this.RemoveAllTabs ();

			foreach (var tabName in m_tabsForInspector) {

				this.AddTab (tabName);

			}

		}



		void Start () {

			// if there is no active tab, activate first one
			if (null == m_activeTab) {
				var tabs = this.Tabs;
				if (tabs.Count > 0)
					SwitchTab (tabs [0]);
			}

		}


	}

}
