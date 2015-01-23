using UnityEngine;
using UnityEditor;

public class UPADrawer : MonoBehaviour {
	
	private static UPAImage CurrentImg {
		get { return UPAEditorWindow.CurrentImg; }
		set { UPAEditorWindow.CurrentImg = value; }
	}


	// VISUAL SETTINGS
	
	private static Color32 toolbarColor = new Color32 (50, 50, 50, 255);
	
	private static string[] gridBGStrings = new string[] {"Black", "White"};
	public static Color gridBGColor = Color.black;
	
	private static GUIStyle style = new GUIStyle();

	private static Vector2 mousePixelPos = Vector2.zero;


	// DRAWING METHODS

	// Draw an image inside the editor window
	public static void DrawImage (UPAImage img) {
		Rect texPos = img.GetImgRect();
		EditorGUI.DrawTextureTransparent (texPos, img.tex);
	
		// Draw a grid above the image (y axis first)
		for (int x = 0; x <= img.width; x += 1) {
			float posX = texPos.xMin + ( (float)texPos.width / (float)img.width ) * x - 0.2f;
			EditorGUI.DrawRect (new Rect (posX, texPos.yMin, 1, texPos.height), gridBGColor);
		}
		// Then x axis
		for (int y = 0; y <= img.height; y += 1) {
			float posY = texPos.yMin + ( (float)texPos.height / (float)img.height ) * y - 0.2f;
			EditorGUI.DrawRect (new Rect (texPos.xMin, posY, texPos.width, 1), gridBGColor);
		}
	}

	// Draw the settings toolbar
	public static void DrawToolbar (Rect window, Vector2 mousePos) {

		// Draw toolbar bg
		EditorGUI.DrawRect ( new Rect (0,0, window.width, 40), toolbarColor );
		
		if ( GUI.Button (new Rect (5, 4, 50, 30), "New") ) {
			UPAImageCreationWindow.Init ();
		}
		if ( GUI.Button (new Rect (60, 4, 50, 30), "Open") ) {
			CurrentImg = UPASession.OpenImage ();
			if (CurrentImg == null)
				return;
		}
		if ( GUI.Button (new Rect (115, 4, 50, 30), "Export") ) {
			UPAExportWindow.Init(CurrentImg);
		}

		if (GUI.Button (new Rect (179, 6, 25, 25), "+")) {
			CurrentImg.gridSpacing *= 1.2f;
		}
		if (GUI.Button (new Rect (209, 6, 25, 25), "-")) {
			CurrentImg.gridSpacing *= 0.8f;
			CurrentImg.gridSpacing -= 2;
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
		style.fontSize = 12;
		style.fontStyle = FontStyle.Normal;
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

		if (CurrentImg.tool == UPATool.ColorPicker) {
			style.fontStyle = FontStyle.Bold;
			style.fontSize = 15;
			GUI.Label (new Rect (window.width/2f - 140, 60, 100, 30), "Click on a pixel to choose a color.", style);
		}


		mousePixelPos = CurrentImg.GetMousePixelPos(mousePos);
		GUI.Label (new Rect (860, 11, 100, 30), "(" + (int)mousePixelPos.x + "," + ((int)mousePixelPos.y+1)*-1 + ")", style);
	}
	
}
