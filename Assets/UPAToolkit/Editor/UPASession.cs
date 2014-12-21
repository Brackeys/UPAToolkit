using UnityEngine;
using UnityEditor;

public class UPASession {

	public static UPAImage OpenImage () {
		string path = EditorUtility.OpenFilePanel(
			"Find a UPAImage (.asset)",
			"/Assets",
			"asset");
		
		if (path.Length != 0) {
			path = FileUtil.GetProjectRelativePath(path);
			UPAImage img = AssetDatabase.LoadAssetAtPath(path, typeof(UPAImage)) as UPAImage;
		//	Debug.Log (img);
			return img;
		}
		
		return null;
	}
	
}
