//-----------------------------------------------------------------
// This class stores all information about the image.
// It has a full pixel map, width & height properties and some private project data.
// It also hosts functions for calculating how the pixels should be visualized in the editor.
// TODO: Turn into scriptable object for easy instiation and data serialization.
// TODO: Make pixel map one directional to serialize it.
//-----------------------------------------------------------------

using UnityEngine;
using System.Collections;

// Each Pixel has a color & a rect to represent
// it's graphics in the editor window.
public struct Pixel {
	public Rect rect;
	public Color color;
}

[System.Serializable]
public class UPAImage {
	// Helper getters (TODO: Store these inside the UPAImage to make settings stick to their projects.)
	public float gridSpacing {
		get { return UPAEditorWindow.gridSpacing + 1f; }
	}
	public float gridOffsetY {
		get { return UPAEditorWindow.gridOffsetY; }
	}
	public float gridOffsetX {
		get { return UPAEditorWindow.gridOffsetX; }
	}
	public Rect window {
		get { return UPAEditorWindow.window.position; }
	}
	
	// Important data
	public int width;
	public int height;
	public Pixel[,] map;
	
	// Class constructor
	public UPAImage (int w, int h) {
		width = w;
		height = h;
		
		map = new Pixel[w,h];

		// Set all pixels to an alpha of 0
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				map [x,y].color = Color.clear;
			}
		}
	}

	// Calculate how the pixels should be laid out in the editor
	public void UpdateRects () {
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				float xPos = x * gridSpacing + ( window.width / 2f ) - (width * gridSpacing) / 2f;
				float yPos = y * gridSpacing + ( window.height / 2f ) - (height * gridSpacing) / 2f + 20;
				yPos += gridOffsetY;
				xPos += gridOffsetX;

				map[x,y].rect = new Rect (xPos, yPos, gridSpacing - 1, gridSpacing - 1);
			}
		}
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
		for (int x = 0; x < width; x++) {
			for (int y = 0; y < height; y++) {
				if (map[x,y].rect.Contains (pos)) {
					map [x,y].color = color;
				}
			}
		}
	}
}
