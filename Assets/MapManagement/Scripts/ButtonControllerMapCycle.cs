using UnityEngine;

namespace uGameCore.MapManagement {
	
	public class ButtonControllerMapCycle : MonoBehaviour {


		public	void	ChangeMap() {

			MapCycle.singleton.ChangeMapToNextMap ();

		}

		public	void	StartServerWithSpecifiedMap( UnityEngine.UI.Dropdown dropdown ) {

			if (dropdown.value < 0)
				return;

			string sceneName = dropdown.options [dropdown.value].text;

			int index = MapCycle.singleton.mapCycleList.IndexOf (sceneName);
			if (index < 0)
				return;

			UnityEngine.Networking.NetworkManager.singleton.onlineScene = sceneName;

			MapCycle.singleton.SetCurrentMapIndex (index);

			UnityEngine.Networking.NetworkManager.singleton.StartHost ();
		}

	}

}
