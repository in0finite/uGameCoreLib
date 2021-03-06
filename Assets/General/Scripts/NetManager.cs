﻿using UnityEngine;
using UnityEngine.Networking;

namespace uGameCore {
	
	public class NetManager
	{

		public	static	int	defaultListenPortNumber { get { return 7777; } }

		public	static	int	listenPortNumber { get { return NetworkServer.listenPort; } }

		public	static	string	onlineScene {
			get {
				return NetworkManager.singleton.onlineScene;
			}
			set {
				NetworkManager.singleton.onlineScene = value;
			}
		}



		public	static	void	StartServer( int portNumber ) {

			CheckIfNetworkIsStarted ();
			CheckIfPortIsValid (portNumber);
			CheckIfOnlineSceneIsAssigned ();
			SetupNetworkManger( "", portNumber );
			NetworkManager.singleton.StartServer ();

		}

		public	static	void	StartHost( int portNumber ) {

			CheckIfNetworkIsStarted ();
			CheckIfPortIsValid (portNumber);
			CheckIfOnlineSceneIsAssigned ();
			SetupNetworkManger( "", portNumber );
			NetworkManager.singleton.StartHost ();

		}

		public	static	void	StopServer() {

			NetworkManager.singleton.StopServer ();

		}

		public	static	void	StopHost() {

			NetworkManager.singleton.StopHost ();

		}

		public	static	void	StartClient( string ip, int serverPortNumber ) {

			CheckIfNetworkIsStarted ();
			CheckIfIPAddressIsValid (ip);
			CheckIfPortIsValid (serverPortNumber);
			SetupNetworkManger( ip, serverPortNumber );
			NetworkManager.singleton.StartClient ();

		}

		public	static	void	StopClient() {

			NetworkManager.singleton.StopClient ();

		}

		/// <summary>
		/// Stops both server and client.
		/// </summary>
		public	static	void	StopNetwork() {

		//	NetworkManager.singleton.StopHost ();
			NetworkManager.singleton.StopServer ();
			NetworkManager.singleton.StopClient ();

		}


		public	static	void	CheckIfServerIsStarted() {

			if (NetworkStatus.IsServerStarted ())
				throw new System.Exception ("Server already started");
			
		}

		public	static	void	CheckIfClientIsStarted() {

			if (!NetworkStatus.IsClientDisconnected ())
				throw new System.Exception ("Client already started");

		}

		public	static	void	CheckIfNetworkIsStarted() {

			CheckIfServerIsStarted ();
			CheckIfClientIsStarted ();

		}

		public	static	void	CheckIfPortIsValid( int portNumber ) {

			if (portNumber < 1 || portNumber > 65535)
				throw new System.ArgumentOutOfRangeException ( "portNumber", "Invalid port number");

		}

		private	static	void	CheckIfIPAddressIsValid( string ip ) {

			if (string.IsNullOrEmpty (ip))
				throw new System.ArgumentException ("IP address empty");

		//	System.Net.IPAddress.Parse ();

		}

		private	static	void	CheckIfOnlineSceneIsAssigned() {

			if (string.IsNullOrEmpty (NetManager.onlineScene))
				throw new System.Exception ("Online scene is not assigned");

		}


		private	static	void	SetupNetworkManger( string ip, int port ) {

			NetworkManager.singleton.networkAddress = ip;
			NetworkManager.singleton.networkPort = port;

		}


		public	static	NetworkClient	GetClient() {

			return NetworkManager.singleton.client;

		}


	}

}
