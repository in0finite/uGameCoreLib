using UnityEngine;

namespace uGameCore {

	public class RoundSystem2Chat : MonoBehaviour {


		void OnRoundFinished( string winningTeam ) {

			Chat.ChatManager.SendChatMessageToAllPlayersAsServer( "Round ended - winning team: " + winningTeam + "." );

		}

	}

}
