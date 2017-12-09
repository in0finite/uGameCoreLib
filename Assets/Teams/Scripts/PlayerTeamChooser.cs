using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace uGameCore {
	
	public class PlayerTeamChooser : NetworkBehaviour {

		private	Player	m_player = null ;

		private	bool	m_shouldChooseTeam = false;
		private	bool	m_shouldSendChooseTeamMessage = false;

		[SyncVar]	private	string	m_team = "" ;
		public	string Team { get { return m_team; } protected set { m_team = value; } }

		private	bool	m_isSpectating = true ;
		public	bool	IsSpectating { get { return this.m_isSpectating; } }

		public	bool	resetTeamOnSceneChange = true ;

		public	static	event System.Action<string[]>	onReceivedChooseTeamMessage = delegate {};



		void Awake () {
			m_player = GetComponent<Player> ();
		}
		
		// Update is called once per frame
		void Update () {

			if (!this.isServer)
				return;
			

			if (m_shouldSendChooseTeamMessage) {
				// we should send him message saying that he can choose team

				if (!SceneChanger.isLoadingScene && m_player.conn.isReady) {

					m_shouldSendChooseTeamMessage = false;

					m_shouldChooseTeam = true;

					Debug.Log ("Sending choose team message to " + m_player.playerName);

					string[] teams = null;
					if (TeamManager.IsFreeForAllModeOn()) {
						// free for all mode ?

						teams = new string[]{ "Play" };
					} else {
						
						teams = TeamManager.singleton.teams.ToArray ();
					}

					this.RpcChooseTeam (teams);
				}

			}

		}


		/// <summary>
		/// If the current team is different, destroys playing object and changes the player team.
		/// </summary>
		public	void	ChangeTeam( string newTeam ) {

			if (!this.isServer)
				return;

			if (!m_player.IsLoggedIn ())
				return;

			if (m_team != newTeam) {
				
				m_player.DestroyPlayingObject ();

				m_team = newTeam;
			}

		}


		void OnLoggedIn() {

			m_shouldSendChooseTeamMessage = true;

		}

		void OnSceneChanged( SceneChangedInfo info ) {

			if (!this.isServer)
				return;

			if (this.resetTeamOnSceneChange) {
				m_team = "";
				m_isSpectating = true;
				m_shouldChooseTeam = false;
				m_shouldSendChooseTeamMessage = true;
			}

		}

		public	void	SendChooseTeamMessage( string[] teams ) {

			this.RpcChooseTeam (teams);

		}

		[ClientRpc]
		private	void	RpcChooseTeam( string[] teams ) {
			// Server tells us that we can choose team.

			if (!isLocalPlayer) {
				return;
			}

			Debug.Log ("Received choose team message.");

			this.gameObject.BroadcastMessageNoExceptions ("OnReceivedChooseTeamMessage", (object) teams);

			onReceivedChooseTeamMessage (teams);

		}

		public	void	TeamChoosed( string teamName ) {

			this.CmdTeamChoosed (teamName);

		}

		[Command]
		private	void	CmdTeamChoosed( string teamName ) {

			#if SERVER


			if(!m_shouldChooseTeam) {
				return ;
			}

			m_shouldChooseTeam = false ;


			if("Play" == teamName) {
				if(!TeamManager.IsFreeForAllModeOn()) {
					// invalid team
					// send him message again
					m_shouldSendChooseTeamMessage = true ;
					return;

				} else {
					this.Team = "" ;
					m_isSpectating = false ;
				}
			} else {

				if (!TeamManager.singleton.teams.Contains (teamName)) {
					// invalid team
					// send him message again
					m_shouldSendChooseTeamMessage = true ;
					return;
				}

				m_team = teamName;
			//	m_isSpectating = TeamManager.IsTeamSpectatingTeam( teamName );
				m_isSpectating = false ;
			}

			Debug.Log (m_player.playerName + " choosed team: " + teamName);

			this.BroadcastChoosedTeamMessage( teamName );

			#endif

		}

		private	void	BroadcastChoosedTeamMessage( string teamName ) {

			this.gameObject.BroadcastMessageNoExceptions("OnPlayerChoosedTeam", teamName);

		}


	}

}
