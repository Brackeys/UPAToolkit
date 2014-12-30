using UnityEngine;
using UnityEditor;

public class UPADrawer : MonoBehaviour {

	private static Color bgColor = new Color (0.9f, 0.9f, 0.9f, 1);

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
	
}
