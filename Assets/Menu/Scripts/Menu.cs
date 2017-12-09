using UnityEngine;
using UnityEngine.UI;

namespace uGameCore.Menu {

	public class Menu : MonoBehaviour 
	{

		public	string	menuName = "" ;
		public	string	parentMenu = "" ;

		public	bool	disableInputFieldsWhenClosed = false ;

		Canvas	m_canvas = null;

		private	bool	m_wasOpenedLastFrame = false;


		void Start()
		{
			m_canvas = GetComponent<Canvas> ();

			m_wasOpenedLastFrame = m_canvas.enabled = this.menuName == MenuManager.singleton.openedMenuName;


		}

		void Update() 
		{
			bool isOpenedNow = m_canvas.enabled = this.IsThisMenuOpened() ;

			if (isOpenedNow != m_wasOpenedLastFrame) {
				// notify other components that the state of this menu has changed
				if (isOpenedNow) {
					this.gameObject.BroadcastMessageNoExceptions ("OnMenuOpened");
				} else {
					this.gameObject.BroadcastMessageNoExceptions ("OnMenuClosed");
				}
			}

			m_wasOpenedLastFrame = isOpenedNow;
		}

		protected	virtual	bool	IsThisMenuOpened() {

			return this.menuName == MenuManager.singleton.openedMenuName;

		}


		void OnMenuClosed() {

			if (this.disableInputFieldsWhenClosed) {
				foreach (var inputField in GetComponentsInChildren<InputField>()) {
					inputField.enabled = false;
				}
			}

		}

		void OnMenuOpened() {

			if (this.disableInputFieldsWhenClosed) {
				foreach (var inputField in GetComponentsInChildren<InputField>()) {
					inputField.enabled = true;
				}
			}

		}

	}

}
