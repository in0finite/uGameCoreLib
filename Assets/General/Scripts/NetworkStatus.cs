using UnityEngine.Networking;

namespace uGameCore {

	public	enum NetworkClientStatus
	{
		Disconnected = 0,
		Connecting = 1,
		Connected

	}

	public	enum NetworkServerStatus
	{
		Started = 1,
		Starting = 2,
		Stopped = 3


	}

	public class NetworkStatus {

		public	static	NetworkClientStatus	clientStatus {
			get {
				if (!NetworkClient.active)
					return NetworkClientStatus.Disconnected;
				if (0 == NetworkClient.allClients.Count)
					return NetworkClientStatus.Disconnected;

				if (!NetworkClient.allClients [0].isConnected)
					return NetworkClientStatus.Connecting;

				return NetworkClientStatus.Connected;
			}
		}

		public	static	NetworkServerStatus serverStatus {
			get {
				if (!NetworkServer.active)
					return NetworkServerStatus.Stopped;

				return NetworkServerStatus.Started;
			}
		}

		public	static	bool	IsServerStarted() {

			return serverStatus == NetworkServerStatus.Started;
		}

		public	static	bool	IsClientConnected() {

			return clientStatus == NetworkClientStatus.Connected;
		}

		public	static	bool	IsClientConnecting() {

			return clientStatus == NetworkClientStatus.Connecting;
		}

		public	static	bool	IsClientDisconnected() {

			return clientStatus == NetworkClientStatus.Disconnected;
		}


	}

}
