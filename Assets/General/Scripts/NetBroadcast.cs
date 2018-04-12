using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;

namespace uGameCore {

	/// <summary>
	/// Handles broadcasting and listening for games on LAN.
	/// </summary>
	public class NetBroadcast : MonoBehaviour
	{

		public class BroadcastData {
			private	string	fromAddress = "";
			private	Dictionary<string, string>	keyValuePairs = new Dictionary<string, string> ();
			private	float	timeWhenReceived = 0f;
			public BroadcastData (string fromAddress, Dictionary<string, string> keyValuePairs)
			{
				this.fromAddress = fromAddress;
				this.keyValuePairs = keyValuePairs;
				this.timeWhenReceived = Time.realtimeSinceStartup;
			}
			public string FromAddress { get { return this.fromAddress; } }
			public Dictionary<string, string> KeyValuePairs { get { return this.keyValuePairs; } }
			public	float	TimeSinceReceived { get { return Time.realtimeSinceStartup - this.timeWhenReceived; } }
		}

		public	static	event System.Action<BroadcastData>	onReceivedBroadcast = delegate {};

		private	static	List<BroadcastData>	m_allReceivedBroadcastData = new List<BroadcastData>();
		public	static	List<BroadcastData>	allReceivedBroadcastData { get { return m_allReceivedBroadcastData; } }

		private	static	Dictionary<string, string>	m_dataForBroadcasting = new Dictionary<string, string> ();

		public	static	NetBroadcast	singleton { get ; private set ; }

	//	private	static	bool	m_isInitialized = false;

		private	static	bool	m_isBroadcasting = false;
		private	static	bool	m_isListening = false;



		private	static	int	m_serverPort = 18300;
		private	static	int m_clientPort = 18301;
		private	static	UdpClient m_serverUdpCl = null;
		private	static	UdpClient m_clientUdpCl = null;

		public	bool	simulateReceiving = false;


		static	BinaryFormatter	m_binaryFormatter = new BinaryFormatter ();
		static	System.IO.MemoryStream	m_memoryStream = new System.IO.MemoryStream ();




		void Awake ()
		{
			if (null == singleton)
				singleton = this;

		}

		void Start ()
		{
			
			NetworkEventsDispatcher.onServerStarted += this.MyOnServerStarted;
			NetworkEventsDispatcher.onServerStopped += this.MyOnServerStopped;

			StartCoroutine (BroadcastCoroutine ());

			StartCoroutine (SimulateReceivingCoroutine ());

		}

		private	static	System.Collections.IEnumerator	SimulateReceivingCoroutine() {

			yield return new WaitForSeconds (4f);

			// generate some random IPs
			string[] ips = new string[15];
			for (int i = 0; i < ips.Length; i++) {
				string ip = Random.Range( 1, 255 ) + "." + Random.Range( 1, 255 ) + "." + Random.Range( 1, 255 ) + "." + Random.Range( 1, 255 ) ;
				ips [i] = ip;
			}

			while (true) {

				yield return null;

				float pauseTime = Random.Range (0.1f, 0.7f);
				yield return new WaitForSeconds (pauseTime);

				if (!IsListening ())
					continue;

				if (!singleton.simulateReceiving)
					continue;

				string ip = ips [Random.Range (0, ips.Length - 1)];
				var dict = new Dictionary<string, string> () { { "Port", Random.Range (1, 65535).ToString () }, 
					{ "Players", Random.Range (0, 20).ToString () }, { "Map", "gwefrds" }
				};

				OnReceivedBroadcastData( new BroadcastData( ip, dict ) );
			}
			
		}


		void MyOnServerStarted()
		{
			
			StopBroadcastingAndListening ();

			StartBroadcasting ();

		}

		void MyOnServerStopped()
		{

			StopBroadcastingAndListening ();

		}


		void Update ()
		{

			// TODO: call NetworkDiscovery.Update()


			if (NetworkStatus.IsServerStarted ()) {

				UpdateBroadcastData ();

				// assign broadcast data
			//	this.broadcastData = ConvertDictionaryToString (m_dataForBroadcasting);

			}

		}


		public	static	bool	IsListening() {

		//	return singleton.isClient;
			return m_isListening;
		}

		public	static	void	StartListening() {

			if (IsBroadcasting () || IsListening ())
				return;

			EnsureClientIsInitialized ();

//			if (!singleton.StartAsClient ()) {
//				Debug.LogError ("Failed to start listening on LAN");
//			}

			m_isListening = true;

		}

		public	static	bool	IsBroadcasting() {

		//	return singleton.isServer;
			return m_isBroadcasting;
		}

