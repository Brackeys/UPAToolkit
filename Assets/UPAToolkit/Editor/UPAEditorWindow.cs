//-----------------------------------------------------------------
// This script handles the main Pixel Art Editor.
// It selects tools, finds the right pixels to color, handles input events & draws the toolbar gui.
// TODO: Tidy things up. Split functionality into smaller code portions. Make even a bit of optimization?
// TODO: Add method for opening images.
//-----------------------------------------------------------------

using UnityEngine;
using UnityEditor;

// Used for switching tools
enum UPATool {
	PaintBrush,
	Eraser,
	BoxBrush, // TODO: Add BoxBrush
}

public class UPAEditorWindow : EditorWindow {

	public static UPAEditorWindow window;	// The static instance of the window

	public static UPAImage CurrentImg;		// The img currently being edited


	// VIEW & NAVIGATION SETTINGS

	private static float _gridSpacing = 20f;
	public static float gridSpacing {
		get { return _gridSpacing; }
		set { _gridSpacing = Mathf.Clamp (value, 2f, 60f); }
	}
	public static float gridOffsetY = 0;
	public static float gridOffsetX = 0;


	// PAINTING & VISUAL SETTINGS

	public static Color selectedColor = new Color (1,0,0,1);

	private static Color bgColor = new Color (0.9f, 0.9f, 0.9f, 1);
	private static Color32 toolbarColor = new Color32 (50, 50, 50, 255);

	private static UPATool tool = UPATool.PaintBrush;

	private static int gridBGIndex = 0;
	private static string[] gridBGStrings = new string[] {"Black", "White"};
	private static Color gridBGColor = Color.black;
	
	private static GUIStyle style = new GUIStyle();
	

	private static Rect lastPos = new Rect ();	//Used for checking if window has been resized

	// Add Pixel Art Editor to the Window menu
	[MenuItem ("Window/Pixel Art Editor")]
	static void Init () {
		// Get existing open window or if none, make new one
		window = (UPAEditorWindow)EditorWindow.GetWindow (typeof (UPAEditorWindow));
		window.title = "Pixel Art Editor";
	}
	
	// TODO: Add comments
	void OnGUI () {
		if (window == null)
			Init ();

		if (CurrentImg == null || CurrentImg.map == null) { 
			if ( GUI.Button (new Rect (window.position.width / 2f - 140, window.position.height /2f - 25, 130, 50), "New Image") ) {
				UPAImageCreationWindow.Init ();
			}
			if ( GUI.Button (new Rect (window.position.width / 2f + 10, window.position.height /2f - 25, 130, 50), "Open Image") ) {
				Debug.Log ("TODO: Open image.");
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
			} else if (e.keyCode == KeyCode.S) {
				gridOffsetY -= 20f;
				updateRects = true;
			} else if (e.keyCode == KeyCode.A) {
				gridOffsetX += 20f;
				updateRects = true;
			} else if (e.keyCode == KeyCode.D) {
				gridOffsetX -= 20f;
				updateRects = true;
			}
		}
		
		EditorGUI.DrawRect ( CurrentImg.FillRect(), gridBGColor);
		
		for (int x = 0; x < CurrentImg.width; x++) {
			for (int y = 0; y < CurrentImg.height; y++) {
				if (CurrentImg.map[x + y * CurrentImg.width].rect.size == Vector2.zero) {
					updateRects = true;
					continue;
				}

				// Is the rect visible on screen?
				if (!window.position.Contains (new Vector2 (CurrentImg.map[x + y * CurrentImg.width].rect.x, CurrentImg.map[x + y * CurrentImg.width].rect.y))
				    && !window.position.Contains (new Vector2 (CurrentImg.map[x + y * CurrentImg.width].rect.x + CurrentImg.map[x + y * CurrentImg.width].rect.width,
				                                           CurrentImg.map[x + y * CurrentImg.width].rect.y + CurrentImg.map[x + y * CurrentImg.width].rect.height)))
				{
					continue;
				}
				    
				Color c = CurrentImg.map[x + y * CurrentImg.width].color;
				float newR = c.a * c.r + (1 - c.a) * bgColor.r;
				float newG = c.a * c.g + (1 - c.a) * bgColor.g;
				float newB = c.a * c.b + (1 - c.a) * bgColor.b;

				Color fC = new Color (newR, newG, newB, 1);
				
				EditorGUI.DrawRect (CurrentImg.map[x + y * CurrentImg.width].rect, fC);
			}
		}

		// Draw toolbar bg
		EditorGUI.DrawRect ( new Rect (0,0, window.position.width, 40), toolbarColor );
		
		if ( GUI.Button (new Rect (5, 4, 50, 30), "New") ) {
			UPAImageCreationWindow.Init ();
		}
		if ( GUI.Button (new Rect (60, 4, 50, 30), "Open") ) {
			Debug.Log ("Implement open file.");
		}
		if ( GUI.Button (new Rect (115, 4, 50, 30), "Export") ) {
			UPAExportWindow.Init();
		}

		if (GUI.Button (new Rect (179, 6, 25, 25), "+")) {
			gridSpacing *= 1.5f;
			updateRects = true;
		}
		if (GUI.Button (new Rect (209, 6, 25, 25), "-")) {
			gridSpacing *= 0.5f;
			updateRects = true;
		}
	
		selectedColor = EditorGUI.ColorField (new Rect (250, 7, 70, 25), selectedColor);
		EditorGUI.DrawRect ( new Rect (303, 7, 20, 25), toolbarColor );
		//bgColor = EditorGUI.ColorField (new Rect (400, 4, 70, 25), bgColor);

		GUI.backgroundColor = Color.white;
		if (tool == UPATool.PaintBrush)
			GUI.backgroundColor = new Color (0.7f, 0.7f, 0.7f);
		if (GUI.Button (new Rect (320, 4, 60, 30), "Paint")) {
			tool = UPATool.PaintBrush;
		}
		GUI.backgroundColor = Color.white;
		if (tool == UPATool.BoxBrush)
			GUI.backgroundColor = new Color (0.7f, 0.7f, 0.7f);
		if (GUI.Button (new Rect (450, 4, 60, 30), "Box Fill")) {
			tool = UPATool.BoxBrush;
		}
		GUI.backgroundColor = Color.white;
		if (tool == UPATool.Eraser)
			GUI.backgroundColor = new Color (0.7f, 0.7f, 0.7f);
		if (GUI.Button (new Rect (385, 4, 60, 30), "Erase")) {
			tool = UPATool.Eraser;
		}
		GUI.backgroundColor = Color.white;
		
		style.normal.textColor = new Color (0.7f, 0.7f, 0.7f);
		GUI.Label (new Rect (525, 11, 150, 30), "Use WASD to navigate.", style);

		if (GUI.Button (new Rect (670, 4, 80, 30), "Center View")) {
			gridOffsetX = 0;
			gridOffsetY = 0;
		}

		gridBGIndex = GUI.Toolbar (new Rect (760, 4, 90, 30), gridBGIndex, gridBGStrings);

		if (gridBGIndex == 0) {
			gridBGColor = Color.black;
		} else {
			gridBGColor = Color.white;
		}

		if (GUI.changed)
			updateRects = true;

		if (updateRects)
			CurrentImg.UpdateRects();

		e.Use();	// Release event handler
	}
}