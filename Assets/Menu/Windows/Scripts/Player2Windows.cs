using UnityEngine;

namespace uGameCore.Menu.Windows {
	
	public class Player2Windows : MonoBehaviour {


		void OnDisconnectedByServer( string description ) {

			WindowManager.OpenMessageBox (description, true);

		}

	}

}
