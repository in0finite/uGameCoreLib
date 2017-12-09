using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace uGameCore.Commands {

	public class WindowCommands : MonoBehaviour {


		void Start () {

			string[] commands = new string[] { "msgbox", "modal_msgbox" };

			foreach (var cmd in commands) {
				CommandManager.RegisterCommand( cmd, ProcessCommand );
			}

		}

		string ProcessCommand( string command ) {

			//	string invalidSyntaxString = "Invalid syntax.";

			string[] words = command.Split( " ".ToCharArray() );
			int numWords = words.Length ;
		//	string restOfTheCommand = command.Substring (command.IndexOf (' ') + 1);

			string response = "";

		//	var networkManager = UnityEngine.Networking.NetworkManager.singleton;

			if (words [0] == "msgbox") {

				Menu.Windows.WindowManager.OpenMessageBox( "some example text", false );

			} else if (words [0] == "modal_msgbox") {

				Menu.Windows.WindowManager.OpenMessageBox( "this is modal message box", true );

			}

			return response;
		}


	}

}
