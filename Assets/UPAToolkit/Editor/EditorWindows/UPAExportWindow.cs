//-----------------------------------------------------------------
// This script handles the export window for turning an UPAImage into .jpg or .png format.
// It hosts functions for exporting & also draws the proper editor GUI.
// TODO: Add ExportImage method (Kind of crucial don't you think?).
//-----------------------------------------------------------------

using UnityEngine;
using UnityEditor;

public class UPAExportWindow : EditorWindow {
	
	public static UPAExportWindow window;
	
	private string fileName = "PixelArt";
	private TextureType texType = TextureType.sprite;

	public static void Init () {
		// Get existing open window or if none, make new one
		window = (UPAExportWindow)EditorWindow.GetWindow (typeof (UPAExportWindow));
		window.title = "Export Image";
	}

	public static void ExportImage () {
		Debug.Log ("Implement export.");
	}
	
	void OnGUI () {
		if (window == null)
			Init ();
		
		GUILayout.Label ("UPA Export Settings", EditorStyles.boldLabel);
		fileName = EditorGUILayout.TextField ("File Name: ", fileName);
		texType = (TextureType)EditorGUILayout.EnumPopup("Texture Type:", texType);
		
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		
		if ( GUILayout.Button ("Export", GUILayout.Height(30)) ) {
			ExportImage ();
			UPAEditorWindow.window.Repaint();
		}
	}
}