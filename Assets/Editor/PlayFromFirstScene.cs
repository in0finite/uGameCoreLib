using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace NetControl.Editor {
	
	[InitializeOnLoad]
	public static class PlayFromFirstScene
	{
		// click command-0 to go to the first scene and then play

		[MenuItem("Play/Play-Unplay, but from first scene %0")]
		public static void Play()
		{
			if ( EditorApplication.isPlaying )
			{
				EditorApplication.isPlaying = false;
				return;
			}

			EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo ();

			var scene = EditorSceneManager.GetSceneByBuildIndex (0);
			if (scene.IsValid ()) {
				EditorSceneManager.OpenScene (scene.path);
				EditorApplication.isPlaying = true;
			}

		}
	}

}
