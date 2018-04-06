using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace uGameCore {

	/// <summary>
	/// Handles broadcasting and listening for games on LAN.
	/// </summary>
	public class NetBroadcast : NetworkDiscovery
	{

		public class BroadcastData {
			private	string	fromAddress = "";
			private	Dictionary<string, string>	keyValuePairs = new Dictionary<string, string> ();
			public BroadcastData (string fromAddress, Dictionary<string, string> keyValuePairs)
			{
				this.fromAddress = fromAddress;
				this.keyValuePairs = keyValuePairs;
			}
			public string FromAddress { get { return this.fromAddress; } }
			public Dictionary<string, string> KeyValuePairs { get { return this.keyValuePairs; } }
		}

		public	static	event System.Action<BroadcastData>	onReceivedBroadcast = delegate {};

		private	static	Dictionary<string, string>	m_dataForBroadcasting = new Dictionary<string, string> ();

		public	static	NetBroadcast	singleton { get ; private set ; }



		void Awake ()
		{
			if (null == singleton)
				singleton = this;

		}

		void Start ()
		{
			if (!this.Initialize ()) {
				Debug.LogError ("Failed to initialize network discovery");
			}

		}

		void OnServerStarted()
		{
			
			StopBroadcastingAndListening ();

			if (!this.StartAsServer ())
				Debug.LogError ("Failed to start broadcasting on LAN");

		}

		void OnServerStopped()
		{

			StopBroadcastingAndListening ();

		}

		void OnClientDisconnected() {


		}

		void Update ()
		{

			if (NetworkStatus.IsServerStarted ()) {
				// update broadcast data:
				// - port
				// - num players
				// - map
				// - map time - depends on Map management

				RegisterDataForBroadcasting ("Port", NetManager.listenPortNumber.ToString ());
				RegisterDataForBroadcasting ("Players", PlayerManager.players.Count ().ToString ());
				RegisterDataForBroadcasting ("Map", NetManager.onlineScene);


				// assign broadcast data
				this.broadcastData = ConvertDictionaryToString (m_dataForBroadcasting);

			}

		}


		public	static	bool	IsListening() {

			return singleton.isClient;
		}

		public	static	void	StartListening() {

			if (singleton.isServer || singleton.isClient)
				return;

			if (!singleton.StartAsClient ()) {
				Debug.LogError ("Failed to start listening on LAN");
			}

		}

		public	static	void	StopBroadcastingAndListening() {

			singleton.StopBroadcast ();

		}


		public override void OnReceivedBroadcast (string fromAddress, string data)
		{

			// don't allow exceptions to be thrown from here
			Utilities.Utilities.RunExceptionSafe (() => {

				// convert data to dictionary
				var dict = ConvertStringToDictionary (data);

				var broadcastData = new BroadcastData (fromAddress, dict);

				// invoke event
				Utilities.Utilities.InvokeEventExceptionSafe (onReceivedBroadcast, broadcastData);
			}
			);

		}


		public	static	void	RegisterDataForBroadcasting( string key, string value ) {

			m_dataForBroadcasting.AddOrSet (key, value);

		}

		public	static	void	UnRegisterDataForBroadcasting( string key ) {

			m_dataForBroadcasting.Remove (key);

		}


		public	static	string	ConvertDictionaryToString( Dictionary<string, string> dict ) {
			
			XElement xElem = new XElement (
				                 "items",
				                 dict.Select (x => new XElement ("item", new XAttribute ("key", x.Key), new XAttribute ("value", x.Value)))
			                 );

			return xElem.ToString ();
		}

		public	static	Dictionary<string, string>	ConvertStringToDictionary( string str ) {

			XElement xElem = XElement.Parse (str);
			var dict = xElem.Descendants ("item")
				.ToDictionary (x => (string)x.Attribute ("key"), x => (string)x.Attribute ("value"));
			return dict;
		}


	}

}
