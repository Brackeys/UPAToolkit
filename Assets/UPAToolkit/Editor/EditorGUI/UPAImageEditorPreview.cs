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

		GUILayout.BeginArea (new Rect (5,53, Screen.width-10, Screen.height));

		if ( GUILayout.Button ("Open", GUILayout.Height (40)) ) {
			UPAEditorWindow.CurrentImg = UPASession.OpenImageByAsset ( img );
			if (UPAEditorWindow.window != null) {
				UPAEditorWindow.window.Repaint();
			}
		}
		
		if ( GUILayout.Button ("Export", GUILayout.Height (40)) ) {
			UPAExportWindow.Init( img );
		}

		GUILayout.EndArea();

		Texture2D preview = UPASession.PreviewImage (img);
		float ratio = preview.width / preview.height;
		EditorGUI.DrawTextureTransparent (new Rect (5, 150, Screen.width - 10, (Screen.width - 10) * ratio), preview, ScaleMode.ScaleToFit, 0);
		DestroyImmediate (preview);
		//UPADrawer.DrawImageInInspector ( img, new Rect (50,0, Screen.width, Screen.height) );
	}
}