		public	static	void	StartBroadcasting() {

			if (IsBroadcasting () || IsListening ())
				return;

			EnsureServerIsInitialized ();

//			if (!this.StartAsServer ()) {
//				Debug.LogError ("Failed to start broadcasting on LAN");
//			}

			m_isBroadcasting = true;

		}

		public	static	void	StopBroadcastingAndListening() {

//			if (singleton.running) {
//				singleton.StopBroadcast ();
//				m_isInitialized = false;
//			}

			m_isBroadcasting = false;
			m_isListening = false;

		}

		private	static	void	EnsureServerIsInitialized() {

			if (m_serverUdpCl != null)
				return;

			m_serverUdpCl = new UdpClient (new IPEndPoint (IPAddress.Any, m_serverPort));

		}

		private	static	void	EnsureClientIsInitialized() {

			if (m_clientUdpCl != null)
				return;

			var localEP = new IPEndPoint(IPAddress.Any, m_clientPort);
			m_clientUdpCl = new UdpClient(localEP);

			m_clientUdpCl.BeginReceive(new System.AsyncCallback(ReceiveCallback), null);

		}

		private static void ReceiveCallback(System.IAsyncResult ar)
		{

			IPEndPoint remoteEP = null;
			byte[] receivedBytes = m_clientUdpCl.EndReceive (ar, ref remoteEP);

			if (IsListening ()) {
				if (remoteEP != null && receivedBytes != null) {
					string serverIP = remoteEP.Address.ToString ();
					var dict = ConvertByteArrayToDictionary (receivedBytes);

					OnReceivedBroadcastData (new BroadcastData (serverIP, dict));
				}
			}

		}

		private	static	System.Collections.IEnumerator	BroadcastCoroutine() {
			
			while (true)
			{
				yield return new WaitForSeconds (1f);

				if (!m_isBroadcasting)
					continue;

				if (!NetworkStatus.IsServerStarted ())
					continue;

				if (null == m_serverUdpCl)
					continue;

				Utilities.Utilities.RunExceptionSafe (() => {
					byte[] buffer = ConvertDictionaryToByteArray (m_dataForBroadcasting);
					m_serverUdpCl.Send (buffer, buffer.Length, new IPEndPoint (IPAddress.Broadcast, m_clientPort));
				});

			}

		}

		/*
		private	static	void	EnsureItIsInitialized() {

			if (m_isInitialized)
				return;

			if (!singleton.Initialize ()) {
				Debug.LogError ("Failed to initialize network discovery");
			} else {
				m_isInitialized = true;
			}

		}
		*/


		/*
		public override void OnReceivedBroadcast (string fromAddress, string data)
		{

			// don't allow exceptions to be thrown from here
			Utilities.Utilities.RunExceptionSafe (() => {

				// convert data to dictionary
				var dict = ConvertStringToDictionary (data);

				var broadcastData = new BroadcastData (fromAddress, dict);

				OnReceivedBroadcastData( broadcastData );
			}
			);

		}
		*/

		private	static	void	OnReceivedBroadcastData(BroadcastData broadcastData) {

			m_allReceivedBroadcastData.Add (broadcastData);

			// invoke event
			Utilities.Utilities.InvokeEventExceptionSafe (onReceivedBroadcast, broadcastData);

		}


		public	static	void	RegisterDataForBroadcasting( string key, string value ) {

			m_dataForBroadcasting.AddOrSet (key, value);

		}

		public	static	void	UnRegisterDataForBroadcasting( string key ) {

			m_dataForBroadcasting.Remove (key);

		}

		/// <summary>
		/// Adds/updates some default broadcast data.
		/// </summary>
		public	static	void	UpdateBroadcastData() {

			// update broadcast data:
			// - port
			// - num players
			// - map
			// - map time - depends on Map management

			RegisterDataForBroadcasting ("Port", NetManager.listenPortNumber.ToString ());
			RegisterDataForBroadcasting ("Players", PlayerManager.players.Count ().ToString ());
			RegisterDataForBroadcasting ("Map", NetManager.onlineScene);


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

		public	static	byte[]	ConvertDictionaryToByteArray( Dictionary<string, string> dict ) {

			m_memoryStream.SetLength (0);
			m_memoryStream.Seek (0, System.IO.SeekOrigin.Begin);
			m_binaryFormatter.Serialize(m_memoryStream, dict);

			return m_memoryStream.ToArray ();
		}

		public	static	Dictionary<string, string>	ConvertByteArrayToDictionary( byte[] data ) {

			m_memoryStream.SetLength (0);
			m_memoryStream.Seek (0, System.IO.SeekOrigin.Begin);
			m_memoryStream.Write (data, 0, data.Length);

			return (Dictionary<string, string>) m_binaryFormatter.Deserialize (m_memoryStream);
		}


	}

}
