using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Networking;
using uGameCore;

namespace uGameCore.Menu {

	public class MenuManager : MonoBehaviour 
	{

	//	public bool isOpened { get ; protected set ; }


		public string goBackButton = "Cancel";

		public string openedMenuName { get ; protected set ; }

		public	string	startupMenuName = "";
		public	string	inGameMenuName = "";
		public	string	pauseMenuName = "" ;
		public	string	gameOverMenuName = "";

	//	public	GameObject	eventSystemObject = null ;

		public	static	MenuManager	singleton { get ; private set ; }


		void Awake() {

			if (null == singleton) {
				singleton = this;
			}

		}

		void Start ()
		{

			if (null == EventSystem.current) {
				Debug.LogWarning ("EventSystem not found");
			}

			this.Open (this.startupMenuName);

		}

		void OnSceneChanged( SceneChangedInfo info ) {

			if (IsInGameScene ()) {
				this.Close ();
			} else {
				this.Open (startupMenuName);
			}

		}

		/// <summary>
		/// Are we in a game scene, or in the startup scene ?
		/// </summary>
		public	bool	IsInGameScene() {

			Scene scene = SceneManager.GetActiveScene ();

			if (!scene.isLoaded)
				return false;

			return scene.buildIndex != 0 && scene.buildIndex != 1 ;
		}

		public	static	bool	IsInGameMenu() {

			return singleton.openedMenuName == singleton.inGameMenuName ;

		}

		public virtual void QuitToMenu() {

			NetManager.StopNetwork ();

		}

		public	virtual	void	Open(string menu) {

			this.openedMenuName = menu;
			//	this.isOpened = true;
			// enable event system
			//	this.isInputEnabled = true ;

			RemoveFocus ();

		}

		protected	virtual	void	Close() {

			this.openedMenuName = this.inGameMenuName;

			//	this.isOpened = false;

			// disable event system
			//	this.isInputEnabled = false ;

			RemoveFocus ();

		}

		public	virtual	void	Pause() {

			this.Open (this.pauseMenuName);
		}

		public	virtual	void	UnPause() {

			this.Close ();
		}

		public	virtual	void	Resign() {

			this.Open (this.gameOverMenuName);

		}

		/// <summary> Opens parent of the current menu, if it exists. </summary>
		public	void	OpenParentMenu() {

			var currentMenu = FindCanvasByName (this.openedMenuName);
			if (currentMenu != null) {
				var parentMenu = currentMenu.GetComponent<Menu> ().parentMenu;
				if (parentMenu != "") {
					// switch to parent menu
					this.Open (parentMenu);
				}
			}

		}

		/// <summary>
		/// Removes the focus from the current element.
		/// </summary>
		protected	static	void	RemoveFocus() {

			UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject (null);

		}


		void Update()
		{


			if (Input.GetButtonDown (goBackButton)) {
				
				// open parent menu if it exists
				OpenParentMenu ();

			}


		}


		public	static	Menu	FindMenuByName( string menuName ) {

			Menu foundMenu = System.Array.Find (FindObjectsOfType<Menu> (), m => m.menuName == menuName);
			return foundMenu;

		}

		/// <summary>
		/// Finds the menu by name, and throws exception if it is not found.
		/// </summary>
		public	static	Menu	FindMenuByNameOrThrow( string menuName ) {

			var menu = MenuManager.FindMenuByName (menuName);

			if (null == menu)
				throw new System.Exception ("Failed to find a menu with name: " + menuName);

			return menu;
		}

		/// <summary>
		/// Finds canvas which has the menu component attached with specified name.
		/// </summary>
		public	static	Canvas FindCanvasByName(string menuName) {

			var menu = MenuManager.FindMenuByName (menuName);
			if (null == menu)
				return null;

			return menu.GetComponent<Canvas> ();
		}

		public Transform	FindChildOfMenu(RectTransform menu, string childName) {

			return System.Array.Find (menu.GetComponentsInChildren<Transform> (), t => t.name == childName);
		}

		public Transform	FindChildOfMenu(Canvas menu, string childName) {
			return FindChildOfMenu (menu.GetComponent<RectTransform> (), childName);
		}

		private	string	ReadInputField(RectTransform menu, string childName) {

			return FindChildOfMenu( menu, childName).GetComponent<InputField>().text;

		}


		public	void	StartServerWithSpecifiedOptions( bool asHost ) {
			
			if(asHost)
				NetManager.StartHost (NetManager.defaultListenPortNumber);
			else
				NetManager.StartServer (NetManager.defaultListenPortNumber);
			
		}

		/// <summary>
		/// Tries to connect to server with parameters from UI, and notifies scripts in case of failure.
		/// </summary>
		public	void	ConnectToServerWithParameters() {

			try {

				var menu = MenuManager.FindMenuByNameOrThrow ("JoinGameMenu");

				var rectTransform = menu.GetRectTransform ();

				string ip = ReadInputField( rectTransform, "Ip");
				int port = int.Parse (ReadInputField (rectTransform, "PortNumber"));

				NetManager.StartClient (ip, port);

			} catch( System.Exception ex ) {

				Debug.LogException (ex);

				// notify scripts
				Utilities.Utilities.SendMessageToAllMonoBehaviours ("OnFailedToJoinGame", 
					new Utilities.FailedToJoinGameMessage (ex));
			}

		}

	}

}
