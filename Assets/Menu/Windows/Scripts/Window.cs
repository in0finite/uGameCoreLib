using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace uGameCore.Menu.Windows {

	[DisallowMultipleComponent]
	public class Window : MonoBehaviour
	{
		
		// most of these variables were used with old GUI system

		internal	int		id = -1 ;
		internal	bool	isModal = false ;
		internal	string	title = "" ;
		internal	Rect	rect = new Rect ();
		internal	List<string>	displayStrings = new List<string> ();
		internal	bool	isClosed = false ;
		internal	System.Action<Window>	procedure = null;
	//	public	GameObject	gameObject { get ; internal set ; }



		/// <summary> Position and size of window. </summary>
		public	Rect	GetRectangle () {
			var rt = this.gameObject.GetRectTransform ();

			Vector2 parentSize = rt.GetParentDimensions ();

			Vector2 diff = rt.anchorMax - rt.anchorMin;
			Vector2 size = Vector2.Scale (diff, parentSize);

			Vector2 position = Vector2.Scale (rt.anchorMin, parentSize);

			return new Rect (position, size);
		}

		/// <summary> Position and size of window. </summary>
		public	void	SetRectangle( Rect rect ) {
			this.gameObject.GetRectTransform ().SetRectAndAdjustAnchors (rect);
		}

		public	Image	imageComponent { get { return this.gameObject.GetComponent<Image> (); } }
		public	Color	background {
			get {
				if (this.imageComponent)
					return this.imageComponent.color;
				else
					return Color.white;
			}
			set {
				if (this.imageComponent)
					this.imageComponent.color = value;
			}
		}

		public	Transform	titleTransform { get { return this.gameObject.transform.FindChild ("Title"); } }
		public	Text	titleTextComponent {
			get {
				if (this.titleTransform)
					return this.titleTransform.GetComponentInChildren<Text> ();
				else
					return null;
			}
		}
		public	string	Title {
			get {
				if (this.titleTextComponent)
					return this.titleTextComponent.text;
				else
					return "";
			}
			set {
				if (this.titleTextComponent)
					this.titleTextComponent.text = value;
			}
		}
		public	Color	titleColor {
			get {
				if (this.titleTextComponent)
					return this.titleTextComponent.color;
				else
					return Color.black;
			}
			set {
				if (this.titleTextComponent)
					this.titleTextComponent.color = value;
			}
		}

		public	ScrollRect	scrollView { get { return this.gameObject.GetComponentInChildren<ScrollRect> (); } }
		public	Image	scrollViewImageComponent {
			get {
				if (this.scrollView)
					return this.scrollView.GetComponent<Image> ();
				else
					return null;
			}
		}
		public	Color	scrollViewBackground {
			get {
				if (this.scrollViewImageComponent)
					return this.scrollViewImageComponent.color;
				else
					return Color.white;
			}
			set {
				if (this.scrollViewImageComponent)
					this.scrollViewImageComponent.color = value;
			}
		}

		/// <summary>Game object where you can place your UI controls.</summary>
		public	RectTransform	content { get { if(this.scrollView) return this.scrollView.content; else return null; } }
		/// <summary>
		/// Layout group which controls placement of UI controls inside content.
		/// </summary>
		public	LayoutGroup	contentLayoutGroup {
			get {
				if (this.content)
					return this.content.GetComponent<LayoutGroup> ();
				else
					return null;
			}
		}
		public	bool	contentLayoutGroupEnabled {
			get {
				if (this.contentLayoutGroup)
					return this.contentLayoutGroup.enabled;
				else
					return false;
			}
			set {
				if (this.contentLayoutGroup)
					this.contentLayoutGroup.enabled = value;
			}
		}

		// TODO: add buttons, texts to content ; add buttons below content ; bring to top when clicked ;



		void Start ()
		{
		
		}



	}

}
