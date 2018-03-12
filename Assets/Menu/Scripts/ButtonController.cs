using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace uGameCore.Menu {

	/// <summary>
	/// Controls button text component for better visual effects. It detects when button is hovered or clicked,
	/// and changes it's text font size and color, and plays sound accordingly. It also provides methods for menu and
	/// network integration.
	/// </summary>
	public class ButtonController : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
	{
		
		public	bool	modifyFontSize = false;
	//	[UnityEngine.Serialization.FormerlySerializedAs("startingFontSize")]
		public	int	regularFontSize = 15;
		public	int	highlightedFontSize = 18;

	//	[UnityEngine.Serialization.FormerlySerializedAs("modifyFontColor")]
		public	bool	modifyTextColor = false;
	//	[UnityEngine.Serialization.FormerlySerializedAs("regularColor")]
		public	Color	regularTextColor = Color.white;
	//	[UnityEngine.Serialization.FormerlySerializedAs("highlightedColor")]
		public	Color	highlightedTextColor = Color.blue;

		public	bool	playHoverSound = false;
		public	AudioClip	hoverSound = null;

		public	bool	playClickSound = false;
		public	AudioClip	clickSound = null;

		private	Text	m_buttonTextComponent = null;



		void Awake () {

			m_buttonTextComponent = GetComponentInChildren<Text> ();

		}

		void Start () {

			if (modifyFontSize) {
				m_buttonTextComponent.fontSize = regularFontSize;
			}

			if (modifyTextColor) {
				m_buttonTextComponent.color = regularTextColor;
			}

		}


		public void OnPointerDown (PointerEventData eventdata)
		{

			if (playClickSound) {
				var audioSource = GetComponent<AudioSource> ();
				audioSource.clip = clickSound;
				audioSource.Play ();
			}

		}

		public void OnPointerEnter (PointerEventData eventdata)
		{
			
			if (modifyFontSize) {
				m_buttonTextComponent.fontSize = highlightedFontSize;
			}

			if (modifyTextColor) {
				m_buttonTextComponent.color = highlightedTextColor;
			}

			if (playHoverSound) {
				var audioSource = GetComponent<AudioSource> ();
				audioSource.clip = hoverSound ;
				audioSource.Play ();
			}

		}

		public void OnPointerExit (PointerEventData eventdata)
		{

			if (modifyFontSize) {
				m_buttonTextComponent.fontSize = regularFontSize;
			}

			if (modifyTextColor) {
				m_buttonTextComponent.color = regularTextColor ;
			}

		}


		public	void	StartServer() {
			NetManager.StartHost (NetManager.defaultListenPortNumber);
		}

		public	void	StartServerWithSpecifiedOptions( RectTransform menuObject ) {

			MenuManager.singleton.StartServerWithSpecifiedOptions (true);
		}

		public	void	ConnectToServerWithParameters() {

			MenuManager.singleton.ConnectToServerWithParameters ();

		}


		public	void	ExitGame() {
			GameManager.singleton.ExitApplication ();
		}

		public	void	QuitToMenu() {
			MenuManager.singleton.QuitToMenu ();
		}

		public	void	ReturnToGame() {
			MenuManager.singleton.UnPause ();
		}

		public	void	Resign() {
			MenuManager.singleton.Resign ();
		}

		public	void	OpenMenu( string menuName ) {
			MenuManager.singleton.Open (menuName);
		}

		public	void	OpenMenuAndSetItsParentToCurrentMenu( string menu ) {

			var canvas = MenuManager.FindCanvasByName (menu);
			if (null == canvas)
				return;

			canvas.GetComponent<Menu> ().parentMenu = MenuManager.singleton.openedMenuName;

			MenuManager.singleton.Open (menu);
		}

		public	void	GoToParent() {

			MenuManager.singleton.OpenParentMenu ();

		}

	}

}
