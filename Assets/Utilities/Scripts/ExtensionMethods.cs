﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Reflection;
using System.Linq;

namespace uGameCore {
	
	public static	class ExtensionMethods {


		public	static	IEnumerable<T>	WhereNotNull<T>( this IEnumerable<T> enumerable ) where T : class {

			foreach (var el in enumerable) {
				if (el != null)
					yield return el;
			}

		}

		/// <summary>
		/// Finds the desired element in a collection by extracting value and comparing it.
		/// </summary>
		/// <param name="valueSelector">Extracts value from each element.</param>
		/// <param name="valueComparer">Compares 2 values. Should return true if first is selected, otherwise false.</param>
		public	static	T	SelectByValueComparison<T,R>( this IEnumerable<T> enumerable, System.Func<T,R> valueSelector,
			System.Func<R, R, bool> valueComparer ) {

			var en = enumerable.GetEnumerator ();

			if (!en.MoveNext ())
				return default (T);

			var selectedElement = en.Current;
			var selectedValue = valueSelector(selectedElement) ;

		//	System.Func<R, R, bool> valueComparer = (a, b) => a < b ;

			while (en.MoveNext ()) {
				var value = valueSelector (en.Current);
				if (valueComparer (value, selectedValue)) {
					selectedValue = value;
					selectedElement = en.Current;
				}
			}

			return selectedElement;
		}

		public	static	T	SelectMin<T,R>( this IEnumerable<T> enumerable, System.Func<T,R> valueSelector )
			where R : System.IComparable {

			System.Func<R, R, bool> valueComparer = (a, b) => a.CompareTo (b) < 0 ;
			return SelectByValueComparison (enumerable, valueSelector, valueComparer);

		}

		public	static	T	SelectMax<T,R>( this IEnumerable<T> enumerable, System.Func<T,R> valueSelector )
			where R : System.IComparable {

			System.Func<R, R, bool> valueComparer = (a, b) => a.CompareTo (b) > 0 ;
			return SelectByValueComparison (enumerable, valueSelector, valueComparer);

		}


		public	static	bool	IsControllingPhysics( this NetworkBehaviour networkBehaviour ) {

			return networkBehaviour.isServer;

		}

		public	static	bool	IsInputting(this NetworkBehaviour networkBehaviour) {

		//	if (networkBehaviour.isLocalPlayer)	// we can't check only this, because netControl may be null
		//		return true;

			if (!networkBehaviour.isLocalPlayer)	// commands can not be sent if isLocalPlayer is not true
				return false;

			ControllableObject ncp = networkBehaviour.GetComponent<ControllableObject> ();
			if (null == ncp)
				return false;

			if (null == ncp.playerOwner)
				return false;

			return ncp.playerOwner == Player.local;

		}

		public	static	bool	IsServer(this MonoBehaviour monoBehaviour) {
			return monoBehaviour.GetComponent<NetworkIdentity> ().isServer;
		}

		public	static	bool	IsClient(this MonoBehaviour monoBehaviour) {
			return monoBehaviour.GetComponent<NetworkIdentity> ().isClient;
		}

		public	static	bool	IsLocalPlayer(this MonoBehaviour monoBehaviour) {
			return monoBehaviour.GetComponent<NetworkIdentity> ().isLocalPlayer;
		}


