//-----------------------------------------------------------------
// Used in conjunktion with UPAImage.cs
// Allows for previewing and opening UPA Images through inspector.
//-----------------------------------------------------------------

using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UPAImage)), CanEditMultipleObjects]
public class UPAImageEditorPreview : Editor {
	
	//Property declaration
	public SerializedProperty map;
	
	//Editing the properties
	void OnEnable () {
		map = serializedObject.FindProperty ("map");
	}
	
	public override void OnInspectorGUI () {
		UPAImage img = (UPAImage)target;
		
		if ( GUILayout.Button ("Open", GUILayout.Height (40)) ) {
			UPAEditorWindow.CurrentImg = UPASession.OpenImageByAsset ( img );
			if (UPAEditorWindow.window != null) {
				UPAEditorWindow.window.Repaint();
			}
		}
		
		if ( GUILayout.Button ("Export", GUILayout.Height (40)) ) {
			UPAExportWindow.Init( img );
		}
			
		//UPADrawer.DrawImageInInspector ( img, new Rect (50,0, Screen.width, Screen.height) );
	}
}