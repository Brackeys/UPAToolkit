//-----------------------------------------------------------------
// This class stores all information about the image.
// It has a full pixel map, width & height properties and some private project data.
// It also hosts functions for calculating how the pixels should be visualized in the editor.
//-----------------------------------------------------------------

using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

[System.Serializable]
public class UPAImage : ScriptableObject {

	// HELPER GETTERS
	private Rect window {
		get { return UPAEditorWindow.window.position; }
	}
	

	// IMAGE DATA

	public int width;
	public int height;
    [NonSerialized]
    public List<UPALayer> layers = new List<UPALayer>();
    public int activeLayer = 0;
    [NonSerialized]
    public Texture2D backgroundImage;

	// VIEW & NAVIGATION SETTINGS

	[SerializeField]
	private float _gridSpacing = 20f;
	public float gridSpacing {
		get { return _gridSpacing + 1f; }
		set { _gridSpacing = Mathf.Clamp (value, 0, 140f); }
	}

	public float gridOffsetY = 0;
	public float gridOffsetX = 0;
	
	
	// PAINTING SETTINGS

	public Color selectedColor = new Color (1,0,0,1);
	public UPATool tool = UPATool.PaintBrush;
    public int gridBGIndex = 0;


    // Class constructor
    public UPAImage()
    {
		backgroundImage = new Texture2D(2, 2);
		backgroundImage.filterMode = FilterMode.Point;
		backgroundImage.SetPixel(0, 0, Color.white);
		backgroundImage.SetPixel(1, 0, Color.gray);
		backgroundImage.SetPixel(0, 1, Color.gray);
		backgroundImage.SetPixel(1, 1, Color.white);
		backgroundImage.Apply ();
    }

	// This is not called in constructor to have more control
	public void Init (int w, int h) {
		width = w;
		height = h;

        // Create base layer
        UPALayer layer = ScriptableObject.CreateInstance<UPALayer>();
        layers.Add(layer);
        layer.init(w, h);

        AssetDatabase.AddObjectToAsset(layer, AssetDatabase.GetAssetPath(this));
        AssetDatabase.SaveAssets();

        
        EditorUtility.SetDirty(this);
    }

    // Gets the pixel on image based on window coordinates
    public Vector2 CalculateImagePixel(float x, float y)
    {
        float cx = ((x - gridOffsetX) + (width * gridSpacing) / 2f - (window.width / 2f)) / gridSpacing;
        float cy = ((y - gridOffsetY) - 20 + (height * gridSpacing) / 2f - (window.height / 2f)) / gridSpacing;
        return new Vector2(cx, cy);
    }

    // Gets window coordinates based on pixel
    public Vector2 CalculateViewPixel(int x, int y)
    {
        float xPos = x * gridSpacing + (window.width / 2f) - (width * gridSpacing) / 2f;
        float yPos = y * gridSpacing + (window.height / 2f) - (height * gridSpacing) / 2f + 20;
        yPos += gridOffsetY;
        xPos += gridOffsetX;

        return new Vector2(xPos, yPos);
    }

    // Calculate the full extend of all the pixels laid out
    public Rect FillRect()
    {
        return new Rect((window.width / 2f) - (width * gridSpacing) / 2f - 1 + gridOffsetX,
                         (window.height / 2f) - (height * gridSpacing) / 2f - 1 + gridOffsetY + 20,
                            width * gridSpacing + 1,
                            height * gridSpacing + 1);
    }
    // Color a certain pixel by position in window
    public void ColorPixel(Color color, Vector2 pos)
    {
        if (FillRect().Contains(pos))
        {
            Undo.RecordObject(this, "ColorPixel");
            float cx = ((pos.x - gridOffsetX) + (width * gridSpacing) / 2f - (window.width / 2f)) / gridSpacing;
            float cy = ((pos.y - gridOffsetY) - 20 + (height * gridSpacing) / 2f - (window.height / 2f)) / gridSpacing;
            
            // Why is the Y axis mirrored in center?
            if (layers.Count > 0)
            {
                if (layers.Count < activeLayer)
                    activeLayer = layers.Count - 1;
                layers[activeLayer].image.SetPixel((int)Math.Truncate(cx), height - (int)Math.Truncate(cy) - 1, color);
                layers[activeLayer].image.Apply();
                layers[activeLayer].Save();
            }
        }
        EditorUtility.SetDirty(this);
    }
		
    /// <summary>
    /// Creates a default name for use in layer
    /// </summary>
    /// <returns>Layer name "New Layer X"</returns>
    public String GetDefaultLayerName()
    {
        String name = "New Layer ";
        int nr = 0;

        while (nr < 100)
        {
            bool succes = true;
            foreach (UPALayer layer in layers)
            {
                if (layer.name == name + nr)
                    succes = false;
            }

            if (succes)
                return name + nr;
            nr++;
        }

        Exception e = new ArgumentOutOfRangeException("Layer Count", "There are more than 100 layers with default name");
        throw e;
    }

    /// <summary>
    /// Combines Layers to one image
    /// </summary>
    /// <returns>Combined image</returns>
    internal Texture2D CalculateCombinedImage()
    {
        Color[] pixels = new Color[width * height];
        foreach (UPALayer layer in layers)
        {
            Color[] cLayer = layer.image.GetPixels();
            for (int i = 0; i < width * height; i++ ) {
                if (pixels[i] == null)
                    pixels[i] = Color.clear;

                pixels[i] = pixels[i] + cLayer[i];
            }
        }

        Texture2D texture = new Texture2D(width, height);
        texture.SetPixels(pixels);
        texture.Apply();

        return texture;
    }
}
