using System.Collections.Generic;
using UnityEngine;

namespace uGameCore.Commands {

	using CommandCallback = System.Func<string, string> ;

	public class CommandManager : MonoBehaviour {
		
		static	Dictionary<string, CommandCallback>	registeredCommands = new Dictionary<string, CommandCallback>();


		void Awake() {

			RegisterCommand( "help", ProcessHelpCommand );

		}

		void Start () {
			
		}

		public	static	void	RegisterCommand( string command, CommandCallback callback ) {

			if (registeredCommands.ContainsKey (command))
				return;

			registeredCommands.Add (command, callback);

		}

		public	static	string[]	SplitCommandIntoArguments( string command ) {

			return command.Split (new string[]{ " ", "\t" }, System.StringSplitOptions.RemoveEmptyEntries);

		}

		public	static	int		ProcessCommand( string command, ref string response ) {

			response = "Unknown command: " + command;

			string[] words = SplitCommandIntoArguments (command);
			if (0 == words.Length)
				return -1;
			
			CommandCallback callback = null;
			if( registeredCommands.TryGetValue( words[0], out callback ) ) {
				response = callback (command);
				return 0;
			}

			return -1;
		}

		static	string	ProcessHelpCommand( string command ) {

			string response = "List of available commands:\n";

			foreach (var pair in registeredCommands) {
				response += pair.Key + "\n";
			}

			response += "\n";

			return response;
		}
		
		public	static	void	SendCommandToAllPlayers( string command, bool sendResponse ) {

			foreach (var p in PlayerManager.players) {
				p.RpcExecuteCommandOnClient( command, sendResponse );
			}

		}

	}

}
