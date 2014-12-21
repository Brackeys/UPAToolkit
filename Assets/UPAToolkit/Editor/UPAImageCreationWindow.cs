//-----------------------------------------------------------------
// This script handles the Image Creation Window where you can add new UPAImages.
// It draws the proper editor GUI and hosts methods for instantiating images which can be edited
// in the UPAEditorWindow. The images created here can also be exported using the UPAExportWindow.
//-----------------------------------------------------------------

using UnityEngine;
using UnityEditor;

public class UPAImageCreationWindow : EditorWindow {
	
	public static UPAImageCreationWindow window;
	
	private string imageName = "PixelArt";
	private int xRes = 32, yRes = 32;

	public static void Init () {
		// Get existing open window or if none, make new one
		window = (UPAImageCreationWindow)EditorWindow.GetWindow (typeof (UPAImageCreationWindow));
		window.title = "New Image";
		window.position = new Rect(Screen.width/2,Screen.height/2, 250, 150);
		window.ShowPopup();
	}
	
	public void CreateImage () {
		string path = EditorUtility.SaveFilePanel ("Create UPAImage",
		                                           "Assets/", "PixelImage.asset", "asset");
		if (path == "") {
			return;
		}
		
		path = FileUtil.GetProjectRelativePath(path);
		
		UPAImage img = CreateInstance<UPAImage>();
		AssetDatabase.CreateAsset (img, path);
		
		AssetDatabase.SaveAssets();
		
		img.Init(xRes, yRes);
		EditorUtility.SetDirty(img);
		UPAEditorWindow.CurrentImg = img;
		
		if (UPAEditorWindow.window != null)
			UPAEditorWindow.window.Repaint();
		else
			UPAEditorWindow.Init();
	}
	
	void OnGUI () {
		if (window == null)
			Init ();
		
		GUILayout.Label ("UPA Image Settings", EditorStyles.boldLabel);
		imageName = EditorGUILayout.TextField ("Image Name: ", imageName);

		xRes = Mathf.Clamp (EditorGUILayout.IntField ("Width: ", xRes), 0, 128 );
		yRes = Mathf.Clamp (EditorGUILayout.IntField ("Height: ", yRes), 0, 128 );
		
		EditorGUILayout.Space ();
		EditorGUILayout.Space ();
		
		if ( GUILayout.Button ("Create", GUILayout.Height (30))) {
			this.Close();
			CreateImage ();
		}
	}
}