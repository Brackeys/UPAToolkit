//-----------------------------------------------------------------
// This script handles the main Pixel Art Editor.
// It selects tools, finds the right pixels to color, handles input events & draws the toolbar gui.
// TODO: Tidy things up. Split functionality into smaller code portions. Make even a bit of optimization?
//-----------------------------------------------------------------

using UnityEngine;
using UnityEditor;

public class UPAEditorWindow : EditorWindow {

	public static UPAEditorWindow window;	// The static instance of the window

	public static UPAImage CurrentImg;		// The img currently being edited


	// HELPFUL GETTERS AND SETTERS

	private float gridSpacing {
		get { return CurrentImg.gridSpacing; }
		set { CurrentImg.gridSpacing = value; }
	}
	private float gridOffsetX {
		get { return CurrentImg.gridOffsetX; }
		set { CurrentImg.gridOffsetX = value; }
	}
	private float gridOffsetY {
		get { return CurrentImg.gridOffsetY; }
		set { CurrentImg.gridOffsetY = value; }
	}
	
	private UPATool tool {
		get { return CurrentImg.tool; }
		set { CurrentImg.tool = value; }
	}
	private Color32 selectedColor {
		get { return CurrentImg.selectedColor; }
		set { CurrentImg.selectedColor = value; }
	}
	private int gridBGIndex {
		get { return CurrentImg.gridBGIndex; }
		set { CurrentImg.gridBGIndex = value; }
	}


	// MISC TEMP VARIABLES
	
	// Stores the previous tool when temporarily switching
	private static UPATool lastTool = UPATool.Empty;


	// INITIALIZATION
	
	[MenuItem ("Window/Pixel Art Editor %#p")]
	public static void Init () {
		// Get existing open window or if none, make new one
		window = (UPAEditorWindow)EditorWindow.GetWindow (typeof (UPAEditorWindow));
		window.title = "Pixel Art Editor";
        window.wantsMouseMove = false;

		string path = EditorPrefs.GetString ("currentImgPath", "");

		if (path.Length != 0)
			CurrentImg = UPASession.OpenImageAtPath (path);
	}
	
	
	// Draw the Pixel Art Editor.
	// This includes both toolbar and painting area.
	// TODO: Add comments
	void OnGUI () {
		if (window == null)
			Init ();

		if (CurrentImg == null) { 

			string curImgPath = EditorPrefs.GetString ("currentImgPath", "");

			if (curImgPath.Length != 0) {
				CurrentImg = UPASession.OpenImageAtPath (curImgPath);
				return;
			}

			if ( GUI.Button (new Rect (window.position.width / 2f - 140, window.position.height /2f - 25, 130, 50), "New Image") ) {
				UPAImageCreationWindow.Init ();
			}
			if ( GUI.Button (new Rect (window.position.width / 2f + 10, window.position.height /2f - 25, 130, 50), "Open Image") ) {
				CurrentImg = UPASession.OpenImage ();
				return;
			}

			return;
		}

		if (CurrentImg.tex == null) {
			CurrentImg.LoadTexFromMap();
		}


        // the DrawRect left and top of the Rect are taken relative to the window 
        // while the window.position.left and window.position.top are relative to the screen
        // 49,49,49 is the colour of the Sprite Editor background
        EditorGUI.DrawRect(new Rect(0, 0, window.position.width, window.position.height), new Color32(49, 49, 49, 255));

        // DRAW IMAGE
        UPADrawer.DrawImage(CurrentImg);
        // Draw Toolbar
        UPADrawer.DrawToolbar(window.position);

        eventHandling();
	}

    void eventHandling() {
        #region Event handling
        Event e = Event.current;	//Init event handler

        // MouseDown/MouseDrag
        if (e.type == EventType.MouseDown || e.type == EventType.MouseDrag) {
            if (tool == UPATool.PaintBrush) {
                CurrentImg.ColorPixel(selectedColor, e.mousePosition);
            } else if (tool == UPATool.Eraser) {
                CurrentImg.ColorPixel(Color.clear, e.mousePosition);
            } else if (tool == UPATool.BoxBrush) {
                Debug.Log("TODO: Add Box Brush tool.");
            } else if (tool == UPATool.ColorPicker) { // I left this in though it may not really be needed since the colour field provides access to unity's built-in colour picker already.
                Color? newColor = CurrentImg.GetPixelColor(e.mousePosition);
                if (newColor != null && newColor != Color.clear) {
                    selectedColor = (Color)newColor;
                }
            }
        }

        // KeyDown
        if (e.type == EventType.KeyDown) {
            if (e.keyCode == KeyCode.W) {
                gridOffsetY += 20f;
            }
            if (e.keyCode == KeyCode.S) {
                gridOffsetY -= 20f;
            }
            if (e.keyCode == KeyCode.A) {
                gridOffsetX += 20f;
            }
            if (e.keyCode == KeyCode.D) {
                gridOffsetX -= 20f;
            }

            if (e.keyCode == KeyCode.Alpha1) {
                tool = UPATool.PaintBrush;
            }
            if (e.keyCode == KeyCode.Alpha2) {
                tool = UPATool.Eraser;
            }
            if (e.keyCode == KeyCode.P) {
                tool = UPATool.ColorPicker;
            }

            if (e.keyCode == KeyCode.UpArrow) {
                gridSpacing *= 1.2f;
            }
            if (e.keyCode == KeyCode.DownArrow) {
                gridSpacing *= 0.8f;
                gridSpacing -= 2;
            }

        }

        if (e.control) {
            if (lastTool == UPATool.Empty) {
                lastTool = tool;
                tool = UPATool.Eraser;
            }
        } else {
            if (lastTool != UPATool.Empty) {
                tool = lastTool;
                lastTool = UPATool.Empty;
            }
        }

        // TODO: Better way of doing this?
        // Why does it behave so weirdly with my mac tablet.
        // reply: That might be because of the old getter for gridSpacing which actually returned (_gridSpacing + 1)
        // when (gridSpacing -= e.delta.y) expands to (gridSpacing = gridSpacing - e.delta.y) the getter increments the value being returned 
        if (e.type == EventType.scrollWheel) {
            gridSpacing -= e.delta.y;
        }

        // Repaint seems to work for redrawing the screen on demand since e.Use() was preventing other default unity commands from working.
        Repaint();
        #endregion
    }
}