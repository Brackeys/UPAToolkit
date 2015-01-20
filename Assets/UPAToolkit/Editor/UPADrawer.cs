using UnityEngine;
using UnityEditor;
using System;

public class UPADrawer : MonoBehaviour {
	
	private static UPAImage CurrentImg {
		get { return UPAEditorWindow.CurrentImg; }
		set { UPAEditorWindow.CurrentImg = value; }
	}


	// VISUAL SETTINGS
	
	private static Color32 toolbarColor = new Color32 (50, 50, 50, 255);
	
	private static string[] gridBGStrings = new string[] {"Black", "White"};
	public static Color gridBGColor = Color.black;
	
	private static GUIStyle style = new GUIStyle();


	// DRAWING METHODS

	// Draw an image inside the editor window
	public static void DrawImage (UPAImage img) {
		Rect texPos = img.GetImgRect();
		EditorGUI.DrawTextureTransparent (texPos, img.tex);

        if (CurrentImg.gridlines) {
            // Draw a grid above the image (y axis first)
            for (int x = 0; x <= img.width; x += 1) {
                float posX = texPos.xMin + ((float)texPos.width / (float)img.width) * x - 0.2f;
                EditorGUI.DrawRect(new Rect(posX, texPos.yMin, 1, texPos.height), gridBGColor);
            }
            // Then x axis
            for (int y = 0; y <= img.height; y += 1) {
                float posY = texPos.yMin + ((float)texPos.height / (float)img.height) * y - 0.2f;
                EditorGUI.DrawRect(new Rect(texPos.xMin, posY, texPos.width, 1), gridBGColor);
            }
        }
        
	}

	// Draw the settings toolbar
	public static void DrawToolbar (Rect window) {
        // Begin the horizontal layout for the toolbar and utilise the built-in unity toolbar style
        GUILayout.BeginHorizontal(EditorStyles.toolbar);

        // New Button
        if (GUILayout.Button(new GUIContent("New", "Create a new pixel art image asset."), EditorStyles.toolbarButton)) {
            UPAImageCreationWindow.Init();
        }

        // Open Button
        if (GUILayout.Button(new GUIContent("Open", "Open an existing pixel art image asset."), EditorStyles.toolbarButton)) {
            CurrentImg = UPASession.OpenImage();
            if (CurrentImg == null)
                return;
        }

        // Export Button
        if (GUILayout.Button(new GUIContent("Export", "Export a pixel art image to a sprite or texture."), EditorStyles.toolbarButton)) {
            UPAExportWindow.Init(CurrentImg);
        }

        GUILayout.Space(5);

        // Zoom Slider
        CurrentImg.gridSpacing = GUILayout.HorizontalSlider(CurrentImg.gridSpacing, 1, 140, GUILayout.MinWidth(80));

        GUILayout.Space(5);

        // Colour Selector with built in Colour Picker widget
        CurrentImg.selectedColor = EditorGUILayout.ColorField(CurrentImg.selectedColor, GUILayout.MaxWidth(80));

        // Tool Dropdown/Popup
        // The button pulls its label from the UPATool enum for whichever tool is selected
        if (GUILayout.Button(new GUIContent(System.Enum.GetName(typeof(UPATool), CurrentImg.tool), "Select a drawing tool to use."), EditorStyles.toolbarPopup, GUILayout.MinWidth(80))) {
            // Create and Populate the tool menu
            GenericMenu toolMenu = new GenericMenu();           
            foreach (UPATool tool in System.Enum.GetValues(typeof(UPATool))) {
                UPATool t = tool; // Copy By Value instead of using 'tool' which is By Reference
                if (t != UPATool.Empty) { // Show all tools except UPATool.Empty
                    // Add the menu item
                    // The label is the Enum name based on the iteration through the array
                    toolMenu.AddItem(new GUIContent(System.Enum.GetName(typeof(UPATool), t)), false, () => { CurrentImg.tool = t; });
                }
            }
             
            // Show the tool menu
            toolMenu.DropDown(new Rect(298, 16, 0, 0));
        }

        // Gridline Toggle button
        CurrentImg.gridlines = GUILayout.Toggle(CurrentImg.gridlines, new GUIContent("Gridlines", "Show/Hide Gridlines."), EditorStyles.toolbarButton);

        // Gridline Colour Selector because why not?
        gridBGColor = EditorGUILayout.ColorField(gridBGColor, GUILayout.MaxWidth(80));

        // Center image button
        if (GUILayout.Button(new GUIContent("Center", "Center the image in the window."), EditorStyles.toolbarButton)) {
            CurrentImg.gridOffsetX = 0;
            CurrentImg.gridOffsetY = 0;
        }
        // Navigation instructions label
        EditorGUILayout.LabelField("Use WASD to navigate.");

        // Use up the rest of the horizontal space
        GUILayout.FlexibleSpace();

        // End the toolbar layout
        GUILayout.EndHorizontal();
	}
}
