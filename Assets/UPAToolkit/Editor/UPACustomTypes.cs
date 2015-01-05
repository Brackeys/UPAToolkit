using UnityEngine;

// Each Pixel has a color & a rect to represent
// it's graphics in the editor window.
[System.Serializable]
public struct Pixel {
	public Rect rect;
	public Color color;
	
	//public int layerPos;
}

// Used for switching tools
public enum UPATool {
	PaintBrush,
	Eraser,
	BoxBrush, // TODO: Add BoxBrush
	Empty, // Used as null
}

// Used for selecting texture export type
public enum TextureType {
	sprite = 0,
	texture = 1,
}

// Used for selecting texture export exstension
public enum TextureExtension {
	PNG = 0,
	JPG = 1,
}