		[System.Obsolete]
		public	static	void	CopyTo(this Component component, Component targetComponent) {

			System.Type type = component.GetType ();
			System.Type targetType = targetComponent.GetType ();
			if (targetType != type)
				return;

			MemberInfo[] members = type.GetMembers (BindingFlags.Instance
				| BindingFlags.Public | BindingFlags.DeclaredOnly);
			if (null == members || 0 == members.Length)
				return;
		//	MemberInfo[] targetMembers = targetType.GetMembers (BindingFlags.Instance
		//		| BindingFlags.Public);
		//	if (null == targetMembers || 0 == targetMembers.Length)
		//		return;

			foreach (MemberInfo memberInfo in members) {
				if (memberInfo.MemberType != MemberTypes.Field && memberInfo.MemberType != MemberTypes.Property)
					continue;

				if( memberInfo.GetCustomAttributes ( typeof(HideInInspector), true).Length > 0 ) {
					continue;
				}

				object value = null;
				if (memberInfo.MemberType == MemberTypes.Field)
					value = ((FieldInfo)memberInfo).GetValue (component);
				else if (memberInfo.MemberType == MemberTypes.Property) {
					if (((PropertyInfo)memberInfo).CanRead) {
						value = ((PropertyInfo)memberInfo).GetValue (component, null);
					}
				}

			//	MemberInfo targetMember = System.Array.Find<MemberInfo> (targetMembers, m => m.Name == memberInfo.Name &&
			//	                          m.MemberType == memberInfo.MemberType && m.DeclaringType == memberInfo.DeclaringType);
			//	if (targetMember != null) {
					// set member value
				if (memberInfo.MemberType == MemberTypes.Field)
					((FieldInfo)memberInfo).SetValue (targetComponent, value);
				else if (memberInfo.MemberType == MemberTypes.Property) {
					if (((PropertyInfo)memberInfo).CanWrite) {
						try {
							((PropertyInfo)memberInfo).SetValue (targetComponent, value, null);
						} catch {

						}
					}
				}
			//	}
			}

		}


		/// <summary>
		/// Instantiates the object, and if server is active, spawns him.
		/// </summary>
		public	static	GameObject	InstantiateWithNetwork(this GameObject go, Vector3 pos, Quaternion rot) {

			var clone = Object.Instantiate (go, pos, rot);
			if (UnityEngine.Networking.NetworkServer.active) {
				UnityEngine.Networking.NetworkServer.Spawn (clone);
			}
			return clone;
		}

		// NetworkManager extensions

		[System.Obsolete("", true)]
		public	static	void	StartClient( this NetworkManager netMgr, string serverIp, int serverPort ) {

			netMgr.networkAddress = serverIp;
			netMgr.networkPort = serverPort;
			netMgr.StartClient ();

		}

		[System.Obsolete("", true)]
		public	static	void	StopServerAndHost( this NetworkManager netMgr ) {

			netMgr.StopServer ();
			netMgr.StopHost ();

		}

		[System.Obsolete("", true)]
		public	static	bool	IsServer( this NetworkManager netMgr ) {

			return NetworkStatus.IsServerStarted ();

		}

		[System.Obsolete("", true)]
		public	static	bool	IsClient( this NetworkManager netMgr ) {

			return NetworkStatus.IsClientConnected ();

		}

		[System.Obsolete("", true)]
		public	static	bool	IsHost( this NetworkManager netMgr ) {

			if (!netMgr.IsServer ())
				return false;

			return NetworkServer.localClientActive;
		}


