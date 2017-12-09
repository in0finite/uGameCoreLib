using System;
using UnityEngine;

namespace uGameCore.Utilities
{
	public static class Utilities
	{


		public	static	string	GetAssetName() {

			return typeof(Utilities).Assembly.GetName ().Name;

		}

		public	static	string	GetAssetRootFolderName() {

			return GetAssetName ();

		}


		public	static	void	SendMessageToAllMonoBehaviours( string msg, params object[] arguments ) {

			var objects = UnityEngine.Object.FindObjectsOfType<MonoBehaviour> ();

			foreach (var obj in objects) {
				obj.InvokeExceptionSafe (msg, arguments);
			}

		}

		/// <summary>
		/// Invokes all subscribed delegates, and makes sure any exception is caught and logged, so that all
		/// subscribers will be notified.
		/// </summary>
		public	static	void	InvokeEventExceptionSafe( MulticastDelegate eventDelegate, params object[] parameters ) {

			var delegates = eventDelegate.GetInvocationList ();

			foreach (var del in delegates) {
				if (del.Method != null) {
					try {
						del.Method.Invoke (del.Target, parameters);
					} catch(System.Exception ex) {
						UnityEngine.Debug.LogException (ex);
					}
				}
			}

		}

		public	static	T	FindObjectOfTypeOrLogError<T>() where T : Component {

			var obj = UnityEngine.Object.FindObjectOfType<T> ();

			if (null == obj) {
				Debug.LogError ("Object of type " + typeof(T).ToString() + " can not be found." );
			}

			return obj;
		}


		public	static	Vector2	CalcScreenSizeForContent( GUIContent content, GUIStyle style ) {

			return style.CalcScreenSize (style.CalcSize (content));
		}

		public	static	Vector2	CalcScreenSizeForText( string text, GUIStyle style ) {

			return CalcScreenSizeForContent (new GUIContent (text), style);
		}

		public	static	bool	ButtonWithCalculatedSize( string text ) {

			Vector2 size = CalcScreenSizeForText (text, GUI.skin.button);

			return GUILayout.Button (text, GUILayout.Width (size.x), GUILayout.Height (size.y));
		}

		public	static	bool	DisabledButton( bool isEnabled, string text, params GUILayoutOption[] options ) {

			GUIStyle style = isEnabled ? GUI.skin.button : GUI.skin.box;

			if (GUILayout.Button (text, style, options) && isEnabled)
				return true;

			return false;
		}

		public	static	bool	DisabledButtonWithCalculatedSize( bool isEnabled, string text ) {

			GUIStyle style = isEnabled ? GUI.skin.button : GUI.skin.box;
			Vector2 size = CalcScreenSizeForText (text, style);

			var options = new GUILayoutOption[] { GUILayout.Width (size.x), GUILayout.Height (size.y) };

			if (GUILayout.Button (text, style, options) && isEnabled)
				return true;

			return false;
		}

	}
}

