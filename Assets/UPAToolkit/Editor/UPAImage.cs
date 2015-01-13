//-----------------------------------------------------------------
// This class stores all information about the image.
// It has a full pixel map, width & height properties and some private project data.
// It also hosts functions for calculating how the pixels should be visualized in the editor.
//-----------------------------------------------------------------

using UnityEngine;
using UnityEditor;

[System.Serializable]
public class UPAImage : ScriptableObject {

	// HELPER GETTERS
	public Rect window {
		get { return UPAEditorWindow.window.position; }
	}
	

	// IMAGE DATA
	
	[HideInInspector]
	public int width;
	[HideInInspector]
	public int height;
	[HideInInspector]
	public Pixel[] map;

	public int testx;
	public int testy;
	

	// VIEW & NAVIGATION SETTINGS
	
	[HideInInspector]
	[SerializeField]
	private float _gridSpacing = 20f;
	public float gridSpacing {
		get { return _gridSpacing + 1f; }
		set { _gridSpacing = Mathf.Clamp (value, 2f, 60f); }
	}
	[HideInInspector]
	public float gridOffsetY = 0;
	[HideInInspector]
	public float gridOffsetX = 0;
	
	
	// PAINTING SETTINGS
	
	[HideInInspector]
	public Color selectedColor = new Color (1,0,0,1);
	[HideInInspector]
	public UPATool tool = UPATool.PaintBrush;
	[HideInInspector]
	public int gridBGIndex = 0;
	
	
	// Class constructor
	public UPAImage () {
		// do nothing so far
	}

	// This is not called in constructor to have more control
	public void Init (int w, int h) {
		width = w;
		height = h;
	
		map = new Pixel[w * h];
		
		// Set all pixels to an alpha of 0
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				map [x + y * w].color = Color.clear;
			}
		}
		
		EditorUtility.SetDirty (this);
	}

	// Calculate how the pixels should be laid out in the editor
	public void UpdateRects () {
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				float xPos = x * gridSpacing + ( window.width / 2f ) - (width * gridSpacing) / 2f;
				float yPos = y * gridSpacing + ( window.height / 2f ) - (height * gridSpacing) / 2f + 20;
				yPos += gridOffsetY;
				xPos += gridOffsetX;

				map[x + y * width].rect = new Rect (xPos, yPos, gridSpacing - 1, gridSpacing - 1);
			}
		}
		
		EditorUtility.SetDirty (this);
	}

	// Calculate the full extend of all the pixels laid out
	public Rect FillRect () {
		return new Rect ((	window.width / 2f ) - (width * gridSpacing) / 2f - 1 + gridOffsetX,
		                 (	window.height / 2f ) - (height * gridSpacing) / 2f - 1 + gridOffsetY + 20,
		                	width * gridSpacing + 1,
		               		height * gridSpacing + 1);
	}

	// Color a certain pixel by position in window
	public void ColorPixel (Color color, Vector2 pos) {
		Undo.RecordObject (this, "ColorPixel");

		int? position = GetPixelPosition (pos);

		if (position != null){
			map[(int)position].color = color;
		}

		EditorUtility.SetDirty (this);
	}

	// returns color of pixel in position, returns null if position isn't in "Pixel[] map"
	public Color? GetPixelColor(Vector2 pos){
		int? position = GetPixelPosition (pos);

		if (position != null){
			EditorUtility.SetDirty (this);
			return map[(int)position].color;
		}

		return null;
	}

	// Returns pixel position in "Pixel[] map" from real position or null when the result is not in the pixel map
	public int? GetPixelPosition(Vector2 pos){
		float x = (pos.x - gridOffsetX - (window.width / 2f) + (width * gridSpacing) / 2f) / gridSpacing;
		float y = (pos.y - gridOffsetY - (window.height / 2f) + (height * gridSpacing) / 2f - 20) / gridSpacing;
		
		if (x >= 0 && x < width && y >= 0 && y < height)
			return (int)x + (int)y * width;
		
		return null;
	}
}
