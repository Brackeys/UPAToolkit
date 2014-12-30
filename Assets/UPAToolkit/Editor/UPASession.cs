//-----------------------------------------------------------------
// This class hosts utility methods for handling session information.
//-----------------------------------------------------------------

using UnityEngine;
using UnityEditor;

public class UPASession {

	public static void CreateImage (int w, int h) {
		string path = EditorUtility.SaveFilePanel ("Create UPAImage",
		                                           "Assets/", "Pixel Image.asset", "asset");
		if (path == "") {
			return;
		}
		
		path = FileUtil.GetProjectRelativePath(path);
		
		UPAImage img = ScriptableObject.CreateInstance<UPAImage>();
		AssetDatabase.CreateAsset (img, path);
		
		AssetDatabase.SaveAssets();
		
		img.Init(w, h);
		EditorUtility.SetDirty(img);
		UPAEditorWindow.CurrentImg = img;
		
		EditorPrefs.SetString ("currentImgPath", AssetDatabase.GetAssetPath (img));
		
		if (UPAEditorWindow.window != null)
			UPAEditorWindow.window.Repaint();
		else
			UPAEditorWindow.Init();
	}

	public static UPAImage OpenImage () {
		string path = EditorUtility.OpenFilePanel(
			"Find a UPAImage (.asset)",
			"Assets/",
			"asset");
		
		if (path.Length != 0) {
			path = FileUtil.GetProjectRelativePath(path);
			UPAImage img = AssetDatabase.LoadAssetAtPath(path, typeof(UPAImage)) as UPAImage;
			EditorPrefs.SetString ("currentImgPath", path);
			return img;
		}
		
		return null;
	}

	public static UPAImage OpenImageAtPath (string path) {
		if (path.Length != 0) {
			UPAImage img = AssetDatabase.LoadAssetAtPath(path, typeof(UPAImage)) as UPAImage;

			if (img == null) {
				EditorPrefs.SetString ("currentImgPath", "");
				return null;
			}

			EditorPrefs.SetString ("currentImgPath", path);
			return img;
		}
		
		return null;
	}
}
