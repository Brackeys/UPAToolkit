//-----------------------------------------------------------------
// This class hosts utility methods for handling session information.
//-----------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using System.IO;

public class UPASession {

    /// <summary>
    /// Save layer image
    /// </summary>
    /// <param name="layer">Layer to save</param>
	public static void SaveLayer(UPALayer layer) {
        string path = EditorPrefs.GetString("currentLayerPath") + "/" + layer.name + ".png";

		byte[] bytes = layer.image.EncodeToPNG();
		
		//Write to a file in the project folder
        File.WriteAllBytes(path, bytes);
	}

    /// <summary>
    /// Load image from layer file
    /// </summary>
    /// <param name="name">Layer Name</param>
    /// <returns>Layer Texture</returns>
    internal static Texture2D LoadLayerImage(string name)
    {
        string path = EditorPrefs.GetString("currentLayerPath") + "/" + name + ".png";

        byte[] bytes = File.ReadAllBytes(path);

        Texture2D texture = new Texture2D(1, 1);
        texture.LoadImage(bytes);
        texture.filterMode = FilterMode.Point;
        return texture;
    }

	public static void CreateImage (int w, int h) {
		string path = EditorUtility.SaveFilePanel ("Create UPAImage",
		                                           "Assets/", "Pixel Image.asset", "asset");
		if (path == "") {
			return;
		}
		
		path = FileUtil.GetProjectRelativePath(path);

		string layerPath = (path.Substring (0, path.Length - Path.GetExtension (path).Length) + " Layers").Replace(" ", "_");
		Directory.CreateDirectory (layerPath);
		
		UPAImage img = ScriptableObject.CreateInstance<UPAImage>();
		AssetDatabase.CreateAsset (img, path);
		
		AssetDatabase.SaveAssets();
		
		img.Init(w, h);
		EditorUtility.SetDirty(img);
		UPAEditorWindow.CurrentImg = img;

		EditorPrefs.SetString ("currentLayerPath", layerPath);
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

            string layerPath = (path.Substring(0, path.Length - Path.GetExtension(path).Length) + " Layers").Replace(" ", "_");
            EditorPrefs.SetString("currentLayerPath", layerPath);
			EditorPrefs.SetString ("currentImgPath", path);

            // Load Layers
            foreach (Object obj in AssetDatabase.LoadAllAssetsAtPath(path))
            {
                if (obj is UPALayer)
                {
                    UPALayer layer = (UPALayer) obj;
                    layer.LoadImage();
                    img.layers.Add(layer);
                }
            }

            
			return img;
		}
		
		return null;
	}
	
	public static UPAImage OpenImageByAsset (UPAImage img) {

		if (img == null) {
			Debug.LogWarning ("Image is null. Returning null.");
            EditorPrefs.SetString("currentLayerPath", "");
			EditorPrefs.SetString ("currentImgPath", "");
			return null;
		}

		string path = AssetDatabase.GetAssetPath (img);
        string layerPath = (path.Substring(0, path.Length - Path.GetExtension(path).Length) + " Layers").Replace(" ", "_");
        EditorPrefs.SetString("currentLayerPath", layerPath);
		EditorPrefs.SetString ("currentImgPath", path);

        // Load Layers
        foreach (Object obj in AssetDatabase.LoadAllAssetsAtPath(path))
        {
            if (obj is UPALayer)
            {
                UPALayer layer = (UPALayer)obj;
                layer.LoadImage();
                img.layers.Add(layer);
            }
        }

		return img;
	}

	public static UPAImage OpenImageAtPath (string path) {
		if (path.Length != 0) {
			UPAImage img = AssetDatabase.LoadAssetAtPath(path, typeof(UPAImage)) as UPAImage;

			if (img == null) {
                EditorPrefs.SetString("currentLayerPath", "");
				EditorPrefs.SetString ("currentImgPath", "");
				return null;
			}

            string layerPath = (path.Substring(0, path.Length - Path.GetExtension(path).Length) + " Layers").Replace(" ", "_");
            EditorPrefs.SetString("currentLayerPath", layerPath);
			EditorPrefs.SetString ("currentImgPath", path);

            // Load Layers
            foreach (Object obj in AssetDatabase.LoadAllAssetsAtPath(path))
            {
                if (obj is UPALayer)
                {
                    UPALayer layer = (UPALayer)obj;
                    layer.LoadImage();
                    img.layers.Add(layer);
                }
            }

			return img;
		}
		
		return null;
	}

	public static Texture2D PreviewImage (UPAImage img) {
        return img.CalculateCombinedImage();
	}
	
	public static bool ExportImage (UPAImage img, TextureType type, TextureExtension extension) {
		string path = EditorUtility.SaveFilePanel(
			"Export image as " + extension.ToString(),
			"Assets/",
			img.name + "." + extension.ToString(),
			extension.ToString());
		
		if (path.Length == 0)
			return false;

		byte[] bytes;
		if (extension == TextureExtension.PNG) {
			// Encode texture into PNG
            bytes = img.CalculateCombinedImage().EncodeToPNG();
		} else {
			// Encode texture into JPG
			
			#if UNITY_4_2
			bytes = tex.EncodeToPNG();
			#elif UNITY_4_3
			bytes = tex.EncodeToPNG();
			#elif UNITY_4_5
			bytes = tex.EncodeToJPG();
			#else
            bytes = img.CalculateCombinedImage().EncodeToJPG();
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
		
		AssetDatabase.ImportAsset(path); 
		
		return true;
	}
}
