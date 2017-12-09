using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace uGameCore.Menu.Windows {
	
	public class WindowManager : MonoBehaviour {
		

		public	class WindowInfo
		{
			public	int		id = -1 ;
			public	bool	isModal = false ;
			public	string	title = "" ;
			public	Rect	rect = new Rect ();
			public	List<string>	displayStrings = new List<string> ();
			public	bool	isClosed = false ;
			public	System.Action<WindowInfo>	procedure = null;
			public	GameObject	gameObject = null;
		}


		public	static	WindowManager	singleton { get ; private set ; }

		static	int	lastId = 0 ;
		static	List<WindowInfo>	m_openedWindows = new List<WindowInfo> ();
		public	static	IEnumerable<WindowInfo>	OpenedWindows { get { return m_openedWindows; } }

		private	Canvas	windowsCanvas = null;

		public	GameObject	windowPrefab = null;
		public	GameObject	displayStringPrefab = null;
		public	GameObject	buttonPrefab = null;

		public	Vector2	msgBoxSize = new Vector2( 0.25f, 0.2f );



		void Awake () {
			
			singleton = this;

			// find canvas
			this.windowsCanvas = Utilities.Utilities.FindObjectOfTypeOrLogError<WindowsCanvas>().GetComponent<Canvas>();

		}

		void Update () {

			// remove closed windows from list
			m_openedWindows.RemoveAll( wi => wi.isClosed );


		}

		void OnGUI() {


//			// display modal windows
//			bool alreadyShownModalWindow = false;
//			foreach (WindowInfo wi in openedWindows) {
//
//				if (wi.isClosed)
//					continue;
//
//				if( wi.isModal ) {
//					if( ! alreadyShownModalWindow ) {
//						GUI.ModalWindow( wi.id, wi.rect, WindowFunction, wi.title );
//						GUI.FocusWindow( wi.id );
//						alreadyShownModalWindow = true ;
//					} else {
//						// display it as regular window
//						GUI.Window( wi.id, wi.rect, WindowFunction, wi.title );
//					}
//				}
//
//			}
//
//			// display non modal windows
//			foreach (WindowInfo wi in openedWindows) {
//
//				if (wi.isClosed)
//					continue;
//
//				if( ! wi.isModal ) {
//					GUI.Window( wi.id, wi.rect, WindowFunction, wi.title );
//				}
//
//			}


		}


		static	void	WindowFunction (int windowID) {

			// find window info by it's id
			WindowInfo wi = null ;
			foreach( WindowInfo windowInfo in m_openedWindows ) {
				if (windowInfo.id == windowID) {
					wi = windowInfo;
					break;
				}
			}

			if( null == wi )
				return ;

			//	GUILayout.BeginArea (wi.rect);

			// call window procedure
			if (wi.procedure != null) {
				wi.procedure.Invoke (wi);
			}

			//	GUILayout.EndArea ();

			GUI.DragWindow();

		}

		private	static	void	MessageBoxProcedure( WindowInfo wi ) {

			// message box with close button

			if (wi.displayStrings.Count > 0) {
				GUILayout.Label (wi.displayStrings [0]);
			}

			GUILayout.FlexibleSpace ();

			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace ();
			if (GameManager.DrawButtonWithCalculatedSize("Close")) {
				CloseWindow (wi);
			}
			GUILayout.EndHorizontal ();

		}

		public	static	void	CloseWindow( WindowInfo wi ) {

			wi.isClosed = true;

			if (wi.gameObject != null) {
				Destroy (wi.gameObject);
			}

		//	this.UpdateMouseLockAndVisibilityState ();

		}

		private	static	int		GetNewWindowId() {

			int id = ++lastId;
			return id;

		}

		public	static	Rect	GetCenteredRect( float screenWidthPercentage, float screenHeightPercentage ) {

			float width = Screen.width * screenWidthPercentage;
			float height = Screen.height * screenHeightPercentage;
			float x = Screen.width / 2 - width / 2;
			float y = Screen.height / 2 - height / 2;
			return new Rect (x, y, width, height);

		}

		public	static	void	ReduceScrollViewHeightNormalized( WindowInfo window, float amountToReduce ) {

			var scrollView = window.gameObject.GetComponentInChildren<ScrollRect> ();
			if (null == scrollView)
				return;

			var rt = scrollView.GetComponent<RectTransform> ();

			Vector2 min = rt.anchorMin;
			min.y += amountToReduce;
			rt.anchorMin = min;

			rt.CornersToAnchors ();

		}

		public	static	WindowInfo	OpenMessageBox( string text, bool isModal ) {

//			int width = Screen.width / 4;
//			int height = width * 9 / 16;
//			int x = Screen.width / 2 - width / 2 + UnityEngine.Random.Range (-50, 50);
//			int y = Screen.height / 2 - height / 2 + UnityEngine.Random.Range (-50, 50);
			Rect rect = GetCenteredRect (singleton.msgBoxSize.x, singleton.msgBoxSize.y);
			rect.position += UnityEngine.Random.insideUnitCircle * 100;

			Action<string,GameObject> processButton = (s, button) => {
				var el = button.AddComponent<Utilities.StretchToParentLayoutElement>();
				el.width = 1.0f;
				el.height = 0.8f;
				el.stretchElement = button.GetComponentInParent<ScrollRect>().transform as RectTransform ;
			};
			var window = OpenWindow( rect, "", new string[] { text }, isModal, processButton, MessageBoxProcedure );

			// set alignment of scroll view to center
//			var layout = window.gameObject.GetComponentInChildren<VerticalLayoutGroup>();
//			layout.childAlignment = TextAnchor.MiddleCenter ;
//			// rebuild layout
//			LayoutRebuilder.MarkLayoutForRebuild (layout.GetComponent<RectTransform> ());

			// add close button
			var closeButton = singleton.buttonPrefab.InstantiateAsUIElement( window.gameObject.transform );
			closeButton.GetComponentInChildren<Text> ().text = "Close";
			closeButton.GetComponentInChildren<Button> ().onClick.AddListener (() => CloseWindow (window));
			// set it's position
		//	width = (int) rect.width / 3 ;
		//	height = width * 9 / 16 ;
		//	Rect msgBoxRect = new Rect( rect.center.x - width / 2, rect.yMax - 5 - height / 2, width, height);
		//	closeButton.GetComponent<RectTransform>().SetRectAndAdjustAnchors( msgBoxRect );
			closeButton.GetComponent<RectTransform> ().SetNormalizedRectAndAdjustAnchors (new Rect (0.35f, 0.05f, 0.3f, 0.15f));

			// reduce height of scroll view
			ReduceScrollViewHeightNormalized( window, 0.25f );

			return window;
		}

		public	static	WindowInfo	OpenWindow( Rect rect, string title, IEnumerable<string> displayStrings, bool isModal,
			Action<string, GameObject> onDisplayStringCreated, Action<WindowInfo> windowProcedure ) {

			WindowInfo wi = new WindowInfo ();
			wi.title = title;
			wi.displayStrings = new List<string> (displayStrings);
			wi.isModal = isModal;
			wi.rect = new Rect (rect);
			wi.procedure = windowProcedure;


//			if (wi.isModal) {
//				GUI.FocusControl( "" );
//				GUI.UnfocusWindow ();
//			}

			// create window game object
			var go = singleton.windowPrefab.InstantiateAsUIElement( singleton.windowsCanvas.transform );
			wi.gameObject = go;

			var rectTransform = go.GetComponent<RectTransform>();

			// add click handler which will bring windows canvas to top
		//	go.AddComponent<Utilities.UIEventsPickup>().onPointerClick += (arg) => { WindowManager.singleton.windowsCanvas.sortingOrder = int.MaxValue; };

			// position and size
			rectTransform.SetRectAndAdjustAnchors( rect );

			// title
			rectTransform.FindChild("Title").GetComponentInChildren<Text>().text = title ;

			// populate scroll view with display strings
			var scrollViewContent = rectTransform.GetComponentInChildren<ScrollRect>().content ;
			foreach(var s in displayStrings) {
				
				var displayStringObject = Instantiate (singleton.displayStringPrefab);
				displayStringObject.transform.SetParent (scrollViewContent.transform, false);

				displayStringObject.GetComponentInChildren<Text> ().text = s;

				if (onDisplayStringCreated != null)
					onDisplayStringCreated (s, displayStringObject);
			}


			m_openedWindows.Add (wi);

			wi.id = GetNewWindowId();

			return wi;
		}

		public	static	WindowInfo	OpenWindow( Rect rect, string title, IEnumerable<string> displayStrings, bool isModal ) {

			return OpenWindow (rect, title, displayStrings, isModal, null, null);

		}


	}

}
