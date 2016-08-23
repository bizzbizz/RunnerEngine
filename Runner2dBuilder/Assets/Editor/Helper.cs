using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class Helper
{
	static int _mainWindow = 0;
	/// <summary>
	/// 0: Block Editor Tab
	/// 1: Level Editor Tab
	/// </summary>
	public static int CurrentTab
	{
		get { return _mainWindow; }
		set { _mainWindow = value;
			//EditorUtility.SetDirty(EditorWindow.GetWindow<BlockEditorWindow>());
		}
	}

	//style

	public static Texture u { get { if (_u == null) _u = Resources.Load("u") as Texture; return _u; } }
	public static Texture d { get { if (_d == null) _d = Resources.Load("d") as Texture; return _d; } }
	public static Texture l { get { if (_l == null) _l = Resources.Load("l") as Texture; return _l; } }
	public static Texture r { get { if (_r == null) _r = Resources.Load("r") as Texture; return _r; } }
	public static Texture _u, _d, _l, _r;
	public static GUILayoutOption h30 = GUILayout.Height(30);
	public static GUILayoutOption[] w60h24 = new GUILayoutOption[] { GUILayout.Height(18), GUILayout.Width(120) };
	public static GUILayoutOption[] w40h16 = new GUILayoutOption[] { GUILayout.Height(16), GUILayout.Width(40) };
	public static GUILayoutOption[] wh16 = new GUILayoutOption[] { GUILayout.Height(16), GUILayout.Width(16) };
	public static GUILayoutOption[] w40h30 = new GUILayoutOption[] { GUILayout.Height(30), GUILayout.Width(40) };
	public static GUIStyle gsTest = new GUIStyle();

	public static GUIStyle focusedBkg = new GUIStyle();
	public static GUIStyle evenBkg = new GUIStyle();
	public static GUIStyle oddBkg = new GUIStyle();
	public static GUIStyle hdrBkg = new GUIStyle();
	public static GUIStyle errBkg = new GUIStyle();
	public static GUIStyle okBkg = new GUIStyle();

	public static void initGUI()
	{
		gsTest.normal.background = MakeTex(new Color(0, 0, 0, 0.2f));
		focusedBkg.normal.background = MakeTex(new Color(.5f, .75f, 1f, .5f));
		evenBkg.normal.background = MakeTex(new Color(1, 1, 1, .1f));
		oddBkg.normal.background = MakeTex(new Color(0, 0, 0, 0.5f));
		hdrBkg.normal.background = MakeTex(new Color(1, 1, 1, 0.2f));
		hdrBkg.padding = new RectOffset(0, 0, 0, 0);
		hdrBkg.fixedHeight = 16;
		errBkg.normal.background = MakeTex(new Color(1, .2f, 0, 1));
		okBkg.normal.background = MakeTex(new Color(0, 1, .2f, 1));
		EditorGUIUtility.labelWidth = 85;
	}

	private static Texture2D MakeTex(Color col)
	{
		Color[] pix = new Color[1];

		pix[0] = col;

		Texture2D result = new Texture2D(1, 1);
		result.SetPixels(pix);
		result.Apply();

		return result;
	}

}
