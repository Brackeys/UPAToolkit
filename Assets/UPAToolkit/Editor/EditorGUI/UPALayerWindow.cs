//-----------------------------------------------------------------
// This script handles the Image Creation Window where you can add new UPAImages.
// It draws the proper editor GUI and hosts methods for instantiating images which can be edited
// in the UPAEditorWindow. The images created here can also be exported using the UPAExportWindow.
//-----------------------------------------------------------------

using UnityEngine;
using UnityEditor;

public class UPALayerWindow : EditorWindow
{

    public static UPALayerWindow window;

    private static UPALayer layer;

    private string name;

    public static void Init(UPALayer layer)
    {
        UPALayerWindow.layer = layer;
        // Get existing open window or if none, make new one
        window = (UPALayerWindow)EditorWindow.GetWindow(typeof(UPALayerWindow));
        window.title = "Layer - " + layer.name;
        window.position = new Rect(Screen.width / 2 + 250 / 2f, Screen.height / 2 - 80, 250, 105);
        window.ShowPopup();
    }

    void OnGUI()
    {
        if (window == null)
            Init(layer);

        GUILayout.Label("UPA Layer Settings", EditorStyles.boldLabel);

        name = EditorGUILayout.TextField("Hallo");

        EditorGUILayout.Space();

        if (GUILayout.Button("OK", GUILayout.Height(30)))
        {
            this.Close();
            layer.name = name;
        }
    }
}