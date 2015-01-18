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
	private Rect window {
		get { return UPAEditorWindow.window.position; }
	}
	

	// IMAGE DATA

	public int width;
	public int height;
	
	public Texture2D tex;
	public Color[] map;


	// VIEW & NAVIGATION SETTINGS

	[SerializeField]
	private float _gridSpacing = 20f;
	public float gridSpacing {
		get { return _gridSpacing + 1f; }
		set { _gridSpacing = Mathf.Clamp (value, 0, 140f); }
	}

	public float gridOffsetY = 0;
	public float gridOffsetX = 0;
	
	
	// PAINTING SETTINGS

	public Color selectedColor = new Color (1,0,0,1);
	public UPATool tool = UPATool.PaintBrush;
	public int gridBGIndex = 0;
	
	
	// Class constructor
	public UPAImage () {
		// do nothing so far
	}

	// This is not called in constructor to have more control
	public void Init (int w, int h) {
		width = w;
		height = h;

		tex = new Texture2D (w, h);
		map = new Color[w * h];

		for (int x = 0; x < w; x++) {
			for (int y = 0; y < h; y++) {
				tex.SetPixel (x, y, Color.clear);
				map [x + y * w] = Color.clear;
			}
		}

		tex.filterMode = FilterMode.Point;
		tex.Apply();
		
		EditorUtility.SetDirty (this);
	}

	// Color a certain pixel by position in window
	public void ColorPixel (Color color, Vector2 pos) {
		Rect texPos = GetImgRect();
		
		if (!texPos.Contains (pos)) {
			return;
		}

		Undo.RecordObject (tex, "ColorPixel");

		float relX = (pos.x - texPos.x) / texPos.width;
		float relY = (texPos.y - pos.y) / texPos.height;

		int pixelX = (int)( tex.width * relX );
		int pixelY = (int)( tex.height * relY ) - 1;

		tex.SetPixel (pixelX, pixelY, color);
		tex.Apply();

		map [pixelX + pixelY * - 1 * width - height] = color;
		
		EditorUtility.SetDirty (this);
	}

	// Return a certain pixel by position in window
	public Color GetPixelColor (Vector2 pos) {
		Rect texPos = GetImgRect();
		
		if (!texPos.Contains (pos)) {
			return Color.clear;
		}
		
		float relX = (pos.x - texPos.x) / texPos.width;
		float relY = (texPos.y - pos.y) / texPos.height;
		
		int pixelX = (int)( tex.width * relX );
		int pixelY = (int)( tex.height * relY ) - 1;
		
		return tex.GetPixel (pixelX, pixelY);
	}

	// Get the rect of the image as displayed in the editor
	public Rect GetImgRect () {
		float ratio = (float)height / (float)width;
		float w = gridSpacing * 30;
		float h = ratio * gridSpacing * 30;
		
		float xPos = window.width / 2f - w/2f + gridOffsetX;
		float yPos = window.height / 2f - h/2f + 20 + gridOffsetY;

		return new Rect (xPos,yPos, w, h);
	}

	public void LoadTexFromMap () {
		tex = new Texture2D (width, height);

		tex.SetPixels (map);
		
		tex.filterMode = FilterMode.Point;
		tex.Apply();
	}
}
