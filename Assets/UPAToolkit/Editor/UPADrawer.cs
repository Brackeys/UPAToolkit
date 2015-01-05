using UnityEngine;
using UnityEditor;

public class UPADrawer : MonoBehaviour {
	
	private static UPAImage CurrentImg {
		get { return UPAEditorWindow.CurrentImg; }
		set { UPAEditorWindow.CurrentImg = value; }
	}


	// VISUAL SETTINGS

	private static Color bgColor = new Color (0.9f, 0.9f, 0.9f, 1);
	
	private static Color32 toolbarColor = new Color32 (50, 50, 50, 255);
	
	private static string[] gridBGStrings = new string[] {"Black", "White"};
	public static Color gridBGColor = Color.black;
	
	private static GUIStyle style = new GUIStyle();


	// DRAWING METHODS

	// Draw an image inside a window
	// Return true if image rects need to be updated.
	public static bool DrawImage (UPAImage img, Rect window) {
		bool updateRects = false;

		for (int x = 0; x < img.width; x++) {
			for (int y = 0; y < img.height; y++) {
			
				if (img.map[x + y * img.width].rect.size == Vector2.zero) {
					updateRects = true;
					continue;
				}
				
				// Is the rect visible on screen?
				if (!window.Contains (new Vector2 (img.map[x + y * img.width].rect.x, img.map[x + y * img.width].rect.y))
				    && !window.Contains (new Vector2 (img.map[x + y * img.width].rect.x + img.map[x + y * img.width].rect.width,
				                                           img.map[x + y * img.width].rect.y + img.map[x + y * img.width].rect.height)))
				{
					continue;
				}
				
				Color c = img.map[x + y * img.width].color;
				float newR = c.a * c.r + (1 - c.a) * bgColor.r;
				float newG = c.a * c.g + (1 - c.a) * bgColor.g;
				float newB = c.a * c.b + (1 - c.a) * bgColor.b;
				
				Color fC = new Color (newR, newG, newB, 1);
				
				EditorGUI.DrawRect (img.map[x + y * img.width].rect, fC);
			}
		}
		
		return updateRects;
	}
	
	// Draw an image inside inspector
	// TODO: Get this working.
	public static void DrawImageInInspector (UPAImage img, Rect window) {
		
		for (int x = 0; x < img.width; x++) {
			for (int y = 0; y < img.height; y++) {
				
				// Is the rect visible on screen?
				if (!window.Contains (new Vector2 (img.map[x + y * img.width].rect.x, img.map[x + y * img.width].rect.y))
				    && !window.Contains (new Vector2 (img.map[x + y * img.width].rect.x + img.map[x + y * img.width].rect.width,
				                                  img.map[x + y * img.width].rect.y + img.map[x + y * img.width].rect.height)))
				{
					continue;
				}
				
				Color c = img.map[x + y * img.width].color;
				float newR = c.a * c.r + (1 - c.a) * bgColor.r;
				float newG = c.a * c.g + (1 - c.a) * bgColor.g;
				float newB = c.a * c.b + (1 - c.a) * bgColor.b;
				
				Color fC = new Color (newR, newG, newB, 1);
				
				EditorGUI.DrawRect (img.map[x + y * img.width].rect, fC);
			}
		}
		
	}

	// Draw the settings toolbar
	// Return true if image rects need to be updated.
	public static bool DrawToolbar (Rect window) {
		bool updateRects = false;

		// Draw toolbar bg
		EditorGUI.DrawRect ( new Rect (0,0, window.width, 40), toolbarColor );
		
		if ( GUI.Button (new Rect (5, 4, 50, 30), "New") ) {
			UPAImageCreationWindow.Init ();
		}
		if ( GUI.Button (new Rect (60, 4, 50, 30), "Open") ) {
			CurrentImg = UPASession.OpenImage ();
			if (CurrentImg == null)
				return false;
		}
		if ( GUI.Button (new Rect (115, 4, 50, 30), "Export") ) {
			UPAExportWindow.Init(CurrentImg);
		}

		if (GUI.Button (new Rect (179, 6, 25, 25), "+")) {
			CurrentImg.gridSpacing *= 1.5f;
			updateRects = true;
		}
		if (GUI.Button (new Rect (209, 6, 25, 25), "-")) {
			CurrentImg.gridSpacing *= 0.5f;
			updateRects = true;
		}
		
		CurrentImg.selectedColor = EditorGUI.ColorField (new Rect (250, 7, 70, 25), CurrentImg.selectedColor);
		EditorGUI.DrawRect ( new Rect (303, 7, 20, 25), toolbarColor );
		//bgColor = EditorGUI.ColorField (new Rect (400, 4, 70, 25), bgColor);
		
		GUI.backgroundColor = Color.white;
		if (CurrentImg.tool == UPATool.PaintBrush)
			GUI.backgroundColor = new Color (0.7f, 0.7f, 0.7f);
		if (GUI.Button (new Rect (320, 4, 60, 30), "Paint")) {
			CurrentImg.tool = UPATool.PaintBrush;
		}
		GUI.backgroundColor = Color.white;
		if (CurrentImg.tool == UPATool.BoxBrush)
			GUI.backgroundColor = new Color (0.7f, 0.7f, 0.7f);
		if (GUI.Button (new Rect (450, 4, 60, 30), "Box Fill")) {
			EditorUtility.DisplayDialog(
				"In Development",
				"This feature is currently being developed.",
				"Get it done please");
			//tool = UPATool.BoxBrush;
		}
		GUI.backgroundColor = Color.white;
		if (CurrentImg.tool == UPATool.Eraser)
			GUI.backgroundColor = new Color (0.7f, 0.7f, 0.7f);
		if (GUI.Button (new Rect (385, 4, 60, 30), "Erase")) {
			CurrentImg.tool = UPATool.Eraser;
		}
		GUI.backgroundColor = Color.white;
		
		style.normal.textColor = new Color (0.7f, 0.7f, 0.7f);
		GUI.Label (new Rect (525, 11, 150, 30), "Use WASD to navigate.", style);
		
		if (GUI.Button (new Rect (670, 4, 80, 30), "Center View")) {
			CurrentImg.gridOffsetX = 0;
			CurrentImg.gridOffsetY = 0;
		}
		
		CurrentImg.gridBGIndex = GUI.Toolbar (new Rect (760, 4, 90, 30), CurrentImg.gridBGIndex, gridBGStrings);
		
		if (CurrentImg.gridBGIndex == 0) {
			gridBGColor = Color.black;
		} else {
			gridBGColor = Color.white;
		}

		return updateRects;
	}
	
}
