//-----------------------------------------------------------------
// This class hosts utility methods for handling session information.
//-----------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using System.IO;

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

		img.gridSpacing = 10 - Mathf.Abs (img.width - img.height)/100f;
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
	
	public static UPAImage OpenImageByAsset (UPAImage img) {

		if (img == null) {
			Debug.LogWarning ("Image is null. Returning null.");
			EditorPrefs.SetString ("currentImgPath", "");
			return null;
		}

		string path = AssetDatabase.GetAssetPath (img);
		EditorPrefs.SetString ("currentImgPath", path);
		
		return img;
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

	public static bool ExportImage (UPAImage img, TextureType type, TextureExtension extension) {
		string path = EditorUtility.SaveFilePanel(
			"Export image as " + extension.ToString(),
			"Assets/",
			img.name + "." + extension.ToString().ToLower(),
			extension.ToString().ToLower());
		
		if (path.Length == 0)
			return false;
		
		byte[] bytes;
		if (extension == TextureExtension.PNG) {
			// Encode texture into PNG
			bytes = img.GetFinalImage(true).EncodeToPNG();
		} else {
			// Encode texture into JPG
			
			#if UNITY_4_2
			bytes = img.GetFinalImage(true).EncodeToPNG();
			#elif UNITY_4_3
			bytes = img.GetFinalImage(true).EncodeToPNG();
			#elif UNITY_4_5
			bytes = img.GetFinalImage(true).EncodeToJPG();
			#else
			bytes = img.GetFinalImage(true).EncodeToJPG();
			#endif
		}
		
		path = FileUtil.GetProjectRelativePath(path);
		
		//Write to a file in the project folder
		File.WriteAllBytes(path, bytes);
		AssetDatabase.Refresh();
		
		TextureImporter texImp = AssetImporter.GetAtPath(path) as TextureImporter; 
		
		if (type == TextureType.texture)
			texImp.textureType = TextureImporterType.Image;
		else if (type == TextureType.sprite) {
			texImp.textureType = TextureImporterType.Sprite;

			#if UNITY_4_2
			texImp.spritePixelsToUnits = 10;
			#elif UNITY_4_3
			texImp.spritePixelsToUnits = 10;
			#elif UNITY_4_5
			texImp.spritePixelsToUnits = 10;
			#else
			texImp.spritePixelsPerUnit = 10;
			#endif
		}
		
		texImp.filterMode = FilterMode.Point;
		texImp.textureFormat = TextureImporterFormat.AutomaticTruecolor;
		
		AssetDatabase.ImportAsset(path); 
		
		return true;
	}
}
