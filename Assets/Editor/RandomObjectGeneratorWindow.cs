using UnityEditor;
using UnityEngine;
using System.Collections.Generic ;


namespace NetControl.Editor {
	
	public class RandomObjectGeneratorWindow : EditorWindow
	{
		public	GameObject	platformPrefab = null ;
		public	GameObject	mediumCratePrefab = null ;
		public	GameObject	spawnPositionPrefab = null ;
	//	public	GameObject	platformPrefab = null ;
		public	List<GameObject>	createdGameObjects = new List<GameObject>();


	    
	    // Add menu item named "Random Object Generator Window" to the Window menu
	    [MenuItem("Window/Random Object Generator Window")]
	    public static void ShowWindow()
	    {
	        //Show existing window instance. If one doesn't exist, make one.
	        EditorWindow.GetWindow(typeof(RandomObjectGeneratorWindow));
	    }
	    
	    void OnGUI()
	    {
	    /*    GUILayout.Label ("Base Settings", EditorStyles.boldLabel);
	        myString = EditorGUILayout.TextField ("Text Field", myString);
	        
	        groupEnabled = EditorGUILayout.BeginToggleGroup ("Optional Settings", groupEnabled);
	            myBool = EditorGUILayout.Toggle ("Toggle", myBool);
	            myFloat = EditorGUILayout.Slider ("Slider", myFloat, -3, 3);
	        EditorGUILayout.EndToggleGroup ();
	   	*/

			this.platformPrefab = (GameObject) EditorGUILayout.ObjectField ("Platform prefab",
				this.platformPrefab, typeof(GameObject), false);

			this.spawnPositionPrefab = (GameObject) EditorGUILayout.ObjectField ("Spawn position prefab",
				this.spawnPositionPrefab, typeof(GameObject), false);

			if (GUILayout.Button ("Generate")) {
				this.RandomGenerateObjects ();
			}

			if (GUILayout.Button ("Destroy all objects")) {
				int numDestroyed = 0;
				foreach (GameObject o in this.createdGameObjects) {
					if (o != null) {
						GameObject.DestroyImmediate (o, false);
						numDestroyed++;
					}
				}
				Debug.Log ("Destroyed " + numDestroyed + " game objects.");
				this.createdGameObjects.Clear ();
			}


	    }


		public	void	RandomGenerateObjects() {

			Random.seed = (int) Time.realtimeSinceStartup;

			int numObjectsCreated = 0;

			Vector3 start = new Vector3 (-60, 1, -60);
			Vector3 end = new Vector3 (60, 1, 60);

			for (float x = start.x; x < end.x; x += Random.Range(7.0f, 9.0f) ) {

				for (float z = start.z; z < end.z; z += Random.Range(7.0f, 10.0f) ) {

					float y = Random.Range (-0.5f, 0.5f) ;
					if ((int)z / 3 * 3 == (int)z) {
						y += 0.6f;
						y *= 7.0f;
					}
					Vector3 pos = new Vector3( x, y, z );
					GameObject o = (GameObject) GameObject.Instantiate( this.platformPrefab, pos, Quaternion.identity );
					if (o != null) {
						o.transform.localScale = new Vector3 ( 6.5f, 0.22f, 6.5f );

						// create spawn position on top of this platform
						Vector3 spawnPosition = new Vector3( Random.Range( -6.0f, 6.0f ), y + 2.5f,
							Random.Range( -6.0f, 6.0f ) );
						GameObject.Instantiate( this.spawnPositionPrefab, spawnPosition, Quaternion.identity );

						this.createdGameObjects.Add (o);
					}

					numObjectsCreated++;
				}

			}

			Debug.Log ("RandomGenerateObjects() finished - number of objects created " + numObjectsCreated);

		}


	}

}