		public	static	void	Invoke( this Component component, string methodName, params object[] args ) {

			var method = component.GetType().GetMethod( methodName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
			if(method != null) {
				method.Invoke( component, args );
			}

		}

		/// <summary>
		/// Invokes the specified method, but catches any exception that may be thrown, and logs it.
		/// </summary>
		public	static	void	InvokeExceptionSafe( this Component component, string methodName, params object[] args ) {

			try {
				component.Invoke( methodName, args );
			} catch (System.Exception ex) {
				Debug.LogException (ex);
			}

		}

	//	public	static	void	InvokeExceptionSafe( this Component component, string methodName, object arg ) {
	//		component.InvokeExceptionSafe( methodName, new object[] {arg} );
	//	}

//		public	static	void	BroadcastMessageNoExceptions( this GameObject go, string msg ) {
//
//			BroadcastMessageNoExceptions (go, msg, new object[] { });
//
//		}
//
//		public	static	void	BroadcastMessageNoExceptions( this GameObject go, string msg, object arg ) {
//
//			BroadcastMessageNoExceptions (go, msg, new object[] { arg });
//
//		}

		public	static	void	BroadcastMessageNoExceptions( this GameObject go, string msg, params object[] args ) {

			var components = go.GetComponentsInChildren<Component> ();
			foreach (var c in components) {
				try {

				//	var method = c.GetType().GetMethod( msg, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public );
				//	if(method != null) {
				//		method.Invoke( c, args );
				//	}
					c.Invoke( msg, args );

				} catch( System.Exception ex ) {
					Debug.LogException (ex);
				}
			}

		}


		public	static	T	AddComponentIfDoesntExist<T> (this GameObject gameObject) where T : Component {

			var component = gameObject.GetComponent<T> ();
			if (null == component)
				component = gameObject.AddComponent<T> ();

			return component;
		}


		public	static	Transform	FindChildOrLogError( this Transform transform, string childName ) {

			var child = transform.FindChild (childName);

			if (null == child) {
				Debug.LogError ("Failed to find child with name " + childName);
			}

			return child;
		}

		public	static	Transform	FindChildRecursivelyOrLogError( this Transform transform, string childName ) {

			var child = transform.GetComponentsInChildren<Transform> ().FirstOrDefault (c => childName == c.name);

			if (null == child) {
				Debug.LogError ("Failed to find child with name " + childName);
			}

			return child;
		}


		public	static	GameObject	InstantiateAsUIElement( this GameObject prefab, Transform parent ) {

			var go = GameObject.Instantiate (prefab);
			go.transform.SetParent (parent, false);
			return go;
		}

		public	static	RectTransform	GetRectTransform( this Component component ) {

			return component.transform as RectTransform;

		}

		public	static	RectTransform	GetRectTransform( this GameObject go ) {

			return go.transform as RectTransform;

		}

		public	static	void	AnchorsToCorners( this RectTransform rectTransform ) {

			RectTransform t = rectTransform;
			RectTransform pt = rectTransform.parent as RectTransform;

			Vector2 newAnchorsMin = new Vector2(t.anchorMin.x + t.offsetMin.x / pt.rect.width,
				t.anchorMin.y + t.offsetMin.y / pt.rect.height);
			Vector2 newAnchorsMax = new Vector2(t.anchorMax.x + t.offsetMax.x / pt.rect.width,
				t.anchorMax.y + t.offsetMax.y / pt.rect.height);

			t.anchorMin = newAnchorsMin;
			t.anchorMax = newAnchorsMax;
			t.offsetMin = t.offsetMax = new Vector2(0, 0);

		}

		public	static	void	CornersToAnchors( this RectTransform rectTransform ) {
			
			rectTransform.offsetMin = rectTransform.offsetMax = new Vector2(0, 0);

		}

		public	static	void	SetRectAndAdjustAnchors( this RectTransform rectTransform, Rect rect ) {

			var parentRectTransform = rectTransform.parent as RectTransform;

			var normalizedRect = new Rect (rect.x / parentRectTransform.rect.width, rect.y / parentRectTransform.rect.height,
				rect.width / parentRectTransform.rect.width, rect.height / parentRectTransform.rect.height);

			rectTransform.SetNormalizedRectAndAdjustAnchors (normalizedRect);

		}

		public	static	void	SetNormalizedRectAndAdjustAnchors( this RectTransform rectTransform, Rect normalizedRect ) {

			// first set anchored position, anchors, and size
			rectTransform.anchorMin = normalizedRect.min;//new Vector2 (0, 1);
			rectTransform.anchorMax = normalizedRect.max; //new Vector2 (0, 1);
			//	Debug.Log ("anchored position before: " + rectTransform.anchoredPosition);
			//	rectTransform.anchoredPosition = new Vector2( rect.center.x, - rect.center.y) ;

			//	rectTransform.offsetMin = Vector2.zero;
			//	rectTransform.offsetMax = Vector2.one / 2;
			//	rectTransform.sizeDelta = rect.size;

			rectTransform.CornersToAnchors ();

			// move anchors to corners
			rectTransform.AnchorsToCorners ();

		//	Debug.Log ("anchored position: " + rectTransform.anchoredPosition + " anchor min: " + rectTransform.anchorMin + 
		//		" anchor max: " + rectTransform.anchorMax + " offset min: " + rectTransform.offsetMin + " offset max: " +
		//		rectTransform.offsetMax );
			
		}

		public	static	Vector2	GetParentDimensions( this RectTransform rectTransform ) {

			if (null == rectTransform.parent)
				return new Vector2 (Screen.width, Screen.height);

			return ((RectTransform)rectTransform.parent).rect.size;
		}

		public	static	Vector2	NormalizePositionRelativeToParent( this RectTransform rectTransform, Vector2 pos ) {

			Vector2 parentSize = rectTransform.GetParentDimensions ();

			return new Vector2 (pos.x / parentSize.x, pos.y / parentSize.y);
		}

	}

}
