//-----------------------------------------------------------------
// This script handles the main Pixel Art Editor.
// It selects tools, finds the right pixels to color, handles input events & draws the toolbar gui.
// TODO: Tidy things up. Split functionality into smaller code portions. Make even a bit of optimization?
//-----------------------------------------------------------------

using UnityEngine;
using UnityEditor;

public class UPAEditorWindow : EditorWindow {

	public static UPAEditorWindow window;	// The static instance of the window

	public static UPAImage CurrentImg;		// The img currently being edited


	// HELPFUL GETTERS AND SETTERS

	private float gridSpacing {
		get { return CurrentImg.gridSpacing; }
		set { CurrentImg.gridSpacing = value; }
	}
	private float gridOffsetX {
		get { return CurrentImg.gridOffsetX; }
		set { CurrentImg.gridOffsetX = value; }
	}
	private float gridOffsetY {
		get { return CurrentImg.gridOffsetY; }
		set { CurrentImg.gridOffsetY = value; }
	}
	
	private UPATool tool {
		get { return CurrentImg.tool; }
		set { CurrentImg.tool = value; }
	}
	private Color32 selectedColor {
		get { return CurrentImg.selectedColor; }
		set { CurrentImg.selectedColor = value; }
	}
	private int gridBGIndex {
		get { return CurrentImg.gridBGIndex; }
		set { CurrentImg.gridBGIndex = value; }
	}


	// MISC TEMP VARIABLES

	//Used for checking if window has been resized
	private static Rect lastPos = new Rect ();


	// INITIALIZATION
	
	[MenuItem ("Window/Pixel Art Editor %#p")]
	public static void Init () {
		// Get existing open window or if none, make new one
		window = (UPAEditorWindow)EditorWindow.GetWindow (typeof (UPAEditorWindow));
		window.title = "Pixel Art Editor";

		string path = EditorPrefs.GetString ("currentImgPath", "");

		if (path.Length != 0)
			CurrentImg = UPASession.OpenImageAtPath (path);
	}
	
	
	// Draw the Pixel Art Editor.
	// This includes both toolbar and painting area.
	// TODO: Add comments
	void OnGUI () {
		if (window == null)
			Init ();

		if (CurrentImg == null || CurrentImg.map == null) { 

			string curImgPath = EditorPrefs.GetString ("currentImgPath", "");

			if (curImgPath.Length != 0) {
				CurrentImg = UPASession.OpenImageAtPath (curImgPath);
				return;
			}

			if ( GUI.Button (new Rect (window.position.width / 2f - 140, window.position.height /2f - 25, 130, 50), "New Image") ) {
				UPAImageCreationWindow.Init ();
			}
			if ( GUI.Button (new Rect (window.position.width / 2f + 10, window.position.height /2f - 25, 130, 50), "Open Image") ) {
				CurrentImg = UPASession.OpenImage ();
			}

			return;
		}

		bool updateRects = false;

		if (window.position != lastPos)
			updateRects = true;
		lastPos = window.position;

		EditorGUI.DrawRect (window.position, new Color32 (30,30,30,255));

		Event e = Event.current;	//Init event handler
		if (e.button == 0) {
			if (e.isMouse && e.mousePosition.y > 40) {
				if (tool == UPATool.Eraser)
					CurrentImg.ColorPixel (Color.clear, e.mousePosition);
				else if (tool == UPATool.PaintBrush)
					CurrentImg.ColorPixel (selectedColor, e.mousePosition);
				else if (tool == UPATool.BoxBrush) {
					Debug.Log ("TODO: Add Box Brush tool.");
				}
			}
		}

		if (e.type == EventType.keyDown && e.button == 0) {
			if (e.keyCode == KeyCode.W) {
				gridOffsetY += 20f;
				updateRects = true;
			}
			if (e.keyCode == KeyCode.S) {
				gridOffsetY -= 20f;
				updateRects = true;
			}
			if (e.keyCode == KeyCode.A) {
				gridOffsetX += 20f;
				updateRects = true;
			}
			if (e.keyCode == KeyCode.D) {
				gridOffsetX -= 20f;
				updateRects = true;
			}

			if (e.keyCode == KeyCode.B) {
				tool = UPATool.PaintBrush;
			}
			if (e.keyCode == KeyCode.E) {
				tool = UPATool.Eraser;
			}

			if (e.keyCode == KeyCode.UpArrow) {
				gridSpacing *= 1.2f;
			}
			if (e.keyCode == KeyCode.DownArrow) {
				gridSpacing *= 0.8f;
			}
		}

		// TODO: Better way of doing this?
		// Why does it behave so weirdly with my mac tablet.
		if (e.type == EventType.scrollWheel) {
			gridSpacing -= e.delta.y;
		}
		
		EditorGUI.DrawRect ( CurrentImg.FillRect(), UPADrawer.gridBGColor);
		
		// DRAW IMAGE
		if ( UPADrawer.DrawImage ( CurrentImg, window.position ) ) {
			updateRects = true;
		}

		if ( UPADrawer.DrawToolbar (window.position) ) {
			updateRects = true;
		}

		if (GUI.changed)
			updateRects = true;

		if (updateRects)
			CurrentImg.UpdateRects();

		e.Use();	// Release event handler
	}
}