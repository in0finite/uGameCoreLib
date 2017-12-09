using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uGameCore.Commands {
	
	public class DefaultCommands : MonoBehaviour {


		void Start () {
			
			string[] commands = new string[] { "camera_disable", "uptime", "server_cmd", "client_cmd", "kick", "kick_instantly",
				"exit"};

			foreach (var cmd in commands) {
				CommandManager.RegisterCommand( cmd, ProcessCommand );
			}

		}
		
		string ProcessCommand( string command ) {

			string invalidSyntaxString = "Invalid syntax.";

			string[] words = command.Split( " ".ToCharArray() );
			int numWords = words.Length ;
			string restOfTheCommand = command.Substring (command.IndexOf (' ') + 1);

			string response = "";

		//	var networkManager = UnityEngine.Networking.NetworkManager.singleton;


			if (2 == numWords && words [0] == "camera_disable") {

				int cameraDisable = int.Parse (words [1]);

				if (Camera.main != null) {

					if (0 == cameraDisable) {
						Camera.main.enabled = true;
					} else if (1 == cameraDisable) {
						Camera.main.enabled = false;
					} else {
						response += "Invalid value.";
					}

				}

			} else if (words [0] == "uptime") {

				int minutes = Mathf.FloorToInt (Time.realtimeSinceStartup / 60);
				int seconds = Mathf.FloorToInt (Time.realtimeSinceStartup) % 60;
				response += minutes + ":" + seconds;

			} else if (words [0] == "server_cmd") {

				if (NetworkStatus.IsClientConnected ()) {
					if (numWords < 2)
						response += invalidSyntaxString;
					else
						Player.local.ExecuteCommandOnServer (restOfTheCommand);
				}

			} else if (words [0] == "client_cmd") {

				if (NetworkStatus.IsServerStarted ()) {
					if (numWords < 2)
						response += invalidSyntaxString;
					else
						CommandManager.SendCommandToAllPlayers (restOfTheCommand, true);
				}

//			} else if (words [0] == "view_log") {
//
//				if (2 == numWords) {
//					int numChars = 0;
//					if (int.TryParse (words [1], out numChars)) {
//						int length = this.logString.Length;
//						if (numChars >= length) {
//							response += this.logString;
//						} else {
//							response += this.logString.Substring (length - numChars);
//						}
//					}
//				} else {
//					response += this.logString;
//				}
//
//			} else if (words [0] == "clear") {
//
//				this.ClearLog ();
//
			} else if (words [0] == "kick") {

				if (NetworkStatus.IsServerStarted ()) {
					var p = PlayerManager.GetPlayerByName (restOfTheCommand);
					if (null == p) {
						response += "There is no such player connected.";
					} else {
						p.DisconnectPlayer ( 3, "You are kicked from server.");
					}

				} else {
					response += "Only server can use this command.";
				}

			} else if (words [0] == "kick_instantly") {

				if (NetworkStatus.IsServerStarted ()) {
					var p = PlayerManager.GetPlayerByName (restOfTheCommand);
					if (null == p) {
						response += "There is no such player connected.";
					} else {
						p.DisconnectPlayer ( 0, "");
					}

				} else {
					response += "Only server can use this command.";
				}

			}

			else if (words [0] == "bot_add") {

				if (NetworkStatus.IsServerStarted ()) {

					//					Player player = this.networkManager.AddBot ();
					//					if (player != null)
					//						response += "Added bot: " + player.playerName;
					//					else
					//						response += "Failed to add bot.";
					//

					/*	GameObject go = GameObject.Instantiate( this.playerObjectPrefab );
					if( go != null ) {
						go.GetComponent<NavMeshAgent>().enabled = true ;

						FPS_Character script = go.GetComponent<FPS_Character>();
						script.isBot = true ;
							//	script.playerName = this.networkManager.CheckPlayerNameAndChangeItIfItExists( "bot" );
						// find random waypoints
						GameObject[] waypoints = GameObject.FindGameObjectsWithTag( "Waypoint" );
						if( waypoints.Length > 0 ) {
							int index1 = Random.Range( 0, waypoints.Length );
							int index2 = Random.Range( 0, waypoints.Length );
							if( index1 == index2 ) {
								index2 ++ ;
								if( index2 >= waypoints.Length )
									index2 = 0 ;
							}

							script.botWaypoints.Add( waypoints[index1].transform );
							script.botWaypoints.Add( waypoints[index2].transform );

							script.botCurrentWaypointIndex = 0;
						}

					//	Player player = this.networkManager.AddLocalPlayer( go );

						// the above function assigns name
					//	script.playerName = player.playerName ;

						NetworkServer.Spawn( go );

						script.respawnOnStart = true;
					//	script.Respawn();

						response += "Added bot." ;

					} else {
						response += "Can't create object for bot." ;
					}
				*/

				} else {
					response += "Only server can use this command.";
				}

			} else if (words [0] == "bot_add_multiple") {

				//				if (this.networkManager.IsServer () && NetworkStatus.IsServerStarted ()) {
				//
				//					int numBotsToAdd = 0;
				//					if (2 == numWords && int.TryParse (words [1], out numBotsToAdd)) {
				//
				//						int numBotsAdded = 0;
				//						for (int i = 0; i < numBotsToAdd; i++) {
				//							Player player = this.networkManager.AddBot ();
				//							if (player != null)
				//								numBotsAdded++;
				//						}
				//
				//						response += "Added " + numBotsAdded + " bots.";
				//
				//					} else {
				//						response += invalidSyntaxString;
				//					}
				//				}

			} else if (words [0] == "remove_all_bots") {

				if (NetworkStatus.IsServerStarted ()) {
					int count = 0;
					foreach (var p in PlayerManager.players) {
						if (p.IsBot ()) {
							p.DisconnectPlayer (0, "");
							count++;
						}
					}
					response += "Removed " + count + " bots.";
				}

			} else if (words [0] == "help") {

				// print all available commands

//				response += "List of available commands:\n" ;
//				response += "msgbox" + "\n" ;
//				response += "modal_msgbox" + "\n" ;
//				if( NetworkStatus.IsServerStarted () ) {
//					response += "kick" + "\n" ;
//					response += "kick_instantly" + "\n" ;
//					response += "team_change" + "\n" ;
//					response += "endround" + "\n" ;
//					response += "bot_add" + "\n" ;
//					response += "bot_add_multiple" + "\n" ;
//					response += "remove_all_bots" + "\n";
//				}
//				response += "list_maps" + "\n" ;
//				if( NetworkStatus.IsServerStarted () ) {
//					response += "timeleft" + "\n" ;
//					response += "nextmap" + "\n" ;
//					response += "change_scene" + "\n" ;
//				}
//				response += "camera_disable" + "\n" ;
//				response += "say" + "\n" ;
//				response += "uptime" + "\n" ;
//				response += "clear" + "\n" ;
//				response += "exit" + "\n" ;

			} else if (words [0] == "exit") {

				GameManager.singleton.ExitApplication();

			} else {
				
			//	response += "Unknown command: " + words[0] + "." ;

			}

			return response ;

		}

	}

}
