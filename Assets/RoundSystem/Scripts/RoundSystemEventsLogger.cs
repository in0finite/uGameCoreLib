using UnityEngine;

namespace uGameCore.RoundManagement {
	
	public class RoundSystemEventsLogger : MonoBehaviour {


		void OnRoundStarted() {

			Debug.Log ("Round started.");

		}

		void OnRoundFinished( string winningTeam ) {

			Debug.Log ("Round ended - winning team: " + winningTeam + ".");

		}

	}

